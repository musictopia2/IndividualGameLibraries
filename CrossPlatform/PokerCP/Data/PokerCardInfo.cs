using BasicGameFrameworkLibrary.RegularDeckOfCards;
namespace PokerCP.Data
{
    public class PokerCardInfo : RegularSimpleCard
    {
        public EnumCardValueList SecondNumber //since i use low ace, here, use there too.
        {
            get
            {
                if (Value != EnumCardValueList.HighAce)
                    return Value;
                return EnumCardValueList.LowAce; //second seemed to lean towards low.
            }
        }
    }
}