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

    // Show only last 5 entries to avoid cluttering the view, 
    // unless user scrolls (which we can't easily detect with simple current implementation logic)

    return (
        <div className="battle-log-container" style={{
            position: 'absolute',
            bottom: '20px',
            left: '50%',
            transform: 'translateX(-50%)',
            width: '600px',
            maxHeight: '180px',

            // Medieval Styling
            background: 'linear-gradient(to bottom, #2a221b, #15100c)',
            border: '2px solid #8b7355',
            borderRadius: '4px',
            boxShadow: '0 0 15px rgba(0,0,0,0.8), inset 0 0 20px rgba(0,0,0,0.5)',

            padding: '10px 15px',
            overflowY: 'auto',
            pointerEvents: 'auto', // Allow scrolling
            fontFamily: '"Cinzel", serif',
            zIndex: 100
        }}>
            {/* Scroll/Parchment Header Decoration (Optional CSS later) */}

            <div style={{ display: 'flex', flexDirection: 'column', gap: '8px' }}>
                {entries.slice(-20).map((entry) => (
                    <div key={entry.id} style={{
                        display: 'flex',
                        alignItems: 'baseline',
                        fontSize: '14px',
                        lineHeight: '1.4',
                        color: getIntensityColor(entry.intensity),
                        borderBottom: '1px solid rgba(139, 115, 85, 0.2)',
                        paddingBottom: '4px',
                        animation: 'fadeIn 0.3s ease-out'
                    }}>
                        <span style={{
                            fontSize: '11px',
                            color: '#8b7355',
                            marginRight: '8px',
                            minWidth: '25px'
                        }}>
                            Turn {entry.turnNumber}
                        </span>

                        <div style={{ flex: 1 }}>
                            {getIconForSpeaker(entry.speaker)}
                            <span style={{ marginLeft: entry.speaker !== 'System' ? '6px' : '0' }}>
                                {entry.text}
                            </span>
                        </div>
                    </div>
                ))}
                <div ref={bottomRef} />
            </div>

            <style>{`
                @import url('https://fonts.googleapis.com/css2?family=Cinzel:wght@400;700&display=swap');
                
                .battle-log-container::-webkit-scrollbar {
                    width: 8px;
                }
                .battle-log-container::-webkit-scrollbar-track {
                    background: #15100c;
                }
                .battle-log-container::-webkit-scrollbar-thumb {
                    background: #8b7355;
                    border-radius: 4px;
                }
                @keyframes fadeIn {
                    from { opacity: 0; transform: translateX(-5px); }
                    to { opacity: 1; transform: translateX(0); }
                }
            `}</style>
        </div>
    );
};

function getIntensityColor(intensity: number): string {
    if (intensity >= 8) return '#ff5555'; // Critical (Red)
    if (intensity >= 5) return '#e0d0b0'; // Normal (Parchment White)
    return '#8a8a8a'; // Glancing (Gray)
}

function getIconForSpeaker(speaker: string) {
    if (speaker === 'System' || speaker === 'Battle') return null;
    return <span style={{ color: '#ffd700', fontWeight: 'bold' }}>{speaker}:</span>;
}
