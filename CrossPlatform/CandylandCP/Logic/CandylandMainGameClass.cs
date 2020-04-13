using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.BasicDrawables.MiscClasses;
using BasicGameFrameworkLibrary.BasicGameDataClasses;
using BasicGameFrameworkLibrary.CommandClasses;
using BasicGameFrameworkLibrary.CommonInterfaces;
using BasicGameFrameworkLibrary.DIContainers;
using BasicGameFrameworkLibrary.MultiplayerClasses.BasicGameClasses;
using BasicGameFrameworkLibrary.MultiplayerClasses.BasicPlayerClasses;
using BasicGameFrameworkLibrary.MultiplayerClasses.Extensions;
using BasicGameFrameworkLibrary.MultiplayerClasses.GameContainers;
using BasicGameFrameworkLibrary.MultiplayerClasses.InterfaceMessages;
using BasicGameFrameworkLibrary.MultiplayerClasses.MainGameInterfaces;
using BasicGameFrameworkLibrary.MultiplayerClasses.SavedGameClasses;
using BasicGameFrameworkLibrary.TestUtilities;
using CandylandCP.Data;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.Messenging;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using System.Linq;
using System.Threading.Tasks; //most of the time, i will be using asyncs.

namespace CandylandCP.Logic
{
    [SingletonGame]
    public class CandylandMainGameClass : BasicGameClass<CandylandPlayerItem, CandylandSaveInfo>, IMiscDataNM, IFinishStart
    {
        public CandylandBoardProcesses GameBoard1; //i think
        public CandylandMainGameClass(IGamePackageResolver resolver,
            IEventAggregator aggregator,
            BasicData basic,
            TestOptions test,
            CandylandVMData model,
            IMultiplayerSaveState state,
            IAsyncDelayer delay,
            CommandContainer command,
            DrawShuffleClass<CandylandCardData, CandylandPlayerItem> shuffle,
            CandylandBoardProcesses gameBoard1,
            BasicGameContainer<CandylandPlayerItem, CandylandSaveInfo> gameContainer
            ) : base(resolver, aggregator, basic, test, model, state, delay, command, gameContainer)
        {
            _test = test;
            _command = command;
            _shuffle = shuffle;
            GameBoard1 = gameBoard1;
            _shuffle.CurrentPlayer = CurrentPlayer;
            _shuffle.AfterDrawingAsync = AfterDrawingAsync;
        }
        public async Task DrawAsync() //this simple now.
        {
            await _shuffle.DrawAsync();
        }
        private CandylandPlayerItem CurrentPlayer()
        {
            return SingleInfo!;
        }

        private readonly TestOptions _test;
        private readonly CommandContainer _command;
        private readonly DrawShuffleClass<CandylandCardData, CandylandPlayerItem> _shuffle;

        public override Task FinishGetSavedAsync()
        {
            LoadControls();
            _shuffle.SaveRoot = SaveRoot; //i think
            //anything else needed is here.
            return Task.CompletedTask;
        }
        private void LoadControls()
        {
            if (IsLoaded == true)
                return;
            GameBoard1.LoadBoard(); //i think
            IsLoaded = true; //i think needs to be here.
        }

        public override async Task SetUpGameAsync(bool isBeginning)
        {
            LoadControls();
            if (FinishUpAsync == null)
            {
                throw new BasicBlankException("The loader never set the finish up code.  Rethink");
            }
            _shuffle.SaveRoot = SaveRoot;
            SaveRoot!.CurrentCard = new CandylandCardData();
            await _shuffle.FirstShuffleAsync(true); //i think this time, can't autodraw because we have other things todo.
            GameBoard1!.ClearBoard(this); //i think its okay to autodraw.
            await FinishUpAsync(isBeginning);
        }

        public async Task GameOverAsync()
        {
            SaveRoot!.CurrentCard!.Visible = false;
            SingleInfo = PlayerList!.GetWhoPlayer();
            if (SingleInfo.CanSendMessage(BasicData!) == true)
                await Network!.SendAllAsync("castle");
            await GameBoard1!.MakeMoveAsync(127, this);
            await ShowWinAsync(); //i think
        }
        async Task IMiscDataNM.MiscDataReceived(string status, string content)
        {
            switch (status)
            {
                case "space":
                    await MakeMoveAsync(int.Parse(content));
                    return;
                case "castle":
                    await GameOverAsync();
                    return;
                default:
                    throw new BasicBlankException($"Nothing for status {status}  with the message of {content}");
            }
        }
        public override async Task StartNewTurnAsync()
        {
            SaveRoot!.DidDraw = false;
            await SaveStateAsync(); //here too.
            SingleInfo = PlayerList!.GetWhoPlayer();
            GameBoard1!.NewTurn(); //has to be after getting singleinfo.
            await DrawAsync();
        }
        public async override Task EndTurnAsync()
        {
            _command.ManuelFinish = true;
            WhoTurn = await PlayerList!.CalculateWhoTurnAsync(true); //you can't do traditional this time.
            this.ShowTurn();
            await StartNewTurnAsync(); //i think
        }
        protected override async Task ComputerTurnAsync()
        {
            if (_test.NoAnimations == false)
                await Delay!.DelaySeconds(3);
            if (GameBoard1!.IsValidMove(127, SaveRoot!.CurrentCard!.WhichCard, this, SaveRoot.CurrentCard.HowMany))
            {
                await GameOverAsync();
                return;
            }
            for (int x = 1; x <= 126; x++)
            {
                if (GameBoard1.IsValidMove(x, SaveRoot.CurrentCard!.WhichCard, this, SaveRoot.CurrentCard.HowMany))
                {
                    await MakeMoveAsync(x);
                    return;
                }
            }
            throw new BasicBlankException("The Computer Is Stuck");
        }
        public async Task MakeMoveAsync(int space)
        {
            if (space < 127)
            {
                if (SingleInfo!.CanSendMessage(BasicData!) == true)
                    await Network!.SendAllAsync("space", space);
            }
            await GameBoard1!.MakeMoveAsync(space, this);
            if (GameBoard1.WillMissNextTurn(this) == true)
            {
                SingleInfo!.MissNextTurn = true;
                if (SingleInfo.PlayerCategory == EnumPlayerCategory.Self)
                    await UIPlatform.ShowMessageAsync("Miss next turn for falling in a pit");
                if (PlayerList.Any(Items => Items.MissNextTurn == true) == false)
                    throw new BasicBlankException("Did fall.  Find out what happened");
            }
            if (space < 127)
                await EndTurnAsync(); //decided to end turn automatically now.
        }

        private bool _wasTemp;

        public async Task AfterDrawingAsync()
        {
            SaveRoot!.DidDraw = true; //i think
            Aggregator.Publish(SaveRoot.CurrentCard!); //maybe i forgot this.
            if (IsLoaded == false)
                return;
            if (_wasTemp == true)
            {
                _wasTemp = false;
                return;
            }
            await ContinueTurnAsync(); //i think
        }
        public async Task FinishStartAsync()
        {
            SingleInfo = PlayerList!.GetWhoPlayer();
            if (SingleInfo == null)
                throw new BasicBlankException("Can't register new turn if we don't have a player yet");
            GameBoard1!.NewTurn();

            if (SaveRoot!.DidDraw == false)
            {
                _wasTemp = true;
                await _shuffle!.DrawAsync();
                return;
            }
            SaveRoot.CurrentCard!.Visible = true;
            Aggregator.Publish(SaveRoot.CurrentCard); //i think this is it.
        }
    }
}
