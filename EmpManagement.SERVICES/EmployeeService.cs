using EmpManagement.Core;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace EmployeeManagement.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IMongoCollection<Employee> _employee;

        public EmployeeService(IStoreDatabase settings, IMongoClient mongoclient)
        {
            var database = mongoclient.GetDatabase(settings.DatabaseName);
            _employee = database.GetCollection<Employee>(settings.EmployeeCollectionName);
        }

        public Employee Create(Employee newEmployee)
        {
            newEmployee.ID = null; // Let MongoDB generate the _id
            _employee.InsertOne(newEmployee);
            return newEmployee;
        }

        public List<Employee> Get()
        {
            return _employee.Find(student => true).ToList();
        }

        public Employee GetEmployee(int id)
        {
            throw new NotImplementedException();
        }

        public void Update(string id, Employee updatedEmployee)
        {
            updatedEmployee.ID = id; // Ensure the ID is correct and valid
            _employee.ReplaceOne(employee => employee.ID == id, updatedEmployee);
        }

        public void Remove(string id)
        {
            _employee.DeleteOne(employee => employee.ID == id);
        }

        public Employee Get(string id)
        {
            return _employee.Find(employee => employee.ID == id).FirstOrDefault();
        }

        public void Delete(Employee newEmployee)
        {
            if (newEmployee == null || string.IsNullOrEmpty(newEmployee.ID))
                throw new ArgumentException("Invalid employee to delete.");

            _employee.DeleteOne(employee => employee.ID == newEmployee.ID);
        }
    }
}
