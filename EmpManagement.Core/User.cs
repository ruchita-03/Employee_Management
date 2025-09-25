﻿using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace EmpManagement.Core
{
    public class User
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("username")]
        public string Username { get; set; }

        [BsonElement("passwordHash")]
        public string PasswordHash { get; set; }

        [BsonElement("role")]
        public string Role { get; set; } // Admin, Moderator, ReadOnly
    }
}
