import { getPieceComponent } from './ChessAssets';
import './BoardTheme.css';

interface PieceAbility {
    abilityDefinitionId: string;
    currentCooldown: number;
    maxCooldown: number;
    upgradeTier: number;
    isReady: boolean;
}

interface Piece {
    type: number;
    color: number;
    position: string;
    loyalty: number;
    maxHP: number;
    currentHP: number;
    level: number;
    xp: number;
    promotionTier: string;
    isDefecting: boolean;
    court: string | null;
    abilities: PieceAbility[];
}

interface PieceInfoPanelProps {
    piece: Piece | null;
    onClose: () => void;
}

const PIECE_NAMES: { [key: number]: string } = {
    0: 'Pawn',
    1: 'Knight',
    2: 'Bishop',
    3: 'Rook',
    4: 'Queen',
    5: 'King'
};

const PIECE_VALUES: { [key: number]: number } = {
    0: 1,
    1: 3,
    2: 3,
    3: 5,
    4: 9,
    5: 0 // King has infinite value
};

const PIECE_DESCRIPTIONS: { [key: number]: string } = {
    0: 'The foot soldier. Moves forward, captures diagonally. Can promote upon reaching the enemy\'s back rank.',
    1: 'The cavalry. Moves in an L-shape and is the only piece that can jump over others.',
    2: 'The holy advisor. Moves diagonally across any number of squares.',
    3: 'The siege tower. Moves horizontally or vertically across any number of squares.',
    4: 'The most powerful piece. Combines the movement of both Rook and Bishop.',
    5: 'The monarch. Limited to one square in any direction, but the game is lost if captured.'
};

export const PieceInfoPanel = ({ piece, onClose }: PieceInfoPanelProps) => {
    // Show empty state when no piece selected
    if (!piece) {
        return (
            <div className="piece-info-panel">
                <div className="piece-info-header">
                    <div className="piece-info-icon" style={{ opacity: 0.3 }}>
                        <div style={{ width: '100%', height: '100%', background: '#333', borderRadius: '8px' }} />
                    </div>
                    <div className="piece-info-title">
                        <h3 style={{ color: '#666' }}>No Piece Selected</h3>
                        <span className="piece-position" style={{ color: '#555' }}>--</span>
                    </div>
                </div>

                <div className="piece-info-body">
                    <div style={{
                        color: '#666',
                        textAlign: 'center',
                        padding: '30px 20px',
                        fontSize: '0.9em',
                        lineHeight: '1.5'
                    }}>
                        <p>Click on a piece to view its details.</p>
                        <p style={{ marginTop: '10px', fontSize: '0.85em', color: '#555' }}>
                            You can see HP, Loyalty, and abilities here.
                        </p>
                    </div>
                </div>
            </div>
        );
    }

    const PieceIcon = getPieceComponent(piece.type, piece.color);
    const pieceName = PIECE_NAMES[piece.type] || 'Unknown';
    const pieceValue = PIECE_VALUES[piece.type];
    const description = PIECE_DESCRIPTIONS[piece.type] || '';
    const colorName = piece.color === 0 ? 'White' : 'Black';
    const hpPercent = piece.maxHP > 0 ? (piece.currentHP / piece.maxHP) * 100 : 100;
    const loyaltyPercent = Math.min(100, Math.max(0, piece.loyalty));

    // XP needed for next level (Level * 100)
    const xpForNextLevel = piece.level * 100;
    const xpPercent = xpForNextLevel > 0 ? (piece.xp / xpForNextLevel) * 100 : 0;

    // Loyalty status
    const getLoyaltyStatus = (loyalty: number) => {
        if (loyalty >= 80) return { label: 'Loyal', color: '#4caf50' };
        if (loyalty >= 50) return { label: 'Steady', color: '#8bc34a' };
        if (loyalty >= 30) return { label: 'Wavering', color: '#ff9800' };
        return { label: 'Disloyal', color: '#f44336' };
    };
    const loyaltyStatus = getLoyaltyStatus(piece.loyalty);

    return (
        <div className="piece-info-panel">
            <div className="piece-info-header">
                <div className="piece-info-icon">
                    {PieceIcon && <PieceIcon style={{ width: '100%', height: '100%' }} />}
                </div>
                <div className="piece-info-title">
                    <h3>{colorName} {pieceName}</h3>
                    <div style={{ display: 'flex', alignItems: 'center', gap: '8px' }}>
                        <span className="piece-position">{piece.position.toUpperCase()}</span>
                        {piece.promotionTier && (
                            <span style={{
                                background: '#e0e0e0',
                                color: '#333',
                                padding: '2px 6px',
                                borderRadius: '4px',
                                fontSize: '0.8em',
                                fontStyle: 'italic'
                            }}>
                                {piece.promotionTier}
                            </span>
                        )}
                        {piece.level > 1 && (
                            <span style={{
                                background: '#ffd700',
                                color: '#000',
                                padding: '2px 6px',
                                borderRadius: '4px',
                                fontSize: '0.8em',
                                fontWeight: 'bold'
                            }}>
                                Lv.{piece.level}
                            </span>
                        )}
                    </div>
                </div>
                <button className="piece-info-close" onClick={onClose}>×</button>
            </div>

            {/* Defection Warning */}
            {piece.isDefecting && (
                <div style={{
                    background: '#ff5722',
                    color: 'white',
                    padding: '8px 12px',
                    textAlign: 'center',
                    fontWeight: 'bold',
                    fontSize: '0.85em'
                }}>
                    ⚠️ DEFECTION IMMINENT
                </div>
            )}

            <div className="piece-info-body">
                {/* Court Position */}
                {piece.court && (
                    <div className="piece-stat-row">
                        <span className="stat-label">Court</span>
                        <span className="stat-value" style={{ color: piece.court === 'KingsCourt' ? '#4fc3f7' : '#ba68c8' }}>
                            {piece.court === 'KingsCourt' ? "👑 King's Court" : "♛ Queen's Court"}
                        </span>
                    </div>
                )}

                {/* Value */}
                <div className="piece-stat-row">
                    <span className="stat-label">Value</span>
                    <span className="stat-value">
                        {piece.type === 5 ? '∞' : pieceValue}
                    </span>
                </div>

                {/* Level & XP */}
                <div className="piece-stat-row">
                    <span className="stat-label">Level {piece.level}</span>
                    <div className="stat-bar-container">
                        <div
                            className="stat-bar"
                            style={{ width: `${xpPercent}%`, background: 'linear-gradient(90deg, #ffd700, #ffeb3b)' }}
                        />
                        <span className="stat-bar-text">{piece.xp}/{xpForNextLevel} XP</span>
                    </div>
                </div>

                {/* HP Bar */}
                <div className="piece-stat-row">
                    <span className="stat-label">HP</span>
                    <div className="stat-bar-container">
                        <div
                            className="stat-bar hp-bar"
                            style={{ width: `${hpPercent}%` }}
                        />
                        <span className="stat-bar-text">{piece.currentHP}/{piece.maxHP}</span>
                    </div>
                </div>

                {/* Loyalty Bar */}
                <div className="piece-stat-row">
                    <span className="stat-label">Loyalty</span>
                    <div className="stat-bar-container">
                        <div
                            className="stat-bar"
                            style={{ width: `${loyaltyPercent}%`, background: loyaltyStatus.color }}
                        />
                        <span className="stat-bar-text" style={{ color: loyaltyStatus.color }}>
                            {piece.loyalty}% ({loyaltyStatus.label})
                        </span>
                    </div>
                </div>

                {/* Description */}
                <div className="piece-description">
                    <p>{description}</p>
                </div>

                {/* Abilities Section */}
                <div className="piece-abilities">
                    <div className="ability-header">Abilities</div>
                    {piece.abilities && piece.abilities.length > 0 ? (
                        <div style={{ display: 'flex', flexDirection: 'column', gap: '8px' }}>
                            {piece.abilities.map((ability, idx) => (
                                <div key={idx} style={{
                                    background: ability.isReady ? 'rgba(76, 175, 80, 0.2)' : 'rgba(100, 100, 100, 0.2)',
                                    padding: '8px',
                                    borderRadius: '4px',
                                    borderLeft: ability.isReady ? '3px solid #4caf50' : '3px solid #666'
                                }}>
                                    <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
                                        <span style={{ fontWeight: 'bold', fontSize: '0.9em' }}>
                                            Tier {ability.upgradeTier + 1} Ability
                                        </span>
                                        {!ability.isReady && (
                                            <span style={{ color: '#ff9800', fontSize: '0.8em' }}>
                                                CD: {ability.currentCooldown}/{ability.maxCooldown}
                                            </span>
                                        )}
                                        {ability.isReady && (
                                            <span style={{ color: '#4caf50', fontSize: '0.8em' }}>
                                                READY
                                            </span>
                                        )}
                                    </div>
                                </div>
                            ))}
                        </div>
                    ) : (
                        <div className="ability-placeholder">
                            <em style={{ color: '#888' }}>No abilities unlocked yet</em>
                        </div>
                    )}
                </div>
            </div>
        </div>
    );
};
