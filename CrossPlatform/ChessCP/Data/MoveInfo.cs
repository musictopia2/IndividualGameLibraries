namespace ChessCP.Data
{
    public class MoveInfo
    {
        public int SpaceFrom { get; set; }
        public int SpaceTo { get; set; }
        public EnumPieceType Piece { get; set; } // this is the piece used for the move.
        public EnumStatusType Results { get; set; }
    }
}