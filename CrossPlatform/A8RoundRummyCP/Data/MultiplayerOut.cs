namespace A8RoundRummyCP.Data
{
    public class MultiplayerOut
    {
        public int Deck { get; set; } // this is the leftover card
        public bool WasGuaranteed { get; set; } // this is whether the player was guaranteed to win or not
    }
}