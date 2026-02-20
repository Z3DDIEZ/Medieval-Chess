import { create } from "zustand";
import axios from "axios";
import { HubConnectionBuilder } from "@microsoft/signalr";

interface PieceAbility {
  abilityDefinitionId: string;
  abilityType: string;
  currentCooldown: number;
  maxCooldown: number;
  upgradeTier: number;
  isReady: boolean;
  name: string;
  description: string;
  apCost: number;
  requiresTarget: boolean;
  range: number;
}

interface CatalogAbility {
  abilityType: string;
  name: string;
  description: string;
  apCost: number;
  xpRequired: number;
  tier: string;
  requiresTarget: boolean;
  range: number;
  isUnlocked: boolean;
}

interface Piece {
  type: number; // Enum value
  color: number; // 0=White, 1=Black
  position: string; // "e4"
  loyalty: number;
  maxHP: number;
  currentHP: number;
  armor: number; // Added
  // Medieval fields
  level: number;
  xp: number;
  promotionTier: string;
  isDefecting: boolean;
  court: string | null;
  abilities: PieceAbility[];
  abilityCatalog?: CatalogAbility[];
}

interface NarrativeEntry {
  id: string;
  turnNumber: number;
  speaker: string;
  text: string;
  intensity: number;
  tags: string[];
}

interface GameState {
  id: string;
  status: number;
  currentTurn: number;
  turnNumber: number;
  isAttritionMode: boolean; // Added
  whiteAP: number; // Added
  blackAP: number; // Added
  pieces: Piece[];
  moveHistory?: string[]; // Algebraic notation moves e.g. ["e4", "e5"]
  narrative: NarrativeEntry[]; // Added
  isCheck?: boolean;
  kingInCheckPosition?: string;
  lastMoveFrom?: string;
  lastMoveTo?: string;
  lastMoveIsBounce?: boolean;
  lastMoveDamage?: number;
}

interface GameStore {
  game: GameState | null;
  loading: boolean;
  error: string | null;
  legalMoves: string[];
  fetchGame: (id: string) => Promise<void>;
  createGame: () => Promise<string | null>;
  executeMove: (
    id: string,
    from: string,
    to: string,
    promotionPiece?: number,
  ) => Promise<void>;
  connectHub: (gameId: string) => Promise<void>;
  resignGame: (id: string, color: number) => Promise<void>;
  offerDraw: (id: string, color: number) => Promise<void>;
  acceptDraw: (id: string, color: number) => Promise<void>;
  getLegalMoves: (id: string, from: string) => Promise<void>;
  clearLegalMoves: () => void;
  // Abilities & Progression
  selectedAbility: string | null;
  setSelectedAbility: (abilityId: string | null) => void;
  executeAbility: (
    id: string,
    from: string,
    abilityId: string,
    target?: string,
  ) => Promise<void>;
  upgradePiece: (
    id: string,
    from: string,
    abilityType: string,
  ) => Promise<void>;
  // AI
  isAIThinking: boolean;
  lastAIMoveTime?: number;
  makeAIMove: (id: string) => Promise<void>;
  // Connection
  connection: any | null;
}

export const useGameStore = create<GameStore>((set, get) => ({
  game: null,
  loading: false,
  error: null,
  legalMoves: [],
  selectedAbility: null,
  isAIThinking: false,
  fetchGame: async (id: string) => {
    set({ loading: true, error: null });
    try {
      const response = await axios.get(`/api/Games/${id}`);
      set({ game: response.data, loading: false });
    } catch (err: any) {
      set({ error: err.message, loading: false });
    }
  },
  createGame: async () => {
    set({ loading: true, error: null });
    try {
      // Match the API contract: POST /api/Games
      const response = await axios.post("/api/Games", {
        // Sending basics, although API might ignore body for now if it just calls Game.StartNew()
        opponentId: null,
        isRanked: false,
      });
      // API returns the GUID string directly or an object with id?
      // Controller: return CreatedAtAction(nameof(Get), new { id = gameId }, gameId);
      // It returns gameId (Guid) as the body.
      return response.data;
    } catch (err: any) {
      set({ error: err.message, loading: false });
      return null;
    }
  },
  executeMove: async (
    id: string,
    from: string,
    to: string,
    promotionPiece?: number,
  ) => {
    try {
      await axios.post(`/api/Games/${id}/moves`, { from, to, promotionPiece });
      // Refresh state after move - SignalR will handle it for opponent, but we can do optimistic/immediate too
      const response = await axios.get(`/api/Games/${id}`);
      set({ game: response.data, legalMoves: [] });
    } catch (err: any) {
      console.error(err);
      set({ error: err.response?.data || "Move failed" });
    }
  },
  connection: null as HubConnectionBuilder | any | null, // Store connection instance

  // ... existing actions ...

  connectHub: async (gameId: string) => {
    const { connection: existingConnection } = get();

    // Clean up existing connection if it exists
    if (existingConnection) {
      try {
        await existingConnection.stop();
        console.log("Stopped existing SignalR connection");
      } catch (err) {
        console.warn("Failed to stop existing connection:", err);
      }
    }

    const connection = new HubConnectionBuilder()
      .withUrl("/gameHub")
      .withAutomaticReconnect()
      .build();

    connection.on("GameStateUpdated", async (id: string) => {
      const currentGame = get().game;
      // Only update if the event matches the current game
      if (currentGame && id === currentGame.id) {
        // Refresh game state
        const response = await axios.get(`/api/Games/${id}`);
        set({ game: response.data });
      }
    });

    try {
      await connection.start();
      await connection.invoke("JoinGame", gameId);
      set({ connection }); // Save connection to store
      console.log(`Connected to GameHub for game ${gameId}`);
    } catch (err) {
      console.error("SignalR Connection Error: ", err);
    }
  },
  resignGame: async (id: string, color: number) => {
    try {
      await axios.post(`/api/Games/${id}/resign`, { color });
    } catch (err: any) {
      console.error(err);
    }
  },
  offerDraw: async (id: string, color: number) => {
    try {
      await axios.post(`/api/Games/${id}/draw/offer`, { color });
    } catch (err: any) {
      console.error(err);
    }
  },
  acceptDraw: async (id: string, color: number) => {
    try {
      await axios.post(`/api/Games/${id}/draw/accept`, { color });
    } catch (err: any) {
      console.error(err);
    }
  },
  getLegalMoves: async (id: string, from: string) => {
    try {
      const response = await axios.get(`/api/Games/${id}/legal-moves/${from}`);
      set({ legalMoves: response.data || [] });
    } catch (err: any) {
      console.error(err);
      set({ legalMoves: [] });
    }
  },
  clearLegalMoves: () => {
    set({ legalMoves: [] });
  },
  setSelectedAbility: (abilityId: string | null) => {
    set({ selectedAbility: abilityId });
  },
  executeAbility: async (
    id: string,
    from: string,
    abilityId: string,
    target?: string,
  ) => {
    try {
      await axios.post(`/api/Games/${id}/abilities`, {
        from,
        abilityId,
        target,
      });
      // Let SignalR handle the state refresh, but we could do it manually here too
      set({ selectedAbility: null }); // Clear selection after use
    } catch (err: any) {
      console.error(err);
      set({ error: err.response?.data || "Ability failed" });
      set({ selectedAbility: null });
    }
  },
  upgradePiece: async (id: string, from: string, abilityType: string) => {
    try {
      await axios.post(`/api/Games/${id}/upgrade`, { from, abilityType });
    } catch (err: any) {
      console.error(err);
      set({ error: err.response?.data || "Upgrade failed" });
    }
  },
  makeAIMove: async (id: string) => {
    const { isAIThinking, lastAIMoveTime } = get();
    const now = Date.now();

    // Guard: Busy or Cooldown (2 seconds)
    if (isAIThinking || (lastAIMoveTime && now - lastAIMoveTime < 2000)) {
      return;
    }

    set({ isAIThinking: true });
    try {
      await axios.post(`/api/Games/${id}/ai-move`, {});
      // Check status - usually SignalR handles refresh, but force it
      const response = await axios.get(`/api/Games/${id}`);
      set({
        game: response.data,
        isAIThinking: false,
        lastAIMoveTime: Date.now(),
      });
    } catch (err: any) {
      console.error(err);
      set({
        error: "AI failed to move",
        isAIThinking: false,
        lastAIMoveTime: Date.now(),
      });
    }
  },
}));
