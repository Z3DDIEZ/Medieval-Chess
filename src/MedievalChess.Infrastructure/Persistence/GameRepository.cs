using MedievalChess.Application.Common.Interfaces;
using MedievalChess.Domain.Aggregates;

namespace MedievalChess.Infrastructure.Persistence;

public class GameRepository : IGameRepository
{
    private static readonly Dictionary<Guid, Game> _games = new();

    public void Add(Game game)
    {
        _games[game.Id] = game;
    }

    public Game? GetById(Guid id)
    {
        return _games.TryGetValue(id, out var game) ? game : null;
    }
}
