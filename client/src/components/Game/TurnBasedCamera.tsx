import { useRef, useEffect } from 'react';
import { useFrame, useThree } from '@react-three/fiber';
import * as THREE from 'three';

interface TurnBasedCameraProps {
    currentTurn: number; // 0 = White, 1 = Black
    enabled?: boolean;
}

/**
 * Camera controller that rotates laterally around the board based on turn.
 * White (turn 0): Faces from south side
 * Black (turn 1): Faces from north side (180° rotation around Y axis)
 */
export const TurnBasedCamera = ({ currentTurn, enabled = true }: TurnBasedCameraProps) => {
    const { camera } = useThree();
    const targetAngle = useRef(0);
    const currentAngle = useRef(0);
    const isInitialized = useRef(false);

    // Camera distance and height
    const distance = 10;
    const height = 8;

    // Get target angle based on turn (in radians)
    const getTargetAngle = (turn: number): number => {
        // White = 0° (from -Z looking toward +Z, white pieces in front)
        // Black = 180° (PI radians, from +Z looking toward -Z, black pieces in front)
        return turn === 0 ? 0 : Math.PI;
    };

    useEffect(() => {
        if (!enabled) return;

        const newAngle = getTargetAngle(currentTurn);
        targetAngle.current = newAngle;

        // Initialize immediately on first render
        if (!isInitialized.current) {
            currentAngle.current = newAngle;
            const x = Math.sin(newAngle) * distance;
            const z = -Math.cos(newAngle) * distance;
            camera.position.set(x, height, z);
            camera.lookAt(0, 0, 0);
            isInitialized.current = true;
        }
    }, [currentTurn, enabled, camera]);

    useFrame(() => {
        if (!enabled) return;

        // Smoothly interpolate angle (lateral rotation)
        const lerpFactor = 0.04;

        // Calculate shortest path rotation
        let angleDiff = targetAngle.current - currentAngle.current;

        // Normalize angle difference to [-PI, PI] for shortest path
        while (angleDiff > Math.PI) angleDiff -= Math.PI * 2;
        while (angleDiff < -Math.PI) angleDiff += Math.PI * 2;

        // Apply smooth rotation
        currentAngle.current += angleDiff * lerpFactor;

        // Convert polar to cartesian
        const x = Math.sin(currentAngle.current) * distance;
        const z = -Math.cos(currentAngle.current) * distance;

        camera.position.set(x, height, z);
        camera.lookAt(0, 0, 0);
    });

    return null;
};
