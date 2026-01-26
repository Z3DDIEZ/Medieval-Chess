import { create } from 'zustand';
import axios from 'axios';

interface Piece {
    type: number; // Enum value
    color: number; // 0=White, 1=Black
    position: string; // "e4"
}

interface GameState {
    id: string;
    status: number;
    currentTurn: number;
    turnNumber: number;
    pieces: Piece[];
}

interface GameStore {
    game: GameState | null;
    loading: boolean;
    error: string | null;
    fetchGame: (id: string) => Promise<void>;
    createGame: () => Promise<string | null>;
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
    }
}));
