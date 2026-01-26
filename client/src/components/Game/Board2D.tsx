import { useState } from 'react';
import { useGameStore } from '../../store/useGameStore';

export const Board2D = () => {
    const { game, executeMove } = useGameStore();
    const [selectedPos, setSelectedPos] = useState<string | null>(null);

    // Helpers
    const toAlgebraic = (x: number, y: number) =>
        `${String.fromCharCode(97 + x)}${y + 1}`;

    const handleSquareClick = (alg: string) => {
        if (!game) return;

        if (selectedPos === alg) {
            setSelectedPos(null); // Deselect
        } else if (selectedPos) {
            // Move attempt
            executeMove(game.id, selectedPos, alg);
            setSelectedPos(null);
        } else {
            // Select if piece exists here (simplified allow click anywhere for now, 
            // but strictly we should only select if piece is yours)
            const piece = game.pieces.find(p => p.position === alg);
            if (piece) setSelectedPos(alg);
        }
    };

    const getPieceSymbol = (type: number, color: number) => {
        // 0=White, 1=Black
        // 0: Pawn, 1: Knight, 2: Bishop, 3: Rook, 4: Queen, 5: King

        if (color === 0) {
            const whiteSyms: Record<number, string> = {
                0: '♙', 1: '♘', 2: '♗', 3: '♖', 4: '♕', 5: '♔'
            };
            return whiteSyms[type];
        } else {
            const blackSyms: Record<number, string> = {
                0: '♟', 1: '♞', 2: '♝', 3: '♜', 4: '♛', 5: '♚'
            };
            return blackSyms[type];
        }
    };

    if (!game) return null;

    const squares = [];
    for (let rank = 7; rank >= 0; rank--) {
        for (let file = 0; file < 8; file++) {
            const isBlack = (file + rank) % 2 === 0; // Standard chess board coloring
            const alg = toAlgebraic(file, rank);
            const isSelected = selectedPos === alg;

            const piece = game.pieces.find(p => p.position === alg);

            squares.push(
                <div
                    key={alg}
                    onClick={() => handleSquareClick(alg)}
                    style={{
                        width: '12.5%',
                        height: '12.5%',
                        display: 'flex',
                        alignItems: 'center',
                        justifyContent: 'center',
                        backgroundColor: isSelected ? 'yellow' : (isBlack ? '#b58863' : '#f0d9b5'),
                        cursor: 'pointer',
                        fontSize: '40px',
                        color: piece?.color === 0 ? 'white' : 'black',
                        textShadow: piece?.color === 0 ? '0 0 2px black' : 'none'
                    }}
                >
                    {piece && getPieceSymbol(piece.type, piece.color)}
                    {/* Tiny stats overlay */}
                    {piece && (isSelected || selectedPos === alg) && (
                        <div style={{ position: 'absolute', fontSize: '10px', bottom: 0, right: 0, background: 'rgba(0,0,0,0.5)', color: 'white' }}>
                            {piece.currentHP}/{piece.maxHP}
                        </div>
                    )}
                </div>
            );
        }
    }

    return (
        <div style={{
            width: '600px',
            height: '600px',
            display: 'flex',
            flexWrap: 'wrap',
            border: '8px solid #4a3c31',
            boxShadow: '0 0 20px rgba(0,0,0,0.5)',
            background: '#f0d9b5'
        }}>
            {squares}
        </div>
    );
};
