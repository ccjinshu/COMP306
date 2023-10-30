using Amazon.DynamoDBv2;
using Amazon.Runtime;
using Amazon.S3;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Lab03.AWS
{
    class Connection
    {
        public AmazonDynamoDBClient Connect()
        {
            var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("AppSettings.json", optional: true, reloadOnChange: true);

            string accessKeyID = builder.Build().GetSection("AWSCredentials").GetSection("AccesskeyID").Value;
            string secretKey = builder.Build().GetSection("AWSCredentials").GetSection("Secretaccesskey").Value;
            var credentials = new BasicAWSCredentials(accessKeyID, secretKey);

            AmazonDynamoDBClient client = new AmazonDynamoDBClient(credentials, Amazon.RegionEndpoint.CACentral1);
            return client;
        }

        public AmazonS3Client ConnectS3()
        {
            var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("AppSettings.json", optional: true, reloadOnChange: true);

            string accessKeyID = builder.Build().GetSection("AWSCredentials").GetSection("AccesskeyID").Value;
            string secretKey = builder.Build().GetSection("AWSCredentials").GetSection("Secretaccesskey").Value;

            //AmazonS3Client client = new AmazonS3Client(accessKeyID, secretKey);
            //get AmazonS3Client for CACentral1 region
            AmazonS3Client client = new AmazonS3Client(accessKeyID, secretKey, Amazon.RegionEndpoint.CACentral1);
            return client;
        }


    }
}
