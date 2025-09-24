using EmployeeManagement.Services;
using EmployeeManagement.Services;
using EmpManagement.Core;
using EmpManagement.Core;
using EmpManagement.INFRA;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using MongoDB.Driver;



var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<EmployeeStoreDBSettings>(
    builder.Configuration.GetSection("EmployeeStoreDBSettings"));




// Register IMongoDatabase separately
builder.Services.AddSingleton<IMongoDatabase>(sp =>
{
    var settings = sp.GetRequiredService<IOptions<EmployeeStoreDBSettings>>().Value;
    var client = new MongoClient(settings.ConnectionString);
    return client.GetDatabase(settings.DatabaseName);
});

// Register Repository and Service, inject their interfaces
builder.Services.AddScoped<IRepository, Repository>();
builder.Services.AddScoped<IEmployeeService, EmployeeService>();

builder.Services.AddLogging();
builder.Logging.AddProvider(new SimpleFileLoggerProvider("Logs/log.txt"));
// Register controllers and swagger (if needed)
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
//app.UseAuthorization();
app.MapControllers();

app.Run();
