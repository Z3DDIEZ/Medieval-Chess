import { useState, useEffect } from 'react';
import { Piece3D } from './Piece3D';
import { Html } from '@react-three/drei';
import { useGameStore } from '../../store/useGameStore';

export const Board3D = () => {
    const { game, executeMove, connectHub } = useGameStore();
    const [selectedPos, setSelectedPos] = useState<string | null>(null);

    useEffect(() => {
        if (game?.id) {
            connectHub(game.id);
        }
    }, [game?.id, connectHub]);

    const handleSquareClick = (alg: string) => {
        if (selectedPos && game) {
            // Move!
            executeMove(game.id, selectedPos, alg);
            setSelectedPos(null);
        }
    };

    const handlePieceClick = (pos: string, e: any) => {
        e.stopPropagation(); // Prevent clicking square underneath
        if (selectedPos === pos) {
            setSelectedPos(null); // Deselect
        } else {
            // If already selecting, maybe capturing?
            if (selectedPos) {
                executeMove(game?.id!, selectedPos, pos);
                setSelectedPos(null);
            } else {
                setSelectedPos(pos);
            }
        }
    };

    // Helper: 0,0 -> "a1"
    const toAlgebraic = (x: number, y: number) =>
        `${String.fromCharCode(97 + x)}${y + 1}`;

    // Generate 8x8 checkerboard
    const squares = [];
    for (let x = 0; x < 8; x++) {
        for (let y = 0; y < 8; y++) {
            const isBlack = (x + y) % 2 === 1;
            const alg = toAlgebraic(x, y);
            const isSelected = selectedPos === alg;

            squares.push(
                <mesh
                    key={alg}
                    position={[x - 3.5, 0, y - 3.5]}
                    receiveShadow
                    onClick={(e) => {
                        e.stopPropagation();
                        handleSquareClick(alg);
                    }}
                >
                    <boxGeometry args={[1, 0.2, 1]} />
                    <meshStandardMaterial color={isSelected ? 'yellow' : (isBlack ? '#5c3a21' : '#d4a87a')} />
                </mesh>
            );
        }
    }

    // Helper map for piece names
    const typeNames: Record<number, string> = { 0: "Pawn", 1: "Knight", 2: "Bishop", 3: "Rook", 4: "Queen", 5: "King" };

    return (
        <group>
            {/* The Board Base */}
            {squares}

            {/* The Pieces */}
            {game?.pieces.map((p, idx) => {
                const isSelected = selectedPos === p.position;
                return (
                    <group key={idx} onClick={(e) => handlePieceClick(p.position, e)}>
                        <Piece3D
                            type={p.type}
                            color={p.color}
                            position={p.position}
                        />
                        {isSelected && (
                            <Html position={[0, 2, 0]} center>
                                <div style={{ background: 'rgba(0,0,0,0.8)', color: 'white', padding: '5px', borderRadius: '4px', pointerEvents: 'none', width: '120px', textAlign: 'center' }}>
                                    <div style={{ fontWeight: 'bold' }}>{typeNames[p.type]}</div>
                                    <div style={{ fontSize: '0.8em', color: '#aaaaaa' }}>Loyalty: {p.loyalty}%</div>
                                    <div style={{ fontSize: '0.8em', color: '#ff6666' }}>HP: {p.currentHP}/{p.maxHP}</div>
                                </div>
                            </Html>
                        )}
                    </group>
                )
            })}
        </group>
    );
};
