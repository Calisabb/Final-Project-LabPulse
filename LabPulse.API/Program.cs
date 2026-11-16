using LabPulse.API.Handlers;
using LabPulse.BLL;
using LabPulse.BLL.Options;
using LabPulse.DAL;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using System.Text;
using System.Text.Json.Serialization;

namespace LabPulse.API
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers()
                .AddJsonOptions(o =>
                    o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

            builder.Services.AddOpenApi();
            builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
            builder.Services.AddProblemDetails();

            builder.Services.AddOptions<JwtOptions>()
                .BindConfiguration(JwtOptions.SectionName)
                .ValidateDataAnnotations()
                .Validate(o => !string.IsNullOrEmpty(o.Key) && o.Key.Length >= 32, "Key must be at least 32 characters.")
                .Validate(o => o.ExpiresMinutes > 0, "ExpiresMinutes must be greater than 0.")
                .ValidateOnStart();

            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    var jwtSection = builder.Configuration.GetSection(JwtOptions.SectionName);
                    var key = jwtSection["Key"];
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidIssuer = jwtSection["Issuer"],
                        ValidateAudience = true,
                        ValidAudience = jwtSection["Audience"],
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = !string.IsNullOrEmpty(key)
                            ? new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key))
                            : null,
                        ClockSkew = TimeSpan.FromMinutes(1)
                    };

                    options.Events = new JwtBearerEvents
                    {
                        OnChallenge = async context =>
                        {
                            context.HandleResponse();
                            if (!context.Response.HasStarted)
                            {
                                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                                var problem = new ProblemDetails
                                {
                                    Status = StatusCodes.Status401Unauthorized,
                                    Title = "Unauthorized",
                                    Detail = !string.IsNullOrEmpty(context.ErrorDescription)
                                        ? context.ErrorDescription
                                        : "You are not authorized to access this resource."
                                };
                                await context.Response.WriteAsJsonAsync(problem);
                            }
                        },
                        OnForbidden = async context =>
                        {
                            if (!context.Response.HasStarted)
                            {
                                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                                var problem = new ProblemDetails
                                {
                                    Status = StatusCodes.Status403Forbidden,
                                    Title = "Forbidden",
                                    Detail = "You do not have permission to access this resource."
                                };
                                await context.Response.WriteAsJsonAsync(problem);
                            }
                        }
                    };
                });

            builder.Services.AddOptions<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme)
                .Configure<IOptions<JwtOptions>>((options, jwtAccessor) =>
                {
                    var jwt = jwtAccessor.Value;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidIssuer = jwt.Issuer,
                        ValidateAudience = true,
                        ValidAudience = jwt.Audience,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = !string.IsNullOrEmpty(jwt.Key)
                            ? new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.Key))
                            : null,
                        ClockSkew = TimeSpan.FromMinutes(1)
                    };
                });

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
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers().RequireAuthorization();

            using (var scope = app.Services.CreateScope())
            {
                await AdminSeeder.SeedAsync(scope.ServiceProvider);
            }

            await app.RunAsync();
        }
    }
}
