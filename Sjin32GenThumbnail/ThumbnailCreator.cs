using System;
using System.IO;
using System.Threading.Tasks;
using Amazon.S3;
using Amazon.S3.Model;
using ImageMagick;

public static class S3ImageProcessor
{
    static string ACCESSKEY = "AKIAX3P3FTUF3RFUKNOA";
    static string SECRETKEY = "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"; 
     
    private static readonly AmazonS3Client _s3Client = new AmazonS3Client(ACCESSKEY, SECRETKEY, Amazon.RegionEndpoint.USEast1);
    public static async Task CreateAndUploadThumbnailToS3(string sourceBucketName, string sourceS3Key, string targetBucketName, int thumbnailWidth, int thumbnailHeight)
    {
        // 从 S3 加载原始图片
        using (Stream originalImageStream = await LoadFromS3Async(sourceBucketName, sourceS3Key))
        {
            // 使用 Magick.NET 生成缩略图
            using (MagickImage image = new MagickImage(originalImageStream))
            {
                image.Resize(thumbnailWidth, thumbnailHeight);

                using (var resultStream = new MemoryStream())
                {
                    image.Write(resultStream, MagickFormat.Jpeg);
                    resultStream.Position = 0;

                    // 将缩略图上传到目标 S3 存储桶
                    await SaveToS3Async(resultStream, targetBucketName, sourceS3Key);
                }
            }
        }
    }

    private static async Task<Stream> LoadFromS3Async(string bucketName, string fileKey)
    {
        var request = new GetObjectRequest
        {
            BucketName = bucketName,
            Key = fileKey
        };

        using (var response = await _s3Client.GetObjectAsync(request))
        {
            var responseStream = new MemoryStream();
            await response.ResponseStream.CopyToAsync(responseStream);
            responseStream.Position = 0;
            return responseStream;
        }
    }

    private static async Task SaveToS3Async(Stream inputStream, string bucketName, string fileKey)
    {
        var request = new PutObjectRequest
        {
            InputStream = inputStream,
            BucketName = bucketName,
            Key = fileKey
        };

        await _s3Client.PutObjectAsync(request);
    }
}
