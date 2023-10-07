using System;
using System.Collections.Generic;
using System.Text;
using Amazon.DynamoDBv2.DataModel;


namespace Lab02.Models
{
    [DynamoDBTable("Books")]
    public class Book
    {
        //define the key
        [DynamoDBHashKey]
        public string BookName { get; set; }
        public string Author { get; set; }
        public string Genre { get; set; }
        public string Publisher { get; set; }
        public int Year { get; set; }
        public string ISBN { get; set; } 
        
    }


    [Amazon.DynamoDBv2.DataModel.DynamoDBTable("Bookshelf")]
    public class BookItem
    {
        [Amazon.DynamoDBv2.DataModel.DynamoDBHashKey]
        public string UserName { get; set; }
        [Amazon.DynamoDBv2.DataModel.DynamoDBRangeKey]
        public string BookId { get; set; }
        public string BookTitle { get; set; }
        public int CurrentPage { get; set; }
        public DateTime LastReadTime { get; set; }
        
        //author 
        public string Author { get; set; }
    }
}
