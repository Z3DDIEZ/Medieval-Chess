using MediatR;
using MedievalChess.Application.Common.Interfaces;
using MedievalChess.Domain.Enums;

namespace MedievalChess.Application.Games.Commands.AcceptDraw;

public record AcceptDrawCommand(Guid GameId, PlayerColor PlayerColor) : IRequest<bool>;

public class AcceptDrawCommandHandler : IRequestHandler<AcceptDrawCommand, bool>
{
    private readonly IGameRepository _repository;

    public AcceptDrawCommandHandler(IGameRepository repository)
    {
        _repository = repository;
    }

    public Task<bool> Handle(AcceptDrawCommand request, CancellationToken cancellationToken)
    {
        var game = _repository.GetById(request.GameId);
        if (game == null) return Task.FromResult(false);

        try
        {
            game.AcceptDraw(request.PlayerColor);
            return Task.FromResult(true);
        }
        catch
        {
            return Task.FromResult(false);
        }
    }
}
