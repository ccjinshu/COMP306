using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Amazon;
using Amazon.Runtime;
using System.Data;
using System.Drawing;
using Color = System.Drawing.Color;
using Table = Amazon.DynamoDBv2.DocumentModel.Table;
using Lab02.Models;
using Microsoft.Extensions.Configuration;
using System.IO;
using Amazon.S3;
using Lab02.AWS;

namespace Lab02
{
    /// <summary>d
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
         DynamoDBHelper dbHelper = new DynamoDBHelper();
        private User user;
        private string tableName = "Users";
        private DynamoDBContext context;


        static Connection conn = new Connection();
        readonly AmazonDynamoDBClient client = conn.Connect();


        public MainWindow()
        {
            InitializeComponent();

          this.creatingTable();

            

            user = new User();

            this.BtnLogin.IsEnabled = true;

        
        }


 

        private void TxtUserEmail_TextChanged(object sender, TextChangedEventArgs e)
        {
            user.UserEmail = TxtUserEmail.Text;
        }

        private void BtnSignup_Click(object sender, RoutedEventArgs e)
        {
            creatingTable();
        }

        private async void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            await loadDataAsync();
        }

        private async void creatingTable()
        {
            if (string.IsNullOrEmpty(TxtUserEmail.Text) || string.IsNullOrEmpty(TxtPassword.Password))
            {
                MessageBox.Show("Fields can't be empty!", "Alert", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
            else
            {
                context = new DynamoDBContext(client);
                List<string> currentTables = client.ListTablesAsync().Result.TableNames;


                if (!currentTables.Contains(tableName))
                {
                    await CreateUserTable(client, tableName);
                    await saveUser(context);
                }
                else
                {
                    await saveUser(context);
                }
                BtnLogin.IsEnabled = true;
            }
        }
        public static async Task CreateUserTable(AmazonDynamoDBClient client, string tableName)
        {
            var tableResponse = client.ListTablesAsync();
            if (!tableResponse.Result.TableNames.Contains(tableName))
            {
                await Task.Run(() =>
                {

                    client.CreateTableAsync(new CreateTableRequest
                    {
                        TableName = "Users",
                        ProvisionedThroughput = new ProvisionedThroughput { ReadCapacityUnits = 5, WriteCapacityUnits = 5 },
                        KeySchema = new List<KeySchemaElement>
                {
                    new KeySchemaElement
                    {
                        AttributeName="UserEmail",
                        KeyType=KeyType.HASH
                    }
                },
                        AttributeDefinitions = new List<AttributeDefinition>
                {
                    new AttributeDefinition
                    {
                        AttributeName="UserEmail",
                        AttributeType=ScalarAttributeType.S
                    }
                }
                    });

                    CreateTableRequest request = new CreateTableRequest
                    {
                        TableName = "Bookshelf",
                        AttributeDefinitions = new List<AttributeDefinition>()
                {
                    new AttributeDefinition
                    {
                        AttributeName = "UserEmail",
                        AttributeType = ScalarAttributeType.S
                    }
                },
                        KeySchema = new List<KeySchemaElement>
                {
                    new KeySchemaElement
                    {
                        AttributeName = "UserEmail",
                        KeyType = KeyType.HASH //Partition key
                    }
                },
                        ProvisionedThroughput = new ProvisionedThroughput
                        {
                            ReadCapacityUnits = 5,
                            WriteCapacityUnits = 5
                        },
                    };
                    var response = client.CreateTableAsync(request);
                    Thread.Sleep(5000);
                });
            }

        }
        private async Task saveUser(DynamoDBContext context)
        {
            bool userExisted;
            Table table = Table.LoadTable(client, "Users");
            Table table2 = Table.LoadTable(client, "Bookshelf");
            string email = TxtUserEmail.Text;
            Document doc = await table.GetItemAsync(email);
            if (doc == null)
            {
                userExisted = false;
            }
            else
            {
                userExisted = true;
            }

            user.Password = TxtPassword.Password;
            var book = new Document();
            book["UserEmail"] = email;
            book["BookTitle1"] = "AWS Certified Solutions Architect Study Guide, 2nd Edition by Ben Piper, David Clinton";
            book["DateTime1"] = DateTime.Now;
            book["LastPage1"] = 1;
            book["BookTitle2"] = "Beginning Serverless Computing Developing with Amazon Web Services, Microsoft Azure, and Google Cloud by Maddie Stigler";
            book["DateTime2"] = DateTime.Now;
            book["LastPage2"] = 1;
            book["BookTitle3"] = "Docker Complete Guide To Docker For Beginners And Intermediates by Berg, Craig";
            book["DateTime3"] = DateTime.Now;
            book["LastPage3"] = 1;

            if (userExisted)
            {
                MessageBox.Show("Account exists already!", "Alert", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                await Task.Run(() =>
                {
                    context.SaveAsync<User>(user);
                    table2.PutItemAsync(book);
                    MessageBox.Show("Account Created Successfully!", "Alert", MessageBoxButton.OK, MessageBoxImage.Information);
                });
            }
        }

        private async Task SaveUser(String UserName,    String Pwd)
        {
            bool userExisted;
            Table table = Table.LoadTable(client, "Users");
            Table table2 = Table.LoadTable(client, "Bookshelf");
            string email = TxtUserEmail.Text;
            Document doc = await table.GetItemAsync(email);
            if (doc == null)
            {
                userExisted = false;
            }
            else
            {
                userExisted = true;
            }

            user.Password = TxtPassword.Password;
            var book = new Document();
            book["UserEmail"] = email;
            book["BookTitle1"] = "AWS Certified Solutions Architect Study Guide, 2nd Edition by Ben Piper, David Clinton";
            book["DateTime1"] = DateTime.Now;
            book["LastPage1"] = 1;
            book["BookTitle2"] = "Beginning Serverless Computing Developing with Amazon Web Services, Microsoft Azure, and Google Cloud by Maddie Stigler";
            book["DateTime2"] = DateTime.Now;
            book["LastPage2"] = 1;
            book["BookTitle3"] = "Docker Complete Guide To Docker For Beginners And Intermediates by Berg, Craig";
            book["DateTime3"] = DateTime.Now;
            book["LastPage3"] = 1;

            if (userExisted)
            {
                MessageBox.Show("Account exists already!", "Alert", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                await Task.Run(() =>
                {
                    context.SaveAsync<User>(user);
                    table2.PutItemAsync(book);
                    MessageBox.Show("Account Created Successfully!", "Alert", MessageBoxButton.OK, MessageBoxImage.Information);
                });
            }
        }

        public async Task loadDataAsync()
        {

            Table table = Table.LoadTable(client, "Users");
            string email = TxtUserEmail.Text;
            string password = TxtPassword.Password;

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Fields can't be empty!", "Alert", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
            else
            {
                    Document doc = await table.GetItemAsync(email);
                if (doc == null)
                {
                    //show error message :Unregistered email
                    MessageBox.Show("Unregistered User!", "Alert", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                else
                {
                    string emailInput = doc.Values.ElementAt(1);
                    string userPasword = doc.Values.ElementAt(0);
                    string result = emailInput;
                    string pass = userPasword;
                    if (email == result & password == pass)
                    {
                        BooksList booksForm = new BooksList(emailInput);
                        MessageBox.Show("Successfully Logged In");
                        booksForm.Show();
                    }
                    else
                    {
                        MessageBox.Show("Incorrect Email or Password entered!");
                    }
                }

            }
        }
       
    }

}
