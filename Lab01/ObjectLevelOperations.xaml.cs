using Amazon;
using Amazon.S3;
using Amazon.S3.Transfer;
using Microsoft.Win32;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Lab01
{
    /// <summary>
    /// Interaction logic for ObjectLevelOperations.xaml
    /// </summary>
    public partial class ObjectLevelOperations : Window
    {
        public ObjectLevelOperations()
        {
            InitializeComponent();
            initDropDownBox();
            ObjectDataGrid.ItemsSource = objectsFromBucket;
        }

        ObservableCollection<S3Object> objectsFromBucket = new ObservableCollection<S3Object>();

        private static readonly RegionEndpoint bucketRegion = RegionEndpoint.CACentral1;

        private void Bucket_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                getS3Objects();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private async void initDropDownBox()
        {
            var s3Client = new AmazonS3Client(Config.ACCESSKEY, Config.SECRETKEY, bucketRegion);
            var buckets = await s3Client.ListBucketsAsync();
            BucketDropDown.Items.Clear();   
            foreach (var bucket in buckets.Buckets)
            {
                BucketDropDown.Items.Add(new ComboBoxItem().Content = bucket.BucketName);
            }
        }

        private static async Task UploadFileAsync(string filePath, string bucketName)
        {
            var s3Client = new AmazonS3Client(Config.ACCESSKEY, Config.SECRETKEY, bucketRegion);

            try
            {
                var fileTransferUtility = new TransferUtility(s3Client);
                await fileTransferUtility.UploadAsync(filePath, bucketName);
            } catch(AmazonS3Exception e)
            {
                MessageBox.Show(e.Message);

            } catch(Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        private void btnBrowse_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = GetFileDirectory();
            Nullable<bool> Result = openFileDialog.ShowDialog();
            if (Result == true)
            {
                this.NewObjectFileTextBox.Text = openFileDialog.FileName;
                this.btnUpload.IsEnabled = true;
            }
            else
            {
                MessageBox.Show("Please select a file.");
                return;
            }
        }

        private string GetFileDirectory()
        {
            string[] List = Environment.CurrentDirectory.Split(
                System.IO.Path.DirectorySeparatorChar);
            string currPath = string.Empty;

            for (int i = 0; i < List.Length - 2; i++)
            {
                currPath = currPath + List[i] + "\\";
            }

            return currPath + "Files\\";
        }

        private async void btnUpload_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string file = NewObjectFileTextBox.Text;
                await UploadFileAsync(file, BucketDropDown.SelectedItem.ToString());
                getS3Objects();
            }
            catch (Exception ex)
            {
                 //show error message
                 MessageBox.Show(ex.Message);
            }
        }

        private async void getS3Objects()
        {
            var s3Client = new AmazonS3Client(Config.ACCESSKEY, Config.SECRETKEY, bucketRegion);
            objectsFromBucket.Clear();

            if (BucketDropDown.SelectedIndex != -1)
            {
                var objects = await s3Client.ListObjectsAsync(BucketDropDown.SelectedItem.ToString());
                if (objects != null)
                {
                    var objectList = objects.S3Objects.Select(x => new S3Object { ObjectName = x.Key, Size = x.Size });
                    foreach (var item in objectList)
                    {
                        objectsFromBucket.Add(item);
                    }
                }

            }
        }

        private void btnHome_Click(object sender, RoutedEventArgs e)
        {
            App.mainWindow.Show();
            this.Hide();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }

        private void Window_Activated(object sender, EventArgs e)
        {
            
        }
    }
}
