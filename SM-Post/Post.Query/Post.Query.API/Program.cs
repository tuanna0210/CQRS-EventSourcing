using Microsoft.EntityFrameworkCore;
using Post.Query.Domain.Repositories;
using Post.Query.Infrastructure.DataAccess;
using Post.Query.Infrastructure.Handlers;
using Post.Query.Infrastructure.Repositories;

namespace Post.Query.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            Action<DbContextOptionsBuilder> configureDbContext = (o => o.UseSqlServer(builder.Configuration.GetConnectionString("SqlServer")));
            builder.Services.AddDbContext<DatabaseContext>(configureDbContext);
            builder.Services.AddSingleton<DatabaseContextFactory>(new DatabaseContextFactory(configureDbContext));
            

            //Create database and tables from code
            var dbContext = builder.Services.BuildServiceProvider().GetService<DatabaseContext>();
            dbContext.Database.EnsureCreated();

            builder.Services.AddScoped<IPostRepository, PostRepository>();
            builder.Services.AddScoped<ICommentRepository, CommentRepository>();
            builder.Services.AddScoped<IEventHandler, Infrastructure.Handlers.EventHandler>();

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