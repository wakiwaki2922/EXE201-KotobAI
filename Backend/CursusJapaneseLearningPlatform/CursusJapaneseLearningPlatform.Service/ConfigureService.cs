using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using CursusJapaneseLearningPlatform.Service.Commons.Implementations;
using CursusJapaneseLearningPlatform.Service.Commons.Interfaces;
using CursusJapaneseLearningPlatform.Service.Interfaces;
using CursusJapaneseLearningPlatform.Service.Implementations;
using CursusJapaneseLearningPlatform.Repository.Implementations.CollectionManagementRepositories;
using Microsoft.Extensions.DependencyInjection;
using Amazon.S3;
using Amazon.Runtime;
using Amazon;

namespace CursusJapaneseLearningPlatform.Service;

public static class ConfigureService
{
    public static IServiceCollection ConfigureServiceLayerService(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAutoMapper();
        services.AddServices(configuration);
        services.AddAWSServiceS3(configuration);
        return services;
    }

    private static void AddAutoMapper(this IServiceCollection services)
    {
        services.AddAutoMapper(Assembly.GetExecutingAssembly());
    }

    private static void AddAWSServiceS3(this IServiceCollection services, IConfiguration configuration) {
        var awsOptions = configuration.GetAWSOptions("AWS");
        var accessKey = configuration["AWS:AccessKey"]; 
        var secretKey = configuration["AWS:SecretKey"];

        var credentials = new BasicAWSCredentials(
            accessKey,
            secretKey
        );

        var config = new AmazonS3Config
        {
            RegionEndpoint = RegionEndpoint.GetBySystemName(awsOptions.Region.SystemName)
        };

        // Tạo và thêm dịch vụ IAmazonS3 vào DI container
        services.AddSingleton<IAmazonS3>(new AmazonS3Client(credentials, config));
    }

    public static void AddServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<ITimeService, TimeService>();
        services.AddTransient<IEmailService, EmailService>();
        services.AddScoped<IUserContextService, UserContextService>();
        services.AddScoped<IAWSS3Service, AWSS3Service>();

        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IRoleService, RoleService>();
        services.AddScoped<IUserService, UserService>();
        services.AddSingleton<IRedisService, RedisService>();   
        services.AddScoped<IFirebaseService, FirebaseService>();

        services.AddScoped<ICollectionService, CollectionService>(); 
        services.AddScoped<IVocabularyService, VocabularyService>();
        services.AddScoped<IPackageService, PackageService>();
        services.AddScoped<ISubscriptionService , SubscriptionService>();
        services.AddHttpClient<IPayPalClient, PayPalClient>();
        services.AddScoped<IPayPalClient, PayPalClient>();
        services.AddScoped<IChatService, ChatService>();
        services.AddScoped<IMessageService, MessageService>();
        services.AddScoped<IFlashcardService, FlashcardService>();
        services.AddScoped<IPaymentService, PaymentService>();


    }

    public static async Task InitializeRolesAsync(this IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var roleService = scope.ServiceProvider.GetRequiredService<IRoleService>();
        await roleService.InitializeRolesAsync();
    }

    public static async Task InitializeAdminAccountAsync(this IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var userService = scope.ServiceProvider.GetRequiredService<IAuthService>();

        await userService.InitializeAdminAccountAsync();
    }
}