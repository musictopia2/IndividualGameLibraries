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
using BasicGameFrameworkLibrary.Attributes;
using MancalaCP.Logic;
using BasicGameFrameworkLibrary.CommandClasses;
using BasicGameFrameworkLibrary.BasicGameDataClasses;
using BasicGameFrameworkLibrary.NetworkingClasses.Interfaces;
using BasicGameFrameworkLibrary.Extensions;
using BasicGameFrameworkLibrary.NetworkingClasses.Extensions;
//i think this is the most common things i like to do
namespace MancalaCP.ViewModels
{
    [SingletonGame]
    [AutoReset]
    public class GameBoardVM
    {
        public GameBoardVM(MancalaMainGameClass mainGame, 
            CommandContainer command,
            BasicData basicData,
            GameBoardProcesses gameBoard1
            )
        {
            _mainGame = mainGame;
            _command = command;
            _basicData = basicData;
            GameBoard1 = gameBoard1;
            _network = _basicData.GetNetwork();
        }
        private readonly INetworkMessages? _network;
        private readonly MancalaMainGameClass _mainGame;
        private readonly CommandContainer _command;
        private readonly BasicData _basicData;

        public GameBoardProcesses GameBoard1 { get; }

        public async Task MakeMoveAsync(int space)
        {
            _command.IsExecuting = true;
            if (_mainGame!.SingleInfo!.ObjectList.Any(x => x.Index == space) == false)
            {
                return;
            }
            _mainGame!.OpenMove();
            if (_basicData!.MultiPlayer == true)
                await _network!.SendMoveAsync(space + 7); //because reversed.
            await GameBoard1!.AnimateMoveAsync(space);
        }

    }
}
