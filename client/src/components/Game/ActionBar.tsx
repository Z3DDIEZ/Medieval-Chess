import React from "react";
import { useGameStore } from "../../store/useGameStore";
import "./BoardTheme.css";

interface ActionBarProps {
  selectedPos: string | null;
}

export const ActionBar: React.FC<ActionBarProps> = ({ selectedPos }) => {
  const { game, selectedAbility, setSelectedAbility } = useGameStore();

  if (!game || !selectedPos) return null;

  const piece = game.pieces.find((p) => p.position === selectedPos);

  // Only show action bar for the current player's pieces
  if (!piece || piece.color !== game.currentTurn) return null;

  const isWhite = game.currentTurn === 0;
  const currentAP = isWhite ? game.whiteAP : game.blackAP;

  const handleAbilityClick = (abilityId: string, e: React.MouseEvent) => {
    e.stopPropagation();
    if (selectedAbility === abilityId) {
      setSelectedAbility(null); // Toggle off
    } else {
      setSelectedAbility(abilityId); // Toggle on
    }
  };

  return (
    <div
      style={{
        position: "absolute",
        bottom: "20px",
        left: "50%",
        transform: "translateX(-50%)",
        background: "rgba(20, 20, 25, 0.95)",
        border: "2px solid #c9a227",
        borderRadius: "12px",
        padding: "12px 24px",
        display: "flex",
        flexDirection: "column",
        alignItems: "center",
        gap: "12px",
        boxShadow: "0 8px 32px rgba(0,0,0,0.8)",
        zIndex: 1000,
        pointerEvents: "auto",
      }}
    >
      <div
        style={{
          color: "#ffd700",
          fontWeight: "bold",
          fontSize: "18px",
          textShadow: "0 2px 4px rgba(0,0,0,0.5)",
        }}
      >
        Action Points (AP): {currentAP}/10
      </div>

      <div style={{ display: "flex", gap: "16px" }}>
        {/* Default Action: Move */}
        <button
          onClick={(e) => {
            e.stopPropagation();
            setSelectedAbility(null);
          }}
          style={{
            padding: "10px 20px",
            background: selectedAbility === null ? "#c9a227" : "#333",
            color: selectedAbility === null ? "#000" : "#ccc",
            border: "2px solid",
            borderColor: selectedAbility === null ? "#fff" : "#555",
            borderRadius: "6px",
            fontWeight: "bold",
            cursor: "pointer",
            transition: "all 0.2s ease",
            boxShadow:
              selectedAbility === null
                ? "0 0 10px rgba(201, 162, 39, 0.5)"
                : "none",
          }}
        >
          Move/Attack
          {game.isAttritionMode && (
            <div style={{ fontSize: "11px", marginTop: "2px", opacity: 0.8 }}>
              Cost: 1 AP
            </div>
          )}
        </button>

        {/* Abilities */}
        {piece.abilities?.map((ability, idx) => {
          const isActive = selectedAbility === ability.abilityDefinitionId;
          const canAfford = true; // In the future, check cost. Assuming 2 AP for generic abilities.

          return (
            <button
              key={idx}
              onClick={(e) =>
                handleAbilityClick(ability.abilityDefinitionId, e)
              }
              disabled={!ability.isReady || !canAfford}
              style={{
                padding: "10px 20px",
                background: isActive ? "#4caf50" : "#333",
                color: isActive ? "#fff" : ability.isReady ? "#fff" : "#666",
                border: "2px solid",
                borderColor: isActive
                  ? "#fff"
                  : ability.isReady
                    ? "#4caf50"
                    : "#444",
                borderRadius: "6px",
                fontWeight: "bold",
                cursor: ability.isReady ? "pointer" : "not-allowed",
                transition: "all 0.2s ease",
                boxShadow: isActive
                  ? "0 0 10px rgba(76, 175, 80, 0.5)"
                  : "none",
                opacity: ability.isReady ? 1 : 0.6,
              }}
            >
              <div
                style={{
                  display: "flex",
                  flexDirection: "column",
                  alignItems: "center",
                }}
              >
                <span>{ability.abilityDefinitionId}</span>
                <div
                  style={{ fontSize: "11px", marginTop: "2px", opacity: 0.8 }}
                >
                  {!ability.isReady
                    ? `Cooldown: ${ability.currentCooldown}/${ability.maxCooldown}`
                    : "Cost: 2 AP"}
                </div>
              </div>
            </button>
          );
        })}
      </div>

      {selectedAbility && (
        <div
          style={{
            color: "#aaa",
            fontSize: "13px",
            fontStyle: "italic",
            marginTop: "4px",
          }}
        >
          Select a target square to cast {selectedAbility}.
        </div>
      )}
    </div>
  );
};
