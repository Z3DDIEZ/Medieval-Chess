using MediatR;
using MedievalChess.Application.Common.Interfaces;
using MedievalChess.Domain.Primitives;

namespace MedievalChess.Application.Games.Commands.ExecuteMove;

public record ExecuteMoveCommand(Guid GameId, string From, string To) : IRequest<bool>;

public class ExecuteMoveCommandHandler : IRequestHandler<ExecuteMoveCommand, bool>
{
    private readonly IGameRepository _repository;

    public ExecuteMoveCommandHandler(IGameRepository repository)
    {
        _repository = repository;
    }

    public Task<bool> Handle(ExecuteMoveCommand request, CancellationToken cancellationToken)
    {
        var game = _repository.GetById(request.GameId);
        if (game == null)
        {
            return Task.FromResult(false); // Game not found
        }

        var fromPos = Position.FromAlgebraic(request.From);
        var toPos = Position.FromAlgebraic(request.To);

        game.ExecuteMove(fromPos, toPos);
        
        // In a real app with EF Core, we would call _repository.SaveChangesAsync() or similar here.
        // Since it's in-memory and reference-based, the state is already "saved".
        
        return Task.FromResult(true);
    }
}
