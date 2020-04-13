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
using BasicGameFrameworkLibrary.CommandClasses;
using BasicGameFrameworkLibrary.ViewModelInterfaces;
using BasicGameFrameworkLibrary.CommonInterfaces;
using BasicGameFrameworkLibrary.Attributes;
using CommonBasicStandardLibraries.Messenging;
using CaliforniaJackCP.Logic;
using BasicGameFrameworkLibrary.DIContainers;
using BasicGameFrameworkLibrary.MultiplayerClasses.MainViewModels;
using BasicGameFrameworkLibrary.MultiplayerClasses.InterfacesForHelpers;
using BasicGameFrameworkLibrary.BasicGameDataClasses;
using BasicGameFrameworkLibrary.TestUtilities;
using BasicGameFrameworkLibrary.RegularDeckOfCards;
using CaliforniaJackCP.Data;
using CaliforniaJackCP.Cards;
using BasicGameFrameworkLibrary.DrawableListsObservable;
//i think this is the most common things i like to do
namespace CaliforniaJackCP.ViewModels
{
    [InstanceGame]
    public class CaliforniaJackMainViewModel : TrickCardGamesVM<CaliforniaJackCardInformation, EnumSuitList>
    {
        private readonly CaliforniaJackVMData _model;

        public CaliforniaJackMainViewModel(CommandContainer commandContainer,
            CaliforniaJackMainGameClass mainGame,
            CaliforniaJackVMData viewModel, 
            BasicData basicData,
            TestOptions test,
            IGamePackageResolver resolver
            )
            : base(commandContainer, mainGame, viewModel, basicData, test, resolver)
        {
            _model = viewModel;
            _model.Deck1.NeverAutoDisable = true;
            _model.PlayerHand1!.Maximum = 6;
            _model.Deck1!.DeckStyle = DeckObservablePile<CaliforniaJackCardInformation>.EnumStyleType.AlwaysKnown;
        }
        //anything else needed is here.
        protected override bool CanEnableDeck()
        {
            return false; //otherwise, can't compile.
        }

        protected override bool CanEnablePile1()
        {
            return false; //otherwise, can't compile.
        }

        protected override async Task ProcessDiscardClickedAsync()
        {
            //if we have anything, will be here.
            await Task.CompletedTask;
        }
        public override bool CanEnableAlways()
        {
            return true;
        }
    }
}