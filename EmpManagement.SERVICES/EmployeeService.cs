using EmployeeManagement.Services;
using EmpManagement.Core;
using EmpManagement.INFRA;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EmployeeManagement.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IRepository _repository;

        public EmployeeService(IRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public Task<List<Employee>> GetAllAsync()
        {
            if (_repository == null) throw new InvalidOperationException("Repository is not initialized.");
            return _repository.GetAllEmployeesAsync();
        }

        public Task<Employee> GetByIdAsync(string id)
        {
            if (string.IsNullOrEmpty(id)) throw new ArgumentException("Id cannot be null or empty.", nameof(id));
            if (_repository == null) throw new InvalidOperationException("Repository is not initialized.");
            return _repository.GetEmployeeByIdAsync(id);
        }

        public Task<Employee> AddAsync(Employee employee)
        {
            if (employee == null) throw new ArgumentNullException(nameof(employee));
            if (_repository == null) throw new InvalidOperationException("Repository is not initialized.");
            return _repository.CreateEmployeeAsync(employee);
        }

        public Task UpdateAsync(Employee employee)
        {
            if (employee == null) throw new ArgumentNullException(nameof(employee));
            if (string.IsNullOrEmpty(employee.Id)) throw new ArgumentException("Employee Id cannot be null or empty.", nameof(employee));
            if (_repository == null) throw new InvalidOperationException("Repository is not initialized.");
            return _repository.UpdateEmployeeAsync(employee.Id, employee);
        }

        public Task DeleteAsync(string id)
        {
            if (string.IsNullOrEmpty(id)) throw new ArgumentException("Id cannot be null or empty.", nameof(id));
            if (_repository == null) throw new InvalidOperationException("Repository is not initialized.");
            return _repository.DeleteEmployeeAsync(id);
        }

        public Task<List<Employee>> GetInactiveEmployeesAsync()
        {
            if (_repository == null) throw new InvalidOperationException("Repository is not initialized.");
            return _repository.GetInactiveEmployeesAsync();
        }

        public Task<List<Employee>> GetEmployeesByDepartmentAsync(string department)
        {
            if (string.IsNullOrEmpty(department))
                throw new ArgumentException("Department cannot be null or empty.", nameof(department));
            if (_repository == null) throw new InvalidOperationException("Repository is not initialized.");
            return _repository.GetEmployeesByDepartmentAsync(department);
        }

        public Task<List<Employee>> GetEmployeesBySalaryAsync(double salary, bool includeEqual)
        {
            if (salary < 0)
                throw new ArgumentException("Salary cannot be negative.", nameof(salary));
            if (_repository == null) throw new InvalidOperationException("Repository is not initialized.");
            return _repository.GetEmployeesBySalaryAsync(salary, includeEqual);
        }

        public Task<List<Employee>> GetEmployeesByNameAsync(string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("Name cannot be null or empty.", nameof(name));
            if (_repository == null) throw new InvalidOperationException("Repository is not initialized.");
            return _repository.GetEmployeesByNameAsync(name);
        }
    }
}