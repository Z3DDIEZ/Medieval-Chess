using MediatR;
using System;

namespace MedievalChess.Application.Games.Commands.SpendXP;

public record SpendXPCommand(
    Guid GameId,
    string SourceAlgebraic,
    string AbilityType) : IRequest<bool>;
