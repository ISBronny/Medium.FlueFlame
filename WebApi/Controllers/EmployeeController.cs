using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApi.Contexts;
using WebApi.Models;

namespace WebApi.Controllers;

[ApiController]
[Route("api/employee")]
[Authorize(Roles = "admin")]
public class EmployeeController : ControllerBase
{
	private readonly ApplicationContext _applicationContext;

	public EmployeeController(ApplicationContext applicationContext)
	{
		_applicationContext = applicationContext;
	}
	
	[Route("{guid:guid}")]
	[HttpGet]
	public async Task<IActionResult> GetById(Guid guid)
	{
		var employee = await _applicationContext.Employees.FirstOrDefaultAsync(x => x.Guid == guid);
		if (employee == null)
			return NotFound();
		return Ok(employee);
	}
    
	[Route("")]
	[HttpPost]
	public async Task<ActionResult<Employee>> Create([FromBody] Employee employee)
	{
		if(!ModelState.IsValid)
			return BadRequest(ModelState);
        
		if (await _applicationContext.Employees.AnyAsync(e=>e.Guid == employee.Guid))
			return BadRequest("Already exists");
        
		var created =  _applicationContext.Add(employee).Entity;
		await _applicationContext.SaveChangesAsync();

		return new ActionResult<Employee>(created);
	}
}