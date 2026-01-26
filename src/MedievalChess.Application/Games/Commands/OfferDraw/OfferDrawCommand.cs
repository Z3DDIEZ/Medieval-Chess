using MediatR;
using MedievalChess.Application.Common.Interfaces;
using MedievalChess.Domain.Enums;

namespace MedievalChess.Application.Games.Commands.OfferDraw;

public record OfferDrawCommand(Guid GameId, PlayerColor PlayerColor) : IRequest<bool>;

public class OfferDrawCommandHandler : IRequestHandler<OfferDrawCommand, bool>
{
    private readonly IGameRepository _repository;

    public OfferDrawCommandHandler(IGameRepository repository)
    {
        _repository = repository;
    }

    public Task<bool> Handle(OfferDrawCommand request, CancellationToken cancellationToken)
    {
        var game = _repository.GetById(request.GameId);
        if (game == null) return Task.FromResult(false);

        try
        {
            game.MakeDrawOffer(request.PlayerColor);
            return Task.FromResult(true);
        }
        catch
        {
            return Task.FromResult(false);
        }
    }
}
