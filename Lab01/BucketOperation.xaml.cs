using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Lab01
{
    /// <summary>
    /// Interaction logic for CreateBucket.xaml
    /// </summary>
    public partial class BucketOperation : Window
    {
        ObservableCollection<Amazon.S3.Model.S3Bucket> myBuckets
          = new ObservableCollection<Amazon.S3.Model.S3Bucket>();

        private static readonly RegionEndpoint bucketRegion = RegionEndpoint.CACentral1;
        public BucketOperation()
        {
            InitializeComponent();
            initDataGrid1();
            getMyBuckets(); 
        }

        void initDataGrid1()
        { 

            //add column to datagrid1
            DataGridTextColumn col1 = new DataGridTextColumn();
            col1.Binding = new Binding("BucketName");
            col1.Header = "Bucket Name";
            col1.Width = 200;
            dataGrid1.Columns.Add(col1);

            DataGridTextColumn col2 = new DataGridTextColumn();
            col2.Binding = new Binding("CreationDate");
            col2.Header = "Creation Date";
            col2.Width = 200;
            dataGrid1.Columns.Add(col2);


            dataGrid1.AutoGenerateColumns = false;
            //set readonly
            dataGrid1.IsReadOnly = true;

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            App.mainWindow.Show();
            this.Hide();
        }

        //create bucket
        private async void createBucket(string newBucketName)
        {
            try
            {
                var s3Client = new AmazonS3Client(Config.ACCESSKEY, Config.SECRETKEY, bucketRegion);
                var response = await s3Client.PutBucketAsync(new PutBucketRequest
                {
                    BucketName = newBucketName
                });

                getMyBuckets();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        //define function to delete bucket
        private async void deleteBucket(string bucketName)
        {
            try
            {
                var s3Client = new AmazonS3Client(Config.ACCESSKEY, Config.SECRETKEY, bucketRegion);
                var response = await s3Client.DeleteBucketAsync(bucketName);
                getMyBuckets();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


        //get all buckets

        private async void getMyBuckets()
        {
            var s3Client = new AmazonS3Client(Config.ACCESSKEY, Config.SECRETKEY, bucketRegion);
            var buckets = await s3Client.ListBucketsAsync();
           
            //buckets sort by  create time desc
            buckets.Buckets.Sort((x, y) => y.CreationDate.CompareTo(x.CreationDate));

            myBuckets.Clear();
            foreach (var bucket in buckets.Buckets)
            {
                if (myBuckets.Count(b => b.BucketName == bucket.BucketName) == 0)
                {
                    myBuckets.Add(bucket);
                }
            }
            dataGrid1.ItemsSource = myBuckets;
        }


 

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }

        private async void btn_CreateBucket_Click(object sender, RoutedEventArgs e)
        {
            string newBucketName = bucketNameInput.Text;
             createBucket(newBucketName);
            

        }

        private async void btn_DelBucket_Click(object sender, RoutedEventArgs e)
        {
            //delete selected bucket 
            //get selected bucket name which the datagrid1 selected row ,and the bucket name is the first column

            //get selected item by property Name 'BucketName'
            Amazon.S3.Model.S3Bucket selectedBucket = (Amazon.S3.Model.S3Bucket ) dataGrid1.SelectedItem ;
            //string  selectedBucketName = (string)selectedBucket.GetType().GetProperty("BucketName").GetValue(selectedBucket, null);
            string selectedBucketName = selectedBucket.BucketName;

            //show message box to confirm delete

            MessageBoxResult result = MessageBox.Show("Are you sure to delete bucket " + selectedBucketName + " ?", "Confirm", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                deleteBucket(selectedBucketName);
            }


        }
    }
}
