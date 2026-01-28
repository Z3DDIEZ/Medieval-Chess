# Medieval Chess - Frontend Client

A React Three Fiber application visualizing a feudal chess variant.

## 🌟 Key Features
- **Hybrid Visualization**: Toggle between an immersive **3D View** (GLTF models) and a tactical **2D Map** (SVG).
- **Interactive Board**: Drag-and-drop movement, click-to-select, and legal move highlighting.
- **Narrative Battle Log**: A customized "Battle Dialog" that generates flavor text for combat interactions.
- **AI Integration**: Visual feedback for AI "Thinking" states and manual AI trigger controls.
- **Medieval Aesthetics**: Custom UI themes (`BoardTheme.css`), parchment textures, and immersive camera controls.

## 🛠️ Tech Stack
- **Framework**: React 18 + Vite (TypeScript)
- **3D Engine**: React Three Fiber (Three.js)
- **State Management**: Zustand
- **Real-time**: SignalR (via `@microsoft/signalr`)
- **Styling**: Vanilla CSS + CSS Modules

## 🚀 Development
### Prerequisites
- Node.js 20+

### Setup
1. `npm install`
2. `npm run dev`

Game runs on `http://localhost:5173`.
Must be run alongside the Backend API (`dotnet run`).

## 📁 Project Structure
- `src/components/Game/Board3D.tsx`: Main 3D scene (Lights, Camera, Board).
- `src/components/Game/Board2D.tsx`: SVG-based tactical view.
- `src/components/Game/BattleDialog.tsx`: Scrolling narrative log.
- `src/components/Game/RightPanel.tsx`: Sidebar with Move History and AI Controls.
- `src/store/useGameStore.ts`: Central state (Game object, SignalR, API calls).
