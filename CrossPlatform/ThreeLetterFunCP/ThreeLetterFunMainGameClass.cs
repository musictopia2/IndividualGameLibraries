using BasicGameFramework.Attributes;
using BasicGameFramework.BasicDrawables.Interfaces;
using BasicGameFramework.DIContainers;
using BasicGameFramework.Extensions;
using BasicGameFramework.MultiplayerClasses.BasicGameClasses;
using BasicGameFramework.MultiplayerClasses.Extensions;
using CommonBasicStandardLibraries.Exceptions;
using System.Linq;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;
namespace ThreeLetterFunCP
{
    [SingletonGame]
    public class ThreeLetterFunMainGameClass : BasicGameClass<ThreeLetterFunPlayerItem, ThreeLetterFunSaveInfo>
    {
        public IListShuffler<ThreeLetterFunCardData>? DeckList; //i think this could be okay.
        internal GlobalHelpers? ThisGlobal;
        public ThreeLetterFunMainGameClass(IGamePackageResolver container) : base(container) { }
        private ThreeLetterFunViewModel? _thisMod;
        public override void Init() //decided to have all the code under init to prevent overflow issues.
        {
            base.Init();
            _thisMod = MainContainer.Resolve<ThreeLetterFunViewModel>();
        }
        internal async Task GameOverAsync()
        {
            if (ThisData!.MultiPlayer == false)
            {
                int counts = ThisGlobal!.GameBoard1!.CardsRemaining();
                if (counts == 0)
                    await _thisMod!.ShowGameMessageAsync("Congratulations; you got rid of all 36 cards.  Therefore; you win");
                else
                    await _thisMod!.ShowGameMessageAsync($"{counts} cards left");
                await this.ProtectedGameOverNextAsync(ThisState!);
                return;
            }
            SingleInfo = PlayerList.OrderByDescending(Items => Items.CardsWon).ThenBy(Items => Items.MostRecent).Take(1).Single();
            _thisMod!.PlayerWon = SingleInfo.NickName;
            await ShowWinAsync();
        }
        public override async Task ContinueTurnAsync()
        {
            if (_thisMod!.MainOptionsVisible == false)
                return;
            if (ThisData!.MultiPlayer == true)
                SingleInfo = PlayerList!.GetSelf(); //because everybody takes their turn the same tie.
            await SaveStateAsync();
            if (ThisData.MultiPlayer == false)
            {
                await ShowHumanCanPlayAsync(); //i think.
                return;
            }
            _thisMod.CommandContainer!.ManuelFinish = true; //has to be manuel.  if you can play, not anymore.  has to be proven.
            SingleInfo = PlayerList!.GetSelf(); //because everybody takes their turn the same tie.
            if (SingleInfo.TookTurn == false)
            {
                await ShowHumanCanPlayAsync();
                ThisGlobal!.Stops!.StartTimer();
                ThisCheck!.IsEnabled = false;

                return;
            }
            else
            {
                ThisCheck!.IsEnabled = true; //waiting to hear from other players.
            }

        }
        public override Task PopulateSaveRootAsync()
        {
            SaveRoot!.SavedList = ThisGlobal!.GameBoard1!.ObjectList.ToRegularDeckDict();
            return Task.CompletedTask;
        }
        public override async Task FinishGetSavedAsync()
        {
            LoadControls();
            _thisMod!.PlayerWon = "";
            if (SaveRoot!.CanStart == false)
            {
                await LoadStartOptionsAsync();
                return;
            }
            if (ThisData!.MultiPlayer == true && SaveRoot.Level != EnumLevel.None && DeckList == null) //maybe its needed anyways otherwise, no deck
            {
                DeckList = MainContainer.Resolve<IListShuffler<ThreeLetterFunCardData>>();
                DeckList.OrderedObjects(); //i think
            }
            if (ThisData.MultiPlayer == true && SaveRoot.Level == EnumLevel.Easy)
            {
                PlayerList!.ForEach(ThisPlayer =>
                {
                    ThisPlayer.MainHandList.ForEach(ThisCard => ThisCard.ReloadSaved());
                });
            }
            if (ThisData.MultiPlayer == false)
            {
                if (DeckList == null)
                    DeckList = MainContainer.Resolve<IListShuffler<ThreeLetterFunCardData>>(); //i think.
                DeckList.ClearObjects();
                DeckList.OrderedObjects(); //i think
            }
            ThisGlobal!.GameBoard1!.NewLoadSavedGame();
            ThisGlobal.GameBoard1.Visible = true; //try this too.
            _thisMod.TileBoard1!.UpdateBoard(); //i think.
        }
        private void LoadControls()
        {
            if (IsLoaded == true)
                return;
            ThisGlobal = MainContainer.Resolve<GlobalHelpers>();
            ThisGlobal.LoadItems(); //i think.
            IsLoaded = true; //i think needs to be here.
        }
        protected override Task ComputerTurnAsync()
        {
            throw new BasicBlankException("Computer does not take a turn on single player games for this game");
        }
        public override async Task SetUpGameAsync(bool isBeginning)
        {
            LoadControls();
            if (ThisData!.MultiPlayer == true)
            {
                if (SaveRoot!.PlayerList == null)
                    throw new BasicBlankException("Has to have players for multiplayer.  Rethink now");
                SaveRoot.Level = EnumLevel.None; //you have to prove the level.
                SaveRoot.CanStart = false; //you can't start to begin with either.
                _thisMod!.MainOptionsVisible = false; //you can't show the game no matter what.
                _thisMod.PlayerWon = "";
                await LoadStartOptionsAsync();
            }
            else
            {
                SaveRoot!.Level = EnumLevel.Hard; //if not multiplayer, then choose hard.  the rules only showed for hard for solitaire version.
                StarterClass thisStart = Resolve<StarterClass>();
                await thisStart.StartShufflingAsync();
            }
            if (isBeginning == false)
            {
                INewGame thisGame = MainContainer.Resolve<INewGame>();
                thisGame.NewGame();
            }
            await ThisLoader!.FinishUpAsync(isBeginning);
        }
        protected override void PrepStartTurn() { } //hopefully this way will work (?)
        public override Task StartNewTurnAsync()
        {
            return Task.CompletedTask;
        }
        public override Task EndTurnAsync()
        {
            throw new BasicBlankException("No Ending Turn.  Because the players takes their turns at the same time");
        }
        private async Task LoadStartOptionsAsync()
        {
            if (ThisData!.MultiPlayer == false)
                throw new BasicBlankException("Single player should never load start options");
            StarterClass thisStart = Resolve<StarterClass>();
            _thisMod!.MainOptionsVisible = false; //try to have here just in case.
            thisStart.Init(); //smart enough to do only once.
            thisStart.StartUp();
            if (ThisData.Client == false)
                await ShowHumanCanPlayAsync(); //i think.  hopefully i don't regret this.
            else
                ThisCheck!.IsEnabled = true; //to wait for host to choose options.
        }
        public override bool CanMakeMainOptionsVisibleAtBeginning
        {
            get
            {
                if (ThisData!.MultiPlayer == false)
                    return true;
                return SaveRoot!.CanStart;
            }
        }
        private async Task FinalAnalAsync()
        {
            _thisMod!.CommandContainer!.ManuelFinish = true;
            if (ThisData!.MultiPlayer == false)
                throw new BasicBlankException("Single player cannot figure out the turns");
            if (PlayerList.Any(items => items.TookTurn == false))
            {
                SingleInfo = PlayerList!.GetSelf();
                if (SingleInfo.TookTurn == false)
                    throw new BasicBlankException("I think you should have taken your turn before going through the last step");
                ThisCheck!.IsEnabled = true; //waiting for others to show they took their turns
                return;
            }
            if (PlayerList.Any(Items => Items.TimeToGetWord == 0))
                throw new BasicBlankException("Must have taken longer than 0 to get a word");
            if (PlayerList.All(Items => Items.TimeToGetWord == -1))
            {
                await _thisMod.ShowGameMessageAsync("Nobody found any words.  Therefore; going to the next one");
                await NextOneAsync();
                return;
            }
            if (ThisData.Client == true)
            {
                ThisCheck!.IsEnabled = true;
                return; //has to wait for host.
            }
            SingleInfo = PlayerList.Where(items => items.TimeToGetWord > -1).OrderBy
                (Items => Items.TimeToGetWord).Take(1).Single();
            WhoTurn = SingleInfo.Id;
            await ThisNet!.SendAllAsync("whowon", WhoTurn);
            await ClientResultsAsync(WhoTurn);
        }
        private async Task NextOneAsync()
        {
            ThisGlobal!.GameBoard1!.RemoveTiles();
            if (ThisTest!.ImmediatelyEndGame == false)
                SaveRoot!.TileList.RemoveTiles(_thisMod!);
            if (ThisData!.MultiPlayer == true)
                PlayerList!.TakeTurns();
            if (SaveRoot!.TileList.Count == 0 || ThisTest.ImmediatelyEndGame == true)
            {
                await GameOverAsync();
                return;
            }
            SaveRoot.UpTo++;
            await ContinueTurnAsync();
        }
        internal async Task ClientResultsAsync(int wins)
        {
            WhoTurn = wins;
            SingleInfo = PlayerList!.GetWhoPlayer();
            if (SingleInfo.TileList.Count == 0)
                throw new BasicBlankException("You must have tiles if you won");
            if (SingleInfo.CardUsed == 0)
                throw new BasicBlankException("Don't know what card was used for the word for the player");
            await ThisGlobal!.GameBoard1!.ShowWordAsync(SingleInfo.CardUsed);
            if (ThisGlobal.GameBoard1.CardsRemaining() == 0 || ThisTest!.ImmediatelyEndGame == true)
            {
                await GameOverAsync();
                return;
            }
            if (SaveRoot!.ShortGame == true && PlayerList.Any(Items => Items.CardsWon >= 5))
            {
                await GameOverAsync();
                return;
            }
            if (SaveRoot.Level == EnumLevel.Easy && PlayerList.Any(Items => Items.CardsWon >= SaveRoot.CardsToBeginWith))
            {
                await GameOverAsync();
                return;
            }
            await NextOneAsync();
        }
        public async Task GiveUpAsync()
        {
            if (ThisData!.MultiPlayer == true)
            {
                SingleInfo = PlayerList!.GetWhoPlayer(); //i think this means the who turn has to be whoever gave up.
                SingleInfo.TookTurn = true;
                await _thisMod!.ShowGameMessageAsync($"{SingleInfo.NickName} took turn");
                SingleInfo.TimeToGetWord = -1;
                await FinalAnalAsync();
                return;
            }
            await NextOneAsync();
        }
        public async Task PlayWordAsync(int deck)
        {
            if (ThisData!.MultiPlayer == false)
            {
                var thisCard = ThisGlobal!.GameBoard1!.ObjectList.GetSpecificItem(deck);
                thisCard.Visible = false;
                if (ThisGlobal.GameBoard1.CardsRemaining() == 0 || ThisTest!.ImmediatelyEndGame == true)
                {
                    await GameOverAsync();
                    return;
                }
                await NextOneAsync();
                return;
            }
            SingleInfo!.TookTurn = true;
            SingleInfo.CardUsed = deck;
            await FinalAnalAsync();
        }
    }
}