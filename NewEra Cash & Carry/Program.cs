using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using Serilog;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using NewEra_Cash___Carry.Infrastructure.Data;
using NewEra_Cash___Carry.API.Middlewares;
using NewEra_Cash___Carry.Shared.Settings;
using NewEra_Cash___Carry.Application.Interfaces;
using NewEra_Cash___Carry.Application.Services;
using NewEra_Cash___Carry.Infrastructure.Repositories;
using AutoMapper;
using NewEra_Cash___Carry.Application.Profiles;
using Microsoft.Extensions.Options;
using NewEra_Cash___Carry.Application.Interfaces.CategoryInterfaces;
using NewEra_Cash___Carry.Application.Interfaces.OrderInterfaces;
using NewEra_Cash___Carry.Application.Interfaces.ProductInterfaces;
using NewEra_Cash___Carry.Application.Interfaces.UserInterfaces;
using NewEra_Cash___Carry.Application.Interfaces.PaymentInterfaces;
using NewEra_Cash___Carry.Application.Interfaces.RoleInterfaces;
using NewEra_Cash___Carry.Application.Interfaces.NotifyInterfaces;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add CORS policy
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowAllOrigins",
                builder => builder
                    .AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader());
        });

        // Add Serilog configuration
        Log.Logger = new LoggerConfiguration()
            .WriteTo.Console()
            .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day)
            .CreateLogger();
        builder.Host.UseSerilog();

        // Add services to the container
        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();

        // Swagger Configuration with API Versioning
        builder.Services.AddSwaggerGen(options =>
        {
            var provider = builder.Services.BuildServiceProvider().GetRequiredService<IApiVersionDescriptionProvider>();

            foreach (var description in provider.ApiVersionDescriptions)
            {
                options.SwaggerDoc(description.GroupName, new OpenApiInfo
                {
                    Title = $"NewEra Cash & Carry API {description.ApiVersion}",
                    Version = description.ApiVersion.ToString(),
                    Description = description.IsDeprecated ? "This API version is deprecated." : null
                });
            }

            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "Enter 'Bearer' [space] and your token in the text input below.\n\nExample: 'Bearer abc123xyz'"
            });

            options.AddSecurityRequirement(new OpenApiSecurityRequirement
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
                    Array.Empty<string>()
                }
            });
        });

        // Register DbContext
        builder.Services.AddDbContext<RetailOrderingSystemDbContext>(options =>
            options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

        // Configure AuthSettings
        builder.Services.Configure<AuthSettings>(builder.Configuration.GetSection("AuthSettings"));
        builder.Services.AddSingleton(sp => sp.GetRequiredService<IOptions<AuthSettings>>().Value);

        // Add JWT Authentication
        var authSettings = builder.Configuration.GetSection("AuthSettings").Get<AuthSettings>();
        builder.Services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(authSettings.Secret)),
                ValidateIssuer = false,
                ValidateAudience = false
            };
        });

        // Payment Integration
        builder.Services.Configure<PaymentSettings>(builder.Configuration.GetSection("PaymentSettings"));

        // API Versioning
        builder.Services.AddApiVersioning(options =>
        {
            options.DefaultApiVersion = new ApiVersion(1, 0);
            options.AssumeDefaultVersionWhenUnspecified = true;
            options.ReportApiVersions = true;
        });

        // API Explorer for Versioning
        builder.Services.AddVersionedApiExplorer(options =>
        {
            options.GroupNameFormat = "'v'VVV";
            options.SubstituteApiVersionInUrl = true;
        });

        // Register Repositories and Services
        builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        builder.Services.AddScoped<IUserRepository, UserRepository>();
        builder.Services.AddScoped<IUserService, UserService>();
        builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
        builder.Services.AddScoped<ICategoryService, CategoryService>();
        builder.Services.AddScoped<IOrderRepository, OrderRepository>();
        builder.Services.AddScoped<IOrderService, OrderService>();
        builder.Services.AddScoped<IProductRepository, ProductRepository>();
        builder.Services.AddScoped<IProductService, ProductService>();
        builder.Services.AddScoped<IPaymentService, PaymentService>();
        builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();
        builder.Services.AddScoped<IRoleRepository, RoleRepository>();
        builder.Services.AddScoped<IRoleService, RoleService>();
        builder.Services.AddScoped<INotificationRepository, NotificationRepository>();
        builder.Services.AddScoped<INotificationService, NotificationService>();
        builder.Services.AddScoped<INotifier, EmailNotifier>();
        builder.Services.AddScoped<INotifier, SmsNotifier>();

        // Register AutoMapper
        builder.Services.AddAutoMapper(typeof(MappingProfile).Assembly);

        var app = builder.Build();

        // Configure the HTTP request pipeline
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                var provider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();
                foreach (var description in provider.ApiVersionDescriptions)
                {
                    options.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json", description.GroupName.ToUpperInvariant());
                }
            });
        }

        // Middleware
        app.UseHttpsRedirection();
        app.UseMiddleware<TokenBlacklistMiddleware>();
        app.UseMiddleware<ErrorHandlerMiddleware>();
        app.UseSerilogRequestLogging(); // Serilog Middleware

        // Add authentication and authorization middleware
        app.UseAuthentication();
        app.UseAuthorization();

        //Allow cors
        app.UseCors("AllowAllOrigins");

        // Map Controllers
        app.MapControllers();

        // Serve Static Files
        app.UseStaticFiles();

        app.Run();
    }
}
