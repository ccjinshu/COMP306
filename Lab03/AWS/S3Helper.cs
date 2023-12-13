using Amazon.S3.Model;
using Amazon.S3;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;

namespace Lab03.AWS
{

    public class S3Helper
    {
        //private static string awsAccessKey = "access-key-1";
        //private static string awsSecretKey = "secret-key-1";
        string defaultBucketName = "lab4-image-store";
        static Connection conn = new Connection();
        private readonly AmazonS3Client _s3Client;
        public S3Helper()
        {
            _s3Client = conn.ConnectS3();
             CreateBucketIfNotExistsAsync(defaultBucketName);
        }

        //check if bucket is not  exists ,create bucket
          async Task CreateBucketIfNotExistsAsync(string bucketName)
        {
            //check bucket exists
            ListBucketsResponse response = await _s3Client.ListBucketsAsync();
            bool bucketExists = false;

            if (response.Buckets.Any(b => b.BucketName == bucketName))
            {
                bucketExists = true;
            }

            if (!bucketExists)
            {

                PutBucketRequest request = new PutBucketRequest
                {
                    BucketName = bucketName,
                    UseClientRegion = true, 

                };

                await _s3Client.PutBucketAsync(request);
            }

        }

        public async Task<MemoryStream> GetFileStreamAsync( string keyName)
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

        //upload file to S3 bucket ,UploadFileAsync() method ,return file key 
        public async Task<string> UploadFileAsync(string fileName, Stream fileStream)
        { 
            PutObjectRequest request = new PutObjectRequest
            {
                BucketName = defaultBucketName,
                Key = fileName,
                InputStream = fileStream
            };

            PutObjectResponse response = await _s3Client.PutObjectAsync(request);
            return fileName;
        }
        


        
        //get public file url by file key
        public string GetFileUrl(string fileKey)
        {
            //https://s3-comp306-sjin-lab03-movie.s3.ca-central-1.amazonaws.com/p1u4yYO.jpeg

            return $"https://{defaultBucketName}.s3.ca-central-1.amazonaws.com/{fileKey}";
        }


        //delete file by file key
        public async Task DeleteFileAsync(string fileKey)
        { 
            DeleteObjectRequest request = new DeleteObjectRequest
            {
                BucketName = defaultBucketName,
                Key = fileKey
            };

            DeleteObjectResponse response = await _s3Client.DeleteObjectAsync(request);
        }

        //delete file by fileurl
        public async Task DeleteFileByUrlAsync(string fileUrl)
        {
            string fileKey = fileUrl.Split('/').Last();
            await DeleteFileAsync(fileKey);
        }

    }
}


