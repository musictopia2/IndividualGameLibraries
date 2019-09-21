using BasicGameFramework.BasicDrawables.Interfaces;
using BasicGameFramework.RegularDeckOfCards;
using CommonBasicStandardLibraries.Exceptions;
using Newtonsoft.Json;
using SkiaSharp;
namespace Pinochle2PlayerCP
{
    public class Pinochle2PlayerCardInformation : RegularMultiTRCard, IDeckObject //because of tempsets.
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
                switch (Value)
                {
                    case EnumCardValueList.Nine:
                        return 0;
                    case EnumCardValueList.Ten:
                        return 10;
                    case EnumCardValueList.Jack:
                        return 2;
                    case EnumCardValueList.Queen:
                        return 3;
                    case EnumCardValueList.King:
                        return 4;
                    case EnumCardValueList.HighAce:
                        return 11;
                    default:
                        throw new BasicBlankException("The first number must be greater than 8");
                }
            }
        }
    }
}