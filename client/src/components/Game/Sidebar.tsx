import { useGameStore } from '../../store/useGameStore';
import { getPieceComponent } from './ChessAssets';
import './BoardTheme.css';

interface SidebarProps {
    viewMode: '3d' | '2d';
    onToggleView: () => void;
    autoRotateCamera?: boolean;
    onToggleAutoRotate?: () => void;
    freeCam?: boolean;
    onToggleFreeCam?: () => void;
    boardFlipped?: boolean;
    onToggleBoardFlip?: () => void;
}

export const Sidebar = ({ viewMode, onToggleView, autoRotateCamera, onToggleAutoRotate, freeCam, onToggleFreeCam, boardFlipped, onToggleBoardFlip }: SidebarProps) => {
    const { game, loading, error, resignGame, offerDraw } = useGameStore();

    if (loading) return <div className="game-sidebar">Loading Realm...</div>;
    if (error) return <div className="game-sidebar">Error: {error}</div>;
    if (!game) return <div className="game-sidebar">No active game</div>;

    const isWhiteTurn = game.currentTurn === 0;


    // --- Captured Pieces Logic ---
    // Standard counts
    const initialCounts = {
        0: { 0: 8, 1: 2, 2: 2, 3: 2, 4: 1, 5: 1 }, // White
        1: { 0: 8, 1: 2, 2: 2, 3: 2, 4: 1, 5: 1 }  // Black
    };

    // Count current pieces
    const currentCounts = {
        0: { 0: 0, 1: 0, 2: 0, 3: 0, 4: 0, 5: 0 },
        1: { 0: 0, 1: 0, 2: 0, 3: 0, 4: 0, 5: 0 }
    };

    game.pieces.forEach(p => {
        // @ts-ignore - indexing number to number key
        if (currentCounts[p.color]) currentCounts[p.color][p.type]++;
    });

    // Determine Captured
    const getCapturedList = (color: number) => {
        const captured: number[] = [];
        for (let type = 0; type <= 4; type++) { // Don't track Kings usually, game ends
            // @ts-ignore
            const count = initialCounts[color][type] - currentCounts[color][type];
            for (let i = 0; i < count; i++) {
                captured.push(type);
            }
        }
        // Sort for nice display (Pawn < Knight < Bishop < Rook < Queen)
        return captured.sort((a, b) => a - b);
    };

    const whiteCaptured = getCapturedList(0); // White pieces lost (Material for Black)
    const blackCaptured = getCapturedList(1); // Black pieces lost (Material for White)

    const renderCapturedRow = (capturedTypes: number[], pieceColor: number) => {
        return (
            <div style={{ height: '30px', display: 'flex', alignItems: 'center', marginBottom: '4px' }}>
                {capturedTypes.map((type, idx) => {
                    const PieceComp = getPieceComponent(type, pieceColor);
                    if (!PieceComp) return null;
                    return (
                        <div key={idx} style={{ width: '20px', height: '20px', marginLeft: '-5px' }}>
                            <PieceComp style={{ width: '100%', height: '100%' }} />
                        </div>
                    );
                })}
            </div>
        );
    };

    // Game Status Mapping (matches GameStatus enum in backend)
    const getStatusText = (status: number): string => {
        switch (status) {
            case 0: return 'Not Started';
            case 1: return 'In Progress';
            case 2: return '🏆 Checkmate!';
            case 3: return '🤝 Stalemate';
            case 4: return '🤝 Draw';
            case 5: return '🏳️ Forfeit';
            case 6: return '🏳️ Resignation';
            default: return 'Unknown';
        }
    };

    const isGameOver = game.status >= 2; // Checkmate and above are game-ending states
    const statusText = getStatusText(game.status);

    return (
        <aside className="game-sidebar">
            <header className="sidebar-header">
                <h1 className="sidebar-title">Medieval Chess</h1>
                <div style={{ fontSize: '0.8rem', color: 'var(--text-secondary)', marginTop: '4px' }}>
                    Match ID: {game.id.substring(0, 8)}...
                </div>
            </header>

            <div className="status-card">
                <div style={{
                    display: 'flex',
                    alignItems: 'center',
                    gap: '10px',
                    marginBottom: '10px'
                }}>
                    <div style={{
                        width: '12px',
                        height: '12px',
                        borderRadius: '50%',
                        backgroundColor: isWhiteTurn ? '#fff' : '#000',
                        border: '1px solid #666'
                    }} />
                    <span style={{ fontWeight: 'bold', fontSize: '1.1em' }}>
                        {isGameOver ? statusText : (isWhiteTurn ? "White's Turn" : "Black's Turn")}
                    </span>
                </div>

                <div style={{ display: 'flex', justifyContent: 'space-between', fontSize: '0.9em' }}>
                    <span>Turn #{game.turnNumber}</span>
                    <span style={{ color: isGameOver ? '#ff6b6b' : '#e6c68b' }}>{statusText}</span>
                </div>

                {/* Captured Pieces Display */}
                <div style={{ marginTop: '15px', borderTop: '1px solid #4a3c31', paddingTop: '10px' }}>
                    <div style={{ fontSize: '0.8em', color: '#888', marginBottom: '2px' }}>Advantage White</div>
                    {renderCapturedRow(blackCaptured, 1)}

                    <div style={{ fontSize: '0.8em', color: '#888', marginBottom: '2px', marginTop: '6px' }}>Advantage Black</div>
                    {renderCapturedRow(whiteCaptured, 0)}
                </div>
            </div>

            <div style={{ marginBottom: '8px', fontWeight: 'bold', color: 'var(--text-secondary)' }}>
                Move History
            </div>

            <div className="move-history" style={{
                maxHeight: '400px',
                overflowY: 'auto',
                background: 'rgba(0,0,0,0.2)',
                padding: '8px',
                borderRadius: '4px',
                fontSize: '0.9em'
            }}>
                {(!game.moveHistory || game.moveHistory.length === 0) ? (
                    <div className="history-row">
                        <span style={{ color: '#888' }}>--</span>
                        <span>Start</span>
                    </div>
                ) : (
                    <div style={{ display: 'grid', gridTemplateColumns: 'min-content 1fr 1fr', gap: '5px' }}>
                        {Array.from({ length: Math.ceil(game.moveHistory.length / 2) }).map((_, i) => (
                            <div key={i} style={{ display: 'contents' }}>
                                <span style={{ color: '#888', marginRight: '5px' }}>{i + 1}.</span>
                                <span style={{ color: '#fff' }}>{game.moveHistory![i * 2]}</span>
                                <span style={{ color: '#fff' }}>{game.moveHistory![i * 2 + 1] || ''}</span>
                            </div>
                        ))}
                    </div>
                )}
            </div>

            <div style={{ marginTop: 'auto', display: 'flex', flexDirection: 'column', gap: '10px' }}>
                {/* Camera toggles - only show in 3D mode */}
                {viewMode === '3d' && (
                    <div style={{ display: 'flex', gap: '5px' }}>
                        {onToggleAutoRotate && (
                            <button
                                className="btn-medieval"
                                onClick={onToggleAutoRotate}
                                style={{
                                    flex: 1,
                                    fontSize: '0.75em',
                                    padding: '6px',
                                    background: autoRotateCamera
                                        ? 'linear-gradient(180deg, #2c5c2c 0%, #1b3e1b 100%)'
                                        : 'linear-gradient(180deg, #4a3c31 0%, #2c2418 100%)'
                                }}
                            >
                                🔄 Auto: {autoRotateCamera ? 'ON' : 'OFF'}
                            </button>
                        )}
                        {onToggleFreeCam && (
                            <button
                                className="btn-medieval"
                                onClick={onToggleFreeCam}
                                style={{
                                    flex: 1,
                                    fontSize: '0.75em',
                                    padding: '6px',
                                    background: freeCam
                                        ? 'linear-gradient(180deg, #5c5c2c 0%, #3e3e1b 100%)'
                                        : 'linear-gradient(180deg, #4a3c31 0%, #2c2418 100%)'
                                }}
                            >
                                🎮 Free: {freeCam ? 'ON' : 'OFF'}
                            </button>
                        )}
                    </div>
                )}

                {/* Board Flip Toggle - only show in 2D mode */}
                {viewMode === '2d' && onToggleBoardFlip && (
                    <button
                        className="btn-medieval"
                        onClick={onToggleBoardFlip}
                        style={{
                            fontSize: '0.85em',
                            background: boardFlipped
                                ? 'linear-gradient(180deg, #5c2c5c 0%, #3e1b3e 100%)'
                                : 'linear-gradient(180deg, #4a3c31 0%, #2c2418 100%)'
                        }}
                    >
                        🔄 Flip Board: {boardFlipped ? 'Black' : 'White'}
                    </button>
                )}

                <button className="btn-medieval" onClick={onToggleView}>
                    Switch to {viewMode === '3d' ? '2D Map' : '3D View'}
                </button>

                <div style={{ display: 'flex', gap: '5px' }}>
                    <button
                        className="btn-medieval"
                        style={{ flex: 1, fontSize: '0.8em', padding: '8px' }}
                        onClick={() => {
                            if (game && window.confirm("Offer a draw?")) {
                                offerDraw(game.id, game.currentTurn);
                            }
                        }}
                    >
                        Offer Draw
                    </button>
                    <button
                        className="btn-medieval"
                        style={{ flex: 1, fontSize: '0.8em', padding: '8px', background: 'linear-gradient(180deg, #5c2c2c 0%, #3e1b1b 100%)' }}
                        onClick={() => {
                            if (game && window.confirm("Are you sure you want to resign?")) {
                                resignGame(game.id, game.currentTurn);
                            }
                        }}
                    >
                        Resign
                    </button>
                </div>
            </div>
        </aside>
    );
};
