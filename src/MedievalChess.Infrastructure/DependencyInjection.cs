using MedievalChess.Application.Common.Interfaces;
using MedievalChess.Infrastructure.Persistence;
using Microsoft.Extensions.DependencyInjection;

namespace MedievalChess.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddSingleton<IGameRepository, GameRepository>();
        return services;
    }
}
