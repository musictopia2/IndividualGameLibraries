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
using BasicGameFrameworkLibrary.SolitaireClasses.MainClasses;
using BasicGameFrameworkLibrary.MultiplePilesObservable;
using BasicGameFrameworkLibrary.SolitaireClasses.Cards;
//i think this is the most common things i like to do
namespace FreeCellSolitaireCP.Data
{
    [SingletonGame]
    public class FreeCellSolitaireSaveInfo : SolitaireSavedClass, IMappable
    {
        public CustomBasicList<BasicPileInfo<SolitaireCard>> FreeCards = new CustomBasicList<BasicPileInfo<SolitaireCard>>();
    }
}