namespace TeeItUpCP
{
    public enum EnumStatusType
    {
        Beginning = 1,
        FirstTurn,
        Normal,
        Finished
    }
    public class SendPlay
    {
        public int Deck { get; set; }
        public int Player { get; set; }
    }
}