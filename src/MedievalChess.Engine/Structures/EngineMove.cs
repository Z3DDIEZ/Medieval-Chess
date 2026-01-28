namespace MedievalChess.Engine.Structures
{
    public struct EngineMove
    {
        public byte FromIndex; // Index in PieceState[]
        public byte ToSquare;  // 0-63
        public byte TargetIndex; // Index of victim in PieceState[] or 255
        
        public bool IsCapture => TargetIndex != 255;
        
        public EngineMove(byte from, byte to, byte target = 255)
        {
            FromIndex = from;
            ToSquare = to;
            TargetIndex = target;
        }
    }
}
