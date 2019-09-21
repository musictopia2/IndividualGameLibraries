using BasicGameFramework.BasicDrawables.Interfaces;
using BasicGameFramework.RegularDeckOfCards;
using SkiaSharp;
namespace HuseHeartsCP
{
    public class HuseHeartsCardInformation : RegularTrickCard, IDeckObject
    {
        public HuseHeartsCardInformation()
        {
            DefaultSize = new SKSize(55, 72); //this is neeeded too.
        }
        public int HeartPoints
        {
            get
            {
                if (Suit == EnumSuitList.Hearts)
                    return 1;
                if (Suit == EnumSuitList.Diamonds && Value == EnumCardValueList.Jack)
                    return -10;
                if (Suit == EnumSuitList.Spades && Value == EnumCardValueList.Queen)
                    return 13;
                return 0;
            }
        }
        public bool ContainPoints
        {
            get
            {
                if (Suit == EnumSuitList.Hearts)
                    return true;
                if (Suit == EnumSuitList.Spades && Value == EnumCardValueList.Queen)
                    return true;
                return false;
            }
        }
    }
}