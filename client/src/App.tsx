import { useEffect } from 'react';
import { Canvas } from '@react-three/fiber';
import { OrbitControls, Stars, Environment } from '@react-three/drei';
import { Board3D } from './components/Game/Board3D';
import { useGameStore } from './store/useGameStore';
import './App.css';

function App() {
  const { fetchGame, createGame, game, loading, error } = useGameStore();

  useEffect(() => {
    const initGame = async () => {
      const newGameId = await createGame();
      if (newGameId) {
        await fetchGame(newGameId);
      }
    };
    initGame();
  }, []);

  return (
    <div style={{ width: '100vw', height: '100vh', background: '#111' }}>
      <div className="ui-overlay" style={{ position: 'absolute', color: 'white', zIndex: 10, padding: 20 }}>
        <h1>Medieval Chess</h1>
        {loading && <p>Loading Realm...</p>}
        {error && <p style={{ color: 'red' }}>Error: {error}</p>}
        {game && (
          <div>
            <p>Turn: {game.turnNumber} ({game.currentTurn === 0 ? "White" : "Black"})</p>
            <p>Status: {game.status}</p>
          </div>
        )}
      </div>

      <Canvas shadows camera={{ position: [0, 10, 5], fov: 50 }}>
        <OrbitControls minPolarAngle={0} maxPolarAngle={Math.PI / 2.2} />
        <ambientLight intensity={0.5} />
        <spotLight position={[10, 15, 10]} angle={0.3} penumbra={1} castShadow intensity={100} />
        <pointLight position={[-10, -10, -10]} intensity={0.5} />

        <Stars radius={100} depth={50} count={5000} factor={4} saturation={0} fade speed={1} />
        <Environment preset="night" />

        <Board3D />
      </Canvas>
    </div>
  );
}

export default App;
