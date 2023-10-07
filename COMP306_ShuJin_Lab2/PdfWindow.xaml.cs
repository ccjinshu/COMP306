using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using COMP306_ShuJin_Lab2.AWS;
using COMP306_ShuJin_Lab2.Models;
using Microsoft.VisualBasic;
using Syncfusion.Windows.PdfViewer;

namespace COMP306_ShuJin_Lab2
{
    /// <summary>
    /// Interaction logic for ReadingWindow.xaml
    /// </summary>
    public partial class PdfWindow : Window
    {
        static readonly string ACCESSKEY = "AKIAX3P3FTUF3RFUKNOA";
        static readonly string SECRETKEY = "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";
        string defaultBucketName = "s3-comp306-sjin-lab02";

        S3Helper _s3Helper;
        AwsProxy _awsProxy;
        User _user;
        Book _book; 
        bool _is_bookmark_running = false;
        public PdfWindow(User user  ,Book book)
        {
            InitializeComponent();
            //窗口居中 
            WindowStartupLocation = WindowStartupLocation.CenterScreen;

            _awsProxy = new AwsProxy();
            _s3Helper = new S3Helper();

            _user = user;
            _book = book;

            //register the event handler for  window loaded event
            this.Loaded += PdfWindow_Loaded;

            this.Closing += PdfWindow_Closing;
            this.Closed += PdfWindow_Closed;

            



        }

        private void PdfWindow_Closing(object? sender, CancelEventArgs e)
        {
           e.Cancel= _is_bookmark_running;
            
        }

        private void PdfWindow_Loaded(object sender, RoutedEventArgs e)
        {
             this.LoadPdf();
            
        }

        public  async void LoadPdf_v0()
        {
            try
            {
                // Load the PDF file into the PdfViewerControl
                //PdfViewer.Load(filePath); 

                string fileKey= _book.FileKey;
               var stream  = _s3Helper.GetBookStreamAsync(fileKey);
                //var testfile = "/Users/hover/git/cc/ccjinshu/COMP306/Lab2/Lab#2 (2).pdf";
                var testfile = "D:\\CC\\COMP306\\COMP_3061.pdf";
                var tmpfile=  await _s3Helper.DownloadS3ObjectAsync(fileKey);


                PdfViewer.Load(tmpfile);

                //PdfViewer.Load(_book.FileUrl);

                //AmazonS3Client s3Client = new AmazonS3Client(ACCESSKEY, SECRETKEY, RegionEndpoint.CACentral1);
                //GetObjectRequest request = new GetObjectRequest
                //{
                //    BucketName = defaultBucketName,
                //    Key = fileKey
                //};

                //using (GetObjectResponse response = await s3Client.GetObjectAsync(request))
                //using (Stream responseStream = response.ResponseStream)
                //{
                //    // Use the stream with Syncfusion PdfViewer
                //    PdfViewer.Load(responseStream);
                //}






            }
            catch (Exception ex)
            {
                // Handle any exceptions that occur during PDF loading
                MessageBox.Show($"Error loading PDF: {ex.Message}");
            }
        }



        public async void LoadPdf()
        {
            try
            { 

                var stream = await _s3Helper.GetBookStreamAsync(_book.FileKey);
                // Use the stream with Syncfusion PdfViewer
                PdfViewer.Load(stream); 
                PdfViewer.GotoPage(_book.BookmarkPage);
            }
            catch (Exception ex)
            {
                // Handle any exceptions that occur during PDF loading  


                MessageBox.Show($"Error loading PDF: {ex.Message}");
            }
        }





        private void BookmarkButton_Click(object sender, RoutedEventArgs e)
        {
            _is_bookmark_running = true;
            UpdateBookmarkAsync();
            _is_bookmark_running= false;
        }

        private void PdfWindow_Closed(object sender, EventArgs e)
        {
            UpdateBookmarkAsync();
        }

        private async Task UpdateBookmarkAsync()
        { 
           
            // Update the bookmarked page of the current book
            _book.BookmarkPage = PdfViewer.CurrentPageIndex;
            _book.BookmarkTime = DateTime.Now;

            // Update the bookmark in DynamoDB table
            await _awsProxy.UpdateBookmark(_book);
             

        }
    }
}
