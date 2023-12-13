using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;


public class DynamoDBHelper
{

    static string ACCESSKEY = "AKIAX3P3FTUF3RFUKNOA";
    static string SECRETKEY = "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";


    public static  async Task InsertLabelDataAsync(String imageKey, String label, String confidence)
    {
        var client = new AmazonDynamoDBClient(ACCESSKEY, SECRETKEY, Amazon.RegionEndpoint.USEast1);

        var logRecord = new Dictionary<string, AttributeValue>
        {
            { "Id", new AttributeValue { S = Guid.NewGuid().ToString() } }, // Unique identifier for the log entry
            { "ImageKey", new AttributeValue { S = imageKey } },
            { "Operator", new AttributeValue { S = label } },
            { "Confidence", new AttributeValue { S =confidence } },
            { "OperationTime", new AttributeValue { S = DateTime.UtcNow.ToString("o") } } // ISO 8601 format
        };

        var putItemRequest = new PutItemRequest
        {
            TableName = "ImageLabel",
            Item = logRecord
        };

        try
        {
            var response = await client.PutItemAsync(putItemRequest);
            Console.WriteLine("Log entry inserted successfully.");
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message);
        }

         
    }
}
 