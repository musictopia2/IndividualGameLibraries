namespace BackgammonCP.Data
{
    public class MoveInfo
    {
        public int SpaceFrom { get; set; } // this is the real move i think.
        public int SpaceTo { get; set; }
        public int DiceNumber { get; set; }
        public EnumStatusType Results { get; set; }
    }
}