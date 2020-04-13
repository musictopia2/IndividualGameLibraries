using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.BasicDrawables.Interfaces;
using BasicGameFrameworkLibrary.RegularDeckOfCards;
using System.Collections.Generic;

namespace BladesOfSteelCP.Logic
{
    [SingletonGame] //hopefully okay (?)
    public class CustomSort : ISortObjects<RegularSimpleCard>
    {
        int IComparer<RegularSimpleCard>.Compare(RegularSimpleCard x, RegularSimpleCard y)
        {
            if (x.Color != y.Color)
                return x.Color.CompareTo(y.Color);
            if (x.Suit != y.Suit)
                return x.Suit.CompareTo(y.Suit);
            return x.Value.CompareTo(y.Value);
        }
    }
}