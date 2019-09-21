using BasicGameFramework.RegularDeckOfCards;
namespace CribbageCP
{
    public class CribbageCard : RegularRummyCard
    {
        public bool HasUsed { get; set; } //most games don't require this.
    }
}