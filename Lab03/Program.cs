using Amazon.SimpleSystemsManagement;
using Lab03.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.SqlServer.Storage.Internal;

namespace Lab03
{
    public class Program
    {
        public static void Main(string[] args)
        {

            ////get parameters from aws parameter store
            //var parameterStore = new AmazonSimpleSystemsManagementClient();
            //var parameterRequest = new Amazon.SimpleSystemsManagement.Model.GetParameterRequest();
            //parameterRequest.Name = "/Lab3/ConnectionString";
            //var parameterResponse = parameterStore.GetParameterAsync(parameterRequest).Result;
            //var connectionString = parameterResponse.Parameter.Value;

          




            var builder = WebApplication.CreateBuilder(args);
             

            var awsKey = builder.Configuration["AWSCredentials:AccesskeyID"];
            var awsSecret = builder.Configuration["AWSCredentials:Secretaccesskey"];


            builder.Configuration.AddSystemsManager("/COMP306_Lab03_Movie_App", new Amazon.Extensions.NETCore.Setup.AWSOptions
            {
                Region = Amazon.RegionEndpoint.CACentral1 ,
                Credentials = new Amazon.Runtime.BasicAWSCredentials(awsKey, awsSecret)

            });

            var connectionStringBuilder = new SqlConnectionStringBuilder(builder.Configuration.GetConnectionString("DefaultConnection_local"));
            //connectionStringBuilder.UserID = builder.Configuration["DbUser"];
            //connectionStringBuilder.Password = builder.Configuration["DbPassword"];


             








            // Add services to the container.
            builder.Services.AddControllersWithViews();

            //services.AddSession();
            builder.Services.AddSession (options =>
                           {
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });



            // Add services to the container.
            //builder.Services.AddDbContext<Lab3MovieWebContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection_local")));
            builder.Services.AddDbContext<Lab3MovieWebContext>(options => options.UseSqlServer(connectionStringBuilder.ConnectionString));





            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();
            app.UseSession();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}