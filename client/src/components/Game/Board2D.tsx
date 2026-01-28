import { useState } from 'react';
import { useGameStore } from '../../store/useGameStore';
import { getPieceComponent } from './ChessAssets';
import { DndProvider, useDrag, useDrop } from 'react-dnd';
import { HTML5Backend } from 'react-dnd-html5-backend';
import { PromotionPicker } from './PromotionPicker';
import './BoardTheme.css';

const ItemTypes = {
    PIECE: 'piece'
};

const BoardSquare = ({ x, y, children, onMove, selectedPos, handleSquareClick, isCheck, isLastMove, isLegalMove, flipped, isBounceTarget }: any) => {
    const alg = `${String.fromCharCode(97 + x)}${y + 1}`;
    const isDark = (x + y) % 2 === 0;
    const isSelected = selectedPos === alg;

    // Visual position - flip if needed
    const visualX = flipped ? 7 - x : x;
    const visualY = flipped ? 7 - y : y;

    const [{ isOver, canDrop }, drop] = useDrop(() => ({
        accept: ItemTypes.PIECE,
        drop: (item: any) => onMove(item.from, alg),
        collect: (monitor) => ({
            isOver: !!monitor.isOver(),
            canDrop: !!monitor.canDrop(),
        }),
    }), [x, y, onMove]);

    // Determine background color with priority
    let bgColor = undefined;
    if (isOver) {
        bgColor = canDrop ? 'rgba(0, 255, 0, 0.3)' : 'rgba(255, 0, 0, 0.3)';
    } else if (isCheck) {
        bgColor = 'rgba(255, 0, 0, 0.5)'; // Red for check
    } else if (isLastMove) {
        bgColor = 'rgba(255, 255, 0, 0.3)'; // Yellow for last move
    }

    // Override if bounce target
    if (isBounceTarget) {
        bgColor = 'rgba(255, 0, 0, 0.6)'; // Stronger red for attack impact
    }

    // Bounce/Damage Highlight (Visual Feedback)
    // We pass a new prop `isBounceTarget` or handle it via a style override passed in children?
    // Actually, let's just use a special border or overlay in the square itself if passed down.
    // Ideally we'd modify props, but let's see if we can infer or pass it.
    // The parent passes `isLastMove`. We can add `isBounceTarget`.

    // Show file labels on rank 1 (or 8 if flipped)
    const showFileLabel = flipped ? y === 7 : y === 0;
    // Show rank labels on file a (or h if flipped)
    const showRankLabel = flipped ? x === 7 : x === 0;

    return (
        <div
            ref={(node: any) => { drop(node); }}
            className={`square ${isDark ? 'dark' : 'light'} ${isSelected ? 'highlight-selected' : ''}`}
            onClick={() => handleSquareClick(alg)}
            style={{
                position: 'absolute',
                width: '12.5%',
                height: '12.5%',
                left: `${visualX * 12.5}%`,
                top: `${(7 - visualY) * 12.5}%`,
                backgroundColor: bgColor
            }}
        >
            {children}

            {/* Legal Move Dot */}
            {isLegalMove && !children && (
                <div style={{
                    position: 'absolute',
                    top: '50%',
                    left: '50%',
                    transform: 'translate(-50%, -50%)',
                    width: '25%',
                    height: '25%',
                    borderRadius: '50%',
                    backgroundColor: 'rgba(0, 0, 0, 0.3)'
                }} />
            )}

            {/* Legal Capture Ring */}
            {isLegalMove && children && (
                <div style={{
                    position: 'absolute',
                    top: '50%',
                    left: '50%',
                    transform: 'translate(-50%, -50%)',
                    width: '90%',
                    height: '90%',
                    borderRadius: '50%',
                    border: '3px solid rgba(0, 0, 0, 0.3)',
                    pointerEvents: 'none'
                }} />
            )}

            {/* Rank Number (Left side, or right if flipped) */}
            {showRankLabel && (
                <span className={`coord rank`} style={{
                    position: 'absolute',
                    left: flipped ? undefined : '2px',
                    right: flipped ? '2px' : undefined,
                    top: '2px',
                    fontSize: '0.7rem',
                    fontWeight: 'bold',
                    color: isDark ? '#d4a373' : '#b58863',
                    userSelect: 'none',
                    pointerEvents: 'none'
                }}>{y + 1}</span>
            )}
            {/* File Letter (Bottom side, or top if flipped) */}
            {showFileLabel && (
                <span className={`coord file`} style={{
                    position: 'absolute',
                    right: flipped ? undefined : '2px',
                    left: flipped ? '2px' : undefined,
                    bottom: flipped ? undefined : '2px',
                    top: flipped ? '2px' : undefined,
                    fontSize: '0.7rem',
                    fontWeight: 'bold',
                    color: isDark ? '#d4a373' : '#b58863',
                    userSelect: 'none',
                    pointerEvents: 'none'
                }}>{String.fromCharCode(97 + x)}</span>
            )}
        </div>
    );
};

const DraggablePiece = ({ piece, onSelect }: any) => {
    const PieceComponent = getPieceComponent(piece.type, piece.color);

    const [{ isDragging }, drag] = useDrag(() => ({
        type: ItemTypes.PIECE,
        item: { id: piece.position, from: piece.position, type: piece.type },
        collect: (monitor) => ({
            isDragging: !!monitor.isDragging(),
        }),
    }), [piece]);

    if (!PieceComponent) return null;

    // Loyalty-based border color
    const getLoyaltyBorder = (loyalty: number) => {
        if (loyalty >= 80) return 'transparent';
        if (loyalty >= 50) return 'rgba(139, 195, 74, 0.6)'; // Steady - light green
        if (loyalty >= 30) return 'rgba(255, 152, 0, 0.8)'; // Wavering - orange
        return 'rgba(244, 67, 54, 0.9)'; // Disloyal - red
    };
    const loyaltyBorder = getLoyaltyBorder(piece.loyalty);

    return (
        <div
            ref={(node: any) => { drag(node); }}
            onClick={onSelect}
            style={{
                width: '100%',
                height: '100%',
                opacity: isDragging ? 0.5 : 1,
                cursor: 'grab',
                display: 'flex',
                justifyContent: 'center',
                alignItems: 'center',
                zIndex: 10,
                position: 'relative',
                borderRadius: '50%',
                boxShadow: piece.isDefecting
                    ? '0 0 12px 4px rgba(244, 67, 54, 0.8)'
                    : loyaltyBorder !== 'transparent'
                        ? `0 0 8px 2px ${loyaltyBorder}`
                        : 'none'
            }}
        >
            <PieceComponent
                style={{
                    width: '85%',
                    height: '85%',
                    filter: piece.isDefecting
                        ? 'drop-shadow(1px 4px 4px rgba(244,67,54,0.7))'
                        : 'drop-shadow(1px 4px 4px rgba(0,0,0,0.5))'
                }}
            />

            {/* Level Badge */}
            {piece.level > 1 && (
                <div style={{
                    position: 'absolute',
                    top: '-2px',
                    right: '-2px',
                    background: 'linear-gradient(135deg, #ffd700, #ffb300)',
                    color: '#000',
                    fontSize: '0.6em',
                    fontWeight: 'bold',
                    padding: '2px 4px',
                    borderRadius: '4px',
                    border: '1px solid #b38600',
                    boxShadow: '0 1px 3px rgba(0,0,0,0.3)',
                    zIndex: 20
                }}>
                    {piece.level}
                </div>
            )}

            {/* Defection Warning Icon */}
            {piece.isDefecting && (
                <div style={{
                    position: 'absolute',
                    top: '-4px',
                    left: '-4px',
                    fontSize: '0.8em',
                    zIndex: 20
                }}>
                    ⚠️
                </div>
            )}

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
};

interface Board2DProps {
    onPieceSelect?: (position: string | null) => void;
    flipped?: boolean;
}

interface PendingPromotion {
    from: string;
    to: string;
    color: number;
}

export const Board2D = ({ onPieceSelect, flipped = false }: Board2DProps) => {
    const { game, executeMove, legalMoves, getLegalMoves, clearLegalMoves } = useGameStore();
    const [selectedPos, setSelectedPos] = useState<string | null>(null);
    const [pendingPromotion, setPendingPromotion] = useState<PendingPromotion | null>(null);

    // Notify parent when selection changes
    const updateSelection = (pos: string | null) => {
        setSelectedPos(pos);
        onPieceSelect?.(pos);
    };

    // Helpers
    const toAlgebraic = (x: number, y: number) =>
        `${String.fromCharCode(97 + x)}${y + 1}`;

    // Check if a move is a pawn promotion
    const isPromotionMove = (from: string, to: string): number | null => {
        if (!game) return null;
        const piece = game.pieces.find(p => p.position === from);
        if (!piece || piece.type !== 0) return null; // Not a pawn

        const toRank = parseInt(to[1]);
        // White promotes on rank 8, Black promotes on rank 1
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

        // Same piece deselect
        if (selectedPos === alg) {
            updateSelection(null);
            clearLegalMoves();
            return;
        }

        const clickedPiece = game.pieces.find(p => p.position === alg);

        if (selectedPos) {
            // Logic: if click on own piece -> switch selection
            // if click on empty/enemy -> move
            const selectedPiece = game.pieces.find(p => p.position === selectedPos);

            if (selectedPiece && clickedPiece && selectedPiece.color === clickedPiece.color) {
                updateSelection(alg);
                getLegalMoves(game.id, alg);
            } else {
                // Check if this is a promotion move
                const isLegal = legalMoves.includes(alg); // Check validation first!
                const promotionColor = isPromotionMove(selectedPos, alg);

                if (isLegal && promotionColor !== null) {
                    // Show promotion picker instead of executing immediately
                    setPendingPromotion({ from: selectedPos, to: alg, color: promotionColor });
                } else if (isLegal) {
                    executeMove(game.id, selectedPos, alg);
                    updateSelection(null);
                    clearLegalMoves();
                } else {
                    // Illegal move - ignore or provide feedback
                    // We just clear selection to be clean
                    updateSelection(null);
                    clearLegalMoves();
                }
            }
        } else {
            if (clickedPiece && clickedPiece.color === game.currentTurn) {
                updateSelection(alg);
                getLegalMoves(game.id, alg);
            }
        }
    };

    if (!game) return null;

    const handleMove = (from: string, to: string) => {
        if (from === to) return;

        // Check if this is a promotion move
        const promotionColor = isPromotionMove(from, to);
        if (promotionColor !== null) {
            setPendingPromotion({ from, to, color: promotionColor });
            return;
        }

        executeMove(game.id, from, to);
        updateSelection(null);
        clearLegalMoves();
    };

    // --- Render Board Squares ---
    const squares = [];
    for (let rank = 7; rank >= 0; rank--) {
        for (let file = 0; file < 8; file++) {
            const alg = toAlgebraic(file, rank);
            const piece = game.pieces.find(p => p.position === alg);

            // Check if this square is in check (king's position)
            const isCheck = game.kingInCheckPosition === alg;

            // Check if this is part of the last move
            const isLastMove = game.lastMoveFrom === alg || game.lastMoveTo === alg;

            // Check if this is a legal destination for the selected piece
            const isLegalMove = selectedPos && legalMoves.includes(alg);

            // Check if this square was the target of a bounce attack
            const isBounceTarget = game.lastMoveIsBounce && game.lastMoveTo === alg;

            squares.push(
                <BoardSquare
                    key={alg}
                    x={file}
                    y={rank}
                    flipped={flipped}
                    onMove={handleMove}
                    selectedPos={selectedPos}
                    handleSquareClick={handleSquareClick}
                    isCheck={isCheck}
                    isLastMove={isLastMove}
                    isLegalMove={isLegalMove}
                    isBounceTarget={isBounceTarget}
                >
                    {piece && (
                        <DraggablePiece
                            piece={piece}
                            onSelect={() => handleSquareClick(alg)}
                        />
                    )}

                    {/* Floating Damage Number */}
                    {isBounceTarget && game.lastMoveDamage && (
                        <div style={{
                            position: 'absolute',
                            top: '0',
                            width: '100%',
                            textAlign: 'center',
                            color: 'red',
                            fontWeight: 'bold',
                            fontSize: '1.2em',
                            textShadow: '0 0 2px white',
                            zIndex: 30,
                            animation: 'floatUp 1s ease-out forwards'
                        }}>
                            -{game.lastMoveDamage}
                        </div>
                    )}
                </BoardSquare>
            );
        }
    }

    return (
        <DndProvider backend={HTML5Backend}>
            <div style={{
                width: '100%',
                maxWidth: '600px',
                padding: '20px',
                display: 'flex',
                justifyContent: 'center',
                flexDirection: 'column',
                alignItems: 'center'
            }}>
                <div className="medieval-board" style={{ position: 'relative', width: '100%', paddingBottom: '100%', height: 0 }}>
                    {squares}
                </div>

                <div style={{ display: 'flex', justifyContent: 'space-between', width: '100%', marginTop: '10px', fontWeight: 'bold', color: '#5c2c2c' }}>
                    <span>White: Bottom (1)</span>
                    <span>Black: Top (8)</span>
                </div>

                {/* Status Message for Attrition */}
                {game.lastMoveIsBounce && game.lastMoveTo && (
                    <div style={{
                        marginTop: '10px',
                        padding: '8px',
                        background: '#ffebee',
                        color: '#c62828',
                        border: '1px solid #ef9a9a',
                        borderRadius: '4px',
                        textAlign: 'center',
                        fontWeight: 'bold'
                    }}>
                        ⚔️ Attack Bounced! {game.lastMoveDamage ? `${game.lastMoveDamage} Damage Dealt` : ''}
                    </div>
                )}
            </div>

            {/* Promotion Picker Modal */}
            {pendingPromotion && (
                <PromotionPicker
                    color={pendingPromotion.color}
                    onSelect={handlePromotionSelect}
                    onCancel={() => setPendingPromotion(null)}
                />
            )}
        </DndProvider>
    );
};
