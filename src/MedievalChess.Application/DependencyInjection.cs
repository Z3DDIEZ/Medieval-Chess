using Microsoft.Extensions.DependencyInjection;

namespace MedievalChess.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly));
        services.AddScoped<MedievalChess.Domain.Logic.IEngineService, MedievalChess.Domain.Logic.EngineService>();
        return services;
    }
}
