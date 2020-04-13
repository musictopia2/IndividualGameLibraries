namespace ClockSolitaireCP.EventModels
{
    public class CardsLeftEventModel
    {
        public int CardsLeft { get; set; }
        public CardsLeftEventModel(int cardsLeft)
        {
            CardsLeft = cardsLeft;
        }
    }
}
