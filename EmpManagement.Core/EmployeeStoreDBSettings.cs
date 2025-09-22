using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmpManagement.Core
{
    public class EmployeeStoreDBSettings
    {
        // MongoDB connection string
        public string ConnectionString { get; set; }

        // Name of the MongoDB database
        public string DatabaseName { get; set; }

        // Name of the collection where Employee documents are stored
        public string EmployeeCollectionName { get; set; }
    }

}
