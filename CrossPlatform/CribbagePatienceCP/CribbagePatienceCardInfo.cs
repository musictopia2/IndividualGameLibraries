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
//i think this is the most common things i like to do
namespace CribbagePatienceCP
{
    public class CribbageCard : RegularRummyCard, IComparable<CribbageCard>
    {
        public bool HasUsed { get; set; } //most games don't require this.

        int IComparable<CribbageCard>.CompareTo(CribbageCard other)
        {
            if (Value != other.Value)
                return Value.CompareTo(other.Value);
            return Suit.CompareTo(other.Suit);
        }
    }
}
