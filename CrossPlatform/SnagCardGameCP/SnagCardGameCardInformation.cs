using BasicGameFramework.BasicDrawables.Interfaces;
using BasicGameFramework.RegularDeckOfCards;
using SkiaSharp;
namespace SnagCardGameCP
{
    public class SnagCardGameCardInformation : RegularTrickCard, IDeckObject
    {
        public SnagCardGameCardInformation()
        {
            DefaultSize = new SKSize(55, 72); //this is neeeded too.
        }
    }
}
