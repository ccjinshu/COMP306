﻿using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

class Program
{

    static async Task Main(string[] args)
    {
        //DynamoDBHelper dynamoDBHelper = new DynamoDBHelper();

       await DynamoDBHelper.InsertLabelDataAsync("ImageLabel", "ABC", "0.9");





        static async Task Main222(string[] args)
        {
            string ACCESSKEY = "AKIAX3P3FTUF3RFUKNOA";
            string SECRETKEY = "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";

            var client = new AmazonDynamoDBClient(ACCESSKEY, SECRETKEY, Amazon.RegionEndpoint.USEast1);

            var createTableRequest = new CreateTableRequest
            {
                TableName = "ImageLabel",
                AttributeDefinitions = new List<AttributeDefinition>
            {
                new AttributeDefinition
                {
                    AttributeName = "Id",
                    AttributeType = "S"
                }
            },
                KeySchema = new List<KeySchemaElement>
            {
                new KeySchemaElement
                {
                    AttributeName = "Id",
                    KeyType = "HASH"  //Partition key
                }
            },
                ProvisionedThroughput = new ProvisionedThroughput
                {
                    ReadCapacityUnits = 5,
                    WriteCapacityUnits = 5
                }
            };

            try
            {
                var response = await client.CreateTableAsync(createTableRequest);
                Console.WriteLine("Table Created Successfully");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
        }
    }

}