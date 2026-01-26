using MediatR;
using MedievalChess.Application.Games.Commands.CreateGame;
using MedievalChess.Application.Games.Queries.GetGame;
using Microsoft.AspNetCore.Mvc;

namespace MedievalChess.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GamesController : ControllerBase
{
    private readonly IMediator _mediator;

    public GamesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<ActionResult<Guid>> Create()
    {
        var gameId = await _mediator.Send(new CreateGameCommand());
        return CreatedAtAction(nameof(Get), new { id = gameId }, gameId);
    }

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
            // Simple piece output for verification
            Pieces = game.Board.Pieces.Select(p => new 
            {
                p.Type, p.Color, Position = p.Position?.ToAlgebraic()
            })
        });
    }
}
