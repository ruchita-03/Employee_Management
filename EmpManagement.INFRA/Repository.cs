using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;
using EmpManagement.Core;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using System.Linq;

namespace EmpManagement.INFRA
{
    public class Repository : IRepository
    {
        private readonly IMongoCollection<Employee> _employees;
        private readonly ILogger<Repository> _logger;
        private readonly IMongoCollection<User> _users;



        // single ctor with logger — DI will provide ILogger<Repository>
        public Repository(IMongoDatabase database, ILogger<Repository> logger)
        {
            _employees = database.GetCollection<Employee>("Employees");
            _logger = logger;
            _users = database.GetCollection<User>("Users");
        }

        // optional ctor without logger (keeps compatibility if tests/mock DI expect it)
        public Repository(IMongoDatabase database) : this(database, null) { }

        public async Task<List<Employee>> GetAllEmployeesAsync()
        {
            _logger?.LogInformation("Fetching all employees from DB");
            var list = await _employees.Find(FilterDefinition<Employee>.Empty).ToListAsync();
            _logger?.LogInformation("Fetched {Count} employees", list.Count);
            return list;
        }

        public async Task<Employee> GetEmployeeByIdAsync(string id)
        {
            _logger?.LogInformation("Fetching employee by id: {Id}", id);
            var employee = await _employees.Find(e => e.Id == id).FirstOrDefaultAsync();
            if (employee == null)
                _logger?.LogWarning("Employee not found: {Id}", id);
            else
                _logger?.LogInformation("Employee found: {Name}", employee.Name);
            return employee;
        }

        public async Task<Employee> CreateEmployeeAsync(Employee employee)
        {
            _logger?.LogInformation("Inserting new employee: {Name}", employee?.Name);
            await _employees.InsertOneAsync(employee);
            _logger?.LogInformation("Inserted employee with Id: {Id}", employee.Id);
            return employee;
        }

        public async Task UpdateEmployeeAsync(string id, Employee employee)
        {
            _logger?.LogInformation("Updating employee: {Id}", id);
            var result = await _employees.ReplaceOneAsync(e => e.Id == id, employee);
            if (result.IsAcknowledged && result.ModifiedCount > 0)
                _logger?.LogInformation("Employee updated: {Id}", id);
            else
                _logger?.LogWarning("Employee update did not modify any documents: {Id}", id);
        }

        public async Task DeleteEmployeeAsync(string id)
        {
            _logger?.LogInformation("Deleting employee: {Id}", id);
            var result = await _employees.DeleteOneAsync(e => e.Id == id);
            if (result.DeletedCount > 0)
                _logger?.LogInformation("Employee deleted: {Id}", id);
            else
                _logger?.LogWarning("No employee deleted (not found): {Id}", id);
        }

        // Implementation for IRepository.GetAll()
        public async Task<IEnumerable<Employee>> GetAll()
        {
            var list = await GetAllEmployeesAsync();
            return list.AsEnumerable();
        }

        public async Task<List<Employee>> GetInactiveEmployeesAsync()
        {
            _logger?.LogInformation("Fetching inactive employees");
            var filter = Builders<Employee>.Filter.Eq(e => e.IsActive, false);
            var list = await _employees.Find(filter).ToListAsync();
            _logger?.LogInformation("Found {Count} inactive employees", list.Count);
            return list;
        }

        public async Task<List<Employee>> GetEmployeesByDepartmentAsync(string department)
        {
            _logger?.LogInformation("Fetching employees by department: {Dept}", department);
            var filter = Builders<Employee>.Filter.Eq(e => e.Department, department);
            var list = await _employees.Find(filter).ToListAsync();
            _logger?.LogInformation("Found {Count} employees in {Dept}", list.Count, department);
            return list;
        }

        public async Task<List<Employee>> GetEmployeesBySalaryAsync(double salary, bool includeEqual)
        {
            _logger?.LogInformation("Fetching employees by salary. threshold={Salary} includeEqual={Include}", salary, includeEqual);

            // NOTE: older unit tests used GT when includeEqual=true and LTE when includeEqual=false.
            // to remain consistent with tests, we follow that behavior:
            var filter = includeEqual
                ? Builders<Employee>.Filter.Gt(e => e.Salary, salary)
                : Builders<Employee>.Filter.Lte(e => e.Salary, salary);

            var list = await _employees.Find(filter).ToListAsync();
            _logger?.LogInformation("Found {Count} employees for salary filter", list.Count);
            return list;
        }

        public async Task<List<Employee>> GetEmployeesByNameAsync(string name)
        {
            _logger?.LogInformation("Fetching employees by name pattern: {Name}", name);

            // Case-insensitive contains
            var regex = new MongoDB.Bson.BsonRegularExpression(name, "i");
            var filter = Builders<Employee>.Filter.Regex(e => e.Name, regex);

            var list = await _employees.Find(filter).ToListAsync();
            _logger?.LogInformation("Found {Count} employees matching name '{Name}'", list.Count, name);
            return list;
        }

      
        public async Task<User> CreateUserAsync(User user)
        {
            await _users.InsertOneAsync(user);
            return user;
        }

        public async Task<User> GetUserByUsernameAsync(string username)
        {
            return await _users.Find(u => u.Username == username).FirstOrDefaultAsync();
        }

    }
}
