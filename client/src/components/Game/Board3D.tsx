import { Piece3D } from './Piece3D';
import { useGameStore } from '../../store/useGameStore';

export const Board3D = () => {
    const { game } = useGameStore();

    // Generate 8x8 checkerboard
    const squares = [];
    for (let x = 0; x < 8; x++) {
        for (let y = 0; y < 8; y++) {
            const isBlack = (x + y) % 2 === 1;
            squares.push(
                <mesh key={`${x}-${y}`} position={[x - 3.5, 0, y - 3.5]} receiveShadow>
                    <boxGeometry args={[1, 0.2, 1]} />
                    <meshStandardMaterial color={isBlack ? '#5c3a21' : '#d4a87a'} />
                </mesh>
            );
        }
    }

    return (
        <group>
            {/* The Board Base */}
            {squares}

            {/* The Pieces */}
            {game?.pieces.map((p, idx) => (
                <Piece3D
                    key={idx}
                    type={p.type}
                    color={p.color}
                    position={p.position}
                />
            ))}
        </group>
    );
};
