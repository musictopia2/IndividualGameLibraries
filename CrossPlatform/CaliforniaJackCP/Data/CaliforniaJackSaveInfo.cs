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
using CommonBasicStandardLibraries.MVVMFramework.ViewModels;
using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.BasicGameDataClasses;
using BasicGameFrameworkLibrary.MultiplayerClasses.SavedGameClasses;
using BasicGameFrameworkLibrary.RegularDeckOfCards;
using CaliforniaJackCP.Cards;
using BasicGameFrameworkLibrary.BasicDrawables.Dictionary;
//i think this is the most common things i like to do
namespace CaliforniaJackCP.Data
{
    [SingletonGame]
    public class CaliforniaJackSaveInfo : BasicSavedTrickGamesClass<EnumSuitList, CaliforniaJackCardInformation, CaliforniaJackPlayerItem>
    { //anything needed for autoresume is here.
        public DeckRegularDict<CaliforniaJackCardInformation> CardList = new DeckRegularDict<CaliforniaJackCardInformation>();
    }
}