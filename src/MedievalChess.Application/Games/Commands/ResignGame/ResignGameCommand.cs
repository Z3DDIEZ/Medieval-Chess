using MediatR;
using MedievalChess.Application.Common.Interfaces;
using MedievalChess.Domain.Enums;

namespace MedievalChess.Application.Games.Commands.ResignGame;

public record ResignGameCommand(Guid GameId, PlayerColor PlayerColor) : IRequest<bool>;

public class ResignGameCommandHandler : IRequestHandler<ResignGameCommand, bool>
{
    private readonly IGameRepository _repository;

    public ResignGameCommandHandler(IGameRepository repository)
    {
        _repository = repository;
    }

    public Task<bool> Handle(ResignGameCommand request, CancellationToken cancellationToken)
    {
        var game = _repository.GetById(request.GameId);
        if (game == null) return Task.FromResult(false);

        try
        {
            game.Resign(request.PlayerColor);
            return Task.FromResult(true);
        }
        catch
        {
            return Task.FromResult(false);
        }
    }
}
