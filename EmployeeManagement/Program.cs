
using EmployeeManagement.Services;
using EmpManagement.Core;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

// Add services to the container.
var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<EmployeeStoreDBSettings>(
    builder.Configuration.GetSection(nameof(EmployeeStoreDBSettings)));

builder.Services.AddSingleton<IStoreDatabase>(sp =>
    sp.GetRequiredService<IOptions<EmployeeStoreDBSettings>>().Value);

// Fix: Use the correct section name for the connection string
builder.Services.AddSingleton<IMongoClient>(s =>
    new MongoClient(builder.Configuration.GetValue<string>("EmployeeStoreDBSettings:ConnectionString")));

builder.Services.AddScoped<IEmployeeService, EmployeeService>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();