using COMP306_ShuJin_Project1.Data;
using COMP306_ShuJin_Project1.Repositories;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.SqlServer.Storage.Internal;
using Microsoft.OpenApi.Models;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace COMP306_ShuJin_Project1
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);



            //add dbcontext 

            //builder.Services.AddDbContext<COMP306_ShuJin_Project1.Data.ApplicationDbContext>(options => { 
            //    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection_google"));
            //});

            // This example uses a conditional check, but you might use an environment variable or other configuration setting
            //var useMySql = builder.Configuration.GetValue<bool>("UseMySql");
            //var usePostgreSql = builder.Configuration.GetValue<bool>("UsePostgreSql");

            //if (useMySql)
            //{


            //builder.Services.AddDbContext<ApplicationDbContext>(options =>
            //    options.UseMySql(builder.Configuration.GetConnectionString("MySqlConnection"),
            //        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("MySqlConnection"))));


            //}
            //else if (usePostgreSql)
            //{
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseNpgsql(builder.Configuration.GetConnectionString("PostgreSqlConnection3")));
            //}


            //register repositories 
            builder.Services.AddScoped<IRoomRepository, RoomRepository>(); 
            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<IBookingRepository, BookingRepository>();




            //register automapper


            builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            // Add services to the container.

            builder.Services.AddControllers();

            //add patch support
            builder.Services.AddControllers().AddNewtonsoftJson();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen( c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "COMP306_ShuJin_Project1 API", Version = "v1" });
                //http://localhost:8306/
                //custom server list
                c.AddServer(new OpenApiServer
                {
                    Url = "http://localhost:8306",
                    Description = "localhost  "
                });

                //custom server list
                c.AddServer(new OpenApiServer
                {
                    Url = "http://192.168.2.10:8306",
                    Description = "local Lan server"
                });

                c.AddServer(new OpenApiServer
                {
                    Url = "http://v6-510460395.ca-central-1.elb.amazonaws.com/",
                    Description = "AWS server"
                });

                c.AddServer(new OpenApiServer
                {
                    Url = "https://34.128.145.217.nip.io/bnb_auth_v1",
                    Description = "google proxy server (with apiKey) , https://my-project-apigee-test-406821-project1.apigee.io/docs/ccbnb/1/overview",

                });

                c.AddServer(new OpenApiServer
                {
                    Url = "https://34.128.145.217.nip.io/bnb_v1",
                    Description = "google proxy server , https://my-project-apigee-test-406821-project1.apigee.io/docs/ccbnb/1/overview",

                });

 




            });

            //allow cors
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", builder =>
                {
                    builder.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader();
                });
            });


            var app = builder.Build();

            // Configure the HTTP request pipeline.
            //if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                //app.UseSwaggerUI();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Comp306_ShuJin_Project1 API v1"); 
                    c.RoutePrefix =   string.Empty;
                });

            }

            //app.UseHttpsRedirection();


            app.UseAuthorization();

            app.UseCors("AllowSpecificOrigin");

            app.MapControllers();

            app.Run();
        }
    }
}