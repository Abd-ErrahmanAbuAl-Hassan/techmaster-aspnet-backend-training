using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using System.Text.Json.Serialization;
using Task_05_Business_Rules_Data_Integrity.Data;
using Task_05_Business_Rules_Data_Integrity.Services;
using Task_05_Business_Rules_Data_Integrity.Services.Interfaces;
using Task_05_Business_Rules_Data_Integrity.Utilities;

namespace Task_05_Business_Rules_Data_Integrity
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // ---------- Database ----------
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Connection string 'DefaultConnection' is not configured.");

            builder.Services.AddDbContextPool<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString, sql =>
                {
                    sql.EnableRetryOnFailure(
                        maxRetryCount: 5,
                        maxRetryDelay: TimeSpan.FromSeconds(10),
                        errorNumbersToAdd: null);

                    sql.CommandTimeout(60);
                }));

            // ---------- Application Services ----------
            builder.Services.AddScoped<IStudentService, StudentService>();
            builder.Services.AddScoped<IEnrollmentService, EnrollmentService>();
            builder.Services.AddScoped<IInstructorService, InstructorService>();
            builder.Services.AddScoped<ITrainingTrackService, TrainingTrackService>();
            builder.Services.AddScoped<IPaymentService, PaymentService>();
            builder.Services.AddScoped<IReportService, ReportService>();

            // ---------- Controllers ----------
            builder.Services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
                    options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
                });

            // ---------- Problem Details (RFC 7807) for unhandled errors ----------
            builder.Services.AddProblemDetails();

            // ---------- Health Checks ----------
            builder.Services.AddHealthChecks()
                .AddDbContextCheck<ApplicationDbContext>("database");

            // ---------- Swagger ----------
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
       
            // ---------- Logging ----------
            builder.Logging.ClearProviders();
            builder.Logging.AddConsole();
            builder.Logging.AddDebug();

            var app = builder.Build();

            // ---------- Apply Migrations on Startup ----------
            await ApplyMigrationsAsync(app);

            // ---------- Global Exception Handling ----------
            app.UseExceptionHandler(exceptionHandlerApp =>
            {
                exceptionHandlerApp.Run(async context =>
                {
                    var exceptionFeature = context.Features.Get<IExceptionHandlerFeature>();
                    var exception = exceptionFeature?.Error;

                    var logger = context.RequestServices
                        .GetRequiredService<ILogger<Program>>();

                    logger.LogError(exception, "Unhandled exception for {Method} {Path}",
                        context.Request.Method, context.Request.Path);

                    context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                    context.Response.ContentType = "application/json";

                    var response = new ApiResponse<object>
                    {
                        Success = false,
                        Message = "An unexpected error occurred.",
                        ErrorCode = 500,
                        Errors = app.Environment.IsDevelopment() && exception != null
                            ? new List<string> { exception.Message }
                            : new List<string>()
                    };

                    var json = JsonSerializer.Serialize(response, new JsonSerializerOptions
                    {
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                        Converters = { new JsonStringEnumConverter() }
                    });

                    await context.Response.WriteAsync(json);
                });
            });

            // ---------- HTTP Pipeline ----------
            if (app.Environment.IsDevelopment() || app.Environment.IsStaging())
            {
                await SeedDataAsync(app);
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.MapControllers();

            // ---------- Health Check Endpoint ----------
            app.MapHealthChecks("/health");

            await app.RunAsync();
        }

        private static async Task ApplyMigrationsAsync(WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

            try
            {
                var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                logger.LogInformation("Applying database migrations...");
                await db.Database.MigrateAsync();
                logger.LogInformation("Database migrations applied successfully.");
            }
            catch (Exception ex)
            {
                logger.LogCritical(ex, "Failed to apply database migrations.");
                throw;
            }
        }
        private static async Task SeedDataAsync(WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            try
            {
                await SeedData.SeedAsync(context, logger);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Database seeding failed.");
            }
        }
    }
}