using BasicGameFramework.BasicDrawables.Interfaces;
using BasicGameFramework.RegularDeckOfCards;
using SkiaSharp;
namespace SkuckCardGameCP
{
    public class SkuckCardGameCardInformation : RegularTrickCard, IDeckObject
    {
        public SkuckCardGameCardInformation()
        {
            DefaultSize = new SKSize(55, 72); //this is neeeded too.
        }
    }
}