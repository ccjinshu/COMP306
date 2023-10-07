using System;
using System.Collections.Generic;
using System.Text;
using Amazon.DynamoDBv2.DataModel;


namespace Lab02.Models
{
    [DynamoDBTable("Users")]
    public class User
    {
        [DynamoDBHashKey]
        public string UserEmail { get; set; }
        public string Password { get; set; }
        
        
        public User() { }
        public User(string email, string password)
        {
            UserEmail = email;
            Password = password;
        }

      
    }


}
