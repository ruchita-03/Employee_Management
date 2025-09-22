using MongoDB.Driver;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Threading.Tasks;
using EmpManagement.Core;

public class Repository
{
    private readonly IMongoCollection<Employee> _employees;

    public Repository(IOptions<EmployeeStoreDBSettings> settings)
    {
        var client = new MongoClient(settings.Value.ConnectionString);
        var database = client.GetDatabase(settings.Value.DatabaseName);
        _employees = database.GetCollection<Employee>(settings.Value.EmployeeCollectionName);
    }

    public async Task<List<Employee>> GetAllEmployeesAsync() =>
        await _employees.Find(emp => true).ToListAsync();

    public async Task<Employee> GetEmployeeByIdAsync(string id) =>
        await _employees.Find(emp => emp.Id == id).FirstOrDefaultAsync();

    public async Task CreateEmployeeAsync(Employee employee) =>
        await _employees.InsertOneAsync(employee);

    public async Task UpdateEmployeeAsync(string id, Employee employee) =>
        await _employees.ReplaceOneAsync(emp => emp.Id == id, employee);

    public async Task DeleteEmployeeAsync(string id) =>
        await _employees.DeleteOneAsync(emp => emp.Id == id);
}
