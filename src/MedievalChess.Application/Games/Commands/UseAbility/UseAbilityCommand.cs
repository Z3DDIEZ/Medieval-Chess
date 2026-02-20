using MediatR;
using MedievalChess.Domain.Primitives;
using System;

namespace MedievalChess.Application.Games.Commands.UseAbility;

public record UseAbilityCommand(
    Guid GameId,
    string SourceAlgebraic,
    string AbilityId,
    string? TargetAlgebraic = null) : IRequest<bool>;
