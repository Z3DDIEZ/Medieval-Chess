import type { FC } from 'react';

interface Piece3DProps {
    type: number;
    color: number; // 0 = White, 1 = Black
    position: string; // e.g., "e2"
}

// Helper to convert "e2" -> Vector3(x, 0, z)
const getVectorFromAlgebraic = (alg: string): [number, number, number] => {
    if (!alg || alg.length < 2) return [0, 0, 0];
    const file = alg.charCodeAt(0) - 97; // a=0, h=7
    const rank = parseInt(alg[1]) - 1;   // 1=0, 8=7
    // Center board at 0,0. Each square is 1 unit.
    // Files 0..7 -> -3.5 .. 3.5
    // Ranks 0..7 -> -3.5 .. 3.5
    return [file - 3.5, 0.5, rank - 3.5]; // y=0.5 to sit on board
};

export const Piece3D: FC<Piece3DProps> = ({ type, color, position }) => {
    const [x, , z] = getVectorFromAlgebraic(position);
    const materialColor = color === 0 ? '#eeeeee' : '#333333';

    // Simple geometry based on type (Enum: 0=Pawn, 1=Knight, 2=Bishop, 3=Rook, 4=Queen, 5=King)
    // Detailed models would replace this later.
    let scale: [number, number, number] = [0.4, 0.8, 0.4];

    switch (type) {
        case 0: // Pawn
            scale = [0.3, 0.6, 0.3];
            break;
        case 3: // Rook
            scale = [0.4, 0.7, 0.4];
            break;
        case 5: // King
            scale = [0.4, 1.2, 0.4];
            break;
        default:
            scale = [0.35, 0.9, 0.35];
    }

    return (
        <mesh position={[x, 0.5, z]} castShadow>
            <cylinderGeometry args={[scale[0], scale[2], 1, 16]} />
            <meshStandardMaterial color={materialColor} />
        </mesh>
    );
};
