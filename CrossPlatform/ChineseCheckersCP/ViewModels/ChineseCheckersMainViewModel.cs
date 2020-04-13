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
using ChineseCheckersCP.Logic;
using BasicGameFrameworkLibrary.DIContainers;
using BasicGameFrameworkLibrary.MultiplayerClasses.MainViewModels;
using BasicGameFrameworkLibrary.MultiplayerClasses.InterfacesForHelpers;
using BasicGameFrameworkLibrary.BasicGameDataClasses;
using BasicGameFrameworkLibrary.TestUtilities;
using ChineseCheckersCP.Data;
//i think this is the most common things i like to do
namespace ChineseCheckersCP.ViewModels
{
    [InstanceGame]
    public class ChineseCheckersMainViewModel : SimpleBoardGameVM
    {
        private readonly GameBoardProcesses _gameBoard;

        public ChineseCheckersMainViewModel(CommandContainer commandContainer,
            ChineseCheckersMainGameClass mainGame,
            ChineseCheckersVMData model, 
            BasicData basicData,
            TestOptions test,
            IGamePackageResolver resolver,
            GameBoardProcesses gameBoard
            )
            : base(commandContainer, mainGame, model, basicData, test, resolver)
        {
            _gameBoard = gameBoard;
        }
        //anything else needed is here.
        public override bool CanEndTurn()
        {
            return _gameBoard.WillContinueTurn();
        }
    }
}