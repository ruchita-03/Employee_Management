namespace EmployeeManagement.Models
{
    public interface IStoreDatabase
    {
        string EmployeeCollectionName { get ; set;}
        string ConnectionString { get ; set; }
        string DatabaseName { get ; set; }

    }
}
