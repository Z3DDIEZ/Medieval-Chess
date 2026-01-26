using MedievalChess.Domain.Aggregates;

namespace MedievalChess.Application.Common.Interfaces;

public interface IGameRepository
{
    void Add(Game game);
    Game? GetById(Guid id);
}
