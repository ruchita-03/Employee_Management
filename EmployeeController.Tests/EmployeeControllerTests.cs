using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using EmpManagement.Core;
using EmployeeManagement.Services;

namespace EmployeeManagement.Services.Tests
{
    public class EmployeeControllerTests
    {
        // Mock of the employee service interface used to simulate data operations
        private readonly Mock<IEmployeeService> _mockService;
        // The controller instance that is being tested, with mocked dependencies injected
        private readonly EmployeeController _controller;

        public EmployeeControllerTests()
        {
            _mockService = new Mock<IEmployeeService>();
            // Inject mock service and a mock logger into the controller
            _controller = new EmployeeController(_mockService.Object, Mock.Of<ILogger<EmployeeController>>());
        }

        [Fact]
        // Test that GetAll method returns an HTTP 200 OK response with a list of employees
        // It mocks the service to return one employee and verifies the controller returns that employee as expected
        public async Task GetAll_ReturnsOkWithEmployees()
        {
            var employees = new List<Employee> { new Employee { Id = "1", Name = "User" } };
            _mockService.Setup(s => s.GetAllAsync()).ReturnsAsync(employees);

            var result = await _controller.GetAll();

            // Check if the result is HTTP 200 OK
            var okResult = Assert.IsType<OkObjectResult>(result);
            // Check if the returned value is a list of employees
            var returnEmployees = Assert.IsAssignableFrom<List<Employee>>(okResult.Value);
            // Assert the list has exactly one employee
            Assert.Single(returnEmployees);
            // Assert the employee name matches the mocked data
            Assert.Equal("User", returnEmployees[0].Name);
        }

        [Fact]
        // Test that GetById returns HTTP 200 OK with the employee data when an existing ID is requested
        public async Task GetById_ExistingId_ReturnsOk()
        {
            var employee = new Employee { Id = "1", Name = "User" };
            _mockService.Setup(s => s.GetByIdAsync("1")).ReturnsAsync(employee);

            var result = await _controller.GetById("1");

            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnEmployee = Assert.IsType<Employee>(okResult.Value);
            Assert.Equal("User", returnEmployee.Name);
        }

        [Fact]
        // Test that GetById returns HTTP 404 Not Found when a non-existing ID is requested
        public async Task GetById_NonExistingId_ReturnsNotFound()
        {
            _mockService.Setup(s => s.GetByIdAsync("999")).ReturnsAsync((Employee)null);

            var result = await _controller.GetById("999");

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        // Test that Create adds a valid employee and returns HTTP 201 CreatedAtAction pointing to the new employee
        public async Task Create_ValidEmployee_ReturnsCreatedAtAction()
        {
            var employee = new Employee { Id = "1", Name = "User" };
            _mockService.Setup(s => s.AddAsync(employee)).ReturnsAsync(employee);

            var result = await _controller.Create(employee);

            var createdAtResult = Assert.IsType<CreatedAtActionResult>(result);
            // Check if the action name used for the location header is "GetById"
            Assert.Equal("GetById", createdAtResult.ActionName);
            // Verify the employee object returned matches the input
            Assert.Equal(employee, createdAtResult.Value);
            // Check if the route value "id" matches the employee's ID
            Assert.Equal(employee.Id, createdAtResult.RouteValues["id"]);
        }

        [Fact]
        // Test that Create returns HTTP 400 Bad Request when a null employee is passed
        public async Task Create_NullEmployee_ReturnsBadRequest()
        {
            var result = await _controller.Create(null);

            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        // Test that Create returns BadRequest if the service fails to add the employee (returns null)
        public async Task Create_FailedService_ReturnsBadRequest()
        {
            var employee = new Employee { Id = "1", Name = "User" };
            _mockService.Setup(s => s.AddAsync(employee)).ReturnsAsync((Employee)null);

            var result = await _controller.Create(employee);

            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        // Test that Update returns HTTP 204 No Content if updating an existing employee succeeds
        public async Task Update_ExistingId_ReturnsNoContent()
        {
            var employee = new Employee { Id = "1", Name = "Updated User" };
            _mockService.Setup(s => s.GetByIdAsync("1")).ReturnsAsync(new Employee { Id = "1", Name = "Old User" });
            _mockService.Setup(s => s.UpdateAsync(employee)).Returns(Task.CompletedTask);

            var result = await _controller.Update("1", employee);

            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        // Test that Update returns BadRequest when the input employee is null
        public async Task Update_NullEmployee_ReturnsBadRequest()
        {
            _mockService.Setup(s => s.GetByIdAsync("1")).ReturnsAsync(new Employee { Id = "1", Name = "User" });

            var result = await _controller.Update("1", null);

            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        // Test that Update returns NotFound when trying to update a non-existing employee
        public async Task Update_NonExistingId_ReturnsNotFound()
        {
            var employee = new Employee { Id = "1", Name = "Updated User" };
            _mockService.Setup(s => s.GetByIdAsync("1")).ReturnsAsync((Employee)null);

            var result = await _controller.Update("1", employee);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        // Test that Delete returns No Content if deletion of an existing employee succeeds
        public async Task Delete_ExistingId_ReturnsNoContent()
        {
            _mockService.Setup(s => s.GetByIdAsync("1")).ReturnsAsync(new Employee { Id = "1", Name = "User" });
            _mockService.Setup(s => s.DeleteAsync("1")).Returns(Task.CompletedTask);

            var result = await _controller.Delete("1");

            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        // Test that Delete returns NotFound when attempting to delete a non-existing employee
        public async Task Delete_NonExistingId_ReturnsNotFound()
        {
            _mockService.Setup(s => s.GetByIdAsync("999")).ReturnsAsync((Employee)null);

            var result = await _controller.Delete("999");

            Assert.IsType<NotFoundResult>(result);
        }
    }

}
