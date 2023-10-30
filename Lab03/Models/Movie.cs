using System;
using System.Collections.Generic;
using Amazon.DynamoDBv2.DataModel;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Lab03.Models;
[DynamoDBTable("Movies")]

public partial class Movie
{
    [DynamoDBHashKey] // 主键
    public int Id { get; set; }

    [DynamoDBProperty] // 标记为 DynamoDB 属性
    public string Title { get; set; }

    [DynamoDBProperty]
    public string Genre { get; set; }

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

    

}
