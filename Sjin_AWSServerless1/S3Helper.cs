using System;
using System.IO;
using System.Threading.Tasks;
using Amazon.S3;
using Amazon.S3.Model;

namespace Sjin_AWSServerless1
{
    public static class S3Helper
    {
        static string ACCESSKEY = "AKIAX3P3FTUF3RFUKNOA";
        static string SECRETKEY = "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";
        private static AmazonS3Client _client;

        static S3Helper()
        {
            // 配置 AWS S3 客户端（使用你的凭证和区域）
            _client = new AmazonS3Client(ACCESSKEY, SECRETKEY, Amazon.RegionEndpoint.USEast1);
        }

        public static async Task<Stream> LoadFromS3Async(string bucketName, string fileKey)
        {
            var request = new GetObjectRequest
            {
                BucketName = bucketName,
                Key = fileKey
            };

            using (var response = await _client.GetObjectAsync(request))
            {
                var responseStream = new MemoryStream();
                await response.ResponseStream.CopyToAsync(responseStream);
                responseStream.Position = 0;
                return responseStream;
            }
        }

        public static async Task SaveToS3Async(Stream inputStream, string bucketName, string fileKey)
        {
            var request = new PutObjectRequest
            {
                InputStream = inputStream,
                BucketName = bucketName,
                Key = fileKey
            };

            await _client.PutObjectAsync(request);
        }
    }

}