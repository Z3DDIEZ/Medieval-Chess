import React, { useEffect, useRef } from 'react';

interface NarrativeEntry {
    id: string;
    turnNumber: number;
    speaker: string;
    text: string;
    intensity: number;
    tags: string[];
}

interface BattleDialogProps {
    entries: NarrativeEntry[];
}

export const BattleDialog: React.FC<BattleDialogProps> = ({ entries }) => {
    const bottomRef = useRef<HTMLDivElement>(null);

    useEffect(() => {
        if (bottomRef.current) {
            bottomRef.current.scrollIntoView({ behavior: 'smooth' });
        }
    }, [entries]);

    if (!entries || entries.length === 0) return null;

    return (
        <div style={{
            position: 'absolute',
            bottom: '20px',
            left: '50%',
            transform: 'translateX(-50%)',
            width: '600px',
            maxHeight: '150px',
            background: 'rgba(20, 20, 30, 0.85)',
            backdropFilter: 'blur(10px)',
            border: '1px solid rgba(255, 255, 255, 0.1)',
            borderRadius: '8px',
            padding: '10px 15px',
            overflowY: 'auto',
            pointerEvents: 'none', // Let clicks pass through to board if needed, but maybe we want to scroll
            // Actually, if we want to scroll, pointerEvents auto usually.
            // But if it's over the board... let's keep it 'auto' so user can scroll history.
            display: 'flex',
            flexDirection: 'column',
            gap: '6px',
            boxShadow: '0 4px 6px rgba(0,0,0,0.5)',
            zIndex: 100
        }}>
            {entries.slice().reverse().map((entry) => (
                <div key={entry.id} style={{
                    fontSize: '14px',
                    color: getIntensityColor(entry.intensity), // Function to determine color
                    fontFamily: '"Cinzel", serif', // Assuming we have a fantasy font or fallback
                    textShadow: '0 1px 2px rgba(0,0,0,0.8)',
                    opacity: 0,
                    animation: 'fadeIn 0.5s forwards'
                }}>
                    <span style={{ fontWeight: 'bold', marginRight: '5px', color: '#aaa' }}>
                        [{entry.turnNumber}]
                    </span>
                    {entry.speaker !== 'System' && (
                        <span style={{ fontWeight: 'bold', marginRight: '5px', color: '#ffd700' }}>
                            {entry.speaker}:
                        </span>
                    )}
                    <span>{entry.text}</span>
                </div>
            ))}
            <div ref={bottomRef} />
            <style>{`
                @keyframes fadeIn {
                    from { opacity: 0; transform: translateY(5px); }
                    to { opacity: 1; transform: translateY(0); }
                }
            `}</style>
        </div>
    );
};

function getIntensityColor(intensity: number): string {
    if (intensity >= 8) return '#ff4444'; // Critical / High Impact
    if (intensity >= 5) return '#ffffff'; // Normal
    return '#888888'; // Low impact / Glancing
}
