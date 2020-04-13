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
using BasicGameFrameworkLibrary.RegularDeckOfCards;
using BasicGameFrameworkLibrary.BasicDrawables.Interfaces;
//i think this is the most common things i like to do
namespace TriangleSolitaireCP.Data
{
    public class CustomDeck : IRegularDeckInfo
    {
        int IRegularDeckInfo.HowManyDecks => 1; //most games use one deck.  if there is more than one deck, put here.

        bool IRegularDeckInfo.UseJokers => false;

        int IRegularDeckInfo.GetExtraJokers => 0;

        int IRegularDeckInfo.LowestNumber => 1;

        int IRegularDeckInfo.HighestNumber => 13; //aces will usually be low.


        //requires lots of rethinking for spider solitaire.
        //even for deckcount.
        //has to open another class to see what to do.

        CustomBasicList<ExcludeRCard> IRegularDeckInfo.ExcludeList => new CustomBasicList<ExcludeRCard>();

        CustomBasicList<EnumSuitList> IRegularDeckInfo.SuitList => Helpers.GetCompleteSuitList;

        int IDeckCount.GetDeckCount()
        {
            return 52;
        }
    }
}
