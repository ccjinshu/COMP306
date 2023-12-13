namespace Sjin_AWSServerless
{

    /// <summary>
    /// The state passed between the step function executions.
    /// </summary>
    public class State
    {
        /// <summary>
        /// Input value when starting the execution
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// The message built through the step function execution.
        /// </summary>
        public string? Message { get; set; }

        /// <summary>
        /// The number of seconds to wait between calling the Salutations task and Greeting task.
        /// </summary>
        public int WaitInSeconds { get; set; }



        //上传的文件是否是图片
        public bool IsImageFile { get; set; }
        public string? FileName { get; set; }
        public string? BucketName { get; set; }




    }

}