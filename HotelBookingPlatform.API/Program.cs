using HotelBookingPlatform.API.Extentions;
using HotelBookingPlatform.API.Middlewares;
using HotelBookingPlatform.Application.Extentions;
using HotelBookingPlatform.Infrastructure.Extentions;
using Microsoft.Extensions.FileProviders;
using Microsoft.OpenApi.Models;
namespace HotelBookingPlatform.API;
public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddEndpointsApiExplorer();

        builder.Services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "HotelBookingPlatform.API",
                Version = "v1"
            });
            c.EnableAnnotations(); 

            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.ApiKey,
                Scheme = "bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "JWT Authorization header using the Bearer scheme."
            });

            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    new string[] { }
                }
            });
        });
        builder.Services.AddApplicationDependencies()
                        .AddPresentationDependencies(builder.Configuration)
                        .AddInfrastructureDependencies()
                        .AddServiceRegisteration();



        builder.Services.AddCors(options =>
        {
            options.AddDefaultPolicy(policy =>
            {
                policy.AllowAnyOrigin()
                      .AllowAnyMethod()
                      .AllowAnyHeader();
            });
        });
        var app = builder.Build();
      
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "HotelBookingPlatformAPI_v1");              
            });  
        }
        app.UseCors();
        app.UseStaticFiles(new StaticFileOptions
        {
            FileProvider = new PhysicalFileProvider(Path.Combine(builder.Environment.WebRootPath, "uploads", "City")),
            RequestPath = "/uploads/City"
        });

        app.UseStaticFiles(new StaticFileOptions
        {
            FileProvider = new PhysicalFileProvider(Path.Combine(builder.Environment.WebRootPath, "uploads", "City")),
            RequestPath = "/uploads/Hotel"
        });
        app.UseStaticFiles(new StaticFileOptions
        {
            FileProvider = new PhysicalFileProvider(Path.Combine(builder.Environment.WebRootPath, "uploads", "City")),
            RequestPath = "/uploads/RoomClass"
        });

        app.UseMiddleware<GlobalExceptionHandling>();
        app.UseHttpsRedirection();
        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}
