import { useState } from 'react';
import { useGameStore } from '../../store/useGameStore';
import { getPieceComponent } from './ChessAssets';
import { DndProvider, useDrag, useDrop } from 'react-dnd';
import { HTML5Backend } from 'react-dnd-html5-backend';
import './BoardTheme.css';

const ItemTypes = {
    PIECE: 'piece'
};

const BoardSquare = ({ x, y, children, onMove, selectedPos, handleSquareClick }: any) => {
    const alg = `${String.fromCharCode(97 + x)}${y + 1}`;
    const isDark = (x + y) % 2 === 0;
    const isSelected = selectedPos === alg;

    const [{ isOver, canDrop }, drop] = useDrop(() => ({
        accept: ItemTypes.PIECE,
        drop: (item: any) => onMove(item.from, alg),
        collect: (monitor) => ({
            isOver: !!monitor.isOver(),
            canDrop: !!monitor.canDrop(),
        }),
    }), [x, y, onMove]);

    return (
        <div
            ref={(node: any) => { drop(node); }}
            className={`square ${isDark ? 'dark' : 'light'} ${isSelected ? 'highlight-selected' : ''}`}
            onClick={() => handleSquareClick(alg)}
            style={{
                position: 'absolute',
                width: '12.5%',
                height: '12.5%',
                left: `${x * 12.5}%`,
                top: `${(7 - y) * 12.5}%`,
                backgroundColor: isOver ? (canDrop ? 'rgba(0, 255, 0, 0.3)' : 'rgba(255, 0, 0, 0.3)') : undefined
            }}
        >
            {children}
            {/* Rank Number (Left side only) */}
            {x === 0 && (
                <span className={`coord rank`} style={{
                    position: 'absolute',
                    left: '2px',
                    top: '2px',
                    fontSize: '0.7rem',
                    fontWeight: 'bold',
                    color: isDark ? '#d4a373' : '#b58863' // Contrast
                }}>{y + 1}</span>
            )}
            {/* File Letter (Bottom side only) */}
            {y === 0 && (
                <span className={`coord file`} style={{
                    position: 'absolute',
                    right: '2px',
                    bottom: '2px',
                    fontSize: '0.7rem',
                    fontWeight: 'bold',
                    color: isDark ? '#d4a373' : '#b58863' // Contrast
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
                zIndex: 10
            }}
        >
            <PieceComponent
                style={{
                    width: '85%',
                    height: '85%',
                    filter: 'drop-shadow(1px 4px 4px rgba(0,0,0,0.5))'
                }}
            />
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

export const Board2D = () => {
    const { game, executeMove } = useGameStore();
    const [selectedPos, setSelectedPos] = useState<string | null>(null);

    // Helpers
    const toAlgebraic = (x: number, y: number) =>
        `${String.fromCharCode(97 + x)}${y + 1}`;



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

    const handleMove = (from: string, to: string) => {
        if (from === to) return;
        executeMove(game.id, from, to);
        setSelectedPos(null);
    };

    // --- Render Board Squares ---
    const squares = [];
    for (let rank = 7; rank >= 0; rank--) {
        for (let file = 0; file < 8; file++) {
            const alg = toAlgebraic(file, rank);
            const piece = game.pieces.find(p => p.position === alg);

            squares.push(
                <BoardSquare
                    key={alg}
                    x={file}
                    y={rank}
                    onMove={handleMove}
                    selectedPos={selectedPos}
                    handleSquareClick={handleSquareClick}
                >
                    {piece && (
                        <DraggablePiece
                            piece={piece}
                            onSelect={() => handleSquareClick(alg)}
                        />
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

                {/* External Labels (Optional: if we want them outside instead of inside) */}
                <div style={{ display: 'flex', justifyContent: 'space-between', width: '100%', marginTop: '10px', fontWeight: 'bold', color: '#5c2c2c' }}>
                    <span>White: Bottom (1)</span>
                    <span>Black: Top (8)</span>
                </div>
            </div>
        </DndProvider>
    );
};
