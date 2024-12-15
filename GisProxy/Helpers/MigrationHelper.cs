using Microsoft.EntityFrameworkCore;

namespace GisProxy.Helpers;


public static class MigrationHelper
{
	public static async Task ApplyMigrationsAsync<T>(this WebApplication app) where T : DbContext
	{
		using var scope = app.Services.CreateScope();
		var context = scope.ServiceProvider.GetRequiredService<T>();
		await context.Database.EnsureCreatedAsync();
	}
}
