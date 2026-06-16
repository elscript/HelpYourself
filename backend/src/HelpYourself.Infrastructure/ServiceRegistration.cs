using HelpYourself.Core.Interfaces;
using HelpYourself.Infrastructure.Cache;
using HelpYourself.Infrastructure.Data;
using HelpYourself.Infrastructure.LLM;
using HelpYourself.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace HelpYourself.Infrastructure;

public static class ServiceRegistration
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(opts =>
            opts.UseNpgsql(configuration.GetConnectionString("Postgres")));

        services.AddSingleton<IConnectionMultiplexer>(_ =>
            ConnectionMultiplexer.Connect(configuration.GetConnectionString("Redis")!));

        services.Configure<LlmOptions>(configuration.GetSection(LlmOptions.Section));
        services.AddHttpClient<ILlmClient, LlmClient>(client =>
        {
            var opts = configuration.GetSection(LlmOptions.Section).Get<LlmOptions>() ?? new();
            client.BaseAddress = new Uri(opts.BaseUrl);
            client.Timeout = TimeSpan.FromMinutes(2);
        });

        services.AddScoped<IRitualRepository, RitualRepository>();
        services.AddSingleton<ICacheService, RedisCacheService>();

        return services;
    }
}
