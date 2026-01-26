import { useEffect, useState } from 'react';
import { Canvas } from '@react-three/fiber';
import { OrbitControls, Stars, Environment } from '@react-three/drei';
import { Board3D } from './components/Game/Board3D';
import { Board2D } from './components/Game/Board2D';
import { useGameStore } from './store/useGameStore';
import './App.css';

function App() {
  const { fetchGame, createGame, game, loading, error, connectHub } = useGameStore();
  const [viewMode, setViewMode] = useState<'3d' | '2d'>('3d');

  useEffect(() => {
    const initGame = async () => {
      const newGameId = await createGame();
      if (newGameId) {
        await fetchGame(newGameId);
        // Hub connection is handled by Board components, but we can do it here too if strict
        // better to let Board handle it so we don't duplicate logic, OR move it to App
        connectHub(newGameId);
      }
    };
    initGame();
  }, []);

  return (
    <div style={{ width: '100vw', height: '100vh', background: '#111', display: 'flex' }}>
      {/* Sidebar for Controls & Stats */}
      <div style={{ width: '300px', background: '#1a1a1a', borderRight: '1px solid #333', color: 'white', padding: '20px', display: 'flex', flexDirection: 'column', gap: '20px', zIndex: 10 }}>
        <h1 style={{ margin: 0, borderBottom: '1px solid #444', paddingBottom: '10px' }}>Medieval Chess</h1>

        <button onClick={() => setViewMode(v => v === '3d' ? '2d' : '3d')} style={{ padding: '12px', fontSize: '16px', cursor: 'pointer', background: '#444', color: 'white', border: 'none', borderRadius: '4px' }}>
          Switch to {viewMode === '3d' ? '2D Tactical' : '3D Immersive'}
        </button>

        <div style={{ background: '#222', padding: '15px', borderRadius: '8px' }}>
          {loading && <p>Loading Realm...</p>}
          {error && <p style={{ color: '#ff6b6b' }}>Error: {error}</p>}
          {game && (
            <div style={{ display: 'flex', flexDirection: 'column', gap: '10px' }}>
              <div style={{ fontSize: '1.2em' }}>Current Turn</div>
              <div style={{ fontWeight: 'bold', color: game.currentTurn === 0 ? '#ddd' : '#888' }}>
                {game.currentTurn === 0 ? "White " : "Black "}
                ({game.turnNumber})
              </div>
              <div style={{ fontSize: '0.9em', color: '#aaa' }}>Status: {game.status}</div>
            </div>
          )}
        </div>

        <div style={{ marginTop: 'auto', fontSize: '0.8em', color: '#666' }}>
          ID: {game?.id}
        </div>
      </div>

      {/* Main Viewport */}
      <div style={{ flex: 1, position: 'relative', overflow: 'hidden' }}>
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
          <div style={{ width: '100%', height: '100%', overflow: 'auto', display: 'flex', justifyContent: 'center', alignItems: 'center', background: '#2b2b2b' }}>
            <Board2D />
          </div>
        )}
      </div>
    </div>
  );
}

export default App;
