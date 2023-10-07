using Amazon.S3;
using Amazon.S3.Model;
using System.IO;
using System.Threading.Tasks;

namespace COMP306_ShuJin_Lab2.AWS
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
        public async Task<MemoryStream> GetBookStreamAsync(string keyName)
        {

            GetObjectRequest request = new GetObjectRequest
            {
                BucketName = defaultBucketName,
                Key = keyName
            };

            GetObjectResponse response = await _s3Client.GetObjectAsync(request);
            MemoryStream documentStream = new MemoryStream();
            response.ResponseStream.CopyTo(documentStream);

            // Reset the stream's position to the beginning:
            documentStream.Seek(0, SeekOrigin.Begin);

            return documentStream;
        }




        public async Task<string> DownloadS3ObjectAsync(  string objectKey)
        {
             
            // Initialize the Amazon S3 client
            AmazonS3Client s3Client = _s3Client;

            // Create the request to retrieve the object
            GetObjectRequest request = new GetObjectRequest
            {
                BucketName = defaultBucketName,
                Key = objectKey
            };

            // Generate a unique filename in the temp directory
            string tempFilePath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());

            // Get the object and write it to the temp file
            using (GetObjectResponse response = await s3Client.GetObjectAsync(request))
            using (Stream responseStream = response.ResponseStream)
            using (FileStream fileStream = new FileStream(tempFilePath, FileMode.Create))
            {
                await responseStream.CopyToAsync(fileStream);
            }

            // Return the temp file path
            return tempFilePath;
        }
    }
}


