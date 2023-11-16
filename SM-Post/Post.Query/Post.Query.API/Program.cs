using Microsoft.EntityFrameworkCore;
using Post.Query.Infrastructure.DataAccess;

namespace Post.Query.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            var a = builder.Configuration.GetConnectionString("SqlServer");
            // Add services to the container.
            Action<DbContextOptionsBuilder> configureDbContext = (o => o.UseSqlServer(builder.Configuration.GetConnectionString("SqlServer")));
            builder.Services.AddDbContext<DatabaseContext>(configureDbContext);
            builder.Services.AddSingleton<DatabaseContextFactory>(new DatabaseContextFactory(configureDbContext));

            //Create database and tables from code
            var dbContext = builder.Services.BuildServiceProvider().GetService<DatabaseContext>();
            dbContext.Database.EnsureCreated();

            builder.Services.AddControllers();
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

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}