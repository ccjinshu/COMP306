using Amazon.DynamoDBv2.DataModel;
using System;


namespace COMP306_ShuJin_Lab2.Models
{
    [DynamoDBTable("Bookshelf")]
    public class Book
    {
        //define the key
        [DynamoDBHashKey]
        public string Id { get; set; }
        public string UserId { get; set; }
        public string BookId { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public int BookmarkPage { get; set; }
        public DateTime BookmarkTime { get; set; }
        public string FilePath { get; set; }
        public string FileKey { get; set; }
        public string FileUrl  { get; set; }

    }


     


}
