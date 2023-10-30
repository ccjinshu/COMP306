using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lab03.Models;

namespace Lab03.AWS
{
    public class DynamoDBHelper
    {
        static Connection conn = new Connection();

        private readonly AmazonDynamoDBClient _dynamoClient;
        private readonly IDynamoDBContext _dynamoDBContext;
        private readonly string _moviesTableName = "Movies"; // 电影表名称
        private readonly string _commentsTableName = "Comments"; // 电影评论表名称

        public DynamoDBHelper()
        {
            _dynamoClient = conn.Connect();
            // 初始化 DynamoDBContext
            _dynamoDBContext = new DynamoDBContext(_dynamoClient);

          


        }

        //check if table exists

        public async Task<bool> ExistsTable(string tableName)
        {
            var tableResponse = await _dynamoClient.ListTablesAsync();
            return tableResponse.TableNames.Contains(tableName);
        }



        public async Task<Document> GetUserAsync(string userName)
        {
            var table = Table.LoadTable(_dynamoClient, "Users");
            return await table.GetItemAsync(userName);
        }




        //Programmatically create a DynamoDB table to store user’s credentials (user email name & password)

        public async Task CreateTableMoives()
        {

            //check if table exists
            if (await ExistsTable("Movies"))
            {
                return;
            }



            AmazonDynamoDBClient client = _dynamoClient;
            // 创建 DynamoDB 表的请求
            var createTableRequest = new CreateTableRequest
            {
                TableName = "Movies", // 表名
                KeySchema = new List<KeySchemaElement>
    {
        new KeySchemaElement
        {
            AttributeName = "Id", // 主键名
            KeyType = KeyType.HASH // 主键类型 (HASH 表示哈希键)
        }
    },
                AttributeDefinitions = new List<AttributeDefinition>
    {
        new AttributeDefinition
        {
            AttributeName = "Id", // 主键名
            AttributeType = ScalarAttributeType.S // 主键类型 (N 表示数字)
        },
        new AttributeDefinition
        {
            AttributeName = "Genre", // 电影类型属性名
            AttributeType = ScalarAttributeType.N // 电影类型属性类型 (S 表示字符串)
        },
                new AttributeDefinition
        {
            AttributeName = "Rating", // 评分属性名
            AttributeType = ScalarAttributeType.N // 评分属性类型 (N 表示数字)
        }
    },
                ProvisionedThroughput = new ProvisionedThroughput
                {
                    ReadCapacityUnits = 5, // 读取容量单位
                    WriteCapacityUnits = 5 // 写入容量单位
                },
                GlobalSecondaryIndexes = new List<GlobalSecondaryIndex>
    {
        // 创建评分索引
        new GlobalSecondaryIndex
        {
            IndexName = "RatingIndex", // 索引名称
            KeySchema = new List<KeySchemaElement>
            {
                new KeySchemaElement
                {
                    AttributeName = "Rating", // 索引键名
                    KeyType = KeyType.HASH // 索引键类型
                }
            },
            Projection = new Projection
            {
                ProjectionType = ProjectionType.ALL // 投影类型 (使用 ALL 表示投影所有属性)
            },
            ProvisionedThroughput = new ProvisionedThroughput
            {
                ReadCapacityUnits = 5, // 读取容量单位
                WriteCapacityUnits = 5 // 写入容量单位
            }
        },
        // 创建电影类型索引
        new GlobalSecondaryIndex
        {
            IndexName = "GenreIndex", // 索引名称
            KeySchema = new List<KeySchemaElement>
            {
                new KeySchemaElement
                {
                    AttributeName = "Genre", // 索引键名
                    KeyType = KeyType.HASH // 索引键类型
                }
            },
            Projection = new Projection
            {
                ProjectionType = ProjectionType.ALL // 投影类型 (使用 ALL 表示投影所有属性)
            },
            ProvisionedThroughput = new ProvisionedThroughput
            {
                ReadCapacityUnits = 5, // 读取容量单位
                WriteCapacityUnits = 5 // 写入容量单位
            }
        }
    }
            };




            var response = await client.CreateTableAsync(createTableRequest);
        }

        // 获取全部电影列表
        public async Task<List<Movie>> GetAllMoviesAsync()
        { 


            var scanRequest = new ScanRequest
            {
                TableName = _moviesTableName // 电影表名称
            };

            var response = await _dynamoClient.ScanAsync(scanRequest);

            // 将 response.Items 转换为 LINQ 查询集合
            var allMovies = response.Items.Select(item =>
            {
                // 将 DynamoDB 文档转换为 Movie 对象
                var movie = _dynamoDBContext.FromDocument<Movie>(Document.FromAttributeMap(item));
                return movie;
            }).ToList();

            return allMovies;
        }

         
        // 添加电影 , return movie id
        public async Task<Movie> AddMovieAsync(Movie movie)
        {
            await _dynamoDBContext.SaveAsync(movie);
            return movie;
        }

        // 获取电影信息
        public async Task<Movie> GetMovieAsync(string id)
        {
            return await _dynamoDBContext.LoadAsync<Movie>(id);
        }

        // 更新电影信息
        public async Task<Movie> UpdateMovieAsync(Movie movie)
        {
            await _dynamoDBContext.SaveAsync(movie);
            return movie;
        }

        // 删除电影
        public async Task DeleteMovieAsync(string id)
        {
            await _dynamoDBContext.DeleteAsync<Movie>(id);
           
        }

        //find movies by filters  
        public async Task<List<Movie>> QueryMoviesByFiltersAsync(MovieGenre genre, double? minRating, double? maxRating)
        {
            //IndexName : GenreIndex or RatingIndex

            string useIndex = null;

            List<ScanCondition> filters = new List<ScanCondition>(); 
            if (genre != MovieGenre.None)
            {
                filters.Add(new ScanCondition("Genre", ScanOperator.Equal, genre));
                useIndex = "GenreIndex";
            }


            if (minRating != null) //compare with min rating
            {
                filters.Add(new ScanCondition("Rating", ScanOperator.GreaterThanOrEqual, minRating));
                useIndex = useIndex == null ? "RatingIndex" : useIndex;
            }

            if (maxRating != null) //compare with max rating
            {
                filters.Add(new ScanCondition("Rating", ScanOperator.LessThanOrEqual, maxRating));
                useIndex = useIndex == null ? "RatingIndex" : useIndex;

            }

            var config = new DynamoDBOperationConfig
            {
                IndexName = useIndex, // 使用 GenreIndex 二级索引
            };

            var  movies = await _dynamoDBContext.ScanAsync<Movie> (filters).GetRemainingAsync();
             

            return movies;
        }


        // 查询电影列表
        public async Task<List<Movie>> QueryMoviesByGenreAsync(MovieGenre genre)
        {
            var config = new DynamoDBOperationConfig
            {
                IndexName = "GenreIndex", // 使用 GenreIndex 二级索引
                QueryFilter = new List<ScanCondition>
            {
                new ScanCondition("Genre", ScanOperator.Equal, genre)
            }
            };

            return await _dynamoDBContext.QueryAsync<Movie>(genre, config).GetRemainingAsync();
        }

        // 查询高评分电影列表
        public async Task<List<Movie>> QueryHighRatedMoviesAsync(decimal ratingThreshold)
        {
            var config = new DynamoDBOperationConfig
            {
                IndexName = "RatingIndex", // 使用 RatingIndex 二级索引
                QueryFilter = new List<ScanCondition>
            {
                new ScanCondition("Rating", ScanOperator.GreaterThan, ratingThreshold)
            }
            };

            return await _dynamoDBContext.QueryAsync<Movie>(ratingThreshold, config).GetRemainingAsync();
        }

        // 添加电影评论
        public async Task AddCommentAsync(Comment comment)
        {
            await _dynamoDBContext.SaveAsync(comment);
        }

        // 获取电影评论列表
        public async Task<List<Comment>> GetMovieCommentsAsync(string movieId)
        {
            var config = new DynamoDBOperationConfig
            {
                QueryFilter = new List<ScanCondition>
            {
                new ScanCondition("MovieId", ScanOperator.Equal, movieId)
            }
            };

            return await _dynamoDBContext.QueryAsync<Comment>(movieId, config).GetRemainingAsync();
        }

        // 更新电影评论
        public async Task UpdateCommentAsync(Comment updatedComment)
        {
            await _dynamoDBContext.SaveAsync(updatedComment);
        }



        public async Task GenerateMoviesData()
        {
            //if table exists, return 
            if (!await ExistsTable("Movies"))
            {
                //create table if not exists
                await CreateTableMoives();
            }
            else
            {
                //get status of table (active or not)
                var tableStatus = await _dynamoClient.DescribeTableAsync("Movies");
                if (tableStatus.Table.TableStatus != TableStatus.ACTIVE)
                {
                     return;
                }

            }



            var movies = new List<Movie>
    {
        new Movie
        {
            //Id = 1,
            Title = "The Good, the Bad and the Ugly",
            Genre =  MovieGenre.Western,
            Director = "Sergio Leone",
            ReleaseTime = new DateTime(1966, 12, 29),
            Rating = 8.8,
            CoverImage = "https://i.imgur.com/yvNpc8k.jpeg",
            FileKey = "good_bad_ugly.mp4",
            FileUrl = "https://example.com/movies/good_bad_ugly.mp4",
            Comments = new List<Comment>
            {
                new Comment
                {
                    //Id = 1,
                    Content = "Great movie!",
                    UserId = "1",
                    UpdateTime = DateTime.UtcNow,
                    Rating = 9.0
                },
                new Comment
                {
                    //Id = 2,
                    Content = "One of the best Westerns!",
                    UserId = "2",
                    UpdateTime = DateTime.UtcNow,
                    Rating = 8.5
                }
            }
        },
        new Movie
        {
            //Id = 2,
            Title = "Die Hard",
            Genre =  MovieGenre.Action,
            Director = "John McTiernan",
            ReleaseTime = new DateTime(1988, 7, 15),
            Rating = 8.2,
            CoverImage = "https://i.imgur.com/wye0WpU.jpeg",
            FileKey = "die_hard.mp4",
            FileUrl = "https://example.com/movies/die_hard.mp4",
            Comments = new List<Comment>
            {
                new Comment
                {
                    //Id = 3,
                    Content = "Awesome action scenes!",
                    UserId = "3",
                    UpdateTime = DateTime.UtcNow,
                    Rating = 8.7
                },
                new Comment
                {
                    //Id = 4,
                    Content = "Classic action movie!",
                    UserId = "4",
                    UpdateTime = DateTime.UtcNow,
                    Rating = 8.5
                }
            }
        },
        new Movie
        {
            //Id = 3,
            Title = "Star Wars: Episode IV - A New Hope",
            Genre =  MovieGenre.Science,
            Director = "George Lucas",
            ReleaseTime = new DateTime(1977, 5, 25),
            Rating = 8.6,
            CoverImage = "https://i.imgur.com/onxfmOE.jpeg",
            FileKey = "star_wars.mp4",
            FileUrl = "https://example.com/movies/star_wars.mp4",
            Comments = new List<Comment>
            {
                new Comment
                {
                    //Id = 5,
                    Content = "Epic space adventure!",
                    UserId = "5",
                    UpdateTime = DateTime.UtcNow,
                    Rating = 9.0
                },
                new Comment
                {
                    //Id = 6,
                    Content = "May the Force be with you!",
                    UserId = "1",
                    UpdateTime = DateTime.UtcNow,
                    Rating = 8.8
                }
            }
        },
        new Movie
        {
            //Id = 4,
            Title = "Toy Story",
            Genre =  MovieGenre.Animation,
            Director = "John Lasseter",
            ReleaseTime = new DateTime(1995, 11, 22),
            Rating = 8.3,
            CoverImage = "https://i.imgur.com/KRWBSzO.jpeg",
            FileKey = "toy_story.mp4",
            FileUrl = "https://example.com/movies/toy_story.mp4",
            Comments = new List<Comment>
            {
                new Comment
                {
                    //Id = 7,
                    Content = "Fun for all ages!",
                    UserId = "1",
                    UpdateTime = DateTime.UtcNow,
                    Rating = 8.5
                },
                new Comment
                {
                    //Id = 8,
                    Content = "Love the characters!",
                    UserId = "2",
                    UpdateTime = DateTime.UtcNow,
                    Rating = 8.0
                }
            }
        },
        new Movie
        {
            //Id = 5,
            Title = "Avatar",
            Genre =  MovieGenre.Science,
            Director = "James Cameron",
            ReleaseTime = new DateTime(2009, 12, 18),
            Rating = 7.8,
            CoverImage = "https://i.imgur.com/t3YTePb.png",
            FileKey = "avatar.mp4",
            FileUrl = "https://example.com/movies/avatar.mp4",
            Comments = new List<Comment>
            {
                new Comment
                {
                    //Id = 9,
                    Content = "Stunning visuals!",
                    UserId = "1",
                    UpdateTime = DateTime.UtcNow,
                    Rating = 8.0
                },
                new Comment
                {
                    //Id = 10,
                    Content = "Great world-building!",
                    UserId = "2",
                    UpdateTime = DateTime.UtcNow,
                    Rating = 8.2
                }
            }
        },
        new Movie
        {
            //Id = 6,
            Title = "Inception",
            Genre =  MovieGenre.Science,
            Director = "Christopher Nolan",
            ReleaseTime = new DateTime(2010, 7, 16),
            Rating = 8.8,
            CoverImage = "https://i.imgur.com/ETiM8hT.jpeg",
            FileKey = "inception.mp4",
            FileUrl = "https://example.com/movies/inception.mp4",
            Comments = new List<Comment>
            {
                new Comment
                {
                    //Id = 11,
                    Content = "Mind-bending!",
                    UserId = "1",
                    UpdateTime = DateTime.UtcNow,
                    Rating = 9.0
                },
                new Comment
                {
                    //Id = 12,
                    Content = "Great concept!",
                    UserId = "2",
                    UpdateTime = DateTime.UtcNow,
                    Rating = 8.5
                }
            }
        },
        new Movie
        {
            //Id = 7,
            Title = "Jurassic Park",
            Genre =  MovieGenre.Science,
            Director = "Steven Spielberg",
            ReleaseTime = new DateTime(1993, 6, 11),
            Rating = 8.1,
            CoverImage = "https://i.imgur.com/M3UuUh7.jpeg",
            FileKey = "jurassic_park.mp4",
            FileUrl = "https://example.com/movies/jurassic_park.mp4",
            Comments = new List<Comment>
            {
                new Comment
                {
                    //Id = 13,
                    Content = "Dinosaurs are awesome!",
                    UserId = "1",
                    UpdateTime = DateTime.UtcNow,
                    Rating = 8.3
                },
                new Comment
                {
                    //Id = 14,
                    Content = "Classic Spielberg!",
                    UserId = "2",
                    UpdateTime = DateTime.UtcNow,
                    Rating = 8.0
                }
            }
        },
        new Movie
        {
            //Id = 8,
            Title = "The Lion King",
            Genre =  MovieGenre.Animation,
            Director = "Roger Allers, Rob Minkoff",
            ReleaseTime = new DateTime(1994, 6, 15),
            Rating = 8.5,
            CoverImage = "https://i.imgur.com/En4oaVc.jpeg",
            FileKey = "lion_king.mp4",
            FileUrl = "https://example.com/movies/lion_king.mp4",
            Comments = new List<Comment>
            {
                new Comment
                {
                    //Id = 15,
                    Content = "Hakuna Matata!",
                    UserId = "2",
                    UpdateTime = DateTime.UtcNow,
                    Rating = 8.7
                },
                new Comment
                {
                    //Id = 16,
                    Content = "Heartwarming story!",
                    UserId = "1",
                    UpdateTime = DateTime.UtcNow,
                    Rating = 8.3
                }
            }
        },
        // add more movies here...
    };

            foreach (var movie in movies)
            {
                await this.AddMovieAsync(movie); // 插入电影数据到 DynamoDB 表
            }
        }





    }
}
