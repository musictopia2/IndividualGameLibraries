namespace ThreeLetterFunCP.EventModels
{
    public class CardsChosenEventModel
    {
        public CardsChosenEventModel(int howManyCards)
        {
            HowManyCards = howManyCards;
        }

        public int HowManyCards { get; }
    }
}