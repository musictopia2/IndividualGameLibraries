using BasicGameFramework.BasicDrawables.Interfaces;
using BasicGameFramework.RegularDeckOfCards;
namespace GalaxyCardGameCP
{
    public class GalaxyCardGameCardInformation : RegularMultiTRCard, IDeckObject
    {
        protected override void FinishPopulatingCard()
        {
            if (Value == EnumCardValueList.HighAce)
            {
                Points = 10;
                return;
            }

            if (Value >= EnumCardValueList.Ten)
            {
                Points = 10;
                return;
            }
            Points = (int)Value;
        }
    }
}