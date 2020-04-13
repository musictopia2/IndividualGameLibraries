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
using TroubleCP.Logic;
using BasicGameFrameworkLibrary.DIContainers;
using BasicGameFrameworkLibrary.MultiplayerClasses.MainViewModels;
using BasicGameFrameworkLibrary.MultiplayerClasses.InterfacesForHelpers;
using BasicGameFrameworkLibrary.BasicGameDataClasses;
using BasicGameFrameworkLibrary.TestUtilities;
using TroubleCP.Data;
using BasicGameFrameworkLibrary.Dice;
//i think this is the most common things i like to do
namespace TroubleCP.ViewModels
{
    [InstanceGame]
    public class TroubleMainViewModel : BoardDiceGameVM
    {
        private readonly TroubleMainGameClass _mainGame; //if we don't need, delete.

        public TroubleMainViewModel(CommandContainer commandContainer,
            TroubleMainGameClass mainGame,
            TroubleVMData model, 
            BasicData basicData,
            TestOptions test,
            IGamePackageResolver resolver,
            IStandardRollProcesses roller,
            TroubleGameContainer gameContainer
            )
            : base(commandContainer, mainGame, model, basicData, test, resolver, roller)
        {
            _mainGame = mainGame;
            gameContainer.CanRollDice = CanRollDice;
            gameContainer.RollDiceAsync = RollDiceAsync;
        }
        //anything else needed is here.
        public override bool CanRollDice()
        {
            return _mainGame.SaveRoot.DiceNumber == 0;
        }
        public override async Task RollDiceAsync() //if any changes, do here.
        {
            await base.RollDiceAsync();
        }
    }
}