using Amazon.Lambda.Core;


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

        //开启定时器,等待 DetectLabelHandler 线程结束
        Task.Run(() => DetectLabelHandler(state, context));

        //等待 DetectLabelHandler 线程结束
        Task.WaitAll();
        return state;
    }


    public async Task DetectLabelHandler(State state, ILambdaContext context)
    {
        String fileName = state.FileName;
        String bucketName = state.BucketName;
        RekognitionHelper rekognitionHelper = new RekognitionHelper();

        await rekognitionHelper.detectLabelAndSaveLabelToDb(bucketName, fileName);

        state.Message = "Detect Label And Save Label To Db  Finish ";

        return;
    }

    // Generate Thumbnail save to S3

    public State GenerateThumbnailToS3(State state, ILambdaContext context)
    {

        // ,等待 GenerateThumbnailToS3Handler 线程结束 
        Task.Run(() => GenerateThumbnailToS3Handler(state, context));
        Task.WaitAll(); 

        return state;
    }

    //GenerateThumbnailToS3Handler
    public async Task GenerateThumbnailToS3Handler(State state, ILambdaContext context)
    {
        
        String fileName = state.FileName;
        String bucketName = state.BucketName;
        String targetbucketName = state.TargetBucketName;


        await   S3ImageProcessor.CreateAndUploadThumbnailToS3(bucketName, fileName, targetbucketName, 150, 150);

        state.Message = "Generate Thumbnail To S3 Finish ";

        return;
        

    }
}



public class NotImageException : Exception
{
    public NotImageException(string message) : base(message)
    {

    }
}


