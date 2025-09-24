using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmpManagement.Core
{
    public class SimpleFileLoggerProvider : ILoggerProvider
    {
        private readonly string _filePath;

        public SimpleFileLoggerProvider(string filePath)
        {
            _filePath = filePath;
        }

        public ILogger CreateLogger(string categoryName)
        {
            return new SimpleFileLogger(categoryName, _filePath);
        }

        public void Dispose()
        {
        }
    }
}
