using Amazon.Lambda.Core;
using Amazon.Rekognition;
using Amazon.Rekognition.Model;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

 
 namespace Sjin_AWSServerless1;

//define Exception : NotImageException


public class StepFunctionTasks
{
    /// <summary>
    /// Default constructor that Lambda will invoke.
    /// </summary>
    public StepFunctionTasks()
    {
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
        RekognitionHelper rekognitionHelper = new RekognitionHelper();

        rekognitionHelper.detectLabelAndSaveLabelToDb(bucketName, fileName);





        return state;

    }

    // Generate Thumbnail save to S3

    public State GenerateThumbnailToS3(State state, ILambdaContext context)
    {

        String fileName = state.FileName;
        String bucketName = state.BucketName;


        S3ImageProcessor.CreateAndUploadThumbnailToS3("source-bucket", "source-key.jpg", "target-bucket", 150, 150);




        return state;
    }




 





}

public class NotImageException : Exception
{
    public NotImageException(string message) : base(message)
    {

    }
}


 