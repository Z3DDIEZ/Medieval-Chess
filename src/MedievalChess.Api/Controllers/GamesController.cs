using MediatR;
using MedievalChess.Application.Games.Commands.CreateGame;
using MedievalChess.Application.Games.Commands.ExecuteMove;
using MedievalChess.Application.Games.Queries.GetGame;
using MedievalChess.Application.Games.Commands.UseAbility;
using MedievalChess.Application.Games.Commands.SpendXP;
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

    [HttpPost("{id}/ai-move")]
    public async Task<ActionResult> AIMove(Guid id)
    {
        try
        {
            var result = await _mediator.Send(new MedievalChess.Application.Games.Commands.AIMove.AIMoveCommand(id));
            if (!result) return BadRequest("AI failed to move (No moves or Game Over)");
            
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
    public record AbilityRequest(string From, string AbilityId, string? Target = null);
    public record UpgradeRequest(string From, string AbilityType);

    [HttpPost("{id}/abilities")]
    public async Task<ActionResult> UseAbility(Guid id, [FromBody] AbilityRequest request)
    {
        try
        {
            var result = await _mediator.Send(new UseAbilityCommand(id, request.From, request.AbilityId, request.Target));
            if (!result) return BadRequest("Ability failed");

            await _hubContext.Clients.Group(id.ToString()).SendAsync("GameStateUpdated", id);
            return Ok();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("{id}/upgrade")]
    public async Task<ActionResult> SpendXP(Guid id, [FromBody] UpgradeRequest request)
    {
        try
        {
            var result = await _mediator.Send(new SpendXPCommand(id, request.From, request.AbilityType));
            if (!result) return BadRequest("Upgrade failed");

            await _hubContext.Clients.Group(id.ToString()).SendAsync("GameStateUpdated", id);
            return Ok();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

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
            game.IsAttritionMode,
            game.WhiteAP,
            game.BlackAP,
            IsCheck = isCheck,
            KingInCheckPosition = isCheck && currentKing?.Position != null 
                ? currentKing.Position.Value.ToAlgebraic() 
                : null,
            LastMoveFrom = lastMove?.From.ToAlgebraic(),
            LastMoveTo = lastMove?.To.ToAlgebraic(),
            LastMoveIsBounce = lastMove?.IsAttackBounce ?? false,
            LastMoveDamage = lastMove?.DamageDealt,
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
                    p.Armor,
                    // New Medieval fields
                    p.Level,
                    p.XP,
                    p.PromotionTier,
                    IsDefecting = p.IsDefecting,
                    Court = p.Position.HasValue 
                        ? MedievalChess.Domain.Enums.CourtHelper.GetCourt(p.Position.Value).ToString() 
                        : null,
                    Abilities = p.Abilities.Select(a => {
                        var resolvedType = MedievalChess.Domain.Logic.AbilityManager.ResolveAbilityType(a.AbilityDefinitionId);
                        var def = resolvedType.HasValue ? MedievalChess.Domain.Logic.AbilityCatalog.Get(resolvedType.Value) : null;
                        return new
                        {
                            a.AbilityDefinitionId,
                            AbilityType = resolvedType?.ToString() ?? "Unknown",
                            a.CurrentCooldown,
                            a.MaxCooldown,
                            a.UpgradeTier,
                            a.IsReady,
                            Name = def?.Name ?? "Unknown",
                            Description = def?.Description ?? "",
                            APCost = def?.APCost ?? 0,
                            RequiresTarget = def?.RequiresTarget ?? false,
                            Range = def?.Range ?? 0
                        };
                    }).ToList(),
                    AbilityCatalog = MedievalChess.Domain.Logic.AbilityCatalog.GetForPieceType(p.Type).Select(ab => new
                    {
                        AbilityType = ab.Type.ToString(),
                        ab.Name,
                        ab.Description,
                        ab.APCost,
                        ab.XPRequired,
                        Tier = ab.Tier.ToString(),
                        ab.RequiresTarget,
                        ab.Range,
                        IsUnlocked = p.Abilities.Any(pa => pa.AbilityDefinitionId == MedievalChess.Domain.Logic.AbilityManager.GetAbilityDefinitionId(ab.Type))
                    }).ToList()
                }),
            // Include move history for the sidebar
            MoveHistory = game.PlayedMoves.Select(m => m.Notation).ToList(),
            // Include recent narrative entries
            Narrative = game.GameNarrative?.Entries
                .OrderByDescending(e => e.TurnNumber) // Latest first
                .Take(15)
                .Select(e => (object)new 
                {
                    e.Id,
                    e.TurnNumber,
                    Speaker = e.Speaker.ToString(),
                    e.Text,
                    e.Intensity,
                    e.Tags
                }).ToList() ?? new List<object>()
        });
    }
}
