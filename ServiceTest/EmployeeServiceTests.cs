using EmployeeManagement.Services;
using EmpManagement.Core;
using EmpManagement.INFRA;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace EmployeeManagement.Services.Tests
{
    public class EmployeeServiceTests
    {
        private readonly Mock<IRepository> _mockRepository;
        private readonly EmployeeService _service;

        public EmployeeServiceTests()
        {
            _mockRepository = new Mock<IRepository>();
            _service = new EmployeeService(_mockRepository.Object);
        }

        [Fact]
        // Tests that the EmployeeService constructor throws ArgumentNullException if the repository dependency is null
        public void Constructor_WithNullRepository_ThrowsArgumentNullException()
        {
            var exception = Assert.Throws<ArgumentNullException>(() => new EmployeeService(null));
            Assert.Equal("repository", exception.ParamName);
        }

        [Fact]
        // Tests that GetAllAsync returns an empty list when the underlying repository returns no employees
        public async Task GetAllAsync_WithEmptyRepository_ReturnsEmptyList()
        {
            _mockRepository.Setup(x => x.GetAllEmployeesAsync()).ReturnsAsync(new List<Employee>());

            var result = await _service.GetAllAsync();

            Assert.Empty(result);
            _mockRepository.Verify(x => x.GetAllEmployeesAsync(), Times.Once);
        }

        [Fact]
        // Tests that GetByIdAsync throws ArgumentException if passed a null ID and does not call repository method
        public async Task GetByIdAsync_WithNullId_ThrowsArgumentException()
        {
            await Assert.ThrowsAsync<ArgumentException>(() => _service.GetByIdAsync(null));
            _mockRepository.Verify(x => x.GetEmployeeByIdAsync(It.IsAny<string>()), Times.Never);
        }

        [Fact]
        // Tests that GetByIdAsync throws ArgumentException if passed an empty ID string and does not call repository method
        public async Task GetByIdAsync_WithEmptyId_ThrowsArgumentException()
        {
            await Assert.ThrowsAsync<ArgumentException>(() => _service.GetByIdAsync(""));
            _mockRepository.Verify(x => x.GetEmployeeByIdAsync(It.IsAny<string>()), Times.Never);
        }

        [Fact]
        // Tests that GetByIdAsync returns the employee object when given a valid ID and repository returns matching employee
        public async Task GetByIdAsync_WithValidId_ReturnsEmployee()
        {
            var employee = new Employee { Id = "1", Name = "Test" };
            _mockRepository.Setup(x => x.GetEmployeeByIdAsync("1")).ReturnsAsync(employee);

            var result = await _service.GetByIdAsync("1");

            Assert.Equal(employee, result);
            _mockRepository.Verify(x => x.GetEmployeeByIdAsync("1"), Times.Once);
        }

        [Fact]
        // Tests that AddAsync throws ArgumentNullException if the employee to add is null and does not call repository
        public async Task AddAsync_WithNullEmployee_ThrowsArgumentNullException()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(() => _service.AddAsync(null));
            _mockRepository.Verify(x => x.CreateEmployeeAsync(It.IsAny<Employee>()), Times.Never);
        }

        [Fact]
        // Tests that AddAsync returns the added employee when repository successfully creates it
        public async Task AddAsync_WithValidEmployee_CallsRepositoryCreate()
        {
            var employee = new Employee { Id = "1", Name = "Test" };
            _mockRepository.Setup(x => x.CreateEmployeeAsync(employee)).ReturnsAsync(employee);

            var result = await _service.AddAsync(employee);

            Assert.Equal(employee, result);
            _mockRepository.Verify(x => x.CreateEmployeeAsync(employee), Times.Once);
        }

        [Fact]
        // Tests that UpdateAsync throws ArgumentNullException if employee to update is null and does not call repository
        public async Task UpdateAsync_WithNullEmployee_ThrowsArgumentNullException()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(() => _service.UpdateAsync(null));
            _mockRepository.Verify(x => x.UpdateEmployeeAsync(It.IsAny<string>(), It.IsAny<Employee>()), Times.Never);
        }

        [Fact]
        // Tests that UpdateAsync throws ArgumentException if employee ID is empty and does not call repository
        public async Task UpdateAsync_WithEmptyIdEmployee_ThrowsArgumentException()
        {
            var employee = new Employee { Id = "", Name = "Test" };
            await Assert.ThrowsAsync<ArgumentException>(() => _service.UpdateAsync(employee));
            _mockRepository.Verify(x => x.UpdateEmployeeAsync(It.IsAny<string>(), It.IsAny<Employee>()), Times.Never);
        }

        [Fact]
        // Tests that DeleteAsync throws ArgumentException if given a null ID and does not call repository
        public async Task DeleteAsync_WithNullId_ThrowsArgumentException()
        {
            await Assert.ThrowsAsync<ArgumentException>(() => _service.DeleteAsync(null));
            _mockRepository.Verify(x => x.DeleteEmployeeAsync(It.IsAny<string>()), Times.Never);
        }

        [Fact]
        // Tests that DeleteAsync throws ArgumentException if given an empty ID and does not call repository
        public async Task DeleteAsync_WithEmptyId_ThrowsArgumentException()
        {
            await Assert.ThrowsAsync<ArgumentException>(() => _service.DeleteAsync(""));
            _mockRepository.Verify(x => x.DeleteEmployeeAsync(It.IsAny<string>()), Times.Never);
        }

        [Fact]
        // Tests that DeleteAsync calls repository to delete the employee when given a valid ID
        public async Task DeleteAsync_WithValidId_CallsRepositoryDelete()
        {
            _mockRepository.Setup(x => x.DeleteEmployeeAsync("1")).Returns(Task.CompletedTask);

            await _service.DeleteAsync("1");

            _mockRepository.Verify(x => x.DeleteEmployeeAsync("1"), Times.Once);
        }

        [Fact]
        // Tests that GetInactiveEmployeesAsync returns an empty list when the repository returns no inactive employees
        public async Task GetInactiveEmployeesAsync_CallsRepository()
        {
            _mockRepository.Setup(x => x.GetInactiveEmployeesAsync()).ReturnsAsync(new List<Employee>());

            var result = await _service.GetInactiveEmployeesAsync();

            Assert.Empty(result);
            _mockRepository.Verify(x => x.GetInactiveEmployeesAsync(), Times.Once);
        }

        [Fact]
        // Tests that GetEmployeesByDepartmentAsync throws ArgumentException if department parameter is null and does not call repository
        public async Task GetEmployeesByDepartmentAsync_WithNullDepartment_ThrowsArgumentException()
        {
            await Assert.ThrowsAsync<ArgumentException>(() => _service.GetEmployeesByDepartmentAsync(null));
            _mockRepository.Verify(x => x.GetEmployeesByDepartmentAsync(It.IsAny<string>()), Times.Never);
        }

        [Fact]
        // Tests that GetEmployeesByDepartmentAsync throws ArgumentException if department parameter is empty and does not call repository
        public async Task GetEmployeesByDepartmentAsync_WithEmptyDepartment_ThrowsArgumentException()
        {
            await Assert.ThrowsAsync<ArgumentException>(() => _service.GetEmployeesByDepartmentAsync(""));
            _mockRepository.Verify(x => x.GetEmployeesByDepartmentAsync(It.IsAny<string>()), Times.Never);
        }

        [Fact]
        // Tests that GetEmployeesBySalaryAsync throws ArgumentException if salary parameter is negative and does not call repository
        public async Task GetEmployeesBySalaryAsync_WithNegativeSalary_ThrowsArgumentException()
        {
            await Assert.ThrowsAsync<ArgumentException>(() => _service.GetEmployeesBySalaryAsync(-1, true));
            _mockRepository.Verify(x => x.GetEmployeesBySalaryAsync(It.IsAny<double>(), It.IsAny<bool>()), Times.Never);
        }

        [Fact]
        // Tests that GetEmployeesByNameAsync throws ArgumentException if name parameter is null and does not call repository
        public async Task GetEmployeesByNameAsync_WithNullName_ThrowsArgumentException()
        {
            await Assert.ThrowsAsync<ArgumentException>(() => _service.GetEmployeesByNameAsync(null));
            _mockRepository.Verify(x => x.GetEmployeesByNameAsync(It.IsAny<string>()), Times.Never);
        }

        [Fact]
        // Tests that GetEmployeesByNameAsync throws ArgumentException if name parameter is empty and does not call repository
        public async Task GetEmployeesByNameAsync_WithEmptyName_ThrowsArgumentException()
        {
            await Assert.ThrowsAsync<ArgumentException>(() => _service.GetEmployeesByNameAsync(""));
            _mockRepository.Verify(x => x.GetEmployeesByNameAsync(It.IsAny<string>()), Times.Never);
        }

    }
}