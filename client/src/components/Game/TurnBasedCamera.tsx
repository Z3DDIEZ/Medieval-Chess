import { useRef, useEffect, useCallback } from 'react';
import { useFrame, useThree } from '@react-three/fiber';

interface TurnBasedCameraProps {
    currentTurn: number; // 0 = White, 1 = Black
    enabled?: boolean;   // Auto-rotate on turn change
    freeCam?: boolean;   // When true, allow orbit after animation completes
    onAnimatingChange?: (isAnimating: boolean) => void; // Callback for animation state
}

/**
 * Camera controller that rotates laterally around the board based on turn.
 * Supports smooth rotation even with free cam - disables orbit during transition.
 */
export const TurnBasedCamera = ({
    currentTurn,
    enabled = true,
    freeCam = false,
    onAnimatingChange
}: TurnBasedCameraProps) => {
    const { camera } = useThree();
    const targetAngle = useRef(0);
    const currentAngle = useRef(0);
    const isInitialized = useRef(false);
    const isAnimating = useRef(false);

    // Camera distance and height
    const distance = 7;
    const height = 10;

    // Threshold for considering animation complete
    const ANIMATION_THRESHOLD = 0.01;

    // Get target angle based on turn (in radians)
    const getTargetAngle = (turn: number): number => {
        return turn === 0 ? 0 : Math.PI;
    };

    const setAnimating = useCallback((value: boolean) => {
        if (isAnimating.current !== value) {
            isAnimating.current = value;
            onAnimatingChange?.(value);
        }
    }, [onAnimatingChange]);

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
        } else {
            // Start animating on turn change
            setAnimating(true);
        }
    }, [currentTurn, enabled, camera, setAnimating]);

    useFrame(() => {
        if (!enabled) return;

        // Calculate angle difference
        let angleDiff = targetAngle.current - currentAngle.current;

        // Normalize to shortest path
        while (angleDiff > Math.PI) angleDiff -= Math.PI * 2;
        while (angleDiff < -Math.PI) angleDiff += Math.PI * 2;

        // Check if we're close enough to stop
        if (Math.abs(angleDiff) < ANIMATION_THRESHOLD) {
            currentAngle.current = targetAngle.current;
            setAnimating(false);

            // If freeCam is on and not animating, don't override camera
            if (freeCam) return;
        } else {
            // Still animating - take control of camera
            setAnimating(true);
        }

        // Smoothly interpolate angle
        const lerpFactor = 0.05; // Slightly faster for better feel
        currentAngle.current += angleDiff * lerpFactor;

        // Convert polar to cartesian
        const x = Math.sin(currentAngle.current) * distance;
        const z = -Math.cos(currentAngle.current) * distance;

        camera.position.set(x, height, z);
        camera.lookAt(0, 0, 0);
    });

    return null;
};
