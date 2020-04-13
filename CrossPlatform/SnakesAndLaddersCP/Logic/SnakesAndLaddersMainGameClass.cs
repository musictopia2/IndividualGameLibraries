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
using BasicGameFrameworkLibrary.CommonInterfaces;
using SnakesAndLaddersCP.Data;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using CommonBasicStandardLibraries.Messenging;
using BasicGameFrameworkLibrary.BasicEventModels;
using BasicGameFrameworkLibrary.DIContainers;
using BasicGameFrameworkLibrary.MultiplayerClasses.BasicGameClasses;
using BasicGameFrameworkLibrary.MultiplayerClasses.InterfaceMessages;
using BasicGameFrameworkLibrary.BasicGameDataClasses;
using BasicGameFrameworkLibrary.TestUtilities;
using BasicGameFrameworkLibrary.MultiplayerClasses.SavedGameClasses;
using BasicGameFrameworkLibrary.CommandClasses;
using BasicGameFrameworkLibrary.Extensions; //most likely will be used.
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.RandomGenerator;
using BasicGameFrameworkLibrary.Dice;
using BasicGameFrameworkLibrary.MultiplayerClasses.Extensions;
using BasicGameFrameworkLibrary.NetworkingClasses.Extensions;
using BasicGameFrameworkLibrary.MultiplayerClasses.BasicPlayerClasses;
using BasicGameFrameworkLibrary.MultiplayerClasses.MainGameInterfaces;
using BasicGameFrameworkLibrary.MultiplayerClasses.GameContainers;

namespace SnakesAndLaddersCP.Logic
{
    [SingletonGame]
    public class SnakesAndLaddersMainGameClass : BasicGameClass<SnakesAndLaddersPlayerItem, SnakesAndLaddersSaveInfo>, IFinishStart, IMoveNM
    {
        public SnakesAndLaddersMainGameClass(IGamePackageResolver resolver,
            IEventAggregator aggregator,
            BasicData basic,
            TestOptions test,
            SnakesAndLaddersVMData model,
            IMultiplayerSaveState state,
            IAsyncDelayer delay,
            CommandContainer command,
            StandardRollProcesses<SimpleDice, SnakesAndLaddersPlayerItem> roll,
            GameBoardProcesses gameBoard1,
            BasicGameContainer<SnakesAndLaddersPlayerItem, SnakesAndLaddersSaveInfo> gameContainer
            ) : base(resolver, aggregator, basic, test, model, state,delay, command, gameContainer)
        {
            _test = test;
            _model = model;
            Roll = roll;
            GameBoard1 = gameBoard1;
            Roll.AfterRollingAsync = AfterRollingAsync; //hopefully this simple.
            roll.CurrentPlayer = GetCurrentPlayer;
            GameBoard1.CurrentPlayer = GetCurrentPlayer;
        }
        private SnakesAndLaddersPlayerItem GetCurrentPlayer() => SingleInfo!;
        internal StandardRollProcesses<SimpleDice, SnakesAndLaddersPlayerItem> Roll { get; set; }
        internal GameBoardProcesses GameBoard1 { get; set; }

        private readonly TestOptions _test;
        private readonly SnakesAndLaddersVMData _model;

        public override Task FinishGetSavedAsync()
        {
            _model.LoadCup(SaveRoot, true);
            SaveRoot!.DiceList.MainContainer = MainContainer;
            _model.Cup!.CanShowDice = SaveRoot.HasRolled;

            //anything else needed is here.
            return Task.CompletedTask;
        }
        

        public async Task MakeMoveAsync(int space)
        {
            if (SingleInfo!.CanSendMessage(BasicData!) == true)
                await Network!.SendMoveAsync(space);
            await GameBoard1!.MakeMoveAsync(space);
            if (_test.NoAnimations == false)
                await Delay!.DelayMilli(800); //to see move made
            if (GameBoard1.IsGameOver() == true)
            {
                await ShowWinAsync();
                return;
            }
            await EndTurnAsync();
        }
        public async Task MoveReceivedAsync(string data)
        {
            await MakeMoveAsync(int.Parse(data));
        }
        public override async Task EndTurnAsync()
        {
            WhoTurn = await PlayerList!.CalculateWhoTurnAsync();
            await StartNewTurnAsync();
        }
        protected override async Task ComputerTurnAsync()
        {
            if (_test.NoAnimations == false)
                await Delay!.DelayMilli(200);
            if (SaveRoot!.HasRolled == false)
            {
                await Roll.RollDiceAsync();
                return;
            }
            int NewSpace = SingleInfo!.SpaceNumber + _model.Cup!.ValueOfOnlyDice;
            await MakeMoveAsync(NewSpace);
        }

        public override async Task SetUpGameAsync(bool isBeginning)
        {
            if (FinishUpAsync == null)
            {
                throw new BasicBlankException("The loader never set the finish up code.  Rethink");
            } //hopefully no need for the load since its new game each time now.
            _model.LoadCup(SaveRoot!, false);
            SaveRoot!.DiceList.MainContainer = MainContainer;
            SaveRoot!.ImmediatelyStartTurn = true;
            PlayerList!.ForEach(thisPlayer => thisPlayer.SpaceNumber = 0);
            await FinishUpAsync(isBeginning);
        }
        public override async Task StartNewTurnAsync()
        {
            PrepStartTurn(); //anything else is below.
            SaveRoot!.HasRolled = false;
            _model.Cup!.CanShowDice = false;
            GameBoard1!.NewTurn(this); //i think here.
            await ContinueTurnAsync(); //most of the time, continue turn.  can change to what is needed
        }
        public async Task AfterRollingAsync()
        {
            SaveRoot!.HasRolled = true;
            if (GameBoard1!.HasValidMove() == false)
            {
                if (SingleInfo!.PlayerCategory == EnumPlayerCategory.Self)
                    await UIPlatform.ShowMessageAsync("No moves available.  Therefore, the turn will end");
                await EndTurnAsync();
                return;
            }
            await ContinueTurnAsync();
        }
        //public Task AfterSelectUnselectDiceAsync()
        //{
        //    throw new BasicBlankException("There is no select/unselect dice");
        //}
        public Task FinishStartAsync()
        {
            SingleInfo = PlayerList!.GetWhoPlayer();
            GameBoard1!.NewTurn(this);
            return Task.CompletedTask;
        }
    }
}
