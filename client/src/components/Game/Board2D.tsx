import { useState } from 'react';
import { useGameStore } from '../../store/useGameStore';
import { getPieceComponent } from './ChessAssets';
import './BoardTheme.css';

export const Board2D = () => {
    const { game, executeMove } = useGameStore();
    const [selectedPos, setSelectedPos] = useState<string | null>(null);

    // Helpers
    const toAlgebraic = (x: number, y: number) =>
        `${String.fromCharCode(97 + x)}${y + 1}`;

    const getPositionStyle = (alg: string): React.CSSProperties => {
        if (!alg || alg.length < 2) return {};
        const file = alg.charCodeAt(0) - 97; // 'a' -> 0
        const rank = parseInt(alg[1]) - 1;   // '1' -> 0

        // rank 0 is bottom (high percentage top), rank 7 is top (low percentage top)
        // 8x8 grid -> each is 12.5%
        return {
            left: `${file * 12.5}%`,
            top: `${(7 - rank) * 12.5}%`
        };
    };

    const handleSquareClick = (alg: string) => {
        if (!game) return;

        // Same piece deselect
        if (selectedPos === alg) {
            setSelectedPos(null);
            return;
        }

        const clickedPiece = game.pieces.find(p => p.position === alg);

        if (selectedPos) {
            // Logic: if click on own piece -> switch selection
            // if click on empty/enemy -> move
            const selectedPiece = game.pieces.find(p => p.position === selectedPos);

            if (selectedPiece && clickedPiece && selectedPiece.color === clickedPiece.color) {
                setSelectedPos(alg);
            } else {
                executeMove(game.id, selectedPos, alg);
                setSelectedPos(null);
            }
        } else {
            if (clickedPiece) {
                setSelectedPos(alg);
            }
        }
    };

    if (!game) return null;

    // --- Render Grid (Background) ---
    // We render this efficiently once
    const gridSquares = [];
    for (let rank = 7; rank >= 0; rank--) {
        for (let file = 0; file < 8; file++) {
            const isDark = (file + rank) % 2 === 0;
            const alg = toAlgebraic(file, rank);
            const isSelected = selectedPos === alg;

            // Highlight logic helpers
            // (In a real app, 'last moved' squares would come from moveHistory)

            gridSquares.push(
                <div
                    key={alg}
                    className={`square ${isDark ? 'dark' : 'light'} ${isSelected ? 'highlight-selected' : ''}`}
                    onClick={() => handleSquareClick(alg)}
                    style={{
                        position: 'absolute',
                        width: '12.5%',
                        height: '12.5%',
                        ...getPositionStyle(alg)
                        // Note: top/left reuse same logic as pieces
                    }}
                >
                    {/* Rank Number (Left side only) */}
                    {file === 0 && (
                        <span className={`coord rank`}>{rank + 1}</span>
                    )}

                    {/* File Letter (Bottom side only) */}
                    {rank === 0 && (
                        <span className={`coord file`}>{String.fromCharCode(97 + file)}</span>
                    )}
                </div>
            );
        }
    }

    // --- Render Pieces (Foreground Layer) ---
    // These are rendered OUTSIDE the squares so they can animate between them
    const pieceElements = game.pieces.map((piece) => {
        const PieceComponent = getPieceComponent(piece.type, piece.color);
        if (!PieceComponent) return null;

        // Unique key is crucial for animation. ideally piece.id, but assume index or similar if no ID.
        // If piece objects are stable references, we can key by type+coord (but coord changes).
        // Best to use an ID if available. Looking at store: GameState pieces doesn't show ID.
        // We will construct a synthetic key based on starting position or a stable ID if we had one.
        // For now, keying by index in array MIGHT cause component remounting if array shuffles.
        // BETTER: If no ID, we accept that 'same type same color' at new pos is same piece? 
        // No, that fails promotion.
        // Let's rely on React diffing. To animate, the DIV needs to move.
        // So key needs to be PERSISTENT for that piece instance.
        // CRITICAL: Without piece IDs, animations might be jumpy if the array order changes.
        // Assuming array order is relatively stable or we use index as key strictly 
        // if pieces array represents "slot 0 piece", "slot 1 piece".
        // But likely pieces list changes length on capture.
        // WORKAROUND: Generate a weak key: `${piece.color}-${piece.type}-${piece.position}` 
        // -> wait, position changes, so key changing means REMOUNT which KILLS animation.
        // Component needs to stay, style needs to change.
        // We need a stable identifier. If not in DB, maybe we just use index and hope list is stable?
        // Let's assume list is "All pieces currently on board".
        // Use index for now as fallback.

        return (
            <div
                key={`${piece.color}-${piece.type}-${game.pieces.indexOf(piece)}`}
                className="chess-piece-container"
                style={{
                    position: 'absolute',
                    width: '12.5%',
                    height: '12.5%',
                    transition: 'all 0.3s ease-in-out', // THE ANIMATION MAGIC
                    pointerEvents: 'none', // Clicks go through to grid for validation
                    zIndex: 10,
                    ...getPositionStyle(piece.position)
                }}
            >
                <PieceComponent
                    style={{
                        width: '85%',
                        height: '85%',
                        filter: 'drop-shadow(1px 4px 4px rgba(0,0,0,0.5))'
                    }}
                />
                {/* HP Bar */}
                {piece.currentHP < piece.maxHP && (
                    <div style={{
                        position: 'absolute',
                        bottom: '5%',
                        left: '50%',
                        transform: 'translateX(-50%)',
                        width: '80%',
                        height: '4px',
                        background: '#330000',
                        border: '1px solid #000',
                        borderRadius: '2px',
                    }}>
                        <div style={{
                            width: `${(piece.currentHP / piece.maxHP) * 100}%`,
                            height: '100%',
                            background: '#d32f2f',
                            transition: 'width 0.3s'
                        }} />
                    </div>
                )}
            </div>
        );
    });

    return (
        <div style={{
            width: '100%',
            maxWidth: '600px',
            padding: '20px',
            display: 'flex',
            justifyContent: 'center'
        }}>
            <div className="medieval-board">
                {/* 1. Grid Layer */}
                {gridSquares}

                {/* 2. Piece Layer */}
                {pieceElements}
            </div>
        </div>
    );
};
