namespace CheckersCP.Data
{
    public class MoveInfo
    {
        public int SpaceFrom { get; set; }
        public int SpaceTo { get; set; }
        public int PlayerCaptured { get; set; }//this is the index of the player being captured.  0 means nobody.  so when checking out the moves, if any show a player can be captured, then must make that move.
    }
}