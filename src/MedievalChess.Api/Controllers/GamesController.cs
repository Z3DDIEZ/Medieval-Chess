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
            await _mediator.Send(new ExecuteMoveCommand(id, request.From, request.To));
            
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

    public record MoveRequest(string From, string To);
    public record PlayerColorRequest(MedievalChess.Domain.Enums.PlayerColor Color);

    [HttpGet("{id}")]
    public async Task<ActionResult<object>> Get(Guid id)
    {
        var game = await _mediator.Send(new GetGameQuery(id));
        if (game == null)
            return NotFound();

        return Ok(new
        {
            game.Id,
            game.Status,
            game.CurrentTurn,
            game.TurnNumber,
            Pieces = game.Board.Pieces.Select(p => new 
            {
                p.Type, 
                p.Color, 
                Position = p.Position?.ToAlgebraic(),
                Loyalty = p.Loyalty.Value,
                p.MaxHP,
                p.CurrentHP
            })
        });
    }
}
