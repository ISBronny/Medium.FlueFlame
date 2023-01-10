using System.Net;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using WebApi.Contexts;
using WebApi.Models;
using WebApi.Tests.TestDataBuilders;

namespace WebApi.Tests.Controllers;

public class EmployeeControllerTests : TestBase
{
		[Fact]
		public void GetById_EmployeeExists_ReturnsEmployee()
		{
			var employee = new EmployeeTestDataBuilder()
					.WithFullName("Linus Torvalds")
					.Build(Context);

			HttpHost.Get
				.Url($"/api/employee/{employee.Guid}")
				.Send()
				.Response
					.AssertStatusCode(HttpStatusCode.OK)
					.AsJson
						.AssertThat<Employee>(response => response.FullName.Should().Be(employee.FullName));
		}

		[Fact]
		public void Create_Duplicate_ReturnsBadRequest()
		{
			var employee = new EmployeeTestDataBuilder()
				.WithGuid(Guid.NewGuid())
				.Build(Context);
			
			HttpHost.Post
				.Url("/api/employee")
				.Json(employee)
				.Send()
				.Response
					.AssertStatusCode(HttpStatusCode.BadRequest);
		}

	
}