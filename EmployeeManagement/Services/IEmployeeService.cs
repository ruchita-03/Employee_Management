using EmployeeManagement.Models;
namespace EmployeeManagement.Services
{
    public interface IEmployeeService
    {

        List<Employee> Get();
        Employee Get(string id);

        Employee Create(Employee newEmployee);

        void Update(string id, Employee updatedEmployee);

        void Remove(string id); 
        void Delete(Employee newEmployee);  
    }
}
