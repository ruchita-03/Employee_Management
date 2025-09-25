using EmployeeManagement.Services;
using EmpManagement.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EmployeeManagement.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // JWT required for all endpoints
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
        [Authorize(Roles = "Admin,Moderator,ReadOnly")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetAll()
        {
            _logger.LogInformation("Fetching all employees");
            try
            {
                var employees = await _employeeService.GetAllAsync();
                _logger.LogInformation("Fetched {Count} employees", employees.Count);
                return Ok(employees);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching all employees");
                return StatusCode(500, "Internal server error");
            }
        }

        // GET: api/employee/{id}
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,ReadOnly")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetById(string id)
        {
            _logger.LogInformation("Fetching employee with Id: {Id}", id);
            try
            {
                var employee = await _employeeService.GetByIdAsync(id);
                if (employee == null)
                {
                    _logger.LogWarning("Employee not found: {Id}", id);
                    return NotFound();
                }

                _logger.LogInformation("Employee found: {Name}", employee.Name);
                return Ok(employee);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching employee: {Id}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        // POST: api/employee
        [HttpPost]
        [Authorize(Roles = "Admin,Moderator")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Create([FromBody] Employee employee)
        {
            if (employee == null)
            {
                _logger.LogWarning("Attempted to create a null employee");
                return BadRequest();
            }

            _logger.LogInformation("Creating a new employee: {Name}", employee.Name);
            try
            {
                var createdEmployee = await _employeeService.AddAsync(employee);
                if (createdEmployee == null)
                {
                    _logger.LogError("Failed to create employee");
                    return BadRequest("Could not create employee");
                }

                _logger.LogInformation("Employee created successfully with Id: {Id}", createdEmployee.Id);
                return CreatedAtAction(nameof(GetById), new { id = createdEmployee.Id }, createdEmployee);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating employee: {Name}", employee.Name);
                return StatusCode(500, "Internal server error");
            }
        }

        // PUT: api/employee/{id}
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Moderator")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Update(string id, [FromBody] Employee employee)
        {
            if (employee == null)
            {
                _logger.LogWarning("Null employee update attempted");
                return BadRequest();
            }

            try
            {
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating employee: {Id}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        // DELETE: api/employee/{id}
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Delete(string id)
        {
            try
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting employee: {Id}", id);
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
