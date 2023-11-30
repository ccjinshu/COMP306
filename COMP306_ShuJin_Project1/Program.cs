using COMP306_ShuJin_Project1.Data;
using COMP306_ShuJin_Project1.Repositories;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.SqlServer.Storage.Internal;


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
                options.UseNpgsql(builder.Configuration.GetConnectionString("PostgreSqlConnection2")));
            //}


            //register repositories 
            builder.Services.AddScoped<IRoomRepository, RoomRepository>();

            //register automapper

             
            builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            // Add services to the container.

            builder.Services.AddControllers();

            //add patch support
            builder.Services.AddControllers().AddNewtonsoftJson();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}