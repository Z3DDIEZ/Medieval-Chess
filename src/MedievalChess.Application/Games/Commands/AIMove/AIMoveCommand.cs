using MediatR;
using MedievalChess.Application.Common.Interfaces;
using MedievalChess.Domain.Enums;
using MedievalChess.Domain.Entities; // Added correct namespace
using MedievalChess.Domain.Primitives;

namespace MedievalChess.Application.Games.Commands.AIMove;

public record AIMoveCommand(Guid GameId) : IRequest<bool>;

public class AIMoveCommandHandler : IRequestHandler<AIMoveCommand, bool>
{
    private readonly IGameRepository _repository;
    private readonly Domain.Logic.IEngineService _engineService; // Validation engine
    private readonly Domain.Common.IRNGService _rngService;
    private readonly Domain.Common.INarrativeEngineService _narrativeService;
    
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

        // Strategy: Try Minimax -> If Fail, Try Random -> If Fail, Return False
        Move? move = null;
        
        try
        {
            var bot = new MedievalChess.Engine.Bots.MinimaxBot(2);
            move = bot.CalculateBestMove(game, 2000);
        }
        catch (Exception)
        {
            // Minimax Failed (Log somewhere in real app). Fallback to Random.
        }

        if (move == null)
        {
             // Fallback
             var randomBot = new MedievalChess.Engine.Bots.RandomBot(_engineService);
             try 
             {
                move = randomBot.CalculateBestMove(game, 500);
             }
             catch
             {
                 return Task.FromResult(false); // Even random failed (likely no moves)
             }
        }
        
        if (move != null)
        {
            try 
            {
                var promotionPiece = move.PromotionPiece;
                if (!promotionPiece.HasValue && move.Piece.Type == PieceType.Pawn)
                {
                     if (move.To.Rank == 0 || move.To.Rank == 7)
                     {
                         promotionPiece = PieceType.Queen;
                     }
                }
                
                game.ExecuteMove(move.From, move.To, _engineService, _rngService, _narrativeService, promotionPiece);
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
