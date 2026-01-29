using MedievalChess.Domain.Common;
using MedievalChess.Domain.Entities;
using MedievalChess.Domain.Enums;
using MedievalChess.Domain.Primitives;

namespace MedievalChess.Domain.Aggregates;

public class Game : AggregateRoot<Guid>
{
    // Game Mode Flags
    public bool IsStressState { get; private set; }
    public bool IsAttritionMode { get; private set; }
    public int CombatSeed { get; private set; }
    public const int AttritionModeStartTurn = 3;

    public CourtControl KingsCourtControl { get; private set; }
    public CourtControl QueensCourtControl { get; private set; }
    public int KingsCourtContestedTurns { get; private set; }
    public int QueensCourtContestedTurns { get; private set; }

    public int TotalWhiteLevel { get; private set; }
    public int TotalBlackLevel { get; private set; }
    
    public Board Board { get; private set; }
    public PlayerColor CurrentTurn { get; private set; }
    public GameStatus Status { get; private set; }
    public int TurnNumber { get; private set; }
    
    // Action Points (Max 10)
    public int WhiteAP { get; private set; }
    public int BlackAP { get; private set; }
    public const int MaxAP = 10;
    public const int APPerTurn = 5;

    private readonly List<Move> _playedMoves = new();
    public IReadOnlyCollection<Move> PlayedMoves => _playedMoves.AsReadOnly();

    public PlayerColor? DrawOfferedBy { get; private set; }
    
    // Loyalty Relationships (Feudal Hierarchy)
    private readonly List<LoyaltyRelationship> _loyaltyRelationships = new();
    public IReadOnlyCollection<LoyaltyRelationship> LoyaltyRelationships => _loyaltyRelationships.AsReadOnly();

    public GameNarrative GameNarrative { get; internal set; } = null!;

    private Game() 
    {
        Board = null!; // EF Core binding
    }

    public static Game StartNew()
    {
        var game = new Game
        {
            Id = Guid.NewGuid(),
            Board = Board.CreateStandardSetup(),
            CombatSeed = new Random().Next(),

            CurrentTurn = PlayerColor.White,
            Status = GameStatus.InProgress,
            TurnNumber = 1,
            WhiteAP = 5,
            BlackAP = 5
        };
        game.GameNarrative = new GameNarrative(game.Id);
        return game;
    }

    public void ExecuteMove(Position from, Position to, Logic.IEngineService engine, IRNGService rngService, INarrativeEngineService narrativeService, PieceType? promotionPiece = null)
    {
        if (Status != GameStatus.InProgress)
            throw new InvalidOperationException("Game is not in progress");

        // 1. Validate Legal Move (Engine)
        if (!engine.IsMoveLegal(Board, from, to, CurrentTurn))
        {
            throw new InvalidOperationException("Illegal move");
        }
        
        // 2. Validate AP (Transaction start)
        SpendAP(CurrentTurn, 1);

        var piece = Board.GetPieceAt(from); 
        if (piece == null) throw new InvalidOperationException("System Error: Piece vanished during validation");

        var targetPiece = Board.GetPieceAt(to);
        
        // Create Move record
        var move = new Move(from, to, piece, targetPiece);

        // --- Detect Special Moves ---
        
        // Castling detection (King moves 2 squares)
        bool isCastling = piece.Type == PieceType.King && Math.Abs(to.File - from.File) == 2;
        if (isCastling)
        {
            move.IsCastling = true;
            move.IsKingsideCastle = to.File > from.File;
            
            // Move the rook as well
            int rookFromFile = move.IsKingsideCastle ? 7 : 0;
            int rookToFile = move.IsKingsideCastle ? 5 : 3;
            var rookFrom = new Position(rookFromFile, from.Rank);
            var rookTo = new Position(rookToFile, from.Rank);
            
            var rook = Board.GetPieceAt(rookFrom);
            if (rook != null)
            {
                rook.MoveTo(rookTo);
                rook.MarkAsMoved();
            }
        }
        
        // En passant detection (Pawn captures diagonally to empty square)
        bool isEnPassant = piece.Type == PieceType.Pawn && 
                           from.File != to.File && 
                           targetPiece == null;
        if (isEnPassant && Board.EnPassantTarget == to)
        {
            move.IsEnPassant = true;
            
            // Capture the pawn that made the double move (on same rank as moving pawn, target file)
            var capturedPawnPos = new Position(to.File, from.Rank);
            var capturedPawn = Board.GetPieceAt(capturedPawnPos);
            if (capturedPawn != null)
            {
                capturedPawn.Capture();
                Board.ResetHalfMoveClock();
            }
        }
        
        // Attrition Mode Combat
        if (IsAttritionMode && targetPiece != null && !isCastling && !isEnPassant)
        {
            var combatManager = new Logic.CombatManager(this);
            var result = combatManager.CalculateCombat(piece, targetPiece);
            
            targetPiece.TakeDamage(result.DamageDealt);
            move.DamageDealt = result.DamageDealt;

            // Narrative Generation
            // Glancing logic: if armor reduced more than 50% of damage? or loosely based on ratio
            bool isGlancing = result.ArmorReduced > result.BaseDamage * 0.5; 
            var entry = narrativeService.GenerateCombatNarrative(TurnNumber, piece, targetPiece, result.DamageDealt, result.IsCritical, isGlancing);
            GameNarrative.AddEntry(entry);
            
            if (targetPiece.IsCaptured)
            {
                Board.ResetHalfMoveClock();
            }
            else
            {
                // Attack Bounce: Attacker stays, defender survives (damaged)
                move.IsAttackBounce = true;
                
                // Do NOT move the piece
                // Do NOT increment half-move clock (it was an irreversible action - happened - damage)
                // Actually reset it? Damage is progress. Capture is reset. Pawn move is reset.
                // Resetting implies it's "progress". Yes, damage is permanent progress.
                Board.ResetHalfMoveClock(); 
            }
        }
        else if (targetPiece != null)
        {
            // Standard Instant Capture
            targetPiece.Capture();
            Board.ResetHalfMoveClock();
        }

        // Execute the move (only if not a bounce)
        if (!move.IsAttackBounce)
        {
            piece.MoveTo(to);
            piece.MarkAsMoved();

            // Pawn promotion (Only if actually moved)
            if (piece.Type == PieceType.Pawn && Entities.Pieces.Pawn.IsPromotionRank(to.Rank, piece.Color))
            {
                // Strict Domain: Requires explicit promotion choice.
                if (!promotionPiece.HasValue)
                {
                    throw new InvalidOperationException("Promotion piece must be specified for this move.");
                }

                var promoteToType = promotionPiece.Value;
                move.PromotionPiece = promoteToType;
                
                // Actually replace the pawn with the promoted piece
                piece.Capture(); // Remove the pawn from the board
                
                Piece promotedPiece = promoteToType switch
                {
                    PieceType.Queen => new Entities.Pieces.Queen(piece.Color, to),
                    PieceType.Rook => new Entities.Pieces.Rook(piece.Color, to),
                    PieceType.Bishop => new Entities.Pieces.Bishop(piece.Color, to),
                    PieceType.Knight => new Entities.Pieces.Knight(piece.Color, to),
                    _ => new Entities.Pieces.Queen(piece.Color, to)
                };
                promotedPiece.MarkAsMoved();
                Board.AddPiece(promotedPiece);
            }
        }
        
        // --- Update Board State ---
        
        // Update castling rights
        Board.UpdateCastlingRights(piece, from);
        
        // Set en passant target (only if pawn moved 2 squares)
        if (piece.Type == PieceType.Pawn && Math.Abs(to.Rank - from.Rank) == 2)
        {
            int epRank = (from.Rank + to.Rank) / 2;
            Board.SetEnPassantTarget(new Position(from.File, epRank));
        }
        else
        {
            Board.SetEnPassantTarget(null);
        }
        
        // Update half-move clock (pawn moves reset it)
        if (piece.Type == PieceType.Pawn)
        {
            Board.ResetHalfMoveClock();
        }
        else if (targetPiece == null && !isEnPassant)
        {
            Board.IncrementHalfMoveClock();
        }
        
        // Generate proper notation
        move.Notation = move.ToAlgebraicNotation();
        _playedMoves.Add(move);

        // XP Awards
        var xpManager = new Logic.XPManager(this);
        
        // 1. Capture XP
        if (move.CapturedPiece != null) // Works for standard and attrition kills
        {
            xpManager.AwardCaptureXP(move.Piece, move.CapturedPiece);
        }
        
        // 2. Check XP (calculated later in method when verifying move legality/check status for notation)
        // Wait, check status is calculated below in EndTurn or at end of ExecuteMove?
        // It's calculated in "EndTurn" logic usually, or here for move property.
        // Lines 283+ in EndTurn set IsCheck. The engine call happens there.
        // I should add XP award there or move check logic here?
        // Let's add it where IsCheck is determined.



        // Deduct AP (Move costs 1 AP) - Do this LAST to ensure we don't charge for failed moves? 
        // NO. If we do it last, and it fails, the move has already happened!
        // We must check first, but maybe deduct last? 
        // Actually, we should Check first.
        // Or better: Spend first. If other things fail, we might need to refund?
        // But validation (IsMoveLegal) is non-mutating.
        // So: Validate -> Spend -> Mutate.
        
        EndTurn(engine);
    }
    
    public void SpendAP(PlayerColor player, int amount)
    {
        if (player == PlayerColor.White)
        {
            if (WhiteAP < amount) throw new InvalidOperationException("Not enough AP");
            WhiteAP -= amount;
        }
        else
        {
            if (BlackAP < amount) throw new InvalidOperationException("Not enough AP");
            BlackAP -= amount;
        }
    }
    
    // Updated signature to include new managers would happen here, but for now we instantiate them or assume they are stateless/helper
    // Ideally we'd pass them in, but to avoid large refactors we can instantiate them inside or Pass them via method injection.
    // For now I'm instantiating them locally / using static logic if possible, OR pass them in.
   
    
    
    public void AddLoyaltyRelationship(LoyaltyRelationship relationship)
    {
        _loyaltyRelationships.Add(relationship);
    }
    
    private void CheckStressState(Logic.IEngineService engine)
    {
        // 1. King in Check
        bool kingInCheck = engine.IsKingInCheck(Board, CurrentTurn);

        // 2. Queen Captured
        var queen = Board.Pieces.FirstOrDefault(p => p.Color == CurrentTurn && p.Type == PieceType.Queen);
        bool queenCaptured = queen == null || queen.IsCaptured;

        // 3. 50% Losses
        var startingCount = 16;
        var currentCount = Board.Pieces.Count(p => p.Color == CurrentTurn && !p.IsCaptured);
        bool heavyLosses = currentCount <= startingCount / 2;

        if (kingInCheck || queenCaptured || heavyLosses)
        {
            if (!IsStressState)
            {
                IsStressState = true;
                // Trigger transition events via LoyaltyManager if needed (e.g. transfer vassals)
                // For now, IsStressState flag is enough for LoyaltyManager to check
            }
        }
    }

    private void CheckCourtControl()
    {
        // King's Court: Files 0-3 (a-d). Queen's Court: Files 4-7 (e-h)
        // Count pieces for each side in each court
        int whiteKingsCourt = CountPiecesInFiles(PlayerColor.White, 0, 3);
        int blackKingsCourt = CountPiecesInFiles(PlayerColor.Black, 0, 3);
        
        int whiteQueensCourt = CountPiecesInFiles(PlayerColor.White, 4, 7);
        int blackQueensCourt = CountPiecesInFiles(PlayerColor.Black, 4, 7);

        KingsCourtControl = DetermineControl(whiteKingsCourt, blackKingsCourt);
        QueensCourtControl = DetermineControl(whiteQueensCourt, blackQueensCourt);
        
        // Update Contested Counters
        if (KingsCourtControl == CourtControl.Contested) KingsCourtContestedTurns++;
        else KingsCourtContestedTurns = 0;

        if (QueensCourtControl == CourtControl.Contested) QueensCourtContestedTurns++;
        else QueensCourtContestedTurns = 0;
    }

    private int CountPiecesInFiles(PlayerColor color, int minFile, int maxFile)
    {
        return Board.Pieces.Count(p => 
            p.Color == color && 
            !p.IsCaptured && 
            p.Position.HasValue && 
            p.Position.Value.File >= minFile && 
            p.Position.Value.File <= maxFile);
    }

    private CourtControl DetermineControl(int whiteCount, int blackCount)
    {
        if (whiteCount > blackCount + 1) return CourtControl.WhiteControlled;
        if (blackCount > whiteCount + 1) return CourtControl.BlackControlled;
        if (whiteCount == 0 && blackCount == 0) return CourtControl.Neutral;
        return CourtControl.Contested; // Close numbers implies contested
    }

    private void EndTurn(Logic.IEngineService engine)
    {
        // 0. Update Game State (Stress, Courts)
        CheckStressState(engine);
        CheckCourtControl();

        // 1. Loyalty Updates
        var loyaltyManager = new Logic.LoyaltyManager(this);
        loyaltyManager.UpdateLoyalty();
        
        // 2. Court Bonuses
        loyaltyManager.ApplyCourtBonuses();
        
        // 3. Process Defections
        loyaltyManager.ProcessDefections();
        
        // 4. Ability/Effect Updates
        var abilityManager = new Logic.AbilityManager(this);
        abilityManager.AdvanceCooldowns();
        abilityManager.TickEffects();
        
        // 5. Survival XP (Every 5 turns)
        // If TurnNumber is multiple of 5, award surviving pieces?
        // Note: TurnNumber increments below for the next player.
        // We should probably check before incrementing or check (TurnNumber) for the player who just finished?
        // Game turn is global. Let's do it after increment to be safe or before.
        // If we do global turn 5, 10, 15...
        // Let's do it here:
        if (TurnNumber > 0 && TurnNumber % 5 == 0)
        {
            var xpManager = new Logic.XPManager(this);
            foreach (var piece in Board.Pieces.Where(p => !p.IsCaptured))
            {
                xpManager.AwardSurvivalXP(piece);
            }
        }

        // 3. Switch Turn
        CurrentTurn = CurrentTurn == PlayerColor.White ? PlayerColor.Black : PlayerColor.White;
        if (CurrentTurn == PlayerColor.White)
        {
            TurnNumber++;
            
            // Check for Attrition Mode Trigger
            if (!IsAttritionMode && TurnNumber >= AttritionModeStartTurn)
            {
                IsAttritionMode = true;
                // Once triggered, stays on? Rules say "Activates". Assuming permanent.
                // Could verify if we need to initialize anything (HP is already set).
            }
        }
        
        // 4. Grant AP
        if (CurrentTurn == PlayerColor.White)
            WhiteAP = Math.Min(MaxAP, WhiteAP + APPerTurn);
        else
            BlackAP = Math.Min(MaxAP, BlackAP + APPerTurn);
        
        // Set check/checkmate flags on the last move
        if (_playedMoves.Count > 0)
        {
            var lastMove = _playedMoves[^1];
            if (engine.IsKingInCheck(Board, CurrentTurn))
            {
                lastMove.IsCheck = true;
                
                // Award XP for Check
                new Logic.XPManager(this).AwardCheckXP(lastMove.Piece);
                
                if (engine.IsCheckmate(Board, CurrentTurn))
                {
                    lastMove.IsCheckmate = true;
                    lastMove.Notation = lastMove.ToAlgebraicNotation(); // Re-generate with checkmate symbol
                    Status = GameStatus.Checkmate;
                    return;
                }
            }
        }
        
        // Check for stalemate
        if (engine.IsStalemate(Board, CurrentTurn))
        {
            Status = GameStatus.Stalemate;
            return;
        }
        
        // Check for 50-move rule
        if (Board.IsFiftyMoveRule)
        {
            Status = GameStatus.Draw;
        }
    }

    public void Resign(PlayerColor player)
    {
        if (Status != GameStatus.InProgress)
            throw new InvalidOperationException("Game is not in progress");

        Status = GameStatus.Resignation;
        // Logic for winner would go here
    }

    public void MakeDrawOffer(PlayerColor player)
    {
        if (Status != GameStatus.InProgress) return;
        if (DrawOfferedBy == player) return; // Already offered
        
        if (DrawOfferedBy.HasValue && DrawOfferedBy != player)
        {
            // Both offered -> Draw
            Status = GameStatus.Draw;
            DrawOfferedBy = null;
        }
        else
        {
            DrawOfferedBy = player;
        }
    }

    public void AcceptDraw(PlayerColor player)
    {
        if (Status != GameStatus.InProgress) return;
        if (DrawOfferedBy.HasValue && DrawOfferedBy != player)
        {
            Status = GameStatus.Draw;
            DrawOfferedBy = null;
        }
    }
}
