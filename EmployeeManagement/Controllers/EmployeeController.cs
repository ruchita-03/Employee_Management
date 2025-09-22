using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Bson;

[ApiController]
[Route("api/[controller]")]
public class EmployeeController : ControllerBase
{
    private readonly IEmployeeService _employeeService;

    public EmployeeController(IEmployeeService employeeService)
    {
        _employeeService = employeeService;
    }

    // GET api/employee
    [HttpGet]
    public async Task<ActionResult<List<Employee>>> Get()
    {
        var employees = await _employeeService.GetAllAsync();
        return Ok(employees);
    }

    // GET api/employee/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<Employee>> Get(string id)
    {
        var employee = await _employeeService.GetByIdAsync(id);
        if (employee == null)
            return NotFound();
        return Ok(employee);
    }

    // POST api/employee
    [HttpPost]
    public async Task<IActionResult> Post([FromBody] Employee employee)
    {
        if (employee == null)
            return BadRequest("Employee data is required.");

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        // Validate Id format is a valid MongoDB ObjectId
        if (!ObjectId.TryParse(employee.Id, out _))
            return BadRequest("Invalid Id format.");

        await _employeeService.CreateAsync(employee);

        return CreatedAtAction(nameof(Get), new { id = employee.Id }, employee);
    }

    // PUT api/employee/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> Put(string id, [FromBody] Employee employee)
    {
        await _employeeService.UpdateAsync(id, employee);
        return NoContent();
    }

    // DELETE api/employee/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        await _employeeService.DeleteAsync(id);
        return NoContent();
    }
}
