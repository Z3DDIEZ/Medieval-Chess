import { useEffect, useState, useRef } from 'react';
import { Canvas } from '@react-three/fiber';
import { OrbitControls } from '@react-three/drei';
import { Board3D } from './components/Game/Board3D';
import { Board2D } from './components/Game/Board2D';
import { RightPanel } from './components/Game/RightPanel';
import { TurnBasedCamera } from './components/Game/TurnBasedCamera';
import { BattleDialog } from './components/Game/BattleDialog';
import { SceneLighting, BoardFrame, GroundPlane } from './components/Game/SceneEnvironment';
import { useGameStore } from './store/useGameStore';
import './App.css';

function App() {
  const { fetchGame, createGame, connectHub, game } = useGameStore();

  // ... (existing code for state)
  const [viewMode, setViewMode] = useState<'3d' | '2d'>('3d');
  const [selectedPiecePos, setSelectedPiecePos] = useState<string | null>(null);
  const [autoRotateCamera, setAutoRotateCamera] = useState(true); // Auto-rotate on turn change
  const [freeCam, setFreeCam] = useState(false); // Free cam allows manual orbit
  const [cameraAnimating, setCameraAnimating] = useState(false); // Track if camera is rotating
  const [boardFlipped, setBoardFlipped] = useState(false); // Flip board perspective

  // Use a ref to prevent double-initialization in React Strict Mode which causes race conditions
  const initializedRef = useRef(false);

  useEffect(() => {
    if (initializedRef.current) return;
    initializedRef.current = true;

    const initGame = async () => {
      // Check for existing game ID in URL
      const params = new URLSearchParams(window.location.search);
      let gameId = params.get('gameId');

      if (gameId) {
        console.log("Loading existing game:", gameId);
        await fetchGame(gameId);
        connectHub(gameId);
      } else {
        console.log("Creating new game...");
        const newGameId = await createGame();
        if (newGameId) {
          // Update URL without reload so refresh keeps the game
          const newUrl = `${window.location.pathname}?gameId=${newGameId}`;
          window.history.pushState({ path: newUrl }, '', newUrl);

          await fetchGame(newGameId);
          connectHub(newGameId);
        }
      }
    };
    initGame();
  }, []);

  // Get the selected piece from game state (now includes all Medieval fields)
  const selectedPiece = game && selectedPiecePos
    ? game.pieces.find(p => p.position === selectedPiecePos) || null
    : null;

  // Current turn for camera rotation (default to White if no game)
  const currentTurn = game?.currentTurn ?? 0;

  // OrbitControls enabled only when freeCam is ON and camera is not animating
  const orbitEnabled = freeCam && !cameraAnimating;

  return (
    <div style={{ width: '100vw', height: '100vh', background: '#111', display: 'flex' }}>


      {/* Main Viewport */}
      <div style={{ flex: 1, position: 'relative', overflow: 'hidden', backgroundColor: '#000000' }}>
        {viewMode === '3d' ? (
          <Canvas shadows camera={{ position: [0, 8, 6], fov: 50 }} gl={{ antialias: true }} onCreated={({ gl }) => gl.setClearColor('#050505')}>
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

            {/* Tournament-style Scene Environment */}
            <SceneLighting />
            <BoardFrame />
            <GroundPlane />

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

        {/* Battle Narrative Dialog - Overlay */}
        <BattleDialog entries={game?.narrative || []} />

      </div>

      <RightPanel
        viewMode={viewMode}
        onToggleView={() => setViewMode(v => v === '3d' ? '2d' : '3d')}
        autoRotateCamera={autoRotateCamera}
        onToggleAutoRotate={() => setAutoRotateCamera(v => !v)}
        freeCam={freeCam}
        onToggleFreeCam={() => setFreeCam(v => !v)}
        boardFlipped={boardFlipped}
        onToggleBoardFlip={() => setBoardFlipped(v => !v)}
        selectedPiece={selectedPiece}
        onClosePieceInfo={() => setSelectedPiecePos(null)}
      />
    </div>
  );
}

export default App;
