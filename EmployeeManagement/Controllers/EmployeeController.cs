using EmployeeManagement.Services;
using EmpManagement.Core;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController(IEmployeeService employeeService) : ControllerBase
    {
        private readonly IEmployeeService _employeeService = employeeService;

        // GET: api/<EmployeeController>
        [HttpGet]
        public ActionResult<List<Employee>> Get()
        {
            return Ok(_employeeService.Get());
        }

        // GET api/<EmployeeController>/5
        [HttpGet("{id}")]
        public ActionResult<Employee> Get(string id)
        {
            var employee = _employeeService.Get(id);

            if (employee == null)
            {
                return NotFound($"Employee with Id = {id} not found");
            }

            return Ok(employee);
        }

        // POST api/<EmployeeController>
        [HttpPost]
        public ActionResult<Employee> Post([FromBody] Employee employee)
        {
            _employeeService.Create(employee);
            return CreatedAtAction(nameof(Get), new { id = employee.ID }, employee);
        }

        // PUT api/<EmployeeController>/5
        [HttpPut("{id}")]
        public ActionResult Put(string id, [FromBody] Employee employee)
        {
            var existingEmployee = _employeeService.Get(id);

            if (existingEmployee == null)
            {
                return NotFound($"Employee with Id = {id} not found");
            }

            _employeeService.Update(id, employee);
            return Ok(employee);
        }

        // DELETE api/<EmployeeController>/5
        [HttpDelete("{id}")]
        public ActionResult Delete(string id)
        {
            var employee = _employeeService.Get(id);

            if (employee == null)
            {
                return NotFound($"Employee with Id = {id} not found");
            }

            _employeeService.Remove(employee.ID);
            return Ok($"Employee with Id = {id} deleted");
        }
    }
}
