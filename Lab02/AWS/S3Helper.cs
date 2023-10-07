using Amazon.S3.Model;
using Amazon.S3;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;

namespace Lab02.AWS
{

    public class S3Helper
    {
        //private static string awsAccessKey = "access-key-1";
        //private static string awsSecretKey = "secret-key-1";
        string defaultBucketName = "s3-comp306-sjin-lab02";
        static Connection conn = new Connection();
        private readonly AmazonS3Client _s3Client;
        public S3Helper()
        {
            _s3Client = conn.ConnectS3();
        }
        public async Task<MemoryStream> GetBookStreamAsync( string keyName)
        {

            GetObjectRequest request = new GetObjectRequest
            {
                BucketName = defaultBucketName ,
                Key = keyName
            };

            GetObjectResponse response = await _s3Client.GetObjectAsync(request);
            MemoryStream documentStream = new MemoryStream();
            response.ResponseStream.CopyTo(documentStream);

            // Reset the stream's position to the beginning:
            documentStream.Seek(0, SeekOrigin.Begin);

            return documentStream;
        }
    }
}


