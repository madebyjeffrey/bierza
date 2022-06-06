using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Bierza.Data;

public static class DataExtensions
{
    public static IServiceCollection AddData(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<DataDbContext>(options =>
            options.UseNpgsql(connectionString, x => x.MigrationsAssembly("Bierza.Data")));

        return services;
    }
}