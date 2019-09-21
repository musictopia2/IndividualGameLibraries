using BasicGameFramework.BasicDrawables.Interfaces;
using BasicGameFramework.RegularDeckOfCards;
using SkiaSharp;
namespace RoundsCardGameCP
{
    public class RoundsCardGameCardInformation : RegularTrickCard, IDeckObject
    {
        public RoundsCardGameCardInformation()
        {
            DefaultSize = new SKSize(55, 72); //this is neeeded too.
        }
    }
}