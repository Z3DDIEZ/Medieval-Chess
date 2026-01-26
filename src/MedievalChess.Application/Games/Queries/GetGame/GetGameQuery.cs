using MediatR;
using MedievalChess.Application.Common.Interfaces;
using MedievalChess.Domain.Aggregates;

namespace MedievalChess.Application.Games.Queries.GetGame;

public record GetGameQuery(Guid Id) : IRequest<Game?>;

public class GetGameQueryHandler : IRequestHandler<GetGameQuery, Game?>
{
    private readonly IGameRepository _repository;

    public GetGameQueryHandler(IGameRepository repository)
    {
        _repository = repository;
    }

    public Task<Game?> Handle(GetGameQuery request, CancellationToken cancellationToken)
    {
        var game = _repository.GetById(request.Id);
        return Task.FromResult(game);
    }
}
