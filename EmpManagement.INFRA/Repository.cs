using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;
using EmpManagement.Core;
using EmpManagement.INFRA;
using Microsoft.Extensions.Logging;

public class Repository : IRepository
{
    private readonly IMongoCollection<Employee> _employees;
    private ILogger<Repository> @object;

    public Repository(IMongoDatabase database)
    {
        _employees = database.GetCollection<Employee>("Employees");
    }

    public Repository(IMongoDatabase database, ILogger<Repository> @object) : this(database)
    {
        this.@object = @object;
    }

    public async Task<List<Employee>> GetAllEmployeesAsync()
    {
        return await _employees.Find(_ => true).ToListAsync();
    }

    public async Task<Employee> GetEmployeeByIdAsync(string id)
    {
        return await _employees.Find(e => e.Id == id).FirstOrDefaultAsync();
    }

    public async Task<Employee> CreateEmployeeAsync(Employee employee)
    {
        await _employees.InsertOneAsync(employee);
        return employee;
    }

    public async Task UpdateEmployeeAsync(string id, Employee employee)
    {
        await _employees.ReplaceOneAsync(e => e.Id == id, employee);
    }

    public async Task DeleteEmployeeAsync(string id)
    {
        await _employees.DeleteOneAsync(e => e.Id == id);
    }

    public Task<IEnumerable<Employee>> GetAll()
    {
        throw new NotImplementedException();
    }

    public Task<List<Employee>> GetInactiveEmployeesAsync()
    {
        throw new NotImplementedException();
    }

    public Task<List<Employee>> GetEmployeesByDepartmentAsync(string department)
    {
        throw new NotImplementedException();
    }

    public Task<List<Employee>> GetEmployeesBySalaryAsync(double salary, bool includeEqual)
    {
        throw new NotImplementedException();
    }

    public Task<List<Employee>> GetEmployeesByNameAsync(string name)
    {
        throw new NotImplementedException();
    }
}
