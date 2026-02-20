using MediatR;
using MedievalChess.Application.Common.Interfaces;
using MedievalChess.Domain.Logic;
using MedievalChess.Domain.Primitives;

namespace MedievalChess.Application.Games.Commands.UseAbility;

public class UseAbilityCommandHandler : IRequestHandler<UseAbilityCommand, bool>
{
    private readonly IGameRepository _repository;
    private readonly IEngineService _engineService;

    public UseAbilityCommandHandler(IGameRepository repository, IEngineService engineService)
    {
        _repository = repository;
        _engineService = engineService;
    }

    public Task<bool> Handle(UseAbilityCommand request, CancellationToken cancellationToken)
    {
        var game = _repository.GetById(request.GameId);
        if (game == null) return Task.FromResult(false);

        var sourcePos = Position.FromAlgebraic(request.SourceAlgebraic);
        Position? targetPos = request.TargetAlgebraic != null ? Position.FromAlgebraic(request.TargetAlgebraic) : null;

        try
        {
            game.ExecuteAbility(sourcePos, request.AbilityId, targetPos, _engineService);
        }
        catch
        {
            return Task.FromResult(false);
        }

        return Task.FromResult(true);
    }
}
