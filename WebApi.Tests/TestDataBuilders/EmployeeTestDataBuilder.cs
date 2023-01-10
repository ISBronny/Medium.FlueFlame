using WebApi.Contexts;
using WebApi.Models;

namespace WebApi.Tests.TestDataBuilders;

public class EmployeeTestDataBuilder
{
	private Guid? _guid;
	private string? _position;
	private string? _fullName;

	protected static readonly string DefaultPosition = "Junior Php Developer";
	protected static readonly string DefaultFullName = "Elon Musk";

	public EmployeeTestDataBuilder WithGuid(Guid guid)
	{
		_guid = guid;
		return this;
	}

	public EmployeeTestDataBuilder WithFullName(string fullName)
	{
		_fullName = fullName;
		return this;
	}
	
	public EmployeeTestDataBuilder WithPosition(string position)
	{
		_position = position;
		return this;
	}
	
	public Employee Build()
	{
		var employee = new Employee()
		{
			Guid = _guid,
			Position = _position ?? DefaultPosition,
			FullName = _fullName ?? DefaultFullName,
		};

		return employee;
	}

	public Employee Build(ApplicationContext applicationContext)
	{
		var employee = Build();
		employee.Guid ??= Guid.NewGuid();

		applicationContext.Employees.Add(employee);
		applicationContext.SaveChanges();

		return employee;
	}
}