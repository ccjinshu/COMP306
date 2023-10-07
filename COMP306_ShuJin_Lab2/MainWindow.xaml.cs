using COMP306_ShuJin_Lab2.AWS;
using System;
using System.Windows;

namespace COMP306_ShuJin_Lab2
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        AwsProxy awsProxy;

        public MainWindow()
        {
            InitializeComponent();

            //窗口居中 
            WindowStartupLocation = WindowStartupLocation.CenterScreen;


            // initAppDb();
            awsProxy = new AwsProxy();
            awsProxy.initAppDb();

            //regiest event : windows Closed
            this.Closed += MainWindow_Closed;
        }

        private void MainWindow_Closed(object? sender, EventArgs e)
        {
           //exit the application
            Application.Current.Shutdown();
        }


        //get all tables
        private async void btnRegister_Click(object sender, RoutedEventArgs e)
        {

            string username = UsernameTextBox.Text;
            string password = PasswordTextBox.Password;

            //register user in dynamodb 
            await awsProxy.RegisterUser(username, password);


        }

        private async void btnLogin_Click(object sender, RoutedEventArgs e)
        {

            string username = UsernameTextBox.Text;
            string password = PasswordTextBox.Password;

            var currentUser = await awsProxy.AuthenticateUser(username, password);

            if (currentUser != null)
            {
                // Successful Login
                // Navigate to Bookshelf page
                //MessageBox.Show("Login Successful");

                // Navigate to Bookshelf page
                this.Hide();
                MybooksWindow mybooksWindow = new MybooksWindow(currentUser);
                mybooksWindow.Show();
                

            }
            else
            {
                MessageBox.Show("Invalid Credentials");
            }
        }
    }
}
