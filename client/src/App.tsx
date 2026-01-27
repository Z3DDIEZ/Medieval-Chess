import { useEffect, useState } from 'react';
import { Canvas } from '@react-three/fiber';
import { OrbitControls, Stars, Environment } from '@react-three/drei';
import { Board3D } from './components/Game/Board3D';
import { Board2D } from './components/Game/Board2D';
import { Sidebar } from './components/Game/Sidebar';
import { PieceInfoPanel } from './components/Game/PieceInfoPanel';
import { TurnBasedCamera } from './components/Game/TurnBasedCamera';
import { useGameStore } from './store/useGameStore';
import './App.css';

interface SelectedPiece {
  type: number;
  color: number;
  position: string;
  loyalty: number;
  maxHP: number;
  currentHP: number;
}

function App() {
  const { fetchGame, createGame, connectHub, game } = useGameStore();
  const [viewMode, setViewMode] = useState<'3d' | '2d'>('3d');
  const [selectedPiecePos, setSelectedPiecePos] = useState<string | null>(null);
  const [autoRotateCamera, setAutoRotateCamera] = useState(true); // Auto-rotate on turn change
  const [freeCam, setFreeCam] = useState(false); // Free cam allows manual orbit
  const [cameraAnimating, setCameraAnimating] = useState(false); // Track if camera is rotating
  const [boardFlipped, setBoardFlipped] = useState(false); // Flip board perspective

  useEffect(() => {
    const initGame = async () => {
      // For now, auto-create game if none exists (simple logic from previous iteration)
      // In production this would probably check URL params or a lobby
      const newGameId = await createGame();
      if (newGameId) {
        await fetchGame(newGameId);
        connectHub(newGameId);
      }
    };
    initGame();
  }, []);

  // Get the selected piece from game state
  const selectedPiece: SelectedPiece | null = game && selectedPiecePos
    ? game.pieces.find(p => p.position === selectedPiecePos) || null
    : null;

  // Current turn for camera rotation (default to White if no game)
  const currentTurn = game?.currentTurn ?? 0;

  // OrbitControls enabled only when freeCam is ON and camera is not animating
  const orbitEnabled = freeCam && !cameraAnimating;

  return (
    <div style={{ width: '100vw', height: '100vh', background: '#111', display: 'flex' }}>

      <Sidebar
        viewMode={viewMode}
        onToggleView={() => setViewMode(v => v === '3d' ? '2d' : '3d')}
        autoRotateCamera={autoRotateCamera}
        onToggleAutoRotate={() => setAutoRotateCamera(v => !v)}
        freeCam={freeCam}
        onToggleFreeCam={() => setFreeCam(v => !v)}
        boardFlipped={boardFlipped}
        onToggleBoardFlip={() => setBoardFlipped(v => !v)}
      />

      {/* Main Viewport */}
      <div style={{ flex: 1, position: 'relative', overflow: 'hidden', backgroundColor: '#1e1e1e' }}>
        {viewMode === '3d' ? (
          <Canvas shadows camera={{ position: [0, 8, 6], fov: 50 }}>
            {/* Turn-based camera controller */}
            <TurnBasedCamera
              currentTurn={currentTurn}
              enabled={autoRotateCamera}
              freeCam={freeCam}
              onAnimatingChange={setCameraAnimating}
            />

            {/* Allow manual orbit only when freeCam is enabled and not animating */}
            <OrbitControls
              enabled={orbitEnabled}
              enablePan={false}
              minPolarAngle={0.3}
              maxPolarAngle={Math.PI / 2.2}
              minDistance={5}
              maxDistance={15}
            />

            <ambientLight intensity={0.5} />
            <spotLight position={[10, 15, 10]} angle={0.3} penumbra={1} castShadow intensity={100} />
            <pointLight position={[-10, -10, -10]} intensity={0.5} />

            <Stars radius={100} depth={50} count={5000} factor={4} saturation={0} fade speed={1} />
            <Environment preset="night" />

            <Board3D onPieceSelect={setSelectedPiecePos} />
          </Canvas>
        ) : (
          <div style={{
            width: '100%',
            height: '100%',
            overflow: 'hidden',
            display: 'flex',
            justifyContent: 'center',
            alignItems: 'center',
            background: 'radial-gradient(circle at center, #2c241b 0%, #15100c 100%)',
            touchAction: 'none' // Prevent two-finger gestures/scrolling
          }}>
            <Board2D onPieceSelect={setSelectedPiecePos} flipped={boardFlipped} />
          </div>
        )}
      </div>

      {/* Piece Info Panel (Right Side - Always Visible) */}
      <PieceInfoPanel
        piece={selectedPiece}
        onClose={() => setSelectedPiecePos(null)}
      />
    </div>
  );
}

export default App;
