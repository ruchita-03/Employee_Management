using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EmpManagement.Core;
using EmpManagement.INFRA;
using Microsoft.Extensions.Logging;
using Moq;
using MongoDB.Driver;
using Xunit;

namespace EmpManagement.INFRA.Tests
{
    public class RepositoryTests
    {
        private readonly Mock<IMongoCollection<Employee>> _mockCollection;
        private readonly Mock<ILogger<Repository>> _mockLogger;
        private readonly Repository _repository;
        private readonly Mock<IMongoDatabase> _mockDatabase;

        public RepositoryTests()
        {
            _mockCollection = new Mock<IMongoCollection<Employee>>();
            _mockLogger = new Mock<ILogger<Repository>>();
            _mockDatabase = new Mock<IMongoDatabase>();
            _mockDatabase.Setup(db => db.GetCollection<Employee>("Employees", null))
                .Returns(_mockCollection.Object);
            _repository = new Repository(_mockDatabase.Object, _mockLogger.Object);
        }

        // Helper to mock async cursor
        private IAsyncCursor<T> MockCursor<T>(IEnumerable<T> items)
        {
            var mockCursor = new Mock<IAsyncCursor<T>>();
            mockCursor.Setup(c => c.Current).Returns(items);
            mockCursor.SetupSequence(c => c.MoveNextAsync(It.IsAny<System.Threading.CancellationToken>()))
                .ReturnsAsync(true) // First call returns true to indicate items are available
                .ReturnsAsync(false); // Second call returns false to end iteration
            mockCursor.Setup(c => c.Dispose()).Verifiable();
            return mockCursor.Object;
        }

        // Check GetAllEmployeesAsync returns a list of all employees correctly
        [Fact]
        public async Task GetAllEmployeesAsync_ReturnsAllEmployees()
        {
            var employees = new List<Employee>
            {
                new Employee { Id = "1", Name = "John", Salary = 50000, Department = "IT", IsActive = true },
                new Employee { Id = "2", Name = "Jane", Salary = 60000, Department = "HR", IsActive = true }
            };
            var mockCursor = MockCursor(employees);
            _mockCollection.Setup(c => c.FindAsync(It.IsAny<FilterDefinition<Employee>>(), It.IsAny<FindOptions<Employee>>(), default))
                .ReturnsAsync(mockCursor);

            var result = await _repository.GetAllEmployeesAsync();

            Assert.Equal(2, result.Count);
            Assert.Equal("John", result[0].Name);
            Assert.Equal("Jane", result[1].Name);
            _mockLogger.VerifyLog(LogLevel.Information, Times.Exactly(2));
        }

        // Check GetAllEmployeesAsync returns an empty list when no employees exist
        [Fact]
        public async Task GetAllEmployeesAsync_EmptyCollection_ReturnsEmptyList()
        {
            var employees = new List<Employee>();
            var mockCursor = MockCursor(employees);
            _mockCollection.Setup(c => c.FindAsync(It.IsAny<FilterDefinition<Employee>>(), It.IsAny<FindOptions<Employee>>(), default))
                .ReturnsAsync(mockCursor);

            var result = await _repository.GetAllEmployeesAsync();

            Assert.Empty(result);
            _mockLogger.VerifyLog(LogLevel.Information, Times.Exactly(2));
        }

        // Verify GetEmployeeByIdAsync returns employee if it exists
        [Fact]
        public async Task GetEmployeeByIdAsync_ValidId_ReturnsEmployee()
        {
            var employee = new Employee { Id = "1", Name = "John", Salary = 50000, Department = "IT", IsActive = true };
            var mockCursor = MockCursor(new[] { employee });
            _mockCollection.Setup(c => c.FindAsync(It.Is<FilterDefinition<Employee>>(f => f == Builders<Employee>.Filter.Eq(e => e.Id, "1")), It.IsAny<FindOptions<Employee>>(), default))
                .ReturnsAsync(mockCursor);

            var result = await _repository.GetEmployeeByIdAsync("1");

            Assert.NotNull(result);
            Assert.Equal("John", result.Name);
            _mockLogger.VerifyLog(LogLevel.Information, Times.Exactly(2));
        }

        // Verify GetEmployeeByIdAsync returns null when no employee found
        [Fact]
        public async Task GetEmployeeByIdAsync_InvalidId_ReturnsNull()
        {
            var mockCursor = MockCursor(new Employee[] { });
            _mockCollection.Setup(c => c.FindAsync(It.Is<FilterDefinition<Employee>>(f => f == Builders<Employee>.Filter.Eq(e => e.Id, "999")), It.IsAny<FindOptions<Employee>>(), default))
                .ReturnsAsync(mockCursor);

            var result = await _repository.GetEmployeeByIdAsync("999");

            Assert.Null(result);
            _mockLogger.VerifyLog(LogLevel.Warning, Times.Once());
        }

        // Make sure CreateEmployeeAsync inserts new employee
        [Fact]
        public async Task CreateEmployeeAsync_InsertsEmployee()
        {
            var employee = new Employee { Id = "1", Name = "John", Salary = 50000, Department = "IT", IsActive = true };
            _mockCollection.Setup(c => c.InsertOneAsync(employee, null, default)).Returns(Task.CompletedTask);

            await _repository.CreateEmployeeAsync(employee);

            _mockCollection.Verify(c => c.InsertOneAsync(employee, null, default), Times.Once());
            _mockLogger.VerifyLog(LogLevel.Information, Times.Exactly(2));
        }

        // Confirm UpdateEmployeeAsync updates employee with valid ID
        [Fact]
        public async Task UpdateEmployeeAsync_ValidId_UpdatesEmployee()
        {
            var employee = new Employee { Id = "1", Name = "John Updated", Salary = 60000, Department = "IT", IsActive = true };
            var replaceResult = new ReplaceOneResult.Acknowledged(1, 1, null);
            _mockCollection.Setup(c => c.ReplaceOneAsync(It.Is<FilterDefinition<Employee>>(f => f == Builders<Employee>.Filter.Eq(e => e.Id, "1")), employee, It.IsAny<ReplaceOptions>(), default))
                .ReturnsAsync(replaceResult);

            await _repository.UpdateEmployeeAsync("1", employee);

            _mockCollection.Verify(c => c.ReplaceOneAsync(It.IsAny<FilterDefinition<Employee>>(), employee, It.IsAny<ReplaceOptions>(), default), Times.Once());
            _mockLogger.VerifyLog(LogLevel.Information, Times.Exactly(2));
        }

        // Verify UpdateEmployeeAsync logs warning with invalid ID
        [Fact]
        public async Task UpdateEmployeeAsync_InvalidId_LogsWarning()
        {
            var employee = new Employee { Id = "999", Name = "John", Salary = 50000, Department = "IT", IsActive = true };
            var replaceResult = new ReplaceOneResult.Acknowledged(0, 0, null);
            _mockCollection.Setup(c => c.ReplaceOneAsync(It.Is<FilterDefinition<Employee>>(f => f == Builders<Employee>.Filter.Eq(e => e.Id, "999")), employee, It.IsAny<ReplaceOptions>(), default))
                .ReturnsAsync(replaceResult);

            await _repository.UpdateEmployeeAsync("999", employee);

            _mockLogger.VerifyLog(LogLevel.Warning, Times.Once());
        }

        // Check DeleteEmployeeAsync deletes employee on valid ID
        [Fact]
        public async Task DeleteEmployeeAsync_ValidId_DeletesEmployee()
        {
            var deleteResult = new DeleteResult.Acknowledged(1);
            _mockCollection.Setup(c => c.DeleteOneAsync(It.Is<FilterDefinition<Employee>>(f => f == Builders<Employee>.Filter.Eq(e => e.Id, "1")), default))
                .ReturnsAsync(deleteResult);

            await _repository.DeleteEmployeeAsync("1");

            _mockCollection.Verify(c => c.DeleteOneAsync(It.IsAny<FilterDefinition<Employee>>(), default), Times.Once());
            _mockLogger.VerifyLog(LogLevel.Information, Times.Exactly(2));
        }

        // Confirm DeleteEmployeeAsync logs warning on invalid ID
        [Fact]
        public async Task DeleteEmployeeAsync_InvalidId_LogsWarning()
        {
            var deleteResult = new DeleteResult.Acknowledged(0);
            _mockCollection.Setup(c => c.DeleteOneAsync(It.Is<FilterDefinition<Employee>>(f => f == Builders<Employee>.Filter.Eq(e => e.Id, "999")), default))
                .ReturnsAsync(deleteResult);

            await _repository.DeleteEmployeeAsync("999");

            _mockLogger.VerifyLog(LogLevel.Warning, Times.Once());
        }

        // Test GetEmployeesBySalaryAsync returns employees with salary greater than threshold
        [Fact]
        public async Task GetEmployeesBySalaryAsync_GreaterThan_ReturnsMatchingEmployees()
        {
            var employees = new List<Employee>
            {
                new Employee { Id = "1", Name = "John", Salary = 60000, Department = "IT", IsActive = true },
                new Employee { Id = "2", Name = "Jane", Salary = 40000, Department = "HR", IsActive = true }
            };
            var mockCursor = MockCursor(employees.Where(e => e.Salary > 50000));
            _mockCollection.Setup(c => c.FindAsync(It.Is<FilterDefinition<Employee>>(f => f == Builders<Employee>.Filter.Gt(e => e.Salary, 50000)), It.IsAny<FindOptions<Employee>>(), default))
                .ReturnsAsync(mockCursor);

            var result = await _repository.GetEmployeesBySalaryAsync(50000, true);

            Assert.Single(result);
            Assert.Equal("John", result[0].Name);
            _mockLogger.VerifyLog(LogLevel.Information, Times.Exactly(2));
        }

        // Test GetEmployeesBySalaryAsync returns employees with salary less than or equal to threshold
        [Fact]
        public async Task GetEmployeesBySalaryAsync_LessThanOrEqual_ReturnsMatchingEmployees()
        {
            var employees = new List<Employee>
            {
                new Employee { Id = "1", Name = "John", Salary = 60000, Department = "IT", IsActive = true },
                new Employee { Id = "2", Name = "Jane", Salary = 40000, Department = "HR", IsActive = true }
            };
            var mockCursor = MockCursor(employees.Where(e => e.Salary <= 50000));
            _mockCollection.Setup(c => c.FindAsync(It.Is<FilterDefinition<Employee>>(f => f == Builders<Employee>.Filter.Lte(e => e.Salary, 50000)), It.IsAny<FindOptions<Employee>>(), default))
                .ReturnsAsync(mockCursor);

            var result = await _repository.GetEmployeesBySalaryAsync(50000, false);

            Assert.Single(result);
            Assert.Equal("Jane", result[0].Name);
            _mockLogger.VerifyLog(LogLevel.Information, Times.Exactly(2));
        }

        // Test GetEmployeesByNameAsync returns employees whose names contain the query string
        [Fact]
        public async Task GetEmployeesByNameAsync_ReturnsMatchingEmployees()
        {
            var employees = new List<Employee>
            {
                new Employee { Id = "1", Name = "John Doe", Salary = 50000, Department = "IT", IsActive = true },
                new Employee { Id = "2", Name = "Jane Doe", Salary = 60000, Department = "HR", IsActive = true }
            };
            var mockCursor = MockCursor(employees.Where(e => e.Name.Contains("John")));
            _mockCollection.Setup(c => c.FindAsync(It.Is<FilterDefinition<Employee>>(f => f.RenderToBsonDocument().ToString().Contains("John", StringComparison.OrdinalIgnoreCase)), It.IsAny<FindOptions<Employee>>(), default))
                .ReturnsAsync(mockCursor);

            var result = await _repository.GetEmployeesByNameAsync("John");

            Assert.Single(result);
            Assert.Equal("John Doe", result[0].Name);
            _mockLogger.VerifyLog(LogLevel.Information, Times.Exactly(2));
        }

        // Test GetInactiveEmployeesAsync returns only inactive employees
        [Fact]
        public async Task GetInactiveEmployeesAsync_ReturnsInactiveEmployees()
        {
            var employees = new List<Employee>
            {
                new Employee { Id = "1", Name = "John", Salary = 50000, Department = "IT", IsActive = false },
                new Employee { Id = "2", Name = "Jane", Salary = 60000, Department = "HR", IsActive = true }
            };
            var mockCursor = MockCursor(employees.Where(e => !e.IsActive));
            _mockCollection.Setup(c => c.FindAsync(It.Is<FilterDefinition<Employee>>(f => f == Builders<Employee>.Filter.Eq(e => e.IsActive, false)), It.IsAny<FindOptions<Employee>>(), default))
                .ReturnsAsync(mockCursor);

            var result = await _repository.GetInactiveEmployeesAsync();

            Assert.Single(result);
            Assert.Equal("John", result[0].Name);
            _mockLogger.VerifyLog(LogLevel.Information, Times.Exactly(2));
        }

        // Test GetEmployeesByDepartmentAsync returns employees from the specified department only
        [Fact]
        public async Task GetEmployeesByDepartmentAsync_ReturnsMatchingEmployees()
        {
            var employees = new List<Employee>
            {
                new Employee { Id = "1", Name = "John", Salary = 50000, Department = "IT", IsActive = true },
                new Employee { Id = "2", Name = "Jane", Salary = 60000, Department = "HR", IsActive = true }
            };
            var mockCursor = MockCursor(employees.Where(e => e.Department == "IT"));
            _mockCollection.Setup(c => c.FindAsync(It.Is<FilterDefinition<Employee>>(f => f == Builders<Employee>.Filter.Eq(e => e.Department, "IT")), It.IsAny<FindOptions<Employee>>(), default))
                .ReturnsAsync(mockCursor);

            var result = await _repository.GetEmployeesByDepartmentAsync("IT");

            Assert.Single(result);
            Assert.Equal("John", result[0].Name);
            _mockLogger.VerifyLog(LogLevel.Information, Times.Exactly(2));
        }
    }

    // Helper extension for verifying logger calls
    public static class LoggerExtensions
    {
        public static void VerifyLog<T>(this Mock<ILogger<T>> logger, LogLevel level, Times times)
        {
            logger.Verify(
                x => x.Log(
                    It.Is<LogLevel>(l => l == level),
                    It.IsAny<EventId>(),
                    It.IsAny<It.IsAnyType>(),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                times);
        }
    }
}
