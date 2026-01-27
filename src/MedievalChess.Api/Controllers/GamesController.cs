using MediatR;
using MedievalChess.Application.Games.Commands.CreateGame;
using MedievalChess.Application.Games.Commands.ExecuteMove;
using MedievalChess.Application.Games.Queries.GetGame;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace MedievalChess.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GamesController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly Microsoft.AspNetCore.SignalR.IHubContext<Hubs.GameHub> _hubContext;

    public GamesController(IMediator mediator, Microsoft.AspNetCore.SignalR.IHubContext<Hubs.GameHub> hubContext)
    {
        _mediator = mediator;
        _hubContext = hubContext;
    }

    [HttpPost]
    public async Task<ActionResult<Guid>> Create()
    {
        var gameId = await _mediator.Send(new CreateGameCommand());
        return CreatedAtAction(nameof(Get), new { id = gameId }, gameId);
    }

    [HttpPost("{id}/moves")]
    public async Task<ActionResult> Move(Guid id, [FromBody] MoveRequest request)
    {
        try
        {
            // Parse promotion piece if provided (0=Pawn, 1=Knight, 2=Bishop, 3=Rook, 4=Queen)
            MedievalChess.Domain.Enums.PieceType? promotionPiece = request.PromotionPiece.HasValue 
                ? (MedievalChess.Domain.Enums.PieceType)request.PromotionPiece.Value 
                : null;
            
            await _mediator.Send(new ExecuteMoveCommand(id, request.From, request.To, promotionPiece));
            
            // Notify clients
            await _hubContext.Clients.Group(id.ToString()).SendAsync("GameStateUpdated", id);
            
            return Ok();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("{id}/resign")]
    public async Task<ActionResult> Resign(Guid id, [FromBody] PlayerColorRequest request)
    {
        try
        {
            var result = await _mediator.Send(new MedievalChess.Application.Games.Commands.ResignGame.ResignGameCommand(id, request.Color));
            if (!result) return BadRequest("Action failed");

            await _hubContext.Clients.Group(id.ToString()).SendAsync("GameStateUpdated", id);
            return Ok();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("{id}/draw/offer")]
    public async Task<ActionResult> OfferDraw(Guid id, [FromBody] PlayerColorRequest request)
    {
        try
        {
            var result = await _mediator.Send(new MedievalChess.Application.Games.Commands.OfferDraw.OfferDrawCommand(id, request.Color));
            if (!result) return BadRequest("Action failed");
            
            // Should probably notify specific user "Draw Offered", but generic update works for now
            await _hubContext.Clients.Group(id.ToString()).SendAsync("GameStateUpdated", id);
            return Ok();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("{id}/draw/accept")]
    public async Task<ActionResult> AcceptDraw(Guid id, [FromBody] PlayerColorRequest request)
    {
        try
        {
            var result = await _mediator.Send(new MedievalChess.Application.Games.Commands.AcceptDraw.AcceptDrawCommand(id, request.Color));
            if (!result) return BadRequest("Action failed");

            await _hubContext.Clients.Group(id.ToString()).SendAsync("GameStateUpdated", id);
            return Ok();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("{id}/legal-moves/{from}")]
    public async Task<ActionResult<IEnumerable<string>>> GetLegalMoves(Guid id, string from)
    {
        var game = await _mediator.Send(new GetGameQuery(id));
        if (game == null) return NotFound();
        
        try
        {
            var fromPos = MedievalChess.Domain.Primitives.Position.FromAlgebraic(from);
            var engine = new MedievalChess.Domain.Logic.EngineService();
            var legalMoves = engine.GetLegalDestinations(game.Board, fromPos);
            return Ok(legalMoves.Select(p => p.ToAlgebraic()));
        }
        catch
        {
            return Ok(Array.Empty<string>());
        }
    }

    public record MoveRequest(string From, string To, int? PromotionPiece = null);
    public record PlayerColorRequest(MedievalChess.Domain.Enums.PlayerColor Color);

    [HttpGet("{id}")]
    public async Task<ActionResult<object>> Get(Guid id)
    {
        var game = await _mediator.Send(new GetGameQuery(id));
        if (game == null)
            return NotFound();

        // Check if current player is in check
        var engine = new MedievalChess.Domain.Logic.EngineService();
        var isCheck = engine.IsKingInCheck(game.Board, game.CurrentTurn);
        
        // Get last move for highlighting
        var lastMove = game.PlayedMoves.LastOrDefault();
        
        // Find king position for check indicator
        var currentKing = game.Board.GetKing(game.CurrentTurn);

        return Ok(new
        {
            game.Id,
            game.Status,
            game.CurrentTurn,
            game.TurnNumber,
            IsCheck = isCheck,
            KingInCheckPosition = isCheck && currentKing?.Position != null 
                ? currentKing.Position.Value.ToAlgebraic() 
                : null,
            LastMoveFrom = lastMove?.From.ToAlgebraic(),
            LastMoveTo = lastMove?.To.ToAlgebraic(),
            // Only include active pieces (not captured)
            Pieces = game.Board.Pieces
                .Where(p => p.Position != null)
                .Select(p => new 
                {
                    p.Type, 
                    p.Color, 
                    Position = p.Position?.ToAlgebraic(),
                    Loyalty = p.Loyalty.Value,
                    p.MaxHP,
                    p.CurrentHP,
                    // New Medieval fields
                    p.Level,
                    p.XP,
                    IsDefecting = p.IsDefecting,
                    Court = p.Position.HasValue 
                        ? MedievalChess.Domain.Enums.CourtHelper.GetCourt(p.Position.Value).ToString() 
                        : null,
                    Abilities = p.Abilities.Select(a => new
                    {
                        a.AbilityDefinitionId,
                        a.CurrentCooldown,
                        a.MaxCooldown,
                        a.UpgradeTier,
                        a.IsReady
                    }).ToList()
                }),
            // Include move history for the sidebar
            MoveHistory = game.PlayedMoves.Select(m => m.Notation).ToList()
        });
    }
}
