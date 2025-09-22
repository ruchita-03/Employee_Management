using System.Collections.Generic;
using System.Threading.Tasks;
using EmpManagement.Core; // Adjust namespace if needed

public interface IEmployeeService
{
    Task<List<Employee>> GetAllAsync();
    Task<Employee> GetByIdAsync(string id);
    Task CreateAsync(Employee employee);
    Task UpdateAsync(string id, Employee employee);
    Task DeleteAsync(string id);
}

public class EmployeeService : IEmployeeService
{
    private readonly Repository _repository;

    public EmployeeService(Repository repository)
    {
        _repository = repository;
    }

    public Task<List<Employee>> GetAllAsync()
    {
        return _repository.GetAllEmployeesAsync();
    }

    public Task<Employee> GetByIdAsync(string id)
    {
        return _repository.GetEmployeeByIdAsync(id);
    }

    public Task CreateAsync(Employee employee)
    {
        return _repository.CreateEmployeeAsync(employee);
    }

    public Task UpdateAsync(string id, Employee employee)
    {
        return _repository.UpdateEmployeeAsync(id, employee);
    }

    public Task DeleteAsync(string id)
    {
        return _repository.DeleteEmployeeAsync(id);
    }
}
