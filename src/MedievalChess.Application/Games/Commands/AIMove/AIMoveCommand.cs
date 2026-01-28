using MediatR;
using MedievalChess.Application.Common.Interfaces;
using MedievalChess.Domain.Enums;
using MedievalChess.Domain.Primitives;

namespace MedievalChess.Application.Games.Commands.AIMove;

public record AIMoveCommand(Guid GameId) : IRequest<bool>;

public class AIMoveCommandHandler : IRequestHandler<AIMoveCommand, bool>
{
    private readonly IGameRepository _repository;
    private readonly Domain.Logic.IEngineService _engineService; // Validation engine
    private readonly Domain.Common.IRNGService _rngService;
    private readonly Domain.Common.INarrativeEngineService _narrativeService;
    
    // We need the AI Bot logic. Usually injected as a service.
    // Since MinimaxBot is in Engine project and instantiated, we might need an interface or instantiate it.
    // For now, let's instantiate directly or assume interface.
    // But MinimaxBot needs engine logic.
    // Let's assume we can use the ENGINE project's namespace.

    public AIMoveCommandHandler(IGameRepository repository, 
                                Domain.Logic.IEngineService engineService, 
                                Domain.Common.IRNGService rngService,
                                Domain.Common.INarrativeEngineService narrativeService)
    {
        _repository = repository;
        _engineService = engineService;
        _rngService = rngService;
        _narrativeService = narrativeService;
    }

    public Task<bool> Handle(AIMoveCommand request, CancellationToken cancellationToken)
    {
        var game = _repository.GetById(request.GameId);
        if (game == null || game.Status != GameStatus.InProgress) return Task.FromResult(false);

        // Instantiate Bot
        // Depth 3 is standard for medieval chess complexity
        var bot = new MedievalChess.Engine.Bots.MinimaxBot(3); 
        
        // Calculate Best Move
        // Note: MinimaxBot.CalculateBestMove expects the Domain Game object
        var bestMove = bot.CalculateBestMove(game, 1000);
        
        if (bestMove != null)
        {
            try 
            {
                // Execute Logic
                // Note: bestMove contains From/To/Promotion
                // AI Default Logic: If Engine suggests a move that results in promotion but logic didn't specify piece (usually Queen in engine), we set it.
                // Engines usually just verify "To Square" is rank 0/7 for pawn.
                
                var promotionPiece = bestMove.PromotionPiece;
                if (!promotionPiece.HasValue && bestMove.Piece.Type == PieceType.Pawn) // Check if pawn
                {
                     // Check rank
                     if (bestMove.To.Rank == 0 || bestMove.To.Rank == 7)
                     {
                         promotionPiece = PieceType.Queen;
                     }
                }
                
                game.ExecuteMove(bestMove.From, bestMove.To, _engineService, _rngService, _narrativeService, promotionPiece);
                return Task.FromResult(true);
            }
            catch
            {
                return Task.FromResult(false);
            }
        }

        return Task.FromResult(false);
    }
}
