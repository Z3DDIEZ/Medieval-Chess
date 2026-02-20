using MediatR;
using MedievalChess.Application.Common.Interfaces;
using MedievalChess.Domain.Enums;
using MedievalChess.Domain.Primitives;

namespace MedievalChess.Application.Games.Commands.SpendXP;

public class SpendXPCommandHandler : IRequestHandler<SpendXPCommand, bool>
{
    private readonly IGameRepository _repository;

    public SpendXPCommandHandler(IGameRepository repository)
    {
        _repository = repository;
    }

    public Task<bool> Handle(SpendXPCommand request, CancellationToken cancellationToken)
    {
        var game = _repository.GetById(request.GameId);
        if (game == null) return Task.FromResult(false);

        var sourcePos = Position.FromAlgebraic(request.SourceAlgebraic);

        try
        {
            var piece = game.Board.GetPieceAt(sourcePos);
            if (piece == null) return Task.FromResult(false);

            if (!Enum.TryParse<AbilityType>(request.AbilityType, out var abilityType))
                return Task.FromResult(false);

            game.UpgradePieceAbility(piece, abilityType);
        }
        catch
        {
            return Task.FromResult(false);
        }

        return Task.FromResult(true);
    }
}
