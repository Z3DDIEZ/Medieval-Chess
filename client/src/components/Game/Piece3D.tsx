import { useRef, useMemo } from 'react';
import { useGLTF } from '@react-three/drei';
import * as THREE from 'three';

interface Piece3DProps {
    type: number;    // 0=Pawn, 1=Knight, 2=Bishop, 3=Rook, 4=Queen, 5=King
    color: number;   // 0=White, 1=Black
    position: string; // e.g., "e2"
}

// Mapping from piece type to model node name prefix
const PIECE_TYPE_NAMES: Record<number, string> = {
    0: 'Pawn',
    1: 'Knight',
    2: 'Bishop',
    3: 'Rook',
    4: 'Queen',
    5: 'King',
};

// Helper to convert "e2" -> Vector3(x, 0, z)
// Board orientation: White (rank 1-2) near camera, Black (rank 7-8) far
// Files a-h go left to right (x: -3.5 to +3.5)
// Ranks 1-8: rank 1 at z=+3.5 (near), rank 8 at z=-3.5 (far)
const getPositionFromAlgebraic = (alg: string): [number, number, number] => {
    if (!alg || alg.length < 2) return [0, 0.15, 0];
    const file = alg.charCodeAt(0) - 97; // a=0, h=7
    const rank = parseInt(alg[1]) - 1;   // 1=0, 8=7
    return [file - 3.5, 0.15, 3.5 - rank];
};

// Scale to fit pieces on the board properly
const PIECE_SCALE = 0.22;

// Preload the GLTF file
useGLTF.preload('/models/scene.gltf');

export const Piece3D = ({ type, color, position }: Piece3DProps) => {
    const meshRef = useRef<THREE.Mesh>(null);

    // Load the GLTF file containing all chess pieces
    const gltf = useGLTF('/models/scene.gltf');

    // Get the correct geometry based on piece type and color
    const geometry = useMemo(() => {
        const typeName = PIECE_TYPE_NAMES[type];
        const colorName = color === 0 ? 'White' : 'Black';
        const searchPattern = `Chess_${typeName}_${colorName}`;

        // Traverse the entire scene to find the matching mesh
        let foundGeometry: THREE.BufferGeometry | null = null;

        gltf.scene.traverse((child) => {
            if (foundGeometry) return; // Already found

            // Check if this node's name matches our pattern
            if (child.name.includes(searchPattern)) {
                // This is the parent node, look for mesh children
                child.traverse((subChild) => {
                    if (foundGeometry) return;
                    if (subChild instanceof THREE.Mesh && subChild.geometry) {
                        foundGeometry = subChild.geometry;
                    }
                });
            }

            // Also check if this node itself is a mesh matching our pattern
            if (!foundGeometry && child instanceof THREE.Mesh && child.name.includes(searchPattern)) {
                foundGeometry = child.geometry;
            }
        });

        if (!foundGeometry) {
            console.warn(`Could not find piece: ${typeName} ${colorName}`);
        }

        return foundGeometry;
    }, [gltf.scene, type, color]);

    // Get material based on color
    const material = useMemo(() => {
        if (color === 0) {
            // White pieces - use a warm cream color with slight shine
            return new THREE.MeshStandardMaterial({
                color: '#f5f0e6',
                emissive: '#e8e0d0',
                emissiveIntensity: 0.05,
                metalness: 0.2,
                roughness: 0.4,
            });
        } else {
            // Black pieces - dark grey with stronger sheen for visibility
            return new THREE.MeshStandardMaterial({
                color: '#2a2a2a',
                emissive: '#1a1a1a',
                emissiveIntensity: 0.1,
                metalness: 0.4,
                roughness: 0.3,
            });
        }
    }, [color]);

    const [x, y, z] = getPositionFromAlgebraic(position);

    // Fix piece orientation:
    // GLTF models are oriented with Y-up but may face wrong direction
    // All pieces should face toward the opponent (toward center of board)
    // White pieces face toward -Z (toward black), Black pieces face toward +Z (toward white)
    // Since our board has white at +Z, white pieces need -90° Y rotation, black need +90°
    const baseRotation = -Math.PI / 2; // Rotate 90° to face across the board
    const colorRotation = color === 1 ? Math.PI : 0; // Black faces opposite direction
    const rotation: [number, number, number] = [0, baseRotation + colorRotation, 0];

    if (!geometry) {
        return null;
    }

    return (
        <mesh
            ref={meshRef}
            position={[x, y, z]}
            rotation={rotation}
            geometry={geometry}
            material={material}
            scale={[PIECE_SCALE, PIECE_SCALE, PIECE_SCALE]}
            castShadow
            receiveShadow
        />
    );
};

