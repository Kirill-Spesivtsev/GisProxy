using GisProxy.Configuration;
using GisProxy.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using GisProxy.Helpers;
using System.Collections.Concurrent;

var EndpointRequestCounts = new ConcurrentDictionary<string, ConcurrentDictionary<string, int>>();

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddHttpClient();

builder.Services.AddDbContext<ApplicationDbContext>(options => {
	options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));

options.ConfigureWarnings(warnings =>
		warnings.Ignore(RelationalEventId.PendingModelChangesWarning));
	});

builder.Logging.ClearProviders();
builder.Logging.AddSerilogConfiguration();

builder.Services.AddScoped<EndpointLimiterService>();
builder.Services.AddSingleton(EndpointRequestCounts);

builder.Services.AddCors(options => options.AddPolicy("DefaultCORS", p => p.WithOrigins("http://localhost:4200").AllowAnyMethod().AllowAnyHeader()));

builder.Services.AddMemoryCache();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseCors("DefaultCORS");

app.UseRouting();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

await app.ApplyMigrationsAsync<ApplicationDbContext>();

app.Run();

