using BasicGameFramework.Attributes;
using BasicGameFramework.BasicDrawables.Interfaces;
using BasicGameFramework.RegularDeckOfCards;
using System.Collections.Generic;
namespace BladesOfSteelCP
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