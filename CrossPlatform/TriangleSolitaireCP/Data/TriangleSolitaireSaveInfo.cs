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
using BasicGameFrameworkLibrary.DrawableListsObservable;
using BasicGameFrameworkLibrary.SolitaireClasses.Cards;
using BasicGameFrameworkLibrary.SolitaireClasses.TriangleClasses;
//i think this is the most common things i like to do
namespace TriangleSolitaireCP.Data
{
    [SingletonGame]
    public class TriangleSolitaireSaveInfo : ObservableObject, IMappable
    {
        public CustomBasicList<int> DeckList { get; set; } = new CustomBasicList<int>();
        //anything else needed to save a game will be here.
        public SavedDiscardPile<SolitaireCard>? PileData { get; set; }
        public SavedTriangle? TriangleData { get; set; }
        public EnumIncreaseList Incs { get; set; }
    }
}