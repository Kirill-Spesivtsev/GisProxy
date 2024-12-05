using GisProxy.Data;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/limit")]
public class LimitController : ControllerBase
{
	private readonly EndpointLimiterService _businessRuleService;
	private readonly ApplicationDbContext _context;

	public LimitController(EndpointLimiterService businessRuleService, ApplicationDbContext context)
	{
		_businessRuleService = businessRuleService;
		_context = context;
	}

	[HttpPost("set")]
	public async Task<IActionResult> SetRequestLimit([FromQuery] string endpoint, [FromQuery] int limit)
	{
		await _businessRuleService.SetRequestLimitAsync(endpoint, limit);
		return Ok($"Request limit for {endpoint} set to {limit}");
	}

	[HttpGet("get")]
	public async Task<IActionResult> GetRequestLimit([FromQuery] string endpoint)
	{
		var limit = await _businessRuleService.GetRequestLimitAsync(endpoint);
		return Ok(limit);
	}
}
