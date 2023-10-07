using COMP306_ShuJin_Lab2.AWS;
using System.Windows;

namespace COMP306_ShuJin_Lab2
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {

        //public AwsProxy awsProxy;
        public App()
        {
            InitializeComponent();
            //Register Syncfusion license
            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("Ngo9BigBOggjHTQxAR8/V1NHaF5cXmVCf1JpQ3xbf1xzZFRMZVVbQHNPIiBoS35RdURjWXhfc3FcQmVUVEJ2");
            //awsProxy = new AwsProxy();
        }

    }
}
