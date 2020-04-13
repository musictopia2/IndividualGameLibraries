using BasicGameFrameworkLibrary.RegularDeckOfCards;

namespace CribbageCP.Data
{
    public class CribbageCard : RegularRummyCard
    {
        public bool HasUsed { get; set; } //most games don't require this.
    }
}