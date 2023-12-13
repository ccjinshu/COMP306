using Amazon.Rekognition;
using Amazon.Rekognition.Model;
using Amazon.S3;
using Amazon.S3.Model;




public static class RekognitionHelper
{
    static string ACCESSKEY = "AKIAX3P3FTUF3RFUKNOA";
    static string SECRETKEY = "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";


    private static readonly AmazonS3Client S3Client 
        = new AmazonS3Client(ACCESSKEY, SECRETKEY, Amazon.RegionEndpoint.USEast1);
    private static readonly AmazonRekognitionClient RekognitionClient 
        = new AmazonRekognitionClient(ACCESSKEY, SECRETKEY, Amazon.RegionEndpoint.USEast1);

    //生成  ng 安装 AmazonRekognition  SDK
    //dotnet add package AWSSDK.Rekognition --version
    static RekognitionHelper()
    {

     

    }



    public static void detectLabelAndSaveLabelToDb(String srcBucket, String srcFileKey)
    {



        String key = srcFileKey;
        String bucketName = srcBucket;

        String[] keyArray = key.Split("/");
        String imgName = keyArray[keyArray.Length - 1];


        var detectResponses = RekognitionClient.DetectLabelsAsync(new DetectLabelsRequest
        {
            MinConfidence = 90f,
            Image = new Image
            {
                S3Object = new Amazon.Rekognition.Model.S3Object
                {
                    Bucket = bucketName,
                    Name = key
                }
            }
        });

        var tags = new List<Amazon.S3.Model.Tag>();
        foreach (var label in detectResponses.Result.Labels)
        {
            if (tags.Count < 10)
            {
                Console.WriteLine($"\tFound Label {label.Name} with confidence {label.Confidence}");
                tags.Add(new Amazon.S3.Model.Tag { Key = label.Name, Value = label.Confidence.ToString() });
            }
            else
            {
                Console.WriteLine($"\tSkipped label {label.Name} with confidence {label.Confidence} because the maximum number of tags has been reached");
            }

            //if 置信度大于90，写入数据库
            //if (label.Confidence > 90)
            {
                DynamoDBHelper.InsertLabelDataAsync(imgName, label.Name, label.Confidence.ToString());
            }

        }

        S3Client.PutObjectTaggingAsync(new PutObjectTaggingRequest
        {
            BucketName = bucketName,
            Key = key,
            Tagging = new Tagging
            {
                TagSet = tags
            }
        });




        return;
    }
}