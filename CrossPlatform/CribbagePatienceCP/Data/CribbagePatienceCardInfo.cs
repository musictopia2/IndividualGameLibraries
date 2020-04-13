using BasicGameFrameworkLibrary.RegularDeckOfCards;
using System;

namespace CribbagePatienceCP.Data
{
    public class CribbageCard : RegularRummyCard, IComparable<CribbageCard>
    {
        public bool HasUsed { get; set; } //most games don't require this.

        int IComparable<CribbageCard>.CompareTo(CribbageCard other)
        {
            if (Value != other.Value)
                return Value.CompareTo(other.Value);
            return Suit.CompareTo(other.Suit);
        }
    }
}