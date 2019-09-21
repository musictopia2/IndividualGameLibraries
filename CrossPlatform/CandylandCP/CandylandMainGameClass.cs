using BasicGameFramework.Attributes;
using BasicGameFramework.BasicDrawables.MiscClasses;
using BasicGameFramework.DIContainers;
using BasicGameFramework.MultiplayerClasses.BasicGameClasses;
using BasicGameFramework.MultiplayerClasses.BasicPlayerClasses;
using BasicGameFramework.MultiplayerClasses.Extensions;
using BasicGameFramework.MultiplayerClasses.InterfaceMessages;
using BasicGameFramework.MultiplayerClasses.InterfacesForHelpers;
using BasicGameFramework.MultiplayerClasses.LoadingClasses;
using CommonBasicStandardLibraries.Exceptions;
using System.Linq;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace CandylandCP
{
    [SingletonGame]
    public class CandylandMainGameClass : BasicGameClass<CandylandPlayerItem, CandylandSaveInfo>, IMiscDataNM, IAfterDraw<CandylandPlayerItem>, IFinishStart
    {
        public CandylandMainGameClass(IGamePackageResolver container) : base(container) { }
        private DrawShuffleClass<CandylandCardData, CandylandPlayerItem>? _thisShuffle;
        public CandylandBoardProcesses? GameBoard1; //i think
        private CandylandViewModel? _thisMod;
        public override void Init() //decided to have all the code under init to prevent overflow issues.
        {
            base.Init();
            _thisMod = MainContainer.Resolve<CandylandViewModel>();
        }
        public async Task DrawAsync() //this simple now.
        {
            await _thisShuffle!.DrawAsync();
        }
        public override Task FinishGetSavedAsync()
        {
            LoadControls();
            if (_thisShuffle == null)
                _thisShuffle = MainContainer.Resolve<DrawShuffleClass<CandylandCardData, CandylandPlayerItem>>();
            _thisShuffle.SaveRoot = SaveRoot; //i think
            PlayerList!.ForEach(Items => Items.ThisGame = this);
            return Task.CompletedTask;
        }
        private void LoadControls()
        {
            if (IsLoaded == true)
                return;
            PlayerList!.ForEach(Items => Items.ThisGame = this);
            GameBoard1 = MainContainer.Resolve<CandylandBoardProcesses>();
            GameBoard1.LoadBoard(); //i think
            IsLoaded = true; //i think needs to be here.
        }
        public override async Task SetUpGameAsync(bool isBeginning)
        {
            LoadControls();
            if (_thisShuffle == null)
                _thisShuffle = MainContainer.Resolve<DrawShuffleClass<CandylandCardData, CandylandPlayerItem>>();
            _thisShuffle.SaveRoot = SaveRoot;
            SaveRoot!.CurrentCard = new CandylandCardData();
            await _thisShuffle.FirstShuffleAsync(true); //i think this time, can't autodraw because we have other things todo.
            GameBoard1!.ClearBoard(); //i think its okay to autodraw.
            await ThisLoader!.FinishUpAsync(isBeginning);
        }
        public async Task GameOverAsync()
        {
            SaveRoot!.CurrentCard!.Visible = false;
            SingleInfo = PlayerList!.GetWhoPlayer();
            if (SingleInfo.CanSendMessage(ThisData!) == true)
                await ThisNet!.SendAllAsync("castle");
            await GameBoard1!.MakeMoveAsync(127);
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
            _thisMod!.CommandContainer!.ManuelFinish = true;
            WhoTurn = await PlayerList!.CalculateWhoTurnAsync(true); //you can't do traditional this time.
            this.ShowTurn();
            await StartNewTurnAsync(); //i think
        }
        protected override async Task ComputerTurnAsync()
        {
            if (ThisTest!.NoAnimations == false)
                await Delay!.DelaySeconds(3);
            if (GameBoard1!.IsValidMove(127, SaveRoot!.CurrentCard!.WhichCard, SaveRoot.CurrentCard.HowMany))
            {
                await GameOverAsync();
                return;
            }
            for (int x = 1; x <= 126; x++)
            {
                if (GameBoard1.IsValidMove(x, SaveRoot.CurrentCard!.WhichCard, SaveRoot.CurrentCard.HowMany))
                {
                    await MakeMoveAsync(x);
                    return;
                }
            }
            throw new BasicBlankException("The Computer Is Stuck");
        }
        public async Task MakeMoveAsync(int Space)
        {
            if (Space < 127)
            {
                if (SingleInfo!.CanSendMessage(ThisData!) == true)
                    await ThisNet!.SendAllAsync("space", Space);
            }
            await GameBoard1!.MakeMoveAsync(Space);
            if (GameBoard1.WillMissNextTurn() == true)
            {
                SingleInfo!.MissNextTurn = true;
                if (SingleInfo.PlayerCategory == EnumPlayerCategory.Self)
                    await _thisMod!.ShowGameMessageAsync("Miss next turn for falling in a pit");
                if (PlayerList.Any(Items => Items.MissNextTurn == true) == false)
                    throw new BasicBlankException("Did fall.  Find out what happened");
            }
            if (Space < 127)
                await EndTurnAsync(); //decided to end turn automatically now.
        }
        public async Task AfterDrawingAsync()
        {
            SaveRoot!.DidDraw = true; //i think
            ThisE.Publish(SaveRoot.CurrentCard); //maybe i forgot this.
            if (IsLoaded == false)
                return;
            if (WasTemp == true)
            {
                WasTemp = false;
                return;
            }
            await ContinueTurnAsync(); //i think
        }
        private bool WasTemp;
        public async Task FinishStartAsync()
        {
            SingleInfo = PlayerList!.GetWhoPlayer();
            if (SingleInfo == null)
                throw new BasicBlankException("Can't register new turn if we don't have a player yet");
            GameBoard1!.NewTurn();

            if (SaveRoot!.DidDraw == false)
            {
                WasTemp = true;
                await _thisShuffle!.DrawAsync();
                return;
            }
            SaveRoot.CurrentCard!.Visible = true;
            ThisE.Publish(SaveRoot.CurrentCard); //i think this is it.
        }
    }
}