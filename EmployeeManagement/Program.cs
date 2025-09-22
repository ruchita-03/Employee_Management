using EmpManagement.Core;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<EmployeeStoreDBSettings>(
    builder.Configuration.GetSection("EmployeeStoreDBSettings"));
builder.Services.AddSingleton<Repository>();
builder.Services.AddScoped<IEmployeeService, EmployeeService>();
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

app.UseAuthorization();

app.MapControllers();

app.Run();
