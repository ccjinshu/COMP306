using COMP306_ShuJin_Lab2.AWS;
using System.Windows;

namespace COMP306_ShuJin_Lab2
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        AwsProxy awsProxy;

        public LoginWindow()
        {
            InitializeComponent();


            // initAppDb();
            awsProxy = new AwsProxy();
            awsProxy.initAppDb();
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
                MessageBox.Show("Login Successful");

                // Navigate to Bookshelf page


            }
            else
            {
                MessageBox.Show("Invalid Credentials");
            }
        }
    }
}
