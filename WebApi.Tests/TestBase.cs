using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using FlueFlame.AspNetCore;
using FlueFlame.Http.Host;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using WebApi.Auth;
using WebApi.Contexts;

namespace WebApi.Tests;

public abstract class TestBase
{
	protected TestServer TestServer { get; }
	protected IFlueFlameHttpHost HttpHost { get; }
	protected IServiceProvider ServiceProvider { get; }
	protected ApplicationContext Context => ServiceProvider.CreateScope()
		.ServiceProvider.GetRequiredService<ApplicationContext>();

	protected TestBase()
	{
		var factory = new WebApplicationFactory<Program>()
			.WithWebHostBuilder(builder =>
			{
				builder.UseEnvironment("E2E");

				builder.ConfigureServices(services =>
				{
					//Configure your services here
					var dbContextDescriptor = services.SingleOrDefault(
						d => d.ServiceType ==
						     typeof(DbContextOptions<ApplicationContext>));
					
					services.Remove(dbContextDescriptor);

					//Unique Database name for each test.
					var dbName = $"E2E_{Guid.NewGuid()}";
					
					//Use InMemory Database
					services.AddDbContext<ApplicationContext>(x => x.UseInMemoryDatabase(dbName));
				});
			});

		TestServer = factory.Server;
		ServiceProvider = factory.Services;

		HttpHost = FlueFlameAspNetBuilder.CreateDefaultBuilder(factory)
			.BuildHttpHost(builder =>
			{
				builder.ConfigureHttpClient(client =>
				{
					client.DefaultRequestHeaders.Add("Authorization", $"Bearer {GetJwtToken()}");
				});
			});
	}
	
	protected string GetJwtToken(string role = "admin", TimeSpan? lifetime = null)
	{
		var jwt = new JwtSecurityToken(
			issuer: AuthOptions.ISSUER,
			audience: AuthOptions.AUDIENCE,
			notBefore: DateTime.UtcNow,
			claims: new List<Claim>() { new(ClaimsIdentity.DefaultRoleClaimType, role) },
			expires: DateTime.UtcNow.Add(lifetime ?? TimeSpan.FromMinutes(AuthOptions.LIFETIME)),
			signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(),
				SecurityAlgorithms.HmacSha256));

		return new JwtSecurityTokenHandler().WriteToken(jwt);
	}

	
}