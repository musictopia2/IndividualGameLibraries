namespace SkipboCP.Data
{
    public class SendPlay : SendDiscard
    {
        public int Discard { get; set; }
        public EnumCardType WhichType { get; set; }
    }
}