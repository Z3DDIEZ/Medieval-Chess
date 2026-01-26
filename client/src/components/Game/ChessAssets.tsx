import React from 'react';

// Props for the SVG components
interface PieceProps {
    style?: React.CSSProperties;
}

// Standard Chess Piece SVGs (Based on standard vector representations)

export const WhitePawn: React.FC<PieceProps> = ({ style }) => (
    <svg viewBox="0 0 45 45" style={style} className="chess-piece">
        <path
            d="M22.5 9c-2.21 0-4 1.79-4 4 0 .89.29 1.71.78 2.38C17.33 16.5 16 18.59 16 21c0 2.03.94 3.84 2.41 5.03-3 1.06-7.41 5.55-7.41 13.47h23c0-7.92-4.41-12.41-7.41-13.47 1.47-1.19 2.41-3 2.41-5.03 0-2.41-1.33-4.5-3.28-5.62.49-.67.78-1.49.78-2.38 0-2.21-1.79-4-4-4z"
            fill="#ffffff"
            stroke="#000"
            strokeWidth="1.5"
            strokeLinecap="round"
        />
    </svg>
);

export const WhiteKnight: React.FC<PieceProps> = ({ style }) => (
    <svg viewBox="0 0 45 45" style={style} className="chess-piece">
        <path
            d="M22 10c10.5 1 16.5 8 16 29H15c0-9 10-6.5 8-21"
            fill="#ffffff"
            stroke="#000"
            strokeWidth="1.5"
        />
        <path
            d="M24 18c.38 2.32-4.68 1.97-5 4 4.07 3.42 8.45 2.63 8 9M9.5 25.5A23 23 0 0 1 12 10a19 19 0 0 0 6 5a2 2 0 0 1-1-2c0-6 3.5-8 5-11 5 1 9 4 11 8"
            fill="#ffffff"
            stroke="#000"
            strokeWidth="1.5"
            strokeLinejoin="round"
            strokeLinecap="round"
        />
    </svg>
);

export const WhiteBishop: React.FC<PieceProps> = ({ style }) => (
    <svg viewBox="0 0 45 45" style={style} className="chess-piece">
        <g fill="#ffffff" stroke="#000" strokeWidth="1.5" strokeLinecap="round" strokeLinejoin="round">
            <path d="M9 36c3.39-.97 9.11-1.45 13.5-1.45 4.38 0 10.11.48 13.5 1.45M22.5 32c3.39 0 8.35-7.65 8.35-15 0-4.96-3.74-9-8.35-9-4.61 0-8.35 4.04-8.35 9 0 7.35 4.96 15 8.35 15z" />
            <path d="M22.5 12c.83 0 1.5.67 1.5 1.5S23.33 15 22.5 15 21 14.33 21 13.5 21.67 12 22.5 12zM22.5 7V3" />
            <path d="M20 19h5" />
        </g>
    </svg>
);

export const WhiteRook: React.FC<PieceProps> = ({ style }) => (
    <svg viewBox="0 0 45 45" style={style} className="chess-piece">
        <g fill="#ffffff" stroke="#000" strokeWidth="1.5" strokeLinecap="round" strokeLinejoin="round">
            <path d="M9 39h27v-3H9v3zM12 36v-4h21v4H12zM11 14V9h4v2h5V9h5v2h5V9h4v5" />
            <path d="M34 14l-3 3H14l-3-3" />
            <path d="M31 17v12.5c0 2.25-2.25 4.5-4.5 4.5h-8c-2.25 0-4.5-2.25-4.5-4.5V17" />
        </g>
    </svg>
);

export const WhiteQueen: React.FC<PieceProps> = ({ style }) => (
    <svg viewBox="0 0 45 45" style={style} className="chess-piece">
        <g fill="#ffffff" stroke="#000" strokeWidth="1.5" strokeLinecap="round" strokeLinejoin="round">
            <path d="M8 12a2 2 0 1 1-4 0 2 2 0 0 1 4 0zM24.5 7.5a2 2 0 1 1-4 0 2 2 0 0 1 4 0zM41 12a2 2 0 1 1-4 0 2 2 0 0 1 4 0zM10.5 19.5a2 2 0 1 1-4 0 2 2 0 0 1 4 0zM38.5 19.5a2 2 0 1 1-4 0 2 2 0 0 1 4 0z" />
            <path d="M9 26c8.5-1.5 21-1.5 27 0l2-12-7 11V11l-5.5 13.5-3-15-3 15-5.5-13.5V25L7 14l2 12z" />
            <path d="M9 26c0 2 1.5 2 2.5 4 1 2.5 3 1 5 1 2 0 2.5-1 2.5-3l-2.5-2 2.5 2c0 2 1 3 4 3s4-1 4-3l2.5-2-2.5 2c0 2 1 2.5 3 2.5 2 0 4 1.5 5-1 1-2 2.5-2 2.5-4" />
            <path d="M9 26c9 2.5 18 2.5 27 0" />
        </g>
        <path
            d="M10 38.5a3.5 3.5 0 1 1-7 0 3.5 3.5 0 0 1 7 0zM39 38.5a3.5 3.5 0 1 1-7 0 3.5 3.5 0 0 1 7 0z"
            fill="#ffffff"
            stroke="#000"
            strokeWidth="1.5"
        />
        <path
            d="M9 36.5h27"
            stroke="#000"
            strokeWidth="1.5"
            strokeLinecap="round"
        />
    </svg>
);

export const WhiteKing: React.FC<PieceProps> = ({ style }) => (
    <svg viewBox="0 0 45 45" style={style} className="chess-piece">
        <g fill="#ffffff" stroke="#000" strokeWidth="1.5" strokeLinecap="round" strokeLinejoin="round">
            <path d="M22.5 11.63V6M20 8h5" />
            <path d="M22.5 25s4.5-7.5 3-10.5c0 0-1-2.5-3-2.5s-3 2.5-3 2.5c-1.5 3 3 10.5 3 10.5" />
            <path d="M11.5 37c5.5 3.5 15.5 3.5 21 0v-7s9-4.5 6-10.5c-4-6.5-13.5-3.5-16 4V27v-3.5c-2.5-7.5-12-10.5-16-4-3 6 6 10.5 6 10.5v7z" />
            <path d="M11.5 30c5.5-3 15.5-3 21 0" />
            <path d="M11.5 33.5c5.5-3 15.5-3 21 0" />
            <path d="M11.5 37c5.5-3 15.5-3 21 0" />
        </g>
    </svg>
);

export const BlackPawn: React.FC<PieceProps> = ({ style }) => (
    <svg viewBox="0 0 45 45" style={style} className="chess-piece">
        <path
            d="M22.5 9c-2.21 0-4 1.79-4 4 0 .89.29 1.71.78 2.38C17.33 16.5 16 18.59 16 21c0 2.03.94 3.84 2.41 5.03-3 1.06-7.41 5.55-7.41 13.47h23c0-7.92-4.41-12.41-7.41-13.47 1.47-1.19 2.41-3 2.41-5.03 0-2.41-1.33-4.5-3.28-5.62.49-.67.78-1.49.78-2.38 0-2.21-1.79-4-4-4z"
            stroke="#000"
            strokeWidth="1.5"
            strokeLinecap="round"
        />
    </svg>
);

export const BlackKnight: React.FC<PieceProps> = ({ style }) => (
    <svg viewBox="0 0 45 45" style={style} className="chess-piece">
        <path
            d="M22 10c10.5 1 16.5 8 16 29H15c0-9 10-6.5 8-21"
            stroke="#000"
            strokeWidth="1.5"
        />
        <path
            d="M24 18c.38 2.32-4.68 1.97-5 4 4.07 3.42 8.45 2.63 8 9M9.5 25.5A23 23 0 0 1 12 10a19 19 0 0 0 6 5a2 2 0 0 1-1-2c0-6 3.5-8 5-11 5 1 9 4 11 8"
            stroke="#000"
            strokeWidth="1.5"
            strokeLinejoin="round"
            strokeLinecap="round"
        />
    </svg>
);

export const BlackBishop: React.FC<PieceProps> = ({ style }) => (
    <svg viewBox="0 0 45 45" style={style} className="chess-piece">
        <g stroke="#000" strokeWidth="1.5" strokeLinecap="round" strokeLinejoin="round">
            <path d="M9 36c3.39-.97 9.11-1.45 13.5-1.45 4.38 0 10.11.48 13.5 1.45M22.5 32c3.39 0 8.35-7.65 8.35-15 0-4.96-3.74-9-8.35-9-4.61 0-8.35 4.04-8.35 9 0 7.35 4.96 15 8.35 15z" />
            <path d="M22.5 12c.83 0 1.5.67 1.5 1.5S23.33 15 22.5 15 21 14.33 21 13.5 21.67 12 22.5 12zM22.5 7V3" />
            <path d="M20 19h5" />
        </g>
    </svg>
);

export const BlackRook: React.FC<PieceProps> = ({ style }) => (
    <svg viewBox="0 0 45 45" style={style} className="chess-piece">
        <g stroke="#000" strokeWidth="1.5" strokeLinecap="round" strokeLinejoin="round">
            <path d="M9 39h27v-3H9v3zM12 36v-4h21v4H12zM11 14V9h4v2h5V9h5v2h5V9h4v5" />
            <path d="M34 14l-3 3H14l-3-3" />
            <path d="M31 17v12.5c0 2.25-2.25 4.5-4.5 4.5h-8c-2.25 0-4.5-2.25-4.5-4.5V17" />
        </g>
    </svg>
);

export const BlackQueen: React.FC<PieceProps> = ({ style }) => (
    <svg viewBox="0 0 45 45" style={style} className="chess-piece">
        <g stroke="#000" strokeWidth="1.5" strokeLinecap="round" strokeLinejoin="round">
            <path d="M8 12a2 2 0 1 1-4 0 2 2 0 0 1 4 0zM24.5 7.5a2 2 0 1 1-4 0 2 2 0 0 1 4 0zM41 12a2 2 0 1 1-4 0 2 2 0 0 1 4 0zM10.5 19.5a2 2 0 1 1-4 0 2 2 0 0 1 4 0zM38.5 19.5a2 2 0 1 1-4 0 2 2 0 0 1 4 0z" />
            <path d="M9 26c8.5-1.5 21-1.5 27 0l2-12-7 11V11l-5.5 13.5-3-15-3 15-5.5-13.5V25L7 14l2 12z" />
            <path d="M9 26c0 2 1.5 2 2.5 4 1 2.5 3 1 5 1 2 0 2.5-1 2.5-3l-2.5-2 2.5 2c0 2 1 3 4 3s4-1 4-3l2.5-2-2.5 2c0 2 1 2.5 3 2.5 2 0 4 1.5 5-1 1-2 2.5-2 2.5-4" />
            <path d="M9 26c9 2.5 18 2.5 27 0" />
        </g>
        <path
            d="M10 38.5a3.5 3.5 0 1 1-7 0 3.5 3.5 0 0 1 7 0zM39 38.5a3.5 3.5 0 1 1-7 0 3.5 3.5 0 0 1 7 0z"
            stroke="#000"
            strokeWidth="1.5"
        />
        <path
            d="M9 36.5h27"
            stroke="#000"
            strokeWidth="1.5"
            strokeLinecap="round"
        />
    </svg>
);

export const BlackKing: React.FC<PieceProps> = ({ style }) => (
    <svg viewBox="0 0 45 45" style={style} className="chess-piece">
        <g stroke="#000" strokeWidth="1.5" strokeLinecap="round" strokeLinejoin="round">
            <path d="M22.5 11.63V6M20 8h5" />
            <path d="M22.5 25s4.5-7.5 3-10.5c0 0-1-2.5-3-2.5s-3 2.5-3 2.5c-1.5 3 3 10.5 3 10.5" />
            <path d="M11.5 37c5.5 3.5 15.5 3.5 21 0v-7s9-4.5 6-10.5c-4-6.5-13.5-3.5-16 4V27v-3.5c-2.5-7.5-12-10.5-16-4-3 6 6 10.5 6 10.5v7z" />
            <path d="M11.5 30c5.5-3 15.5-3 21 0" />
            <path d="M11.5 33.5c5.5-3 15.5-3 21 0" />
            <path d="M11.5 37c5.5-3 15.5-3 21 0" />
        </g>
    </svg>
);

export const getPieceComponent = (type: number, color: number) => {
    // 0=White, 1=Black
    // 0: Pawn, 1: Knight, 2: Bishop, 3: Rook, 4: Queen, 5: King

    // Safety check needed because 0 is falsy
    if (color === 0) {
        switch (type) {
            case 0: return WhitePawn;
            case 1: return WhiteKnight;
            case 2: return WhiteBishop;
            case 3: return WhiteRook;
            case 4: return WhiteQueen;
            case 5: return WhiteKing;
            default: return null;
        }
    } else {
        switch (type) {
            case 0: return BlackPawn;
            case 1: return BlackKnight;
            case 2: return BlackBishop;
            case 3: return BlackRook;
            case 4: return BlackQueen;
            case 5: return BlackKing;
            default: return null;
        }
    }
};
