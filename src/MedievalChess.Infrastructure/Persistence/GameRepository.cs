using MedievalChess.Application.Common.Interfaces;
using MedievalChess.Domain.Aggregates;

namespace MedievalChess.Infrastructure.Persistence;

public class GameRepository : IGameRepository
{
    private static readonly Dictionary<Guid, Game> _games = new();

    static GameRepository()
    {
        // SEED DATA FOR TESTING
        var testGame = Game.StartNew();
        // Reflection needed to set ID if setter is private, 
        // but for now we'll just rely on the API flow or create a special seed helper.
        // Actually, since Game.Id uses Guid.NewGuid(), we can't force a specific ID easily without changing the Domain 
        // OR using reflection. Let's use reflection for this dev convenience.
        
        var specificId = Guid.Parse("11111111-1111-1111-1111-111111111111");
        typeof(Game).GetProperty(nameof(Game.Id))!.SetValue(testGame, specificId);
        
        _games[specificId] = testGame;
    }

    public void Add(Game game)
    {
        _games[game.Id] = game;
    }

    public Game? GetById(Guid id)
    {
        return _games.TryGetValue(id, out var game) ? game : null;
    }
}
