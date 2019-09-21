using BasicGameFramework.BasicDrawables.Interfaces;
using BasicGameFramework.RegularDeckOfCards;
using SkiaSharp;
namespace HorseshoeCardGameCP
{
    public class HorseshoeCardGameCardInformation : RegularTrickCard, IDeckObject
    {
        public HorseshoeCardGameCardInformation()
        {
            DefaultSize = new SKSize(55, 72); //this is neeeded too.
        }
    }
}
