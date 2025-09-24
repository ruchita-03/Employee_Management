using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.ComponentModel.DataAnnotations;

public class Employee
{
    // MongoDB ObjectId as string
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }

    [Required(ErrorMessage = "Name is required.")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Name should be between 2 and 100 characters.")]
    [BsonElement("Name")]
    public string Name { get; set; }

    [Required(ErrorMessage = "Department is required.")]
    [StringLength(50, ErrorMessage = "Department name should not exceed 50 characters.")]
    [BsonElement("Department")]
    public string Department { get; set; }

    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Invalid email address.")]
    [BsonElement("Email")]
    public string Email { get; set; }

    [Required(ErrorMessage = "Date of Joining is required.")]
    [DataType(DataType.Date)]
    [BsonElement("DateOfJoining")]
    public DateTime DateOfJoining { get; set; }

    [Required(ErrorMessage = "Job Title is required.")]
    [StringLength(100, ErrorMessage = "Job Title should not exceed 100 characters.")]
    [BsonElement("JobTitle")]
    public string JobTitle { get; set; }

    [Required(ErrorMessage = "Salary is required.")]
    [Range(0, double.MaxValue, ErrorMessage = "Salary must be a positive number.")]
    [BsonElement("Salary")]
    public double Salary { get; set; }

    [BsonElement("IsActive")]
    public bool IsActive { get; set; }

    public object RenderToBsonDocument()
    {
        throw new NotImplementedException();
    }
}
