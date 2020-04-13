using System;
using System.Text;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using System.Linq;
using CommonBasicStandardLibraries.BasicDataSettingsAndProcesses;
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;
using CommonBasicStandardLibraries.CollectionClasses;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using fs = CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.JsonSerializers.FileHelpers;
using js = CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.JsonSerializers.NewtonJsonStrings; //just in case i need those 2.
using BasicGameFrameworkLibrary.BasicDrawables.BasicClasses;
using BasicGameFrameworkLibrary.BasicDrawables.Interfaces;
using SkiaSharp;
using BasicGameFrameworkLibrary.RegularDeckOfCards;
using Newtonsoft.Json;
//i think this is the most common things i like to do
namespace SixtySix2PlayerCP.Cards
{
    public class SixtySix2PlayerCardInformation : RegularTrickCard, IDeckObject
    {
        public SixtySix2PlayerCardInformation()
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
