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
namespace SpiderSolitaireCP.Data
{
    public class CustomDeck : IRegularDeckInfo
    {
        private readonly LevelClass _level;

        public CustomDeck(LevelClass level)
        {
            _level = level;
        }

        int IRegularDeckInfo.HowManyDecks
        {
            get
            {
                if (_level.LevelChosen == 3)
                    return 2;
                if (_level.LevelChosen == 2)
                    return 4;
                if (_level.LevelChosen == 1)
                    return 8;
                throw new BasicBlankException("Needs levels 1, 2, or 3 for figuring out how many decks");
            }
        }

        bool IRegularDeckInfo.UseJokers => false;

        int IRegularDeckInfo.GetExtraJokers => 0;

        int IRegularDeckInfo.LowestNumber => 1;

        int IRegularDeckInfo.HighestNumber => 13; //aces will usually be low.


        //requires lots of rethinking for spider solitaire.
        //even for deckcount.
        //has to open another class to see what to do.

        CustomBasicList<ExcludeRCard> IRegularDeckInfo.ExcludeList => new CustomBasicList<ExcludeRCard>();

        CustomBasicList<EnumSuitList> IRegularDeckInfo.SuitList
        {
            get
            {
                if (_level.LevelChosen == 1)
                    return new CustomBasicList<EnumSuitList> { EnumSuitList.Spades };
                if (_level.LevelChosen == 2)
                    return new CustomBasicList<EnumSuitList> { EnumSuitList.Spades, EnumSuitList.Hearts };
                if (_level.LevelChosen != 3)
                    throw new BasicBlankException("Only 3 levels are supposed for the suit list");
                var tempList = Helpers.GetCompleteSuitList;
                return tempList;
            }
        }

        int IDeckCount.GetDeckCount()
        {
            return 104;
        }
    }
}
