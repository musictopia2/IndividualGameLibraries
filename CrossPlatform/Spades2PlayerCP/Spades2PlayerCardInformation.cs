using BasicGameFramework.BasicDrawables.Interfaces;
using BasicGameFramework.RegularDeckOfCards;
using SkiaSharp;
namespace Spades2PlayerCP
{
    public class Spades2PlayerCardInformation : RegularTrickCard, IDeckObject
    {
        public Spades2PlayerCardInformation()
        {
            DefaultSize = new SKSize(55, 72); //this is neeeded too.
        }
    }
}