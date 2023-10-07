using Amazon.DynamoDBv2;
using Amazon.Runtime;
using Amazon.S3;

namespace COMP306_ShuJin_Lab2.AWS
{
    class Connection
    {
        static readonly string ACCESSKEY = "AKIAX3P3FTUF3RFUKNOA";
        static readonly string SECRETKEY = "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";

        public AmazonDynamoDBClient Connect()
        {

            string accessKeyID = ACCESSKEY;
            string secretKey = SECRETKEY;
            var credentials = new BasicAWSCredentials(accessKeyID, secretKey);

            AmazonDynamoDBClient client = new AmazonDynamoDBClient(credentials, Amazon.RegionEndpoint.CACentral1);
            return client;
        }

        public AmazonS3Client ConnectS3()
        {
            string accessKeyID = ACCESSKEY;
            string secretKey = SECRETKEY;
            AmazonS3Client client = new AmazonS3Client(accessKeyID, secretKey, Amazon.RegionEndpoint.CACentral1);
            return client;
        }



    }
}
