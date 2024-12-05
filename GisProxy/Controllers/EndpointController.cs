using GisProxy.Data;
using GisProxy.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/endpoints")]
public class EndpointController : ControllerBase
{
	private readonly EndpointLimiterService _endpointLimitService;
	private readonly ApplicationDbContext _context;

	public EndpointController(EndpointLimiterService endpointLimitService, ApplicationDbContext context)
	{
		_endpointLimitService = endpointLimitService;
		_context = context;
	}

	[HttpGet("statistics")]
	public async Task<IActionResult> GetEndpointStatistics()
	{
		var endpoints = await _context.Endpoints.Include(e => e.UserLimits).ToListAsync();

		var statistics = new List<EndpointStatistics>();

		foreach (var endpoint in endpoints)
		{
			var limit = await _endpointLimitService.GetRequestLimitAsync(endpoint.Id);

			var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();

			var userRequests = endpoint.UserLimits.ToList().FirstOrDefault(r => r.EndpointId == endpoint.Id && r.UserId == ipAddress);

			if (ipAddress is null || userRequests is null)
			{
				statistics.Add(new EndpointStatistics
				{
					EndpointId = endpoint.Id,
					RequestsLimit = limit,
					RequestsUsed = 0
				});
			}
			else
			{
				statistics.Add(new EndpointStatistics
				{
					EndpointId = endpoint.Id,
					RequestsLimit = limit,
					RequestsUsed = userRequests.RequestsUsed
				});
			}
		}

		return Ok(statistics);
	}

	[HttpGet]
	public async Task<IActionResult> GetAllEndpoints([FromQuery] string endpoint)
	{
		var endpoints = await _context.Endpoints.ToListAsync();
		return Ok(endpoints);
	}

	[HttpPost("update-limit")]
	public async Task<IActionResult> UpdateEndpointLimit(UpdateEndpointRequestModel model)
	{
		var endpoint = await _context.Endpoints.FirstAsync(endpointId => endpointId.Id == model.EndpointId);

		await _endpointLimitService.SetRequestLimitAsync(endpoint.Id, model.Limit);
		
		return Ok();
	}
}
