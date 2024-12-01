using GisProxy.Entities;
using Microsoft.EntityFrameworkCore;
using System.Xml;
using Endpoint = GisProxy.Entities.Endpoint;

namespace GisProxy.Data;

public class ApplicationDbContext : DbContext
{
	public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
	{
	}

	public DbSet<Endpoint> Endpoints { get; set; }
	public DbSet<UserEndpoint> UserEndpoints { get; set; }
	public DbSet<User> Users { get; set; }

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		modelBuilder.Entity<Entities.Endpoint>()
			.HasKey(e => e.Id);

		modelBuilder.Entity<User>()
			.HasKey(e => e.Id);

		modelBuilder.Entity<UserEndpoint>()
			.HasKey(e => new {e.UserId, e.EndpointId });

		modelBuilder.Entity<UserEndpoint>()
			.HasOne(e => e.User)
			.WithMany(u => u.UserLimits)
			.HasForeignKey(e=>e.UserId);

		modelBuilder.Entity<UserEndpoint>()
			.HasOne(e => e.Endpoint)
			.WithMany(u => u.UserLimits)
			.HasForeignKey(e => e.EndpointId);

		modelBuilder.Entity<Endpoint>().HasData(
			new Endpoint 
			{ 
				Title = "Oбзорная карта", 
				Id = "https://portaltest.gismap.by/arcservertest/rest/services/C01_Belarus_WGS84/Belarus_BaseMap_WGS84/MapServer",
				Limit = 100,
				
			},
			new Endpoint
			{
				Title = "Населенные пункты",
				Id = "https://portaltest.gismap.by/arcservertest/rest/services/A06_ATE_TE_WGS84/ATE_Minsk_public/MapServer/1",
				Limit = 100,

			},
			new Endpoint
			{
				Title = "земельные участки",
				Id = "https://portaltest.gismap.by/arcservertest/rest/services/A05_EGRNI_WGS84/Uchastki_Minsk_public/MapServer/0",
				Limit = 100,

			},
			new Endpoint
			{
				Title = "Виды земель",
				Id = "https://portaltest.gismap.by/arcservertest/rest/services/A01_ZIS_WGS84/Land_Minsk_public/MapServer/0",
				Limit = 100,
			}
		);
	}
}
