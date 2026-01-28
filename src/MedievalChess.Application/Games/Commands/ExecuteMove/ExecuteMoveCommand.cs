using MediatR;
using MedievalChess.Application.Common.Interfaces;
using MedievalChess.Domain.Enums;
using MedievalChess.Domain.Primitives;

namespace MedievalChess.Application.Games.Commands.ExecuteMove;

public record ExecuteMoveCommand(Guid GameId, string From, string To, PieceType? PromotionPiece = null) : IRequest<bool>;

public class ExecuteMoveCommandHandler : IRequestHandler<ExecuteMoveCommand, bool>
{
    private readonly IGameRepository _repository;
    private readonly Domain.Logic.IEngineService _engine;
    private readonly Domain.Common.IRNGService _rngService;
    private readonly Domain.Common.INarrativeEngineService _narrativeService;

    public ExecuteMoveCommandHandler(IGameRepository repository, 
                                     Domain.Logic.IEngineService engine, 
                                     Domain.Common.IRNGService rngService,
                                     Domain.Common.INarrativeEngineService narrativeService)
    {
        _repository = repository;
        _engine = engine;
        _rngService = rngService;
        _narrativeService = narrativeService;
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

        try 
        {
            game.ExecuteMove(fromPos, toPos, _engine, _rngService, _narrativeService, request.PromotionPiece);
        }
        catch 
        {
            return Task.FromResult(false);
        }
        
        // In a real app with EF Core, we would call _repository.SaveChangesAsync() or similar here.
        // Since it's in-memory and reference-based, the state is already "saved".
        
        return Task.FromResult(true);
    }
}

