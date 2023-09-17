using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Lab01
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {

        public static BucketOperation bucketOperation;
        public static MainWindow mainWindow;
        public static ObjectLevelOperations objectLevelOperations;

        protected override void OnStartup(StartupEventArgs e)
        {
            bucketOperation = new BucketOperation();
            mainWindow = new MainWindow();
            objectLevelOperations = new ObjectLevelOperations();
        }
    }
}
