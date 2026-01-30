import { useGameStore } from '../../store/useGameStore';
import { getPieceComponent } from './ChessAssets';
import { PieceInfoPanel } from './PieceInfoPanel';
import './BoardTheme.css';

interface RightPanelProps {
    viewMode: '3d' | '2d';
    onToggleView: () => void;
    autoRotateCamera?: boolean;
    onToggleAutoRotate?: () => void;
    freeCam?: boolean;
    onToggleFreeCam?: () => void;
    boardFlipped?: boolean;
    onToggleBoardFlip?: () => void;
    selectedPiece: any | null; // using any to avoid re-defining Piece interface if not exported, but ideally should import
    onClosePieceInfo: () => void;
}

export const RightPanel = ({
    viewMode,
    onToggleView,
    autoRotateCamera,
    onToggleAutoRotate,
    freeCam,
    onToggleFreeCam,
    boardFlipped,
    onToggleBoardFlip,
    selectedPiece,
    onClosePieceInfo
}: RightPanelProps) => {
    const { game, loading, error, resignGame, offerDraw, isAIThinking, makeAIMove, createGame, fetchGame, connectHub } = useGameStore();

    if (loading) return <div className="game-sidebar right-panel">Loading Realm...</div>;
    if (error) return <div className="game-sidebar right-panel">Error: {error}</div>;
    if (!game) return <div className="game-sidebar right-panel">No active game</div>;

    const handleNewGame = async () => {
        if (!window.confirm("Start a new game?")) return;

        const newId = await createGame();
        if (newId) {
            const newUrl = `${window.location.pathname}?gameId=${newId}`;
            window.history.pushState({ path: newUrl }, '', newUrl);
            await fetchGame(newId);
            connectHub(newId);
        }
    };

    const isWhiteTurn = game.currentTurn === 0;

    // --- Captured Pieces Logic (Copied from Sidebar) ---
    const initialCounts = {
        0: { 0: 8, 1: 2, 2: 2, 3: 2, 4: 1, 5: 1 }, // White
        1: { 0: 8, 1: 2, 2: 2, 3: 2, 4: 1, 5: 1 }  // Black
    };

    const currentCounts = {
        0: { 0: 0, 1: 0, 2: 0, 3: 0, 4: 0, 5: 0 },
        1: { 0: 0, 1: 0, 2: 0, 3: 0, 4: 0, 5: 0 }
    };

    game.pieces.forEach(p => {
        // @ts-ignore
        if (currentCounts[p.color]) currentCounts[p.color][p.type]++;
    });

    const getCapturedList = (color: number) => {
        const captured: number[] = [];
        for (let type = 0; type <= 4; type++) {
            // @ts-ignore
            const count = initialCounts[color][type] - currentCounts[color][type];
            for (let i = 0; i < count; i++) {
                captured.push(type);
            }
        }
        return captured.sort((a, b) => a - b);
    };

    const whiteCaptured = getCapturedList(0);
    const blackCaptured = getCapturedList(1);

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

    const isGameOver = game.status >= 2;
    const statusText = getStatusText(game.status);

    return (
        <aside className="game-sidebar right-panel" style={{ width: '360px', borderLeft: '12px solid #4a3c31', borderRight: 'none' }}>
            <header className="sidebar-header">
                <h1 className="sidebar-title">Medieval Chess</h1>
                <div style={{ fontSize: '0.8rem', color: 'var(--text-secondary)', marginTop: '4px' }}>
                    Match ID: {game.id.substring(0, 8)}...
                </div>
            </header>

            {/* Game Status Card */}
            <div className="status-card">
                <div style={{ display: 'flex', alignItems: 'center', gap: '10px', marginBottom: '10px' }}>
                    <div style={{
                        width: '12px', height: '12px', borderRadius: '50%',
                        backgroundColor: isWhiteTurn ? '#fff' : '#000', border: '1px solid #666'
                    }} />
                    <span style={{ fontWeight: 'bold', fontSize: '1.1em' }}>
                        {isGameOver ? statusText : (isWhiteTurn ? "White's Turn" : "Black's Turn")}
                    </span>
                </div>
                <div style={{ display: 'flex', justifyContent: 'space-between', fontSize: '0.9em' }}>
                    <span>Turn #{game.turnNumber}</span>
                    <span style={{ color: isGameOver ? '#ff6b6b' : '#e6c68b' }}>{statusText}</span>
                </div>
                <div style={{ marginTop: '15px', borderTop: '1px solid #4a3c31', paddingTop: '10px' }}>
                    <div style={{ fontSize: '0.8em', color: '#888', marginBottom: '2px' }}>Advantage White</div>
                    {renderCapturedRow(blackCaptured, 1)}
                    <div style={{ fontSize: '0.8em', color: '#888', marginBottom: '2px', marginTop: '6px' }}>Advantage Black</div>
                    {renderCapturedRow(whiteCaptured, 0)}
                </div>
            </div>

            {/* Main Content Area: Detail View or Move History */}
            <div style={{ flex: 1, display: 'flex', flexDirection: 'column', overflow: 'hidden', marginBottom: '10px' }}>
                {selectedPiece ? (
                    <div style={{ flex: 1, display: 'flex', flexDirection: 'column', overflow: 'hidden', animation: 'fadeIn 0.2s' }}>
                        <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '5px' }}>
                            <span style={{ fontWeight: 'bold', color: 'var(--accent-primary)' }}>Unit Details</span>
                            <button
                                onClick={onClosePieceInfo}
                                style={{
                                    background: 'transparent', border: '1px solid #6f5f50', color: '#aaa',
                                    cursor: 'pointer', padding: '2px 8px', fontSize: '0.8em'
                                }}
                            >
                                Back to History
                            </button>
                        </div>
                        {/* Embedding PieceInfoPanel content */}
                        <PieceInfoPanel piece={selectedPiece} onClose={onClosePieceInfo} embedded={true} />
                    </div>
                ) : (
                    <>
                        <div style={{ marginBottom: '8px', fontWeight: 'bold', color: 'var(--text-secondary)' }}>
                            Move History
                        </div>
                        <div className="move-history">
                            {(!game.moveHistory || game.moveHistory.length === 0) ? (
                                <div className="history-row"><span style={{ color: '#888' }}>--</span><span>Start</span></div>
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
                    </>
                )}
            </div>

            {/* Controls Footer */}
            <div style={{ display: 'flex', flexDirection: 'column', gap: '10px', marginTop: 'auto' }}>
                {viewMode === '3d' && (
                    <div style={{ display: 'flex', gap: '5px' }}>
                        {onToggleAutoRotate && (
                            <button className="btn-medieval" onClick={onToggleAutoRotate} style={{ flex: 1, fontSize: '0.75em', padding: '6px', background: autoRotateCamera ? 'linear-gradient(180deg, #2c5c2c 0%, #1b3e1b 100%)' : undefined }}>
                                🔄 Auto: {autoRotateCamera ? 'ON' : 'OFF'}
                            </button>
                        )}
                        {onToggleFreeCam && (
                            <button className="btn-medieval" onClick={onToggleFreeCam} style={{ flex: 1, fontSize: '0.75em', padding: '6px', background: freeCam ? 'linear-gradient(180deg, #5c5c2c 0%, #3e3e1b 100%)' : undefined }}>
                                🎮 Free: {freeCam ? 'ON' : 'OFF'}
                            </button>
                        )}
                    </div>
                )}
                {viewMode === '2d' && onToggleBoardFlip && (
                    <button className="btn-medieval" onClick={onToggleBoardFlip} style={{ fontSize: '0.85em', background: boardFlipped ? 'linear-gradient(180deg, #5c2c5c 0%, #3e1b3e 100%)' : undefined }}>
                        🔄 Flip Board: {boardFlipped ? 'Black' : 'White'}
                    </button>
                )}

                <button className="btn-medieval" onClick={onToggleView}>
                    Switch to {viewMode === '3d' ? '2D Map' : '3D View'}
                </button>

                {!isGameOver ? (
                    <div style={{ display: 'flex', gap: '5px' }}>
                        <button className="btn-medieval" style={{ flex: 1, fontSize: '0.8em', padding: '8px' }} onClick={() => { if (game && window.confirm("Offer a draw?")) offerDraw(game.id, game.currentTurn); }}>
                            Offer Draw
                        </button>
                        <button className="btn-medieval" style={{ flex: 1, fontSize: '0.8em', padding: '8px', background: 'linear-gradient(180deg, #5c2c2c 0%, #3e1b1b 100%)' }} onClick={() => { if (game && window.confirm("Resign?")) resignGame(game.id, game.currentTurn); }}>
                            Resign
                        </button>
                    </div>
                ) : (
                    <button
                        className="btn-medieval"
                        style={{ fontSize: '1em', padding: '12px', background: 'linear-gradient(180deg, #2c5c2c 0%, #1b3e1b 100%)', border: '1px solid #4CAF50' }}
                        onClick={handleNewGame}
                    >
                        ⚔️ Start New Game
                    </button>
                )}

                {/* AI Controls */}
                {!isGameOver && (
                    <button
                        className="btn-medieval"
                        disabled={isAIThinking}
                        style={{
                            fontSize: '0.8em',
                            padding: '8px',
                            background: isAIThinking ? '#444' : 'linear-gradient(180deg, #2c4c5c 0%, #1b2e3e 100%)',
                            cursor: isAIThinking ? 'not-allowed' : 'pointer'
                        }}
                        onClick={() => { if (game) makeAIMove(game.id); }}
                    >
                        {isAIThinking ? '🤖 AI Thinking...' : '🤖 Play AI Move'}
                    </button>
                )}
            </div>
        </aside>
    );
};
