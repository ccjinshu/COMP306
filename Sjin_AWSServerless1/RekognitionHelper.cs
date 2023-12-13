using Amazon.Lambda.Core;
using Amazon.Rekognition;
using Amazon.Rekognition.Model;
using Amazon.S3;
using Amazon.S3.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sjin_AWSServerless1
{
    public   class RekognitionHelper
    {

        /// <summary>
        /// The default minimum confidence used for detecting labels.
        /// </summary>
        public const float DEFAULT_MIN_CONFIDENCE = 70f;

        /// <summary>
        /// The name of the environment variable to set which will override the default minimum confidence level.
        /// </summary>
        public const string MIN_CONFIDENCE_ENVIRONMENT_VARIABLE_NAME = "MinConfidence";

        IAmazonS3 S3Client { get; }

        IAmazonRekognition RekognitionClient { get; }

        float MinConfidence { get; set; } = DEFAULT_MIN_CONFIDENCE;

        HashSet<string> SupportedImageTypes { get; } = new HashSet<string> { ".png", ".jpg", ".jpeg" };



        public async   Task detectLabelAndSaveLabelToDb(String srcBucket, String srcFileKey)
        {

            String key = srcFileKey;
            String bucketName = srcBucket;

            String[] keyArray = key.Split("/");
            String imgName = keyArray[keyArray.Length - 1];


            var detectResponses = await RekognitionClient.DetectLabelsAsync(new DetectLabelsRequest
            {
                MinConfidence = MinConfidence,
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
            foreach (var label in detectResponses.Labels)
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
                    await DynamoDBHelper.InsertLabelDataAsync(imgName, label.Name, label.Confidence.ToString());
                }

            }

            await S3Client.PutObjectTaggingAsync(new PutObjectTaggingRequest
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
}
