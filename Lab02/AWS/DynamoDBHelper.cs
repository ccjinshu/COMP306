using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab02.AWS
{
      public class DynamoDBHelper
    {
        static Connection conn = new Connection();
        private readonly AmazonDynamoDBClient _client;

        public DynamoDBHelper()
        {
            _client =conn.Connect();
        }

        //check if table exists

        public async Task<bool>  ExistsTable (string tableName)
        {
            var tableResponse = await _client.ListTablesAsync();
            return tableResponse.TableNames.Contains(tableName);
        }

         

        public async Task<Document> GetUserAsync(string userName)
        {
            var table = Table.LoadTable(_client, "Users");
            return await table.GetItemAsync(userName);
        }




        //Programmatically create a DynamoDB table to store user’s credentials (user email name & password)

        public static async Task CreateTable(AmazonDynamoDBClient client, string tableName)
        {
            var request = new Amazon.DynamoDBv2.Model.CreateTableRequest
            {
                TableName = tableName,
                AttributeDefinitions = new List<Amazon.DynamoDBv2.Model.AttributeDefinition>()
                {
                    new Amazon.DynamoDBv2.Model.AttributeDefinition
                    {
                        AttributeName = "Email",
                        AttributeType = "S"
                    }
                },
                KeySchema = new List<Amazon.DynamoDBv2.Model.KeySchemaElement>()
                {
                    new Amazon.DynamoDBv2.Model.KeySchemaElement
                    {
                        AttributeName = "Email",
                        KeyType = "HASH"
                    }
                },
                ProvisionedThroughput = new Amazon.DynamoDBv2.Model.ProvisionedThroughput
                {
                    ReadCapacityUnits = 1,
                    WriteCapacityUnits = 1
                }
            };

            var response = await client.CreateTableAsync(request);
        }




    }
}
