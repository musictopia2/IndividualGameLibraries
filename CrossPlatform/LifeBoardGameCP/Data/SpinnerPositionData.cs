namespace LifeBoardGameCP.Data
{
    public class SpinnerPositionData
    {
        public int ChangePositions { get; set; }
        public bool CanBetween { get; set; }
        public int HighSpeedUpTo { get; set; } // this is how long it will do highspeed (so it can vary)
    }
}