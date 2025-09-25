using EmpManagement.Core;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EmpManagement.INFRA
{
    public interface IRepository
    {
        Task<IEnumerable<Employee>> GetAll();
        Task<Employee> CreateEmployeeAsync(Employee employee);

        Task<List<Employee>> GetAllEmployeesAsync();
        Task<Employee> GetEmployeeByIdAsync(string id);
      
        Task UpdateEmployeeAsync(string id, Employee employee);
        Task DeleteEmployeeAsync(string id);
        Task<List<Employee>> GetInactiveEmployeesAsync();
        Task<List<Employee>> GetEmployeesByDepartmentAsync(string department);
        Task<List<Employee>> GetEmployeesBySalaryAsync(double salary, bool includeEqual);
        Task<List<Employee>> GetEmployeesByNameAsync(string name);

        Task<User> GetUserByUsernameAsync(string username);
        Task<User> CreateUserAsync(User user);
    }
}
