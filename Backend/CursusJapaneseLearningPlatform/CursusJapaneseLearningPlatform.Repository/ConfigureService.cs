using System.Reflection;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using CursusJapaneseLearningPlatform.Repository.Implementations;
using CursusJapaneseLearningPlatform.Repository.Implementations.UserManagementRepositories;
using CursusJapaneseLearningPlatform.Repository.Interfaces;
using CursusJapaneseLearningPlatform.Repository.Interfaces.UserManagementRepositories;

namespace CursusJapaneseLearningPlatform.Repository;

public static class ConfigureService
{
    public static IServiceCollection ConfigureRepositoryLayerService(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        services.AddDatabase(configuration);

        return services;
    }

    public static void AddDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
        });
    }
}
