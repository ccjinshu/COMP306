using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.Model;
using Amazon.Runtime;
using Amazon.S3;
using COMP306_ShuJin_Lab2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace COMP306_ShuJin_Lab2.AWS
{


    //2222
    public class AwsProxy
    {
        readonly string ACCESSKEY = "AKIAX3P3FTUF3RFUKNOA";
        readonly string SECRETKEY = "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";
        private const string TABLE_NAME_USERS = "Users";
        private const string TABLE_NAME_BOOKSHELF = "Bookshelf";


        readonly IAmazonDynamoDB _client;
        readonly IAmazonS3 _s3Client;
        DynamoDBContext _context;

        //constructor

        public AwsProxy()
        {
            var credentials = new BasicAWSCredentials(ACCESSKEY, SECRETKEY); // Update with your AWS access and secret keys


            _client = new AmazonDynamoDBClient(credentials, Amazon.RegionEndpoint.CACentral1);
            _s3Client = new AmazonS3Client(credentials, RegionEndpoint.CACentral1);
            _context = new DynamoDBContext(_client);
        }




        //check if table exist
        public async Task<bool> isTableExist(string tableName)
        {
            var tables = await _client.ListTablesAsync();
            return tables.TableNames.Contains(tableName);
        }




        public async Task initAppDb()
        {
            //create table User
            await createTable_Users();
            //gen 3 users
            await Simple_Gen_Three_Users();
            //create table Book
            //await createTable_Bookshelf();

            //gen books data
            //await Simple_Gen_Books();

        }

        public async Task createTable_Users()
        {
            String TABLENAME = "Users";
            //if table not exist, create it
            var isTableExists = await isTableExist(TABLENAME);
            if (!isTableExists)
            {
                try
                {
                    var request = new CreateTableRequest
                    {
                        TableName = TABLENAME,
                        AttributeDefinitions = new List<AttributeDefinition>
                {
                    new AttributeDefinition
                    {
                        AttributeName = "UserId",
                        AttributeType = "S" // String type for UserId
                    }
                },
                        KeySchema = new List<KeySchemaElement>
                {
                    new KeySchemaElement
                    {
                        AttributeName = "UserId",
                        KeyType = "HASH" // Partition key
                    }

                },
                        ProvisionedThroughput = new ProvisionedThroughput
                        {
                            ReadCapacityUnits = 5,
                            WriteCapacityUnits = 5
                        }
                    };

                    var response = await _client.CreateTableAsync(request);

                }
                catch (Exception ex)
                {
                    // Handle any exceptions
                    Console.WriteLine("Error creating " + TABLENAME + " table: " + ex.Message);
                }
            }



        }
        public async Task InsertItemToTable(String tableName, Object obj)
        {

            string TABLENAME = tableName;
            var isTableActive = false;
            while (!isTableActive)
            {
                var tableDescription = await _client.DescribeTableAsync(TABLENAME);
                isTableActive = string.Equals(tableDescription.Table.TableStatus, "ACTIVE", StringComparison.OrdinalIgnoreCase);
                if (!isTableActive)
                {
                    // Sleep for a short while before checking again
                    await Task.Delay(5000);
                }
            }
            await _context.SaveAsync(obj);
        }
        //init tree users to table User
        public async Task Simple_Gen_Three_Users()
        {
            string TABLENAME = "Users";
            var isTableActive = false;
            while (!isTableActive)
            {
                var tableDescription = await _client.DescribeTableAsync(TABLENAME);
                isTableActive = string.Equals(tableDescription.Table.TableStatus, "ACTIVE", StringComparison.OrdinalIgnoreCase);
                if (!isTableActive)
                {
                    // Sleep for a short while before checking again
                    await Task.Delay(5000);
                }
            }
            var user1 = new User("1", "sjin@gmail.com", "123");
            var user2 = new User("2", "sjin@hotmail.com", "123");
            var user3 = new User("3", "sjin32@my.centennialcollege.ca", "123");
            await _context.SaveAsync(user1);
            await _context.SaveAsync(user2);
            await _context.SaveAsync(user3);


        }

        //register user
        public async Task RegisterUser(String username, String password)
        {
            //check if the user exists
            var request = new ScanRequest
            {
                TableName = "Users",
                FilterExpression = "#username = :username",
                ExpressionAttributeNames = new Dictionary<string, string>
                {
                    { "#username", "Username" }
                },
                ExpressionAttributeValues = new Dictionary<string, AttributeValue>
                {
                    { ":username", new AttributeValue { S = username } }
                }
            };

            //scan the table

            var response = await _client.ScanAsync(request);
            //check if the user exists

            if (response.Items.Count > 0)
            {
                MessageBox.Show("User already exists");
            }
            else
            {

                //get max Userid : select *  from   Users  order by UserId desc limit 1;
                //query the table to get the max UserId like sql select *  from   Users  order by UserId desc limit 1;
                //use orderby to sort the result
                //use limit to get the first row

                var scanRequest = new ScanRequest
                {
                    TableName = "Users",
                };

                var scanResponse = await _client.ScanAsync(scanRequest);
                var users = scanResponse.Items;

                if (users != null && users.Count > 0)
                {
                    // Assuming UserId is of type Number in DynamoDB
                    var maxUserIdItem = users.OrderByDescending(u => Decimal.Parse(u["UserId"].S)).FirstOrDefault();
                    var maxUserId = maxUserIdItem["UserId"].S;
                    maxUserId = (int.Parse(maxUserId) + 1).ToString();
                    Console.WriteLine("Max UserId: " + maxUserId);
                    var user = new User(maxUserId, username, password);
                    await _context.SaveAsync(user);
                    //show info
                    MessageBox.Show("User registered successfully");

                }






            }




        }


        public async Task InsertItem()
        {
            PutItemRequest request = new PutItemRequest
            {
                TableName = "Users",
                Item = new Dictionary<string, AttributeValue>()
                {
                    { "Username", new AttributeValue { S = ""}}
                }
            };
            try
            {
                var response = await _client.PutItemAsync(request);
            }
            catch (InternalServerErrorException ex)
            {
                Console.WriteLine("An error occurred on the server side " + ex.Message);
            }
            catch (ResourceNotFoundException ex)
            {
                Console.WriteLine("The operation tried to access a noneexistent table or index." + ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public async Task createTable_Bookshelf()
        {

            String TABLENAME = "Bookshelf";
            var isTableExists = await isTableExist(TABLENAME);
            if (!isTableExists)
            {

                var request = new CreateTableRequest
                {
                    TableName = "Bookshelf",
                                    AttributeDefinitions = new List<AttributeDefinition>
                    {
                        new AttributeDefinition
                        {
                            AttributeName = "Id",
                            AttributeType = "S"
                        },
                        //new AttributeDefinition
                        //{
                        //    AttributeName = "UserId",
                        //    AttributeType = "S"
                        //},
                        //new AttributeDefinition
                        //{
                        //    AttributeName = "BookmarkTime",
                        //    AttributeType = "N"  // 假设你存储的是Unix时间戳或其他数字格式
                        //}
                    },
                                    KeySchema = new List<KeySchemaElement>
                    {
                        new KeySchemaElement
                        {
                            AttributeName = "Id",
                            KeyType = "HASH"
                        }
                    },
                    ProvisionedThroughput = new ProvisionedThroughput
                    {
                        ReadCapacityUnits = 5,
                        WriteCapacityUnits = 5
                    },
    //                // 为UserId和BookmarkTime设置GSI
    //                GlobalSecondaryIndexes = new List<GlobalSecondaryIndex>
    //{
    //    new GlobalSecondaryIndex
    //    {
    //        IndexName = "UserId-BookmarkTime-Index",
    //        KeySchema = new List<KeySchemaElement>
    //        {
    //            new KeySchemaElement
    //            {
    //                AttributeName = "UserId",
    //                KeyType = "HASH"
    //            },
    //            new KeySchemaElement
    //            {
    //                AttributeName = "BookmarkTime",
    //                KeyType = "RANGE"
    //            }
    //        },
    //        Projection = new Projection
    //        {
    //            ProjectionType = "ALL"
    //        },
    //        ProvisionedThroughput = new ProvisionedThroughput
    //        {
    //            ReadCapacityUnits = 5,
    //            WriteCapacityUnits = 5
    //        }
    //    }
    //}
               
                
                };

                var response = await _client.CreateTableAsync(request);


            }
        }

        //Auth username and password ,return user object
        public async Task<User> AuthenticateUser(string username, string password)
        {

            var request = new ScanRequest
            {
                TableName = "Users",
                FilterExpression = "#username = :username and #password = :password",
                ExpressionAttributeNames = new Dictionary<string, string>
                {
                    { "#username", "Username" },
                    { "#password", "Password" }
                },
                ExpressionAttributeValues = new Dictionary<string, AttributeValue>
                {
                    { ":username", new AttributeValue { S = username } },
                    { ":password", new AttributeValue { S = password } }
                }
            };

            //scan the table

            var response = _client.ScanAsync(request);
            //check if the user exists

            if (response.Result.Items.Count > 0)
            {
                var user = new User();
                user.UserId = response.Result.Items[0]["UserId"].S;
                user.Username = response.Result.Items[0]["Username"].S;
                user.Password = response.Result.Items[0]["Password"].S;
                return user;
            }
            else
            {
                return null;
            }


        }




        //get my books from bookshelf
        public async Task<List<Book>> GetBooksByUserId(string userId)
        {

            var request = new ScanRequest
            {
                TableName = "Bookshelf",
                FilterExpression = "#userId = :userId",
                ExpressionAttributeNames = new Dictionary<string, string>
                {
                    { "#userId", "UserId" }
                },
                ExpressionAttributeValues = new Dictionary<string, AttributeValue>
                {
                    { ":userId", new AttributeValue { S = userId } }
                }
            };

            var response = await _client.ScanAsync(request);
            var books = new List<Book>();

            //一共几本书？show message box
            if (response.Items.Count == 0)
            {
                //MessageBox.Show("No books found");
            }
            else
            {
                //MessageBox.Show("You have " + response.Items.Count + " books"); 

                //result.Items is a list of dictionary ,to list
               
                foreach (var item in response.Items)
                {
                  

                    var book = new Book();
                    book.Id = item["Id"].S;
                    book.UserId = item["UserId"].S; 
                    book.Title = item["Title"].S;
                    book.Author = item["Author"].S;
                    book.BookmarkPage = int.Parse(item["BookmarkPage"].N);
                    //book.BookmarkTime = DateTime.Parse(item["BookmarkTime"].N);
                    //book.FileUrl = item["FileUrl"].S;
                    //book.FileKey = item["FileKey"].S;
                    //book.FilePath = item["FilePath"].S;

                    DateTime parsedDate;
                    if (DateTime.TryParse(item["BookmarkTime"].S, out parsedDate))
                    {
                        book.BookmarkTime = parsedDate;
                    }

                    book.FileUrl = item["FileUrl"].S;
                    book.FileKey = item["FileKey"].S;

                    books.Add(book);
                }


                //sort by bookmark time
                books = books.OrderByDescending(b => b.BookmarkTime).ToList();
                 

            }

            return books;
        }



        public async Task UpdateBookmark(Book book )
        {
            try
            {
               
                await _context.SaveAsync(book);
            }
            catch (Exception ex)
            {
                // Handle any exceptions
                Console.WriteLine("Error updating bookmark: " + ex.Message);
            }
        }



        public async Task Simple_Gen_Books()
        {
            string TABLENAME = "Bookshelf";
            var isTableActive = false;
            while (!isTableActive)
            {
                var tableDescription = await _client.DescribeTableAsync(TABLENAME);
                isTableActive = string.Equals(tableDescription.Table.TableStatus, "ACTIVE", StringComparison.OrdinalIgnoreCase);
                if (!isTableActive)
                {
                    // Sleep for a short while before checking again
                    await Task.Delay(5000);
                }
            }

            var list = new List<Book>(); 

            list.Add(new Book
            {
                Id = "1",
                UserId = "1",
                BookId = "1",
                Title = "COMP306 - Lab#1",
                Author = "CC",
                BookmarkPage = 1,
                BookmarkTime = DateTime.Now,
                FileUrl = "https://s3-comp306-sjin-lab02.s3.ca-central-1.amazonaws.com/Lab%231.pdf",
                FileKey = "Lab#1.pdf",
            });
            list.Add(new Book
            {
                Id = "2",
                UserId = "1",
                BookId = "2",
                Title = "COMP306 - Lab#2",
                Author = "CC",
                BookmarkPage = 1,
                BookmarkTime = DateTime.Now,
                FileUrl = "https://s3-comp306-sjin-lab02.s3.ca-central-1.amazonaws.com/Lab%232+(2).pdf",
                FileKey = "Lab#2 (2).pdf",
            });

            list.Add(new Book
            {
                Id = "3",
                UserId = "1",
                BookId = "3",
                Title = "SQL LOADER UTILITY",
                Author = "R.Barry",
                BookmarkPage = 1,
                BookmarkTime = DateTime.Now,
                FileUrl = "https://s3-comp306-sjin-lab02.s3.ca-central-1.amazonaws.com/SQL+LOADER+UTILITY+1133947360_350549.pdf",
                FileKey = "SQL LOADER UTILITY 1133947360_350549.pdf",
            });

            list.Add(new Book
            {
                Id = "4",
                UserId = "2",
                BookId = "4",
                Title = "STATEMENT TUNING",
                Author = "CC",
                BookmarkPage = 1,
                BookmarkTime = DateTime.Now,
                FileUrl = "https://s3-comp306-sjin-lab02.s3.ca-central-1.amazonaws.com/STATEMENT+TUNING+1133947360_350548.pdf",
                FileKey = "STATEMENT TUNING 1133947360_350548.pdf",
            });

            list.Add(new Book
            {
                Id = "5",
                UserId = "2",
                BookId = "5",
                Title = "COMM253  - Letter Formats",
                Author = "CC English",
                BookmarkPage = 1,
                BookmarkTime = DateTime.Now,
                FileUrl = "https://s3-comp306-sjin-lab02.s3.ca-central-1.amazonaws.com/letter+formats.pdf",
                FileKey = "letter formats.pdf",
            });

            

            list.Add(new Book
            {
                Id = "6",
                UserId = "2",
                BookId = "6",
                Title = "COMP_306 Outline",
                Author = "CC COMP306",
                BookmarkPage = 1,
                BookmarkTime = DateTime.Now,
                FileUrl = "https://s3-comp306-sjin-lab02.s3.ca-central-1.amazonaws.com/COMP_3061.pdf",
                FileKey = "COMP_3061.pdf",
            });

            list.Add(new Book
            {
                Id = "7",
                UserId = "3",
                BookId = "7",
                Title = "253 Week 4 22W",
                Author = "CC",
                BookmarkPage = 1,
                BookmarkTime = DateTime.Now,
                FileUrl = "https://s3-comp306-sjin-lab02.s3.ca-central-1.amazonaws.com/253+Week+4+22W.pdf",
                FileKey = "253 Week 4 22W.pdf",
            });

            list.Add(new Book
            {
                Id = "8",
                UserId = "3",
                BookId = "8",
                Title = "TOOL FOR ORACLE APPLICATION DEVELOPERS",
                Author = "CC",
                BookmarkPage = 1,
                BookmarkTime = DateTime.Now,
                FileUrl = "https://s3-comp306-sjin-lab02.s3.ca-central-1.amazonaws.com/TOAD+(TOOL+FOR+ORACLE+APPLICATION+DEVELOPERS)+1133947360_350547.pdf",
                FileKey = "TOAD (TOOL FOR ORACLE APPLICATION DEVELOPERS) 1133947360_350547.pdf",
            });


            /// --------------------
            list.Add(new Book
            {
                Id = "9",
                UserId = "3",
                BookId = "9",
                Title = "SQL LOADER UTILITY",
                Author = "R.Barry",
                BookmarkPage = 1,
                BookmarkTime = DateTime.Now,
                FileUrl = "https://s3-comp306-sjin-lab02.s3.ca-central-1.amazonaws.com/SQL+LOADER+UTILITY+1133947360_350549.pdf",
                FileKey = "SQL LOADER UTILITY 1133947360_350549.pdf",
            });

            list.Add(new Book
            {
                Id = "10",
                UserId = "4",
                BookId = "10",
                Title = "STATEMENT TUNING",
                Author = "CC",
                BookmarkPage = 1,
                BookmarkTime = DateTime.Now,
                FileUrl = "https://s3-comp306-sjin-lab02.s3.ca-central-1.amazonaws.com/STATEMENT+TUNING+1133947360_350548.pdf",
                FileKey = "STATEMENT TUNING 1133947360_350548.pdf",
            });

            list.Add(new Book
            {
                Id = "11",
                UserId = "4",
                BookId = "11",
                Title = "COMM253  - Letter Formats",
                Author = "CC English",
                BookmarkPage = 1,
                BookmarkTime = DateTime.Now,
                FileUrl = "https://s3-comp306-sjin-lab02.s3.ca-central-1.amazonaws.com/letter+formats.pdf",
                FileKey = "letter formats.pdf",
            });



            list.Add(new Book
            {
                Id = "12",
                UserId = "4",
                BookId = "12",
                Title = "COMP_306 Outline",
                Author = "CC COMP306",
                BookmarkPage = 1,
                BookmarkTime = DateTime.Now,
                FileUrl = "https://s3-comp306-sjin-lab02.s3.ca-central-1.amazonaws.com/COMP_3061.pdf",
                FileKey = "COMP_3061.pdf",
            });

            list.Add(new Book
            {
                Id = "13",
                UserId = "5",
                BookId = "13",
                Title = "253 Week 4 22W",
                Author = "CC",
                BookmarkPage = 1,
                BookmarkTime = DateTime.Now,
                FileUrl = "https://s3-comp306-sjin-lab02.s3.ca-central-1.amazonaws.com/253+Week+4+22W.pdf",
                FileKey = "253 Week 4 22W.pdf",
            });

            list.Add(new Book
            {
                Id = "14",
                UserId = "5",
                BookId = "14",
                Title = "TOOL FOR ORACLE APPLICATION DEVELOPERS",
                Author = "CC",
                BookmarkPage = 1,
                BookmarkTime = DateTime.Now,
                FileUrl = "https://s3-comp306-sjin-lab02.s3.ca-central-1.amazonaws.com/TOAD+(TOOL+FOR+ORACLE+APPLICATION+DEVELOPERS)+1133947360_350547.pdf",
                FileKey = "TOAD (TOOL FOR ORACLE APPLICATION DEVELOPERS) 1133947360_350547.pdf",
            });
            list.Add(new Book
            {
                Id = "15",
                UserId = "5",
                BookId = "15",
                Title = "COMM253  - Letter Formats",
                Author = "CC English",
                BookmarkPage = 1,
                BookmarkTime = DateTime.Now,
                FileUrl = "https://s3-comp306-sjin-lab02.s3.ca-central-1.amazonaws.com/letter+formats.pdf",
                FileKey = "letter formats.pdf",
            });


            //save to db
            foreach (var item in list)
            {
                try
                {
                    await _context.SaveAsync(item);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error saving book: " + ex.Message);
                }

            }


        }




    }


}

