using System;
using Xunit;

public class EmployeeModelTests
{
    // This test verifies that all the properties of the Employee model can be assigned and read accurately.
    [Fact]
    public void EmployeeProperties_ShouldBeSetCorrectly()
    {
        var employee = new Employee
        {
            Id = "123",
            Name = "John Doe",
            Department = "Finance",
            Email = "john.doe@example.com",
            DateOfJoining = DateTime.Now,
            JobTitle = "Accountant",
            Salary = 50000,
            IsActive = true
        };

        Assert.Equal("123", employee.Id);
        Assert.Equal("John Doe", employee.Name);
        Assert.Equal("Finance", employee.Department);
        Assert.Equal("john.doe@example.com", employee.Email);
        Assert.Equal("Accountant", employee.JobTitle);
        Assert.Equal(50000, employee.Salary);
        Assert.True(employee.IsActive);
    }

}
