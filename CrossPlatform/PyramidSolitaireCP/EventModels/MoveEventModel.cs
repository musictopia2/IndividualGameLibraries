namespace PyramidSolitaireCP.EventModels
{
    public class MoveEventModel
    {
        public int Deck { get; set; }
        public MoveEventModel(int deck)
        {
            Deck = deck;
        }
    }
}