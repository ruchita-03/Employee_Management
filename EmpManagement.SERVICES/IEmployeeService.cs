using System.Collections.Generic;
using System.Threading.Tasks;
using EmpManagement.Core;

namespace EmployeeManagement.Services
{
    public interface IEmployeeService
    {
        Task<List<Employee>> GetAllAsync();
        Task<Employee> GetByIdAsync(string id);
        Task<Employee> AddAsync(Employee employee); // Updated from Task to Task<Employee>
        Task UpdateAsync(Employee employee);
        Task DeleteAsync(string id);
        Task<List<Employee>> GetInactiveEmployeesAsync();
        Task<List<Employee>> GetEmployeesByDepartmentAsync(string department);
        Task<List<Employee>> GetEmployeesBySalaryAsync(double salary, bool includeEqual);
        Task<List<Employee>> GetEmployeesByNameAsync(string name);
    }
}