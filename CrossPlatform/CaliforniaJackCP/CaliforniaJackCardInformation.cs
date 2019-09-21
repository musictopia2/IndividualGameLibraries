using BasicGameFramework.BasicDrawables.Interfaces;
using BasicGameFramework.RegularDeckOfCards;
using SkiaSharp;
namespace CaliforniaJackCP
{
    public class CaliforniaJackCardInformation : RegularTrickCard, IDeckObject
    {
        public CaliforniaJackCardInformation()
        {
            DefaultSize = new SKSize(55, 72); //this is neeeded too.
        }
    }
}