using System;
using System.Collections.Generic;
using System.Linq;
using MedievalChess.Domain.Entities;
using MedievalChess.Domain.Enums;

namespace MedievalChess.Domain.Aggregates
{
    public class GameNarrative
    {
        public Guid Id { get; private set; }
        public Guid GameId { get; private set; }
        public NarrativeTone CurrentTone { get; private set; }
        
        private readonly List<NarrativeEntry> _entries = new List<NarrativeEntry>();
        public IReadOnlyCollection<NarrativeEntry> Entries => _entries.AsReadOnly();

        private GameNarrative() { } // EF Core

        public GameNarrative(Guid gameId)
        {
            Id = Guid.NewGuid();
            GameId = gameId;
            CurrentTone = NarrativeTone.Neutral;
        }

        public void AddEntry(NarrativeEntry entry)
        {
            _entries.Add(entry);
        }

        public void SetTone(NarrativeTone newTone)
        {
            CurrentTone = newTone;
        }

        public IEnumerable<NarrativeEntry> GetEntriesForTurn(int turnNumber)
        {
            return _entries.Where(e => e.TurnNumber == turnNumber);
        }
    }
}
