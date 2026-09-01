using BookStoreApi.Utilities;
using Task_04___Book_Store_API_Mini_Project.Services.Implementations;
using Task_04___Book_Store_API_Mini_Project.Services.Repositories;

namespace Task_04___Book_Store_API_Mini_Project
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container
            builder.Services.AddScoped<IAuthorService, AuthorService>();
            builder.Services.AddScoped<ICategoryService, CategoryService>();
            builder.Services.AddScoped<IBookService, BookService>();

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
                {
                    Title = "Book Store API",
                    Version = "v1",
                    Description = "A comprehensive Book Store API for managing books, authors, and categories."
                });
            });

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                DataSeeder.SeedInitialData(app.Services);
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
