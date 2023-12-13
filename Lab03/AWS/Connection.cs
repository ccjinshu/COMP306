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
            

            string accessKeyID = "AKIAX3P3FTUF7RSBIFUR";
            string secretKey = "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";
            var credentials = new BasicAWSCredentials(accessKeyID, secretKey);

            AmazonDynamoDBClient client = new AmazonDynamoDBClient(credentials, Amazon.RegionEndpoint.USEast1);
            return client;
        }

        public AmazonS3Client ConnectS3()
        {
           
            string accessKeyID = "AKIAX3P3FTUF7RSBIFUR";
            string secretKey = "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";
            //AmazonS3Client client = new AmazonS3Client(accessKeyID, secretKey);
            //get AmazonS3Client for CACentral1 region
            AmazonS3Client client = new AmazonS3Client(accessKeyID, secretKey, Amazon.RegionEndpoint.USEast1);
            return client;
        }


    }
}
