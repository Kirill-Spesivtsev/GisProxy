using GisProxy.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System;

public class EndpointLimiterService
{
	private readonly ApplicationDbContext _context;
	private readonly IMemoryCache _cache;

	public EndpointLimiterService(ApplicationDbContext context, IMemoryCache cache)
	{
		_context = context;
		_cache = cache;
	}

	public async Task<int> GetRequestLimitAsync(string endpoint)
	{
		var cacheKey = $"RequestLimit_{endpoint}";
		if (!_cache.TryGetValue(cacheKey, out int requestLimit))
		{
			var rule = await _context.Endpoints
				.Where(r => r.Id == endpoint)
				.FirstOrDefaultAsync();

			requestLimit = rule?.Limit ?? 0;

			_cache.Set(cacheKey, requestLimit);
		}

		return requestLimit;
	}

	public async Task SetRequestLimitAsync(string endpoint, int limit)
	{
		var rule = await _context.Endpoints
			.Where(r => r.Id == endpoint)
			.FirstOrDefaultAsync();

		if (rule != null)
		{
			rule.Limit = limit;
			_context.Endpoints.Update(rule);
		}

		await _context.SaveChangesAsync();

		_cache.Set($"RequestLimit_{endpoint}", limit);
	}
}