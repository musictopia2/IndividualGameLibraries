using BasicGameFramework.BasicDrawables.Interfaces;
using BasicGameFramework.RegularDeckOfCards;
using SkiaSharp;
namespace GermanWhistCP
{
    public class GermanWhistCardInformation : RegularTrickCard, IDeckObject
    {
        public GermanWhistCardInformation()
        {
            DefaultSize = new SKSize(55, 72); //this is neeeded too.
        }
    }
}
