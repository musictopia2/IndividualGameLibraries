namespace A8RoundRummyCP.Data
{
    public class RoundClass
    {
        public string Description { get; set; } = "";
        public EnumCategory Category { get; set; }
        public int Points { get; set; } // this is also what round its upto as well
        public EnumRummyType Rummy { get; set; }
    }
}