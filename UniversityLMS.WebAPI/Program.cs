
using Azure.Storage.Blobs;
using Azure.Storage.Queues;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System;
using System.Text;
using UniversityLMS.Application.Interfaces;
using UniversityLMS.Application.Services;
using UniversityLMS.Infrastructure.DbContexts;
using UniversityLMS.Infrastructure.Middleware;
using UniversityLMS.Infrastructure.Repositories;

namespace UniversityLMS.WebAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Get configuration settings
            var jwtSettings = builder.Configuration.GetSection("Jwt");
            var roles = builder.Configuration.GetSection("Roles");

            // Add services to the container.
            builder.Services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve;
                });


            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();


            builder.Services.AddCors(options =>
            {
                options.AddPolicy("MyPolicy", builder => builder.WithOrigins("http://localhost:4200", "https://localhost:4200")
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials());
            });


            builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("SqlServerConnectionStrings")));

            builder.Services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("veryveryveryveryveryverysceret.....")),
                    ValidateAudience = false,
                    ValidateIssuer = false,
                    ClockSkew = TimeSpan.Zero
                };
            });

            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<IAssignmentRepository, AssignmentRepository>();
            builder.Services.AddScoped<ISubmissionRepository, SubmissionRepository>();
            builder.Services.AddScoped<IJudgeService, Judge0Service>();
            builder.Services.AddScoped<IPlagiarism, PlagiarismService>();
            builder.Services.AddScoped<IBlobStorageService, BlobStorageService>();
            builder.Services.AddScoped<IUnitofWorkRepository, UnitofWorkRepository>();
            builder.Services.AddScoped<IOutboxMessageRepository, OutboxMessageRepository>();


            builder.Services.AddSingleton(x => new BlobServiceClient(builder.Configuration["BlobStorage:ConnectionString"]));

            builder.Services.AddSingleton(x => new QueueServiceClient(builder.Configuration["QueueStorage:ConnectionString"]));

            builder.Services.AddHttpClient();  // Add HttpClient for making requests

            builder.Services.AddSwaggerGen(c =>
            {
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "Enter 'Bearer' followed by a space and your token. Example: 'Bearer {token}'",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer" // Must be lowercase
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
                            },
                            Scheme = "bearer",
                            Name = "Bearer",
                            In = ParameterLocation.Header
                        },
                        new List<string>()
                    }
                });
            });


            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("RequireStudentRole", policy => policy.RequireRole( "Student"));
                options.AddPolicy("RequireTeacherRole", policy => policy.RequireRole( "Teacher"));
                options.AddPolicy("RequireAdminRole", policy => policy.RequireRole( "Admin"));
            });


            // Retrieve the connection string from configuration
            var connectionString = builder.Configuration.GetConnectionString("SqlServerConnectionStrings");






            var app = builder.Build();


            // Use custom exception logging middleware
            app.UseMiddleware<ExceptionLogMiddleware>();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
                });
            }


            app.UseHttpsRedirection();
            app.UseCors("MyPolicy");

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
