using System;
using System.Linq;
using MedievalChess.Domain.Aggregates;
using MedievalChess.Domain.Entities;
using MedievalChess.Domain.Enums;

namespace MedievalChess.Engine.Structures
{
    public static class BoardMapper
    {
        /// <summary>
        /// Creates a lightweight EngineGameState from the Domain Game.
        /// Returns the State and an array of Pieces corresponding to the State's internal indices.
        /// </summary>
        public static (EngineGameState State, Piece[] PieceMap) Map(Game game)
        {
            var pieces = game.Board.Pieces.ToArray(); // Snapshot order
            var state = new EngineGameState(pieces.Length);
            
            state.TurnNumber = (ushort)game.TurnNumber;
            state.CurrentTurn = game.CurrentTurn;
            state.WhiteAP = (byte)game.WhiteAP;
            state.BlackAP = (byte)game.BlackAP;
            state.IsAttritionMode = game.IsAttritionMode;
            
            for (int i = 0; i < pieces.Length; i++)
            {
                var p = pieces[i];
                var ps = new PieceState();
                
                ps.Id = (byte)i;
                ps.Type = (byte)p.Type;
                ps.Color = (byte)p.Color;
                
                if (p.Position.HasValue)
                {
                    ps.SquareIndex = (byte)(p.Position.Value.Rank * 8 + p.Position.Value.File);
                    state.BoardSquares[ps.SquareIndex] = (byte)i;
                    state.Pieces[i].IsCaptured = false; // Default is false
                    
                    ulong bit = 1UL << ps.SquareIndex;
                    if (p.Color == PlayerColor.White) 
                        state.WhiteOcc |= bit;
                    else 
                        state.BlackOcc |= bit;
                }
                else
                {
                   ps.SquareIndex = 255;
                   ps.IsCaptured = true;
                }

                ps.CurrentHP = (short)p.CurrentHP;
                ps.MaxHP = (short)p.MaxHP;
                ps.Armor = (short)p.Armor;
                ps.Loyalty = (byte)p.Loyalty.Value;
                ps.Level = (byte)p.Level;
                ps.XP = p.XP;
                
                // Map Cooldowns (Support up to 3)
                if (p.Abilities.Count > 0) ps.Ability1Cooldown = (byte)p.Abilities[0].CurrentCooldown;
                if (p.Abilities.Count > 1) ps.Ability2Cooldown = (byte)p.Abilities[1].CurrentCooldown;
                if (p.Abilities.Count > 2) ps.Ability3Cooldown = (byte)p.Abilities[2].CurrentCooldown;

                state.Pieces[i] = ps;
            }
            
            return (state, pieces);
        }
    }
}
