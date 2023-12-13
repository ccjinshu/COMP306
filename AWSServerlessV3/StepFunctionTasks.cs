using Amazon.Lambda.Core;
using Amazon.Rekognition;
using Amazon.S3;
using System.ComponentModel;


// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace AWSServerlessV3;

public class StepFunctionTasks
{
    /// <summary>
    /// Default constructor that Lambda will invoke.
    /// </summary>
    public StepFunctionTasks()
    {
    }


    public State Greeting(State state, ILambdaContext context)
    {
        state.Message = "Hello";

        if (!string.IsNullOrEmpty(state.Name))
        {
            state.Message += " " + state.Name;
        }

        // Tell Step Function to wait 5 seconds before calling 
        state.WaitInSeconds = 5;

        return state;
    }

    public State Salutations(State state, ILambdaContext context)
    {
        state.Message += ", Goodbye";

        if (!string.IsNullOrEmpty(state.Name))
        {
            state.Message += " " + state.Name;
        }

        return state;
    }


    // DetectLabel 

    public State CheckIfImage(State state, ILambdaContext context)
    {
        HashSet<string> SupportedImageTypes = new HashSet<string> { ".png", ".jpg", ".jpeg" };
        string fileName = state.FileName;
        string extension = Path.GetExtension(fileName).ToLower();

        if (!SupportedImageTypes.Contains(extension))
        {
            state.IsImageFile = false;
            throw new NotImageException($"File extension {extension} is not supported.");
        }

        state.IsImageFile = true;

        return state;

    }
 
 

    public State DetectLabel(State state, ILambdaContext context)
    {
        String fileName = state.FileName;
        String bucketName = state.BucketName;

        //string bucketName = "lab4-image-store";
        //string fileName = "1.jpeg";
        //string targetBucketName = "lab4-image-thumbnail";


        DateTime startTime = DateTime.Now;
        float minConfidence = 90f;

        RekognitionHelper  .detectLabelAndSaveLabelToDb(bucketName, fileName);

        DateTime endTime = DateTime.Now;
        TimeSpan ts = endTime - startTime;
        String timeSpan = ts.TotalMilliseconds.ToString();

        state.Message = "Detect Label And Save Label To Db  Finish , " + timeSpan + " ms";
        return state;
    }

    // Generate Thumbnail save to S3

    public State GenerateThumbnailToS3(State state, ILambdaContext context)
    {
        String fileName = state.FileName;
        String bucketName = state.BucketName;
        String targetbucketName = "lab4-image-thumbnail";// state.TargetBucketName;

        //¼ÆÊ±Æ÷
        DateTime startTime = DateTime.Now;
     
        GenerateThumbnailToS3Handler(bucketName, fileName, targetbucketName, context);


        DateTime endTime = DateTime.Now; 
        TimeSpan ts = endTime - startTime; 
        String timeSpan =  ts.TotalMilliseconds.ToString();


        state.Message = "Generate Thumbnail To S3 Finish , "+ timeSpan + " ms";
        return state;
    }

    //GenerateThumbnailToS3Handler
    public async Task GenerateThumbnailToS3Handler(String bucketName, String fileName, String targetbucketName, ILambdaContext context)
    {
       //sleep 5s
        

         await S3ImageProcessor.CreateAndUploadThumbnailToS3(bucketName, fileName, targetbucketName, 150, 150);



        return;


    }
}



public class NotImageException : Exception
{
    public NotImageException(string message) : base(message)
    {

    }
}


