using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;
using EmpManagement.Core;
using EmployeeManagement.Services;

[ApiController]
[Route("api/[controller]")]
public class EmployeeController : ControllerBase
{
    private readonly IEmployeeService _employeeService;
    private readonly ILogger<EmployeeController> _logger;

    public EmployeeController(IEmployeeService employeeService, ILogger<EmployeeController> logger)
    {
        _employeeService = employeeService;
        _logger = logger;
    }

    // GET: api/employee
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        _logger.LogInformation("Fetching all employees");
        var employees = await _employeeService.GetAllAsync();
        _logger.LogInformation("Fetched {Count} employees", employees.Count);
        return Ok(employees);
    }

    // GET: api/employee/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        _logger.LogInformation("Fetching employee with Id: {Id}", id);
        var employee = await _employeeService.GetByIdAsync(id);
        if (employee == null)
        {
            _logger.LogWarning("Employee not found: {Id}", id);
            return NotFound();
        }

        _logger.LogInformation("Employee found: {Name}", employee.Name);
        return Ok(employee);
    }

    // POST: api/employee
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Employee employee)
    {
        if (employee == null)
        {
            _logger.LogWarning("Attempted to create a null employee");
            return BadRequest();
        }

        _logger.LogInformation("Creating a new employee: {Name}", employee.Name);
        var createdEmployee = await _employeeService.AddAsync(employee);
        if (createdEmployee == null)
        {
            _logger.LogError("Failed to create employee");
            return BadRequest();
        }

        _logger.LogInformation("Employee created successfully with Id: {Id}", createdEmployee.Id);
        return CreatedAtAction(nameof(GetById), new { id = createdEmployee.Id }, createdEmployee);
    }

    // PUT: api/employee/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, [FromBody] Employee employee)
    {
        if (employee == null)
        {
            _logger.LogWarning("Null employee update attempted");
            return BadRequest();
        }

        var existingEmployee = await _employeeService.GetByIdAsync(id);
        if (existingEmployee == null)
        {
            _logger.LogWarning("Employee not found: {Id}", id);
            return NotFound();
        }

        employee.Id = id;
        await _employeeService.UpdateAsync(employee);
        _logger.LogInformation("Employee updated successfully: {Id}", id);
        return NoContent();
    }

    // DELETE: api/employee/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        var existingEmployee = await _employeeService.GetByIdAsync(id);
        if (existingEmployee == null)
        {
            _logger.LogWarning("Employee not found for deletion: {Id}", id);
            return NotFound();
        }

        await _employeeService.DeleteAsync(id);
        _logger.LogInformation("Employee deleted: {Id}", id);
        return NoContent();
    }
}
