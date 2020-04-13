using BasicGameFrameworkLibrary.BasicDrawables.Interfaces;
using BasicGameFrameworkLibrary.RegularDeckOfCards;
using CommonBasicStandardLibraries.Exceptions;
using Newtonsoft.Json;
using SkiaSharp;
namespace Pinochle2PlayerCP.Cards
{
    public class Pinochle2PlayerCardInformation : RegularMultiTRCard, IDeckObject
    {
        public Pinochle2PlayerCardInformation()
        {
            DefaultSize = new SKSize(55, 72); //this is neeeded too.
        }
        [JsonIgnore]
        public int PinochleCardValue
        {
            get
            {
                return Value switch
                {
                    EnumCardValueList.Nine => 0,
                    EnumCardValueList.Ten => 10,
                    EnumCardValueList.Jack => 2,
                    EnumCardValueList.Queen => 3,
                    EnumCardValueList.King => 4,
                    EnumCardValueList.HighAce => 11,
                    _ => throw new BasicBlankException("The first number must be greater than 8"),
                };
            }
        }
    }
}
