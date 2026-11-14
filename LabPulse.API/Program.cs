using LabPulse.API.Handlers;
using LabPulse.BLL;
using LabPulse.DAL;
using Scalar.AspNetCore;
using System.Text.Json.Serialization;

namespace LabPulse.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers()
                .AddJsonOptions(o =>
                    o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

            builder.Services.AddOpenApi();
            builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
            builder.Services.AddProblemDetails();

            builder.Services.AddLabPulseDAL(builder.Configuration);
            builder.Services.AddLabPulseBLL();

            var app = builder.Build();

            app.UseExceptionHandler();

            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
                app.MapScalarApiReference();

            }

            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.MapControllers();

            app.Run();
        }
    }
}
