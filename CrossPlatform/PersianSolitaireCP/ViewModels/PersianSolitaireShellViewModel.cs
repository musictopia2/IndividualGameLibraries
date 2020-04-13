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
using BasicGameFrameworkLibrary.ViewModels;
using BasicGameFrameworkLibrary.CommonInterfaces;
using BasicGameFrameworkLibrary.DIContainers;
using BasicGameFrameworkLibrary.CommandClasses;
using BasicGameFrameworkLibrary.BasicGameDataClasses;
using BasicGameFrameworkLibrary.SolitaireClasses.DataClasses;
using BasicGameFrameworkLibrary.Attributes;
using PersianSolitaireCP.Data;
using CommonBasicStandardLibraries.Messenging;
using BasicGameFrameworkLibrary.SolitaireClasses.MainClasses;
using CommonBasicStandardLibraries.MVVMFramework.ViewModels;
using BasicGameFrameworkLibrary.SolitaireClasses.BasicVMInterfaces;
using BasicGameFrameworkLibrary.ViewModelInterfaces;
using BasicGameFrameworkLibrary.SolitaireClasses.PileInterfaces;
using BasicGameFrameworkLibrary.DrawableListsObservable;
using BasicGameFrameworkLibrary.SolitaireClasses.Cards;
//i think this is the most common things i like to do
namespace PersianSolitaireCP.ViewModels
{
    public class PersianSolitaireShellViewModel : SinglePlayerShellViewModel
    {
        public PersianSolitaireShellViewModel(IGamePackageResolver mainContainer, 
            CommandContainer container,
            IGameInfo GameData, 
            ISaveSinglePlayerClass saves) : base(mainContainer, container, GameData, saves)
        {
        }

        protected override bool AlwaysNewGame => true; //most games allow new game always.

        protected override IMainScreen GetMainViewModel()
        {
            var model = MainContainer.Resolve<PersianSolitaireMainViewModel>();
            return model;
        }
    }



}
