using System.Reflection;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using CursusJapaneseLearningPlatform.Repository;
using CursusJapaneseLearningPlatform.Repository.Entities;

namespace CursusJapaneseLearningPlatform.API;

public static class ConfigureService
{
    public static IServiceCollection ConfigureApiLayerServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddControllers();
        services.ConfigCors();
        services.ConfigSwagger();

        services.AddScoped<UserManager<User>>();
        services.AddScoped<RoleManager<Role>>();
            

        services.AddIdentity<User, Role>(options =>
        {
            // Cấu hình các yêu cầu mật khẩu
            options.Password.RequireDigit = true; // Yêu cầu có số trong mật khẩu
            options.Password.RequireLowercase = true; // Yêu cầu có chữ thường
            options.Password.RequireUppercase = true; // Yêu cầu có chữ hoa
            options.Password.RequireNonAlphanumeric = false; // Không yêu cầu ký tự đặc biệt
            options.Password.RequiredLength = 8; // Độ dài tối thiểu của mật khẩu
            options.Password.RequiredUniqueChars = 1; // Yêu cầu có ít nhất 1 ký tự đặc biệt duy nhất

            // Cấu hình khóa tài khoản
            options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5); // Thời gian khóa sau khi thất bại đăng nhập
            options.Lockout.MaxFailedAccessAttempts = 5; // Số lần thất bại đăng nhập trước khi khóa tài khoản
            options.Lockout.AllowedForNewUsers = true;

            // Cấu hình yêu cầu cho User
            //options.User.RequireUniqueEmail = true; // Yêu cầu email phải là duy nhất
        })
            .AddRoleManager<RoleManager<Role>>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

        services.AddAuthenJwt(configuration);

        return services;
    }

    public static void ConfigCors(this IServiceCollection services)
    {
        services.AddCors(options =>
        {
            options.AddPolicy("AllowAll",
                builder =>
                {
                    builder.WithOrigins("*")
                           .AllowAnyHeader()
                           .AllowAnyMethod();
                });
        });
    }

    public static void ConfigSwagger(this IServiceCollection services)
    {
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Version = "v1",
                Title = "CursusJapaneseLearningPlatform.API"

            });

            // Đọc các nhận xét 
            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            c.IncludeXmlComments(xmlPath);

            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                In = ParameterLocation.Header,
                Description = "JWT Authorization header use scheme Bearer.",
                Type = SecuritySchemeType.Http,
                Name = "Authorization",
                Scheme = "bearer"
            });

            c.AddSecurityRequirement(new OpenApiSecurityRequirement()
            {
            {
                new OpenApiSecurityScheme
                {
                Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    },
                    Scheme = "oauth2",
                    Name = "Bearer",
                    In = ParameterLocation.Header,
                },
                new List<string>()
                }
            });
        }
        );
    }

    public static void AddAuthenJwt(this IServiceCollection services, IConfiguration configuration)
    {
        var jwtSettings = configuration.GetSection("JwtSettings");
        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateIssuerSigningKey = true,
                ValidateLifetime = true,
                ValidIssuer = jwtSettings["Issuer"],
                ValidAudience = jwtSettings["Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]))
            };
            options.SaveToken = true;
            options.RequireHttpsMetadata = true;
            options.Events = new JwtBearerEvents();
        });
    }


}
