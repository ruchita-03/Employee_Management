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
            mockCursor.SetupSequence(c => c.MoveNextAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(true) // First call returns true to indicate items are available
                .ReturnsAsync(false); // Second call returns false to end iteration
            mockCursor.Setup(c => c.Dispose()).Verifiable();
            return mockCursor.Object;
        }

        [Fact]
        public async Task GetAllEmployeesAsync_ReturnsAllEmployees()
        {
            // Arrange
            var employees = new List<Employee>
            {
                new Employee { Id = "1", Name = "John", Salary = 50000, Department = "IT", IsActive = true },
                new Employee { Id = "2", Name = "Jane", Salary = 60000, Department = "HR", IsActive = true }
            };
            var mockCursor = MockCursor(employees);
            _mockCollection.Setup(c => c.FindAsync(It.IsAny<FilterDefinition<Employee>>(), It.IsAny<FindOptions<Employee>>(), default))
                .ReturnsAsync(mockCursor);

            // Act
            var result = await _repository.GetAllEmployeesAsync();

            // Assert
            Assert.Equal(2, result.Count);
            Assert.Equal("John", result[0].Name);
            Assert.Equal("Jane", result[1].Name);
            _mockLogger.VerifyLog(LogLevel.Information, Times.Exactly(2));
        }

        [Fact]
        public async Task GetAllEmployeesAsync_EmptyCollection_ReturnsEmptyList()
        {
            // Arrange
            var employees = new List<Employee>();
            var mockCursor = MockCursor(employees);
            _mockCollection.Setup(c => c.FindAsync(It.IsAny<FilterDefinition<Employee>>(), It.IsAny<FindOptions<Employee>>(), default))
                .ReturnsAsync(mockCursor);

            // Act
            var result = await _repository.GetAllEmployeesAsync();

            // Assert
            Assert.Empty(result);
            _mockLogger.VerifyLog(LogLevel.Information, Times.Exactly(2));
        }

        [Fact]
        public async Task GetEmployeeByIdAsync_ValidId_ReturnsEmployee()
        {
            // Arrange
            var employee = new Employee { Id = "1", Name = "John", Salary = 50000, Department = "IT", IsActive = true };
            var mockCursor = MockCursor(new[] { employee });
            _mockCollection.Setup(c => c.FindAsync(It.Is<FilterDefinition<Employee>>(f => f == Builders<Employee>.Filter.Eq(e => e.Id, "1")), It.IsAny<FindOptions<Employee>>(), default))
                .ReturnsAsync(mockCursor);

            // Act
            var result = await _repository.GetEmployeeByIdAsync("1");

            // Assert
            Assert.NotNull(result);
            Assert.Equal("John", result.Name);
            _mockLogger.VerifyLog(LogLevel.Information, Times.Exactly(2));
        }

        [Fact]
        public async Task GetEmployeeByIdAsync_InvalidId_ReturnsNull()
        {
            // Arrange
            var mockCursor = MockCursor(new Employee[] { });
            _mockCollection.Setup(c => c.FindAsync(It.Is<FilterDefinition<Employee>>(f => f == Builders<Employee>.Filter.Eq(e => e.Id, "999")), It.IsAny<FindOptions<Employee>>(), default))
                .ReturnsAsync(mockCursor);

            // Act
            var result = await _repository.GetEmployeeByIdAsync("999");

            // Assert
            Assert.Null(result);
            _mockLogger.VerifyLog(LogLevel.Warning, Times.Once());
        }

        [Fact]
        public async Task CreateEmployeeAsync_InsertsEmployee()
        {
            // Arrange
            var employee = new Employee { Id = "1", Name = "John", Salary = 50000, Department = "IT", IsActive = true };
            _mockCollection.Setup(c => c.InsertOneAsync(employee, null, default)).Returns(Task.CompletedTask);

            // Act
            await _repository.CreateEmployeeAsync(employee);

            // Assert
            _mockCollection.Verify(c => c.InsertOneAsync(employee, null, default), Times.Once());
            _mockLogger.VerifyLog(LogLevel.Information, Times.Exactly(2));
        }

        [Fact]
        public async Task UpdateEmployeeAsync_ValidId_UpdatesEmployee()
        {
            // Arrange
            var employee = new Employee { Id = "1", Name = "John Updated", Salary = 60000, Department = "IT", IsActive = true };
            var replaceResult = new ReplaceOneResult.Acknowledged(1, 1, null);
            _mockCollection.Setup(c => c.ReplaceOneAsync(It.Is<FilterDefinition<Employee>>(f => f == Builders<Employee>.Filter.Eq(e => e.Id, "1")), employee, It.IsAny<ReplaceOptions>(), default))
                .ReturnsAsync(replaceResult);

            // Act
            await _repository.UpdateEmployeeAsync("1", employee);

            // Assert
            _mockCollection.Verify(c => c.ReplaceOneAsync(It.IsAny<FilterDefinition<Employee>>(), employee, It.IsAny<ReplaceOptions>(), default), Times.Once());
            _mockLogger.VerifyLog(LogLevel.Information, Times.Exactly(2));
        }

        [Fact]
        public async Task UpdateEmployeeAsync_InvalidId_LogsWarning()
        {
            // Arrange
            var employee = new Employee { Id = "999", Name = "John", Salary = 50000, Department = "IT", IsActive = true };
            var replaceResult = new ReplaceOneResult.Acknowledged(0, 0, null);
            _mockCollection.Setup(c => c.ReplaceOneAsync(It.Is<FilterDefinition<Employee>>(f => f == Builders<Employee>.Filter.Eq(e => e.Id, "999")), employee, It.IsAny<ReplaceOptions>(), default))
                .ReturnsAsync(replaceResult);

            // Act
            await _repository.UpdateEmployeeAsync("999", employee);

            // Assert
            _mockLogger.VerifyLog(LogLevel.Warning, Times.Once());
        }

        [Fact]
        public async Task DeleteEmployeeAsync_ValidId_DeletesEmployee()
        {
            // Arrange
            var deleteResult = new DeleteResult.Acknowledged(1);
            _mockCollection.Setup(c => c.DeleteOneAsync(It.Is<FilterDefinition<Employee>>(f => f == Builders<Employee>.Filter.Eq(e => e.Id, "1")), default))
                .ReturnsAsync(deleteResult);

            // Act
            await _repository.DeleteEmployeeAsync("1");

            // Assert
            _mockCollection.Verify(c => c.DeleteOneAsync(It.IsAny<FilterDefinition<Employee>>(), default), Times.Once());
            _mockLogger.VerifyLog(LogLevel.Information, Times.Exactly(2));
        }

        [Fact]
        public async Task DeleteEmployeeAsync_InvalidId_LogsWarning()
        {
            // Arrange
            var deleteResult = new DeleteResult.Acknowledged(0);
            _mockCollection.Setup(c => c.DeleteOneAsync(It.Is<FilterDefinition<Employee>>(f => f == Builders<Employee>.Filter.Eq(e => e.Id, "999")), default))
                .ReturnsAsync(deleteResult);

            // Act
            await _repository.DeleteEmployeeAsync("999");

            // Assert
            _mockLogger.VerifyLog(LogLevel.Warning, Times.Once());
        }

        [Fact]
        public async Task GetEmployeesBySalaryAsync_GreaterThan_ReturnsMatchingEmployees()
        {
            // Arrange
            var employees = new List<Employee>
            {
                new Employee { Id = "1", Name = "John", Salary = 60000, Department = "IT", IsActive = true },
                new Employee { Id = "2", Name = "Jane", Salary = 40000, Department = "HR", IsActive = true }
            };
            var mockCursor = MockCursor(employees.Where(e => e.Salary > 50000));
            _mockCollection.Setup(c => c.FindAsync(It.Is<FilterDefinition<Employee>>(f => f == Builders<Employee>.Filter.Gt(e => e.Salary, 50000)), It.IsAny<FindOptions<Employee>>(), default))
                .ReturnsAsync(mockCursor);

            // Act
            var result = await _repository.GetEmployeesBySalaryAsync(50000, true);

            // Assert
            Assert.Single(result);
            Assert.Equal("John", result[0].Name);
            _mockLogger.VerifyLog(LogLevel.Information, Times.Exactly(2));
        }

        [Fact]
        public async Task GetEmployeesBySalaryAsync_LessThanOrEqual_ReturnsMatchingEmployees()
        {
            // Arrange
            var employees = new List<Employee>
            {
                new Employee { Id = "1", Name = "John", Salary = 60000, Department = "IT", IsActive = true },
                new Employee { Id = "2", Name = "Jane", Salary = 40000, Department = "HR", IsActive = true }
            };
            var mockCursor = MockCursor(employees.Where(e => e.Salary <= 50000));
            _mockCollection.Setup(c => c.FindAsync(It.Is<FilterDefinition<Employee>>(f => f == Builders<Employee>.Filter.Lte(e => e.Salary, 50000)), It.IsAny<FindOptions<Employee>>(), default))
                .ReturnsAsync(mockCursor);

            // Act
            var result = await _repository.GetEmployeesBySalaryAsync(50000, false);

            // Assert
            Assert.Single(result);
            Assert.Equal("Jane", result[0].Name);
            _mockLogger.VerifyLog(LogLevel.Information, Times.Exactly(2));
        }

        [Fact]
        public async Task GetEmployeesByNameAsync_ReturnsMatchingEmployees()
        {
            // Arrange
            var employees = new List<Employee>
            {
                new Employee { Id = "1", Name = "John Doe", Salary = 50000, Department = "IT", IsActive = true },
                new Employee { Id = "2", Name = "Jane Doe", Salary = 60000, Department = "HR", IsActive = true }
            };
            var mockCursor = MockCursor(employees.Where(e => e.Name.Contains("John")));
            _mockCollection.Setup(c => c.FindAsync(It.Is<FilterDefinition<Employee>>(f => f.RenderToBsonDocument().ToString().Contains("John", StringComparison.OrdinalIgnoreCase)), It.IsAny<FindOptions<Employee>>(), default))
                .ReturnsAsync(mockCursor);

            // Act
            var result = await _repository.GetEmployeesByNameAsync("John");

            // Assert
            Assert.Single(result);
            Assert.Equal("John Doe", result[0].Name);
            _mockLogger.VerifyLog(LogLevel.Information, Times.Exactly(2));
        }

        [Fact]
        public async Task GetInactiveEmployeesAsync_ReturnsInactiveEmployees()
        {
            // Arrange
            var employees = new List<Employee>
            {
                new Employee { Id = "1", Name = "John", Salary = 50000, Department = "IT", IsActive = false },
                new Employee { Id = "2", Name = "Jane", Salary = 60000, Department = "HR", IsActive = true }
            };
            var mockCursor = MockCursor(employees.Where(e => !e.IsActive));
            _mockCollection.Setup(c => c.FindAsync(It.Is<FilterDefinition<Employee>>(f => f == Builders<Employee>.Filter.Eq(e => e.IsActive, false)), It.IsAny<FindOptions<Employee>>(), default))
                .ReturnsAsync(mockCursor);

            // Act
            var result = await _repository.GetInactiveEmployeesAsync();

            // Assert
            Assert.Single(result);
            Assert.Equal("John", result[0].Name);
            _mockLogger.VerifyLog(LogLevel.Information, Times.Exactly(2));
        }

        [Fact]
        public async Task GetEmployeesByDepartmentAsync_ReturnsMatchingEmployees()
        {
            // Arrange
            var employees = new List<Employee>
            {
                new Employee { Id = "1", Name = "John", Salary = 50000, Department = "IT", IsActive = true },
                new Employee { Id = "2", Name = "Jane", Salary = 60000, Department = "HR", IsActive = true }
            };
            var mockCursor = MockCursor(employees.Where(e => e.Department == "IT"));
            _mockCollection.Setup(c => c.FindAsync(It.Is<FilterDefinition<Employee>>(f => f == Builders<Employee>.Filter.Eq(e => e.Department, "IT")), It.IsAny<FindOptions<Employee>>(), default))
                .ReturnsAsync(mockCursor);

            // Act
            var result = await _repository.GetEmployeesByDepartmentAsync("IT");

            // Assert
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