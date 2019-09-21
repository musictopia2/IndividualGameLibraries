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
using BasicGameFramework.RegularDeckOfCards;
namespace PokerCP
{
    public class PokerCardInfo : RegularSimpleCard
    {
        //if using rummy card, replace with rummy card.
        public EnumCardValueList SecondNumber //since i use low ace, here, use there too.
        {
            get
            {
                if (Value != EnumCardValueList.HighAce)
                    return Value;
                return EnumCardValueList.LowAce; //second seemed to lean towards low.
            }
        }
    }
}