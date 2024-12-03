using GisProxy.Data;
using GisProxy.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System.Collections.Concurrent;

namespace GisProxy.Controllers;

[ApiController]
[Route("")]
public class ProxyController : ControllerBase
{
	private ILogger<ProxyController> _logger;

	private static readonly HttpClient HttpClient = new HttpClient();

	public readonly IMemoryCache _memoryCache;

	private readonly EndpointLimiterService _endpointLimiterService;

	private readonly ApplicationDbContext _context;

	public ProxyController(
		EndpointLimiterService businessRuleService, 
		ILogger<ProxyController> logger, 
		IMemoryCache memoryCache,
		ApplicationDbContext context)
	{
		_logger = logger;
		_endpointLimiterService = businessRuleService;
		_context = context;
		_memoryCache = memoryCache;
	}

	[HttpGet("arcservertest/rest/services/C01_Belarus_WGS84/Belarus_BaseMap_WGS84/MapServer")]
	public async Task<IActionResult> GetC01(CancellationToken token)
	{
		var targetUrl = $"https://portaltest.gismap.by/arcservertest/rest/services/C01_Belarus_WGS84/Belarus_BaseMap_WGS84/MapServer";
		return await ProxyRequest(targetUrl);
	}

	[HttpGet("arcservertest/rest/services/A06_ATE_TE_WGS84/ATE_Minsk_public/MapServer/1")]
	public async Task<IActionResult> GetA06(CancellationToken token)
	{
		var targetUrl = $"https://portaltest.gismap.by/arcservertest/rest/services/A06_ATE_TE_WGS84/ATE_Minsk_public/MapServer/1";
		return await ProxyRequest(targetUrl);
	}

	[HttpGet("arcservertest/rest/services/A05_EGRNI_WGS84/Uchastki_Minsk_public/MapServer/0")]
	public async Task<IActionResult> GetA05(CancellationToken token)
	{
		var targetUrl = $"https://portaltest.gismap.by/arcservertest/rest/services/A05_EGRNI_WGS84/Uchastki_Minsk_public/MapServer/0";
		return await ProxyRequest(targetUrl);
	}

	[HttpGet("arcservertest/rest/services/A01_ZIS_WGS84/Land_Minsk_public/MapServer/0")]
	public async Task<IActionResult> GetA01(CancellationToken token)
	{
		var targetUrl = $"https://portaltest.gismap.by/arcservertest/rest/services/A01_ZIS_WGS84/Land_Minsk_public/MapServer/0";
		return await ProxyRequest(targetUrl);
	}

	private async Task<IActionResult> ProxyRequest(string targetUrl)
	{
		var endpointKey = targetUrl;
		var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();

		if (ipAddress != null)
		{

			var user = _context.Users.Include(u => u.UserLimits).FirstOrDefault(u => u.Id == ipAddress);
			if (user == null)
			{
				user = new User { Id = ipAddress };
				_context.Users.Add(user);
			}

			int? requestsUsed;
			var restriction = user.UserLimits.FirstOrDefault(r => r.EndpointId == endpointKey && r.UserId == ipAddress);
			if (restriction != null)
			{
				restriction.RequestsUsed++;
				requestsUsed = restriction.RequestsUsed;
				_context.Update(restriction);
			}
			else
			{
				var newRestriction = new UserEndpoint { EndpointId = endpointKey, UserId = ipAddress, RequestsUsed = 1 };
				requestsUsed = 1;
				_context.Add(newRestriction);
			}

			var requestLimit = await _endpointLimiterService.GetRequestLimitAsync(endpointKey);
			if (requestsUsed >= requestLimit)
			{
				return StatusCode(StatusCodes.Status403Forbidden, "Request limit was reached for this endpoint.");
			}

			await _context.SaveChangesAsync();

			_logger.LogInformation($"User {ipAddress} sent to {endpointKey}");
			_logger.LogInformation($"Total {requestsUsed} requests of {requestLimit}");
		}

		var cacheKey = $"Endpoint_{targetUrl}";
		var result = await _memoryCache.GetOrCreateAsync(cacheKey, async entry => {
			entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(60);
			entry.SlidingExpiration = TimeSpan.FromMinutes(10);
			var response = await HttpClient.GetAsync(targetUrl);
			var content = await response.Content.ReadAsStringAsync();
			return Content(content, response.Content.Headers?.ContentType?.ToString()!);
		});

		return result!;
	}
}