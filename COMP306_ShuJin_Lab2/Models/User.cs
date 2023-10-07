using Amazon.DynamoDBv2.DataModel;


namespace COMP306_ShuJin_Lab2.Models
{
    [DynamoDBTable("Users")]
    public class User
    {
        [DynamoDBHashKey]
        public string UserId { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }


        public User() { }
        public User(string userid, string username, string password)
        {
            UserId = userid;
            Username = username;
            Password = password;
        }


    }




}
