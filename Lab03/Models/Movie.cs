using System;
using System.Collections.Generic;
using Amazon.DynamoDBv2.DataModel;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Lab03.Models;
[DynamoDBTable("Movies")]

public partial class Movie
{ 
    [DynamoDBHashKey]
    public string Id { get; set; } // 使用 Guid 作为主键的一部分


    [DynamoDBProperty] // 标记为 DynamoDB 属性
    public string Title { get; set; }

    [DynamoDBProperty]
    public MovieGenre Genre { get; set; }

    [DynamoDBProperty]
    public string Director { get; set; }

    [DynamoDBProperty]
    public DateTime ReleaseTime { get; set; }

    [DynamoDBProperty]
    public double Rating { get; set; }

    [DynamoDBProperty]
    public string CoverImage { get; set; }

    [DynamoDBProperty]
    public string FileKey { get; set; }

    [DynamoDBProperty]
    public string FileUrl { get; set; }

   
    [DynamoDBProperty]
    [BindNever] 
    public List<Comment> Comments { get; set; }

    //constructor
    public Movie()
    {
        Comments = new List<Comment>();
        Id = Guid.NewGuid().ToString();

    }

}
