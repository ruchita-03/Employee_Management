@echo off

(

echo ===== EmployeeManagement\Program.cs =====

type "EmployeeManagement\Program.cs"

echo.
 
echo ===== EmployeeManagement\Controllers\AuthController.cs =====

type "EmployeeManagement\Controllers\AuthController.cs"

echo.
 
echo ===== EmployeeManagement\Controllers\EmployeeController.cs =====

type "EmployeeManagement\Controllers\EmployeeController.cs"

echo.
 
echo ===== EmpManagement.Core\Employee.cs =====

type "EmpManagement.Core\Employee.cs"

echo.
 
echo ===== EmpManagement.Core\EmployeeStoreDBSettings.cs =====

type "EmpManagement.Core\EmployeeStoreDBSettings.cs"

echo.
 
echo ===== EmpManagement.Core\IStoreDatabase.cs =====

type "EmpManagement.Core\IStoreDatabase.cs"

echo.
 
echo ===== EmpManagement.Core\LoginModel.cs =====

type "EmpManagement.Core\LoginModel.cs"

echo.
 
echo ===== EmpManagement.Core\User.cs =====

type "EmpManagement.Core\User.cs"

echo.
 
echo ===== EmpManagement.Core\SimpleFileLogger.cs =====

type "EmpManagement.Core\SimpleFileLogger.cs"

echo.
 
echo ===== EmpManagement.Core\SimpleFileLoggerProvider.cs =====

type "EmpManagement.Core\SimpleFileLoggerProvider.cs"

echo.
 
echo ===== EmpManagement.INFRA\IRepository.cs =====

type "EmpManagement.INFRA\IRepository.cs"

echo.
 
echo ===== EmpManagement.INFRA\Repository.cs =====

type "EmpManagement.INFRA\Repository.cs"

echo.
 
echo ===== EmpManagement.SERVICES\IEmployeeService.cs =====

type "EmpManagement.SERVICES\IEmployeeService.cs"

echo.
 
echo ===== EmpManagement.SERVICES\EmployeeService.cs =====

type "EmpManagement.SERVICES\EmployeeService.cs"

echo.
 
echo ===== EmployeeController.Tests\EmployeeControllerTests.cs =====

type "EmployeeController.Tests\EmployeeControllerTests.cs"

echo.
 
echo ===== EmpManagement.repository.Test\RepositoryTests.cs =====

type "EmpManagement.repository.Test\RepositoryTests.cs"

echo.
 
echo ===== EmpManagement.Tests\EmployeeModelTests.cs =====

type "EmpManagement.Tests\EmployeeModelTests.cs"

echo.
 
echo ===== ServiceTest\EmployeeServiceTests.cs =====

type "ServiceTest\EmployeeServiceTests.cs"

echo.
 
echo ===== EmployeeManagement\appsettings.json =====

type "EmployeeManagement\appsettings.json"

echo.
 
echo ===== EmployeeManagement\appsettings.Development.json =====

type "EmployeeManagement\appsettings.Development.json"

echo.
 
echo ===== EmployeeManagement\Properties\launchSettings.json =====

type "EmployeeManagement\Properties\launchSettings.json"

echo.

) > all_cs_contents.txt

 