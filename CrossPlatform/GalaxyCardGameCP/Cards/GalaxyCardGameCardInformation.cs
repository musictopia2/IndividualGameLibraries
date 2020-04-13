using BasicGameFrameworkLibrary.BasicDrawables.Interfaces;
using BasicGameFrameworkLibrary.RegularDeckOfCards;
namespace GalaxyCardGameCP.Cards
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
