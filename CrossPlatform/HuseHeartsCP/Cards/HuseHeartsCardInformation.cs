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
//i think this is the most common things i like to do
namespace HuseHeartsCP.Cards
{
    public class HuseHeartsCardInformation : RegularTrickCard, IDeckObject
    {
        public HuseHeartsCardInformation()
        {
            DefaultSize = new SKSize(55, 72); //this is neeeded too.
        }
        public int HeartPoints
        {
            get
            {
                if (Suit == EnumSuitList.Hearts)
                    return 1;
                if (Suit == EnumSuitList.Diamonds && Value == EnumCardValueList.Jack)
                    return -10;
                if (Suit == EnumSuitList.Spades && Value == EnumCardValueList.Queen)
                    return 13;
                return 0;
            }
        }
        public bool ContainPoints
        {
            get
            {
                if (Suit == EnumSuitList.Hearts)
                    return true;
                if (Suit == EnumSuitList.Spades && Value == EnumCardValueList.Queen)
                    return true;
                return false;
            }
        }
    }
}
