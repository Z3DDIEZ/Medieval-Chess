import { useState, useEffect } from 'react';
import { Piece3D } from './Piece3D';
import { Html } from '@react-three/drei';
import { useGameStore } from '../../store/useGameStore';
import { PromotionPicker } from './PromotionPicker';

interface Board3DProps {
    onPieceSelect?: (position: string | null) => void;
}

interface PendingPromotion {
    from: string;
    to: string;
    color: number;
}

export const Board3D = ({ onPieceSelect }: Board3DProps) => {
    const { game, executeMove, connectHub, legalMoves, getLegalMoves, clearLegalMoves } = useGameStore();
    const [selectedPos, setSelectedPos] = useState<string | null>(null);
    const [hoveredSquare, setHoveredSquare] = useState<string | null>(null);
    const [pendingPromotion, setPendingPromotion] = useState<PendingPromotion | null>(null);

    useEffect(() => {
        if (game?.id) {
            connectHub(game.id);
        }
    }, [game?.id, connectHub]);

    // Notify parent when selection changes
    const updateSelection = (pos: string | null) => {
        setSelectedPos(pos);
        onPieceSelect?.(pos);
    };

    // Check if move is pawn promotion
    const isPromotionMove = (from: string, to: string): number | null => {
        if (!game) return null;
        const piece = game.pieces.find(p => p.position === from);
        if (!piece || piece.type !== 0) return null; // Not a pawn

        const toRank = parseInt(to[1]);
        if ((piece.color === 0 && toRank === 8) || (piece.color === 1 && toRank === 1)) {
            return piece.color;
        }
        return null;
    };

    const handlePromotionSelect = async (pieceType: number) => {
        if (!game || !pendingPromotion) return;
        await executeMove(game.id, pendingPromotion.from, pendingPromotion.to, pieceType);
        setPendingPromotion(null);
        updateSelection(null);
        clearLegalMoves();
    };

    const handleSquareClick = async (alg: string) => {
        if (!game) return;

        // If already selected, try to move there
        if (selectedPos) {
            const selectedPiece = game.pieces.find(p => p.position === selectedPos);
            const clickedPiece = game.pieces.find(p => p.position === alg);

            // Clicking own piece switches selection
            if (clickedPiece && selectedPiece && clickedPiece.color === selectedPiece.color) {
                updateSelection(alg);
                getLegalMoves(game.id, alg);
                return;
            }

            // Check for promotion
            const promotionColor = isPromotionMove(selectedPos, alg);
            if (promotionColor !== null) {
                setPendingPromotion({ from: selectedPos, to: alg, color: promotionColor });
            } else {
                executeMove(game.id, selectedPos, alg);
                updateSelection(null);
                clearLegalMoves();
            }
        }
    };

    const handlePieceClick = (pos: string, e: any) => {
        e.stopPropagation();
        if (!game) return;

        const clickedPiece = game.pieces.find(p => p.position === pos);

        // Deselect if clicking same piece
        if (selectedPos === pos) {
            updateSelection(null);
            clearLegalMoves();
            return;
        }

        // If already selecting and clicking enemy piece, try capture
        if (selectedPos) {
            const selectedPiece = game.pieces.find(p => p.position === selectedPos);
            if (clickedPiece && selectedPiece && clickedPiece.color !== selectedPiece.color) {
                // Check for promotion
                const promotionColor = isPromotionMove(selectedPos, pos);
                if (promotionColor !== null) {
                    setPendingPromotion({ from: selectedPos, to: pos, color: promotionColor });
                } else {
                    executeMove(game.id, selectedPos, pos);
                    updateSelection(null);
                    clearLegalMoves();
                }
                return;
            }
        }

        // Can only select pieces of current turn
        if (clickedPiece && clickedPiece.color === game.currentTurn) {
            updateSelection(pos);
            getLegalMoves(game.id, pos);
        }
    };

    // Helper: 0,0 -> "a1"
    const toAlgebraic = (file: number, rank: number) =>
        `${String.fromCharCode(97 + file)}${rank + 1}`;

    // Helper: "e4" -> [x, z] in 3D space
    // x: file (a=0 -> x=-3.5, h=7 -> x=+3.5)
    // z: rank (1=0 -> z=+3.5, 8=7 -> z=-3.5) - flipped for camera perspective
    const fromAlgebraic = (alg: string): [number, number] => {
        const file = alg.charCodeAt(0) - 97;
        const rank = parseInt(alg[1]) - 1;
        // Return [x, z] where z is flipped
        return [file - 3.5, 3.5 - rank];
    };

    // Generate 8x8 checkerboard
    const squares = [];
    for (let x = 0; x < 8; x++) {
        for (let y = 0; y < 8; y++) {
            const isDark = (x + y) % 2 === 1;
            const alg = toAlgebraic(x, y);
            const isSelected = selectedPos === alg;
            const isHovered = hoveredSquare === alg;
            const isLegalMove = selectedPos && legalMoves.includes(alg);
            const isCheck = game?.kingInCheckPosition === alg;
            const isLastMove = game?.lastMoveFrom === alg || game?.lastMoveTo === alg;
            const hasPiece = game?.pieces.some(p => p.position === alg);

            // Determine square color
            let squareColor = isDark ? '#5c3a21' : '#d4a87a';
            if (isSelected) {
                squareColor = '#c9a227'; // Gold for selected
            } else if (isCheck) {
                squareColor = '#cc3333'; // Red for check
            } else if (isLastMove) {
                squareColor = isDark ? '#8b7355' : '#e6d4a0'; // Subtle yellow tint
            } else if (isHovered && isLegalMove) {
                squareColor = isDark ? '#4a6b35' : '#7db35a'; // Green tint for hover
            }

            // x position: file 0 -> -3.5, file 7 -> +3.5
            // z position: rank 0 (1) -> +3.5, rank 7 (8) -> -3.5 (flipped)
            squares.push(
                <mesh
                    key={alg}
                    position={[x - 3.5, 0, 3.5 - y]}
                    receiveShadow
                    onClick={(e) => {
                        e.stopPropagation();
                        handleSquareClick(alg);
                    }}
                    onPointerEnter={() => setHoveredSquare(alg)}
                    onPointerLeave={() => setHoveredSquare(null)}
                >
                    <boxGeometry args={[1, 0.2, 1]} />
                    <meshStandardMaterial color={squareColor} />
                </mesh>
            );

            // Legal move indicator (dot for empty, ring around piece done via piece highlight)
            if (isLegalMove && !hasPiece) {
                squares.push(
                    <mesh
                        key={`${alg}-dot`}
                        position={[x - 3.5, 0.15, 3.5 - y]}
                        onClick={(e) => {
                            e.stopPropagation();
                            handleSquareClick(alg);
                        }}
                    >
                        <cylinderGeometry args={[0.12, 0.12, 0.08, 16]} />
                        <meshStandardMaterial
                            color="#1a1a1a"
                            transparent
                            opacity={0.5}
                        />
                    </mesh>
                );
            }
        }
    }

    // Helper map for piece names
    const typeNames: Record<number, string> = {
        0: "Pawn", 1: "Knight", 2: "Bishop", 3: "Rook", 4: "Queen", 5: "King"
    };

    return (
        <>
            <group>
                {/* Board Squares */}
                {squares}

                {/* Pieces */}
                {game?.pieces.map((p, idx) => {
                    const isSelected = selectedPos === p.position;
                    const isLegalCapture = selectedPos && legalMoves.includes(p.position);
                    const [px, pz] = fromAlgebraic(p.position);

                    return (
                        <group key={idx} onClick={(e) => handlePieceClick(p.position, e)}>
                            <Piece3D
                                type={p.type}
                                color={p.color}
                                position={p.position}
                            />

                            {/* Legal capture ring */}
                            {isLegalCapture && !isSelected && (
                                <mesh position={[px, 0.12, pz]} rotation={[-Math.PI / 2, 0, 0]}>
                                    <ringGeometry args={[0.38, 0.45, 32]} />
                                    <meshBasicMaterial color="#cc3333" transparent opacity={0.7} />
                                </mesh>
                            )}

                            {/* Selection indicator */}
                            {isSelected && (
                                <mesh position={[px, 0.12, pz]} rotation={[-Math.PI / 2, 0, 0]}>
                                    <ringGeometry args={[0.35, 0.42, 32]} />
                                    <meshBasicMaterial color="#ffd700" transparent opacity={0.8} />
                                </mesh>
                            )}

                            {/* Always visible HP bar if damaged or in Attrition Mode */}
                            {(p.currentHP < p.maxHP || game?.isAttritionMode) && !isSelected && (
                                <Html position={[px, 0.9, pz]} center style={{ pointerEvents: 'none' }}>
                                    <div style={{ width: '32px', height: '4px', background: '#222', border: '1px solid #111', borderRadius: '1px' }}>
                                        <div style={{
                                            width: `${(p.currentHP / p.maxHP) * 100}%`,
                                            height: '100%',
                                            background: p.currentHP < p.maxHP * 0.3 ? '#ff3333' : '#33ff33',
                                            transition: 'width 0.3s ease'
                                        }} />
                                    </div>
                                </Html>
                            )}

                            {/* Info tooltip on selection */}
                            {isSelected && (
                                <Html
                                    position={[px, 1.2, pz]}
                                    center
                                    style={{ pointerEvents: 'none' }}
                                >
                                    <div style={{
                                        background: 'rgba(0,0,0,0.85)',
                                        color: 'white',
                                        padding: '8px 12px',
                                        borderRadius: '6px',
                                        width: '130px',
                                        textAlign: 'center',
                                        border: '1px solid #444',
                                        fontSize: '12px'
                                    }}>
                                        <div style={{ fontWeight: 'bold', fontSize: '14px', marginBottom: '4px' }}>
                                            {typeNames[p.type]}
                                        </div>
                                        <div style={{ color: '#aaa' }}>Loyalty: {p.loyalty}%</div>
                                        <div style={{ display: 'flex', justifyContent: 'space-between', marginTop: '2px' }}>
                                            <span style={{ color: '#ff8888' }}>HP: {p.currentHP}/{p.maxHP}</span>
                                            <span style={{ color: '#aaa' }}>Def: {p.armor}</span>
                                        </div>
                                    </div>
                                </Html>
                            )}
                        </group>
                    );
                })}
            </group>

            {/* Promotion Picker (rendered as HTML overlay) */}
            {pendingPromotion && (
                <Html fullscreen>
                    <PromotionPicker
                        color={pendingPromotion.color}
                        onSelect={handlePromotionSelect}
                        onCancel={() => setPendingPromotion(null)}
                    />
                </Html>
            )}

            {/* File labels (a-h) along the near edge (White's side, z=+4.5) */}
            {['a', 'b', 'c', 'd', 'e', 'f', 'g', 'h'].map((file, i) => (
                <Html key={`file-${file}`} position={[i - 3.5, 0.15, 4.5]} center style={{ pointerEvents: 'none' }}>
                    <span style={{
                        color: '#c9a227',
                        fontSize: '14px',
                        fontWeight: 'bold',
                        fontFamily: 'serif',
                        textShadow: '1px 1px 2px black',
                        userSelect: 'none',
                        pointerEvents: 'none'
                    }}>
                        {file}
                    </span>
                </Html>
            ))}

            {/* Rank labels (1-8) along the left edge */}
            {/* rank 1 -> z=+3.5, rank 8 -> z=-3.5 */}
            {[1, 2, 3, 4, 5, 6, 7, 8].map((rank) => (
                <Html key={`rank-${rank}`} position={[-4.5, 0.15, 3.5 - (rank - 1)]} center style={{ pointerEvents: 'none' }}>
                    <span style={{
                        color: '#c9a227',
                        fontSize: '14px',
                        fontWeight: 'bold',
                        fontFamily: 'serif',
                        textShadow: '1px 1px 2px black',
                        userSelect: 'none',
                        pointerEvents: 'none'
                    }}>
                        {rank}
                    </span>
                </Html>
            ))}
        </>
    );
};
