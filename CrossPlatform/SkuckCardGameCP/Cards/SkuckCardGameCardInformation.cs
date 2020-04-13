using BasicGameFrameworkLibrary.BasicDrawables.Interfaces;
using BasicGameFrameworkLibrary.RegularDeckOfCards;
using SkiaSharp;
namespace SkuckCardGameCP.Cards
{
    public class SkuckCardGameCardInformation : RegularTrickCard, IDeckObject
    {
        public SkuckCardGameCardInformation()
        {
            DefaultSize = new SKSize(55, 72); //this is neeeded too.
        }
        //public void Populate(int chosen)
        //{
        //    //populating the card.

        //}

        //public void Reset()
        //{
        //    //anything that is needed to reset.
        //}
    }
}
