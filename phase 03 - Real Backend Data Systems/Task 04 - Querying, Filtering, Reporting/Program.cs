using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using Task_04_Querying_Filtering_Reporting.Data;
using Task_04_Querying_Filtering_Reporting.Services;
using Task_04_Querying_Filtering_Reporting.Services.Interfaces;

namespace Task_04_Querying_Filtering_Reporting
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddDbContext<ApplicationDbContext>(op =>
                        op.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.AddScoped<IStudentService, StudentService>();
            builder.Services.AddScoped<IEnrollmentService, EnrollmentService>();
            builder.Services.AddScoped<IInstructorService, InstructorService>();
            builder.Services.AddScoped<ITrainingTrackService, TrainingTrackService>();
            builder.Services.AddScoped<IPaymentService, PaymentService>();
            builder.Services.AddScoped<IReportService, ReportService>();


            builder.Services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
                    options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
                });
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
