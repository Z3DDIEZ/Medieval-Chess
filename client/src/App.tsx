import { useEffect, useState } from 'react';
import { Canvas } from '@react-three/fiber';
import { OrbitControls, Stars, Environment } from '@react-three/drei';
import { Board3D } from './components/Game/Board3D';
import { Board2D } from './components/Game/Board2D';
import { Sidebar } from './components/Game/Sidebar';
import { useGameStore } from './store/useGameStore';
import './App.css';

function App() {
  const { fetchGame, createGame, connectHub } = useGameStore();
  const [viewMode, setViewMode] = useState<'3d' | '2d'>('3d');

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

  return (
    <div style={{ width: '100vw', height: '100vh', background: '#111', display: 'flex' }}>

      <Sidebar viewMode={viewMode} onToggleView={() => setViewMode(v => v === '3d' ? '2d' : '3d')} />

      {/* Main Viewport */}
      <div style={{ flex: 1, position: 'relative', overflow: 'hidden', backgroundColor: '#1e1e1e' }}>
        {viewMode === '3d' ? (
          <Canvas shadows camera={{ position: [0, 10, 5], fov: 50 }}>
            <OrbitControls minPolarAngle={0} maxPolarAngle={Math.PI / 2.2} />
            <ambientLight intensity={0.5} />
            <spotLight position={[10, 15, 10]} angle={0.3} penumbra={1} castShadow intensity={100} />
            <pointLight position={[-10, -10, -10]} intensity={0.5} />

            <Stars radius={100} depth={50} count={5000} factor={4} saturation={0} fade speed={1} />
            <Environment preset="night" />

            <Board3D />
          </Canvas>
        ) : (
          <div style={{
            width: '100%',
            height: '100%',
            overflow: 'auto',
            display: 'flex',
            justifyContent: 'center',
            alignItems: 'center',
            background: 'radial-gradient(circle at center, #2c241b 0%, #15100c 100%)'
          }}>
            <Board2D />
          </div>
        )}
      </div>
    </div>
  );
}

export default App;
