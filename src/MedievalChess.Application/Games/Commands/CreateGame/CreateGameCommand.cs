using MediatR;
using MedievalChess.Application.Common.Interfaces;
using MedievalChess.Domain.Aggregates;

namespace MedievalChess.Application.Games.Commands.CreateGame;

public record CreateGameCommand : IRequest<Guid>;

public class CreateGameCommandHandler : IRequestHandler<CreateGameCommand, Guid>
{
    private readonly IGameRepository _repository;

    public CreateGameCommandHandler(IGameRepository repository)
    {
        _repository = repository;
    }

    public Task<Guid> Handle(CreateGameCommand request, CancellationToken cancellationToken)
    {
        var game = Game.StartNew();
        _repository.Add(game);
        return Task.FromResult(game.Id);
    }
}
