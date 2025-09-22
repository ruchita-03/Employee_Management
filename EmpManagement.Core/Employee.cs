using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.ComponentModel.DataAnnotations;

public class Employee
{
    // Unique Id represented as string but saved as ObjectId in MongoDB
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    [Required(ErrorMessage = "Id is required.")]
    public string Id { get; set; }


    [BsonElement("Name")]
    public string Name { get; set; }

   
    [BsonElement("Department")]
    public string Department { get; set; }

  
    [BsonElement("Email")]
    public string Email { get; set; }

    [BsonElement("DateOfJoining")]
    public DateTime DateOfJoining { get; set; }

    
    [BsonElement("JobTitle")]
    public string JobTitle { get; set; }

 
    [BsonElement("Salary")]
    public double Salary { get; set; }

   
    [BsonElement("IsActive")]
    public bool IsActive { get; set; }
}
