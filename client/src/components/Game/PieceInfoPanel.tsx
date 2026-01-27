import { getPieceComponent } from './ChessAssets';
import './BoardTheme.css';

interface Piece {
    type: number;
    color: number;
    position: string;
    loyalty: number;
    maxHP: number;
    currentHP: number;
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
    if (!piece) return null;

    const PieceIcon = getPieceComponent(piece.type, piece.color);
    const pieceName = PIECE_NAMES[piece.type] || 'Unknown';
    const pieceValue = PIECE_VALUES[piece.type];
    const description = PIECE_DESCRIPTIONS[piece.type] || '';
    const colorName = piece.color === 0 ? 'White' : 'Black';
    const hpPercent = piece.maxHP > 0 ? (piece.currentHP / piece.maxHP) * 100 : 100;
    const loyaltyPercent = Math.min(100, Math.max(0, piece.loyalty));

    return (
        <div className="piece-info-panel">
            <div className="piece-info-header">
                <div className="piece-info-icon">
                    {PieceIcon && <PieceIcon style={{ width: '100%', height: '100%' }} />}
                </div>
                <div className="piece-info-title">
                    <h3>{colorName} {pieceName}</h3>
                    <span className="piece-position">{piece.position.toUpperCase()}</span>
                </div>
                <button className="piece-info-close" onClick={onClose}>×</button>
            </div>

            <div className="piece-info-body">
                {/* Value */}
                <div className="piece-stat-row">
                    <span className="stat-label">Value</span>
                    <span className="stat-value">
                        {piece.type === 5 ? '∞' : pieceValue}
                    </span>
                </div>

                {/* HP Bar (for Medieval mode) */}
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

                {/* Loyalty Bar (for Medieval mode) */}
                <div className="piece-stat-row">
                    <span className="stat-label">Loyalty</span>
                    <div className="stat-bar-container">
                        <div
                            className="stat-bar loyalty-bar"
                            style={{ width: `${loyaltyPercent}%` }}
                        />
                        <span className="stat-bar-text">{piece.loyalty}%</span>
                    </div>
                </div>

                {/* Description */}
                <div className="piece-description">
                    <p>{description}</p>
                </div>

                {/* Medieval Mechanics Hint (placeholder for future) */}
                <div className="piece-abilities">
                    <div className="ability-header">Abilities</div>
                    <div className="ability-placeholder">
                        <em style={{ color: '#888' }}>Standard chess rules active</em>
                    </div>
                </div>
            </div>
        </div>
    );
};
