using EmpManagement.Core;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using System.Security.Cryptography;
using System.Text;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IMongoDatabase _database;

    public AuthController(IMongoDatabase database)
    {
        _database = database;
    }

    private IMongoCollection<User> Users => _database.GetCollection<User>("Users");

    // POST: api/auth/register
    [HttpPost("register")]
    public IActionResult Register([FromBody] RegisterModel model)
    {
        // 1️⃣ Check if username already exists
        var existingUser = Users.Find(u => u.Username == model.Username).FirstOrDefault();
        if (existingUser != null)
        {
            return BadRequest("Username already exists.");
        }

        // 2️⃣ Hash the password
        var passwordHash = HashPassword(model.Password);

        // 3️⃣ Create User object
        var user = new User
        {
            Username = model.Username,
            PasswordHash = passwordHash,
            Role = model.Role
        };

        // 4️⃣ Insert into MongoDB
        Users.InsertOne(user);

        return Ok(new { message = "User registered successfully!" });
    }

    // Helper: Hash password using SHA256 (simplest way; can use stronger hashing like BCrypt)
    private string HashPassword(string password)
    {
        using (var sha = SHA256.Create())
        {
            var bytes = Encoding.UTF8.GetBytes(password);
            var hash = sha.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }
    }
}
