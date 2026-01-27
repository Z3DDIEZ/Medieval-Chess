import { getPieceComponent } from './ChessAssets';
import './BoardTheme.css';

interface PromotionPickerProps {
    color: number; // 0=White, 1=Black
    onSelect: (pieceType: number) => void;
    onCancel: () => void;
}

const PROMOTION_OPTIONS = [
    { type: 4, name: 'Queen' },
    { type: 3, name: 'Rook' },
    { type: 2, name: 'Bishop' },
    { type: 1, name: 'Knight' }
];

export const PromotionPicker = ({ color, onSelect, onCancel }: PromotionPickerProps) => {
    return (
        <div className="promotion-overlay" onClick={onCancel}>
            <div className="promotion-modal" onClick={e => e.stopPropagation()}>
                <h3>Promote Pawn</h3>
                <div className="promotion-options">
                    {PROMOTION_OPTIONS.map(option => {
                        const PieceIcon = getPieceComponent(option.type, color);
                        return (
                            <button
                                key={option.type}
                                className="promotion-option"
                                onClick={() => onSelect(option.type)}
                                title={option.name}
                            >
                                {PieceIcon && <PieceIcon style={{ width: '100%', height: '100%' }} />}
                            </button>
                        );
                    })}
                </div>
            </div>
        </div>
    );
};
