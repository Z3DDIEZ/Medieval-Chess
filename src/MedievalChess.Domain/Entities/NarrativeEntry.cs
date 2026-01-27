using System;
using System.Collections.Generic;
using MedievalChess.Domain.Enums;

namespace MedievalChess.Domain.Entities
{
    public class NarrativeEntry
    {
        public Guid Id { get; private set; }
        public int TurnNumber { get; private set; }
        public NarratorType Speaker { get; private set; }
        public string Text { get; private set; }
        public List<string> Tags { get; private set; }
        public int Intensity { get; private set; } // 1-10
        public Guid? RelatedPieceId { get; private set; }

        private NarrativeEntry() 
        {
            Text = null!;
            Tags = new List<string>();
        } // EF Core

        public NarrativeEntry(int turnNumber, NarratorType speaker, string text, int intensity, Guid? relatedPieceId = null)
        {
            Id = Guid.NewGuid();
            TurnNumber = turnNumber;
            Speaker = speaker;
            Text = text;
            Intensity = intensity;
            RelatedPieceId = relatedPieceId;
            Tags = new List<string>();
        }

        public void AddTag(string tag)
        {
            if (!Tags.Contains(tag))
                Tags.Add(tag);
        }
    }
}
