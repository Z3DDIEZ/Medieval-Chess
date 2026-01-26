# Medieval Chess - API Contract

## 1. REST API
Base URL: `/api/v1`

### Games Resource

#### `POST /games`
Create a new game match.
- **Request:**
  ```json
  {
    "opponentId": "guid-optional",
    "isRanked": true,
    "timeControl": "ASYNC_72H" 
  }
  ```
- **Response (201 Created):**
  ```json
  { 
    "id": "guid", 
    "status": "WAITING_FOR_OPPONENT" 
  }
  ```

#### `GET /games/{id}`
Retrieve full game state.
- **Response (200 OK):**
  ```json
  {
    "id": "guid",
    "board": { ... },
    "whitePlayer": { "id": "...", "username": "..." },
    "blackPlayer": { "id": "...", "username": "..." },
    "turnNumber": 12,
    "loyaltyState": { ... }
  }
  ```

#### `POST /games/{id}/moves`
Submit a move.
- **Request:**
  ```json
  {
    "from": "e2",
    "to": "e4",
    "type": "MOVE", // or "ABILITY"
    "abilityId": null,
    "signature": "base64-rsa-signature",
    "timestamp": "iso-date"
  }
  ```
- **Response (200 OK):**
  ```json
  {
    "outcome": "SUCCESS",
    "capturedPiece": null,
    "loyaltyChanges": [ ... ]
  }
  ```
- **Error (400 Bad Request):**
  ```json
  {
    "code": "ILLEGAL_MOVE",
    "message": "Knight cannot move to occupied square."
  }
  ```

#### `GET /games/{id}/history`
Get move history audit log.

### Users Resource

#### `GET /users/{id}/stats`
Get ELO, win rate, and recent match history.

---

## 2. SignalR Hub
Endpoint: `/hubs/game`

### Client -> Server Methods
- `JoinGame(gameId)`: Subscribe to updates for a specific game.
- `LeaveGame(gameId)`: Unsubscribe.

### Server -> Client Events
- `GameStateUpdated(gameState)`: Full or partial state push after a move.
- `NotificationReceived(message)`: "Your Turn", "Checkmate", "Loyalty Change".
- `OpponentConnectionStatus(status)`: "Online", "Offline".
