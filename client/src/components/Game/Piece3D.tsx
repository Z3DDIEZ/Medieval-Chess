import { useMemo, type FC } from 'react';
import * as THREE from 'three';
import { mergeGeometries } from 'three/addons/utils/BufferGeometryUtils.js';

interface Piece3DProps {
    type: number;
    color: number; // 0 = White, 1 = Black
    position: string; // e.g., "e2"
}

// Helper to convert "e2" -> Vector3(x, 0, z)
// Board orientation: White (rank 1-2) near camera, Black (rank 7-8) far
// Files a-h go left to right (x: -3.5 to +3.5)
// Ranks 1-8: rank 1 at z=+3.5 (near), rank 8 at z=-3.5 (far)
const getVectorFromAlgebraic = (alg: string): [number, number, number] => {
    if (!alg || alg.length < 2) return [0, 0, 0];
    const file = alg.charCodeAt(0) - 97; // a=0, h=7
    const rank = parseInt(alg[1]) - 1;   // 1=0, 8=7
    // x: file 0->-3.5, file 7->+3.5
    // z: rank 0 (1) -> +3.5, rank 7 (8) -> -3.5 (flipped for camera perspective)
    return [file - 3.5, 0.1, 3.5 - rank];
};

// Create a lathe geometry from a profile (for rotationally symmetric pieces)
const createLatheGeometry = (points: [number, number][], segments: number = 32): THREE.BufferGeometry => {
    const vectors = points.map(([x, y]) => new THREE.Vector2(x, y));
    const geo = new THREE.LatheGeometry(vectors, segments);
    geo.computeVertexNormals();
    return geo;
};

// ==================== PIECE GEOMETRIES ====================

// PAWN: Simple tapered body with dome top
const createPawnGeometry = (): THREE.BufferGeometry => {
    const profile: [number, number][] = [
        [0, 0],           // Base center
        [0.18, 0],        // Base edge
        [0.16, 0.05],     // Base rim
        [0.12, 0.15],     // Taper in
        [0.10, 0.25],     // Neck
        [0.13, 0.30],     // Collar widen
        [0.12, 0.40],     // Head base
        [0.11, 0.45],     // Head mid
        [0.08, 0.50],     // Head top curve
        [0.04, 0.53],     // Top curve
        [0, 0.55],        // Top center
    ];
    return createLatheGeometry(profile);
};

// ROOK: Castle tower with crenellations
const createRookGeometry = (): THREE.BufferGeometry => {
    // Base platform
    const baseGeo = new THREE.CylinderGeometry(0.20, 0.22, 0.08, 16);
    baseGeo.translate(0, 0.04, 0);

    // Tower body
    const bodyGeo = new THREE.CylinderGeometry(0.16, 0.18, 0.35, 16);
    bodyGeo.translate(0, 0.255, 0);

    // Top platform
    const topGeo = new THREE.CylinderGeometry(0.19, 0.16, 0.08, 16);
    topGeo.translate(0, 0.47, 0);

    const geometries: THREE.BufferGeometry[] = [baseGeo, bodyGeo, topGeo];

    // Add crenellations (4 merlons around the top)
    for (let i = 0; i < 4; i++) {
        const angle = (i / 4) * Math.PI * 2 + Math.PI / 4;
        const merlonGeo = new THREE.BoxGeometry(0.08, 0.12, 0.08);
        merlonGeo.translate(
            Math.cos(angle) * 0.13,
            0.57,
            Math.sin(angle) * 0.13
        );
        geometries.push(merlonGeo);
    }

    const merged = mergeGeometries(geometries, false);
    merged.computeVertexNormals();
    return merged;
};

// KNIGHT: Stylized horse head
const createKnightGeometry = (): THREE.BufferGeometry => {
    // Base
    const baseGeo = new THREE.CylinderGeometry(0.18, 0.20, 0.08, 16);
    baseGeo.translate(0, 0.04, 0);

    // Body/neck (tilted cylinder)
    const neckGeo = new THREE.CylinderGeometry(0.10, 0.14, 0.30, 16);
    neckGeo.rotateX(0.2);
    neckGeo.translate(0, 0.22, 0.04);

    // Head (elongated box, tilted forward)
    const headGeo = new THREE.BoxGeometry(0.14, 0.22, 0.18);
    headGeo.rotateX(0.3);
    headGeo.translate(0, 0.45, 0.10);

    // Muzzle
    const muzzleGeo = new THREE.BoxGeometry(0.10, 0.12, 0.14);
    muzzleGeo.rotateX(0.4);
    muzzleGeo.translate(0, 0.48, 0.22);

    // Ears
    const ear1Geo = new THREE.ConeGeometry(0.035, 0.10, 6);
    ear1Geo.translate(-0.05, 0.58, 0.05);

    const ear2Geo = new THREE.ConeGeometry(0.035, 0.10, 6);
    ear2Geo.translate(0.05, 0.58, 0.05);

    const geometries: THREE.BufferGeometry[] = [baseGeo, neckGeo, headGeo, muzzleGeo, ear1Geo, ear2Geo];
    const merged = mergeGeometries(geometries, false);
    merged.computeVertexNormals();
    return merged;
};

// BISHOP: Pointed mitre with slot
const createBishopGeometry = (): THREE.BufferGeometry => {
    const profile: [number, number][] = [
        [0, 0],           // Base center
        [0.18, 0],        // Base edge
        [0.16, 0.06],     // Base rim
        [0.11, 0.12],     // Taper
        [0.09, 0.25],     // Neck
        [0.12, 0.30],     // Collar
        [0.11, 0.40],     // Body
        [0.10, 0.50],     // Upper body
        [0.08, 0.58],     // Mitre base
        [0.05, 0.65],     // Mitre taper
        [0.02, 0.70],     // Near top
        [0, 0.72],        // Top point
    ];

    const lathe = createLatheGeometry(profile);

    // Ball on top
    const ballGeo = new THREE.SphereGeometry(0.045, 16, 12);
    ballGeo.translate(0, 0.76, 0);

    const geometries: THREE.BufferGeometry[] = [lathe, ballGeo];
    const merged = mergeGeometries(geometries, false);
    merged.computeVertexNormals();
    return merged;
};

// QUEEN: Elegant crown with multiple points
const createQueenGeometry = (): THREE.BufferGeometry => {
    const profile: [number, number][] = [
        [0, 0],
        [0.20, 0],
        [0.18, 0.06],
        [0.12, 0.15],
        [0.10, 0.30],
        [0.14, 0.42],
        [0.15, 0.55],
        [0.13, 0.62],
        [0.10, 0.65],
    ];

    const lathe = createLatheGeometry(profile);
    const geometries: THREE.BufferGeometry[] = [lathe];

    // Crown points (8 small cones)
    for (let i = 0; i < 8; i++) {
        const angle = (i / 8) * Math.PI * 2;
        const pointGeo = new THREE.ConeGeometry(0.025, 0.14, 6);
        pointGeo.translate(
            Math.cos(angle) * 0.09,
            0.72,
            Math.sin(angle) * 0.09
        );
        geometries.push(pointGeo);
    }

    // Central orb
    const orbGeo = new THREE.SphereGeometry(0.05, 12, 10);
    orbGeo.translate(0, 0.72, 0);
    geometries.push(orbGeo);

    const merged = mergeGeometries(geometries, false);
    merged.computeVertexNormals();
    return merged;
};

// KING: Tall crown with cross on top
const createKingGeometry = (): THREE.BufferGeometry => {
    const profile: [number, number][] = [
        [0, 0],
        [0.20, 0],
        [0.18, 0.06],
        [0.12, 0.15],
        [0.10, 0.30],
        [0.14, 0.44],
        [0.15, 0.58],
        [0.14, 0.64],
        [0.11, 0.68],
    ];

    const lathe = createLatheGeometry(profile);
    const geometries: THREE.BufferGeometry[] = [lathe];

    // Crown rim (8 small boxes)
    for (let i = 0; i < 8; i++) {
        const angle = (i / 8) * Math.PI * 2;
        const pointGeo = new THREE.BoxGeometry(0.05, 0.08, 0.05);
        pointGeo.translate(
            Math.cos(angle) * 0.10,
            0.72,
            Math.sin(angle) * 0.10
        );
        geometries.push(pointGeo);
    }

    // Cross - vertical post
    const crossPostGeo = new THREE.BoxGeometry(0.045, 0.20, 0.045);
    crossPostGeo.translate(0, 0.86, 0);
    geometries.push(crossPostGeo);

    // Cross - horizontal bar
    const crossBarGeo = new THREE.BoxGeometry(0.16, 0.045, 0.045);
    crossBarGeo.translate(0, 0.90, 0);
    geometries.push(crossBarGeo);

    const merged = mergeGeometries(geometries, false);
    merged.computeVertexNormals();
    return merged;
};

// Get geometry for piece type
const getGeometryForType = (type: number): THREE.BufferGeometry => {
    switch (type) {
        case 0: return createPawnGeometry();
        case 1: return createKnightGeometry();
        case 2: return createBishopGeometry();
        case 3: return createRookGeometry();
        case 4: return createQueenGeometry();
        case 5: return createKingGeometry();
        default: return createPawnGeometry();
    }
};

// Scale factors per piece type
const PIECE_SCALES: Record<number, number> = {
    0: 0.85,  // Pawn
    1: 0.95,  // Knight
    2: 0.95,  // Bishop
    3: 0.90,  // Rook
    4: 1.0,   // Queen
    5: 1.05,  // King
};

export const Piece3D: FC<Piece3DProps> = ({ type, color, position }) => {
    const [x, , z] = getVectorFromAlgebraic(position);

    // Memoize geometry (expensive to create)
    const geometry = useMemo(() => getGeometryForType(type), [type]);

    // Material colors with metallic finish
    const materialColor = color === 0 ? '#f0ebe0' : '#252525';
    const emissive = color === 0 ? '#d8d0c0' : '#151515';

    const scale = PIECE_SCALES[type] ?? 1.0;

    // Black pieces face the opposite direction (toward white's side)
    const rotation: [number, number, number] = color === 1 ? [0, Math.PI, 0] : [0, 0, 0];

    return (
        <mesh
            position={[x, 0, z]}
            rotation={rotation}
            geometry={geometry}
            scale={[scale, scale, scale]}
            castShadow
            receiveShadow
        >
            <meshStandardMaterial
                color={materialColor}
                emissive={emissive}
                emissiveIntensity={0.03}
                metalness={0.25}
                roughness={0.5}
            />
        </mesh>
    );
};
