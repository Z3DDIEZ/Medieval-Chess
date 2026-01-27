import { useRef } from 'react';
import * as THREE from 'three';
import { useFrame } from '@react-three/fiber';

/**
 * Enhanced 3D chess scene environment with:
 * - Wooden board frame/border
 * - Better lighting setup
 * - Subtle animations
 * - Ground plane
 */

// Board frame/border component
export const BoardFrame = () => {
    // Create a wooden frame around the board
    const frameThickness = 0.4;
    const frameHeight = 0.25;
    const boardSize = 8;

    // Wood material for the frame
    const frameMaterial = new THREE.MeshStandardMaterial({
        color: '#4a3728',
        roughness: 0.6,
        metalness: 0.1,
    });

    const cornerMaterial = new THREE.MeshStandardMaterial({
        color: '#3d2d20',
        roughness: 0.5,
        metalness: 0.15,
    });

    return (
        <group position={[0, -0.05, 0]}>
            {/* North frame (far side) */}
            <mesh position={[0, frameHeight / 2, -(boardSize / 2 + frameThickness / 2)]} material={frameMaterial} castShadow receiveShadow>
                <boxGeometry args={[boardSize + frameThickness * 2, frameHeight, frameThickness]} />
            </mesh>

            {/* South frame (near side) */}
            <mesh position={[0, frameHeight / 2, (boardSize / 2 + frameThickness / 2)]} material={frameMaterial} castShadow receiveShadow>
                <boxGeometry args={[boardSize + frameThickness * 2, frameHeight, frameThickness]} />
            </mesh>

            {/* West frame (left side) */}
            <mesh position={[-(boardSize / 2 + frameThickness / 2), frameHeight / 2, 0]} material={frameMaterial} castShadow receiveShadow>
                <boxGeometry args={[frameThickness, frameHeight, boardSize]} />
            </mesh>

            {/* East frame (right side) */}
            <mesh position={[(boardSize / 2 + frameThickness / 2), frameHeight / 2, 0]} material={frameMaterial} castShadow receiveShadow>
                <boxGeometry args={[frameThickness, frameHeight, boardSize]} />
            </mesh>

            {/* Corner posts for visual accent */}
            {[[-1, -1], [-1, 1], [1, -1], [1, 1]].map(([xSign, zSign], i) => (
                <mesh
                    key={i}
                    position={[
                        xSign * (boardSize / 2 + frameThickness / 2),
                        frameHeight / 2 + 0.05,
                        zSign * (boardSize / 2 + frameThickness / 2)
                    ]}
                    material={cornerMaterial}
                    castShadow
                >
                    <cylinderGeometry args={[0.15, 0.18, frameHeight + 0.1, 8]} />
                </mesh>
            ))}
        </group>
    );
};

// Floating particles for atmosphere
export const AtmosphericParticles = () => {
    const particlesRef = useRef<THREE.Points>(null);
    const particleCount = 50;

    // Generate random particle positions
    const positions = new Float32Array(particleCount * 3);
    for (let i = 0; i < particleCount; i++) {
        positions[i * 3] = (Math.random() - 0.5) * 15;
        positions[i * 3 + 1] = Math.random() * 8 + 1;
        positions[i * 3 + 2] = (Math.random() - 0.5) * 15;
    }

    useFrame((state) => {
        if (particlesRef.current) {
            particlesRef.current.rotation.y = state.clock.elapsedTime * 0.02;
        }
    });

    return (
        <points ref={particlesRef}>
            <bufferGeometry>
                <bufferAttribute
                    attach="attributes-position"
                    args={[positions, 3]}
                />
            </bufferGeometry>
            <pointsMaterial
                size={0.03}
                color="#ffd700"
                transparent
                opacity={0.4}
                sizeAttenuation
            />
        </points>
    );
};

// Tournament-style spotlight lighting
// Focused bright light from above, everything else in darkness
export const SceneLighting = () => {
    return (
        <>
            {/* Very subtle ambient - just enough to see silhouettes */}
            <ambientLight intensity={0.08} color="#1a1a2e" />

            {/* Main overhead spotlight - the tournament light */}
            <spotLight
                position={[0, 15, 0]}
                angle={0.5}
                penumbra={0.3}
                intensity={200}
                color="#fff8f0"
                castShadow
                shadow-mapSize-width={2048}
                shadow-mapSize-height={2048}
                shadow-bias={-0.0001}
                target-position={[0, 0, 0]}
            />

            {/* Secondary fill from slight angle - softer */}
            <spotLight
                position={[4, 12, 4]}
                angle={0.4}
                penumbra={0.6}
                intensity={40}
                color="#fffaf5"
                castShadow={false}
            />

            {/* Slight back-rim light for piece definition */}
            <pointLight position={[0, 6, -6]} intensity={15} color="#d4af37" />
        </>
    );
};

// Ground plane with shadow
export const GroundPlane = () => {
    return (
        <mesh rotation={[-Math.PI / 2, 0, 0]} position={[0, -0.15, 0]} receiveShadow>
            <planeGeometry args={[30, 30]} />
            <shadowMaterial opacity={0.3} />
        </mesh>
    );
};
