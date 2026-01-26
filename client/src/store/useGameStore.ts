import { create } from 'zustand';
import axios from 'axios';
import { HubConnectionBuilder } from '@microsoft/signalr';

interface Piece {
    type: number; // Enum value
    color: number; // 0=White, 1=Black
    position: string; // "e4"
    loyalty: number;
    maxHP: number;
    currentHP: number;
}

interface GameState {
    id: string;
    status: number;
    currentTurn: number;
    turnNumber: number;
    pieces: Piece[];
    moveHistory?: string[]; // Algebraic notation moves e.g. ["e2e4", "e7e5"]
}

interface GameStore {
    game: GameState | null;
    loading: boolean;
    error: string | null;
    fetchGame: (id: string) => Promise<void>;
    createGame: () => Promise<string | null>;
    executeMove: (id: string, from: string, to: string) => Promise<void>;
    connectHub: (gameId: string) => Promise<void>;
    resignGame: (id: string, color: number) => Promise<void>;
    offerDraw: (id: string, color: number) => Promise<void>;
    acceptDraw: (id: string, color: number) => Promise<void>;
}

export const useGameStore = create<GameStore>((set) => ({
    game: null,
    loading: false,
    error: null,
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
            const response = await axios.post('/api/Games', {
                // Sending basics, although API might ignore body for now if it just calls Game.StartNew()
                opponentId: null,
                isRanked: false
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
    executeMove: async (id: string, from: string, to: string) => {
        try {
            await axios.post(`/api/Games/${id}/moves`, { from, to });
            // Refresh state after move - SignalR will handle it for opponent, but we can do optimistic/immediate too
            const response = await axios.get(`/api/Games/${id}`);
            set({ game: response.data });
        } catch (err: any) {
            console.error(err);
            set({ error: err.response?.data || "Move failed" });
        }
    },
    connectHub: async (gameId: string) => {
        const connection = new HubConnectionBuilder()
            .withUrl("/gameHub")
            .withAutomaticReconnect()
            .build();

        connection.on("GameStateUpdated", async (id: string) => {
            if (id === gameId) {
                // Refresh game state
                const response = await axios.get(`/api/Games/${id}`);
                set({ game: response.data });
            }
        });

        try {
            await connection.start();
            await connection.invoke("JoinGame", gameId);
            console.log("Connected to GameHub");
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
    }
}));
