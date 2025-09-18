namespace EmployeeManagement.Models
{
    public class EmployeeStoreDBSettings : IStoreDatabase
    {
        public string EmployeeCollectionName { get; set; } = null!;
        public string ConnectionString { get; set; } = null!;
        public string DatabaseName { get; set; } = null!;
    }
}