using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.BasicDrawables.Interfaces;
using BasicGameFrameworkLibrary.BasicGameDataClasses;
using BasicGameFrameworkLibrary.CommandClasses;
using BasicGameFrameworkLibrary.CommonInterfaces;
using BasicGameFrameworkLibrary.DIContainers;
using BasicGameFrameworkLibrary.Extensions; //most likely will be used.
using BasicGameFrameworkLibrary.MultiplayerClasses.BasicGameClasses;
using BasicGameFrameworkLibrary.MultiplayerClasses.Extensions;
using BasicGameFrameworkLibrary.MultiplayerClasses.GameContainers;
using BasicGameFrameworkLibrary.MultiplayerClasses.InterfaceMessages;
using BasicGameFrameworkLibrary.MultiplayerClasses.SavedGameClasses;
using BasicGameFrameworkLibrary.TestUtilities;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.Messenging;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using System.Linq;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using ThreeLetterFunCP.Data;
using ThreeLetterFunCP.EventModels;
using js = CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.JsonSerializers.NewtonJsonStrings; //just in case i need those 2.

namespace ThreeLetterFunCP.Logic
{
    [SingletonGame]
    public class ThreeLetterFunMainGameClass : BasicGameClass<ThreeLetterFunPlayerItem, ThreeLetterFunSaveInfo>, IMiscDataNM
    {
        public ThreeLetterFunMainGameClass(IGamePackageResolver resolver,
            IEventAggregator aggregator,
            BasicData basic,
            TestOptions test,
            ThreeLetterFunVMData model,
            IMultiplayerSaveState state,
            IAsyncDelayer delay,
            CommandContainer command,
            GameBoard gameboard,
            GlobalHelpers global,
            IListShuffler<ThreeLetterFunCardData> deck,
            BasicGameContainer<ThreeLetterFunPlayerItem, ThreeLetterFunSaveInfo> gameContainer
            ) : base(resolver, aggregator, basic, test, model, state, delay, command, gameContainer)
        {
            _test = test;
            _model = model;
            _command = command;
            _gameboard = gameboard;
            _global = global;
            _deck = deck;
            _gameboard.SetSelf = (() =>
            {
                SingleInfo = PlayerList.GetSelf();
            });
            _gameboard.SingleInfo = (() => SingleInfo!);
            _gameboard.SaveRoot = (() => SaveRoot);
        }

        private readonly TestOptions _test;
        private readonly ThreeLetterFunVMData _model;
        private readonly CommandContainer _command;
        private readonly GameBoard _gameboard;
        private readonly GlobalHelpers _global;
        private readonly IListShuffler<ThreeLetterFunCardData> _deck;

        private async Task GameOverAsync()
        {
            if (BasicData.MultiPlayer == false)
            {
                int counts = _gameboard.CardsRemaining();
                if (counts == 0)
                    await UIPlatform.ShowMessageAsync("Congratulations; you got rid of all 36 cards.  Therefore; you win");
                else
                    await UIPlatform.ShowMessageAsync($"{counts} cards left");
                await this.ProtectedGameOverNextAsync();
                return;
            }
            SingleInfo = PlayerList.OrderByDescending(x => x.CardsWon).ThenBy(Items => Items.MostRecent).Take(1).Single();
            _model.PlayerWon = SingleInfo.NickName;
            await ShowWinAsync();
        }

        public override async Task ContinueTurnAsync()
        {
            if (SaveRoot.CanStart == false)
            {
                return; //maybe this is good enough (?)
            }
            //hopefully would never run through if you are not even starting yet.  if not, rethink.
            //if (_thisMod!.MainOptionsVisible == false)
            //    return;
            if (BasicData!.MultiPlayer == true)
            {
                SingleInfo = PlayerList!.GetSelf(); //because everybody takes their turn the same tie.
            }
            await SaveStateAsync();
            if (BasicData.MultiPlayer == false)
            {
                await ShowHumanCanPlayAsync(); //i think.
                _model.TileBoard1!.ReportCanExecuteChange(); //try this way now.
                _gameboard.ReportCanExecuteChange();//try this too.
                return;
            }
            _command.ManuelFinish = true; //has to be manuel.  if you can play, not anymore.  has to be proven.
            SingleInfo = PlayerList!.GetSelf(); //because everybody takes their turn the same tie.
            if (SingleInfo.TookTurn == false)
            {
                await ShowHumanCanPlayAsync();
                _model.TileBoard1!.ReportCanExecuteChange(); //try this way now.
                _gameboard.ReportCanExecuteChange();//try this too.
                _global.Stops!.StartTimer();
                Check!.IsEnabled = false;

                return;
            }
            else
            {
                Check!.IsEnabled = true; //waiting to hear from other players.
            }
        }
        public override Task PopulateSaveRootAsync()
        {
            SaveRoot!.SavedList = _gameboard.ObjectList.ToRegularDeckDict();
            return Task.CompletedTask;
        }

        public override async Task FinishGetSavedAsync()
        {
            LoadControls();
            _model.PlayerWon = "";
            if (SaveRoot!.CanStart == false)
            {
                await LoadStartOptionsAsync();
                return;
            }
            if (BasicData.MultiPlayer == true && SaveRoot.Level != EnumLevel.None) //maybe its needed anyways otherwise, no deck
            {
                _deck.OrderedObjects(); //i think
            }
            if (BasicData.MultiPlayer == true && SaveRoot.Level == EnumLevel.Easy)
            {
                PlayerList!.ForEach(player =>
                {
                    player.MainHandList.ForEach(card => card.ReloadSaved());
                });
            }
            if (BasicData.MultiPlayer == false)
            {
                _deck.ClearObjects();
                _deck.OrderedObjects(); //i think
            }
            _gameboard.NewLoadSavedGame();
            _gameboard.Visible = true; //try this too.
            _model.TileBoard1!.UpdateBoard(); //i think.
        }
        private void LoadControls()
        {
            if (IsLoaded == true)
                return;

            IsLoaded = true; //i think needs to be here.
        }
        protected override Task ComputerTurnAsync()
        {
            throw new BasicBlankException("Computer does not take a turn on single player games for this game");
        }
        public override async Task SetUpGameAsync(bool isBeginning)
        {
            LoadControls();
            if (FinishUpAsync == null)
            {
                throw new BasicBlankException("The loader never set the finish up code.  Rethink");
            }

            if (BasicData.MultiPlayer == false)
            {
                SaveRoot.Level = EnumLevel.Hard;
                IShuffleTiles tiles = MainContainer.Resolve<IShuffleTiles>();
                await tiles.StartShufflingAsync(this);
            }
            else
            {
                SaveRoot.Level = EnumLevel.None;
                SaveRoot.CanStart = false;
                _model.PlayerWon = "";
                await LoadStartOptionsAsync();
            }
            //hopefully will be this simple this time (?)

            await FinishUpAsync(isBeginning);
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
            if (BasicData!.MultiPlayer == false)
            {
                throw new BasicBlankException("Single player should never load start options");
            }

            //await Aggregator.PublishAsync(new FirstStartUpEventModel());
            //hopefully this is enough (?)
            //StarterClass thisStart = Resolve<StarterClass>();
            //_thisMod!.MainOptionsVisible = false; //try to have here just in case.
            //thisStart.Init(); //smart enough to do only once.
            //thisStart.StartUp();
            if (BasicData.Client == false)
            {
                await ShowHumanCanPlayAsync(); //i think.  hopefully i don't regret this.
            }
            else
            {
                Check!.IsEnabled = true; //to wait for host to choose options.
            }
        }
        public override bool CanMakeMainOptionsVisibleAtBeginning
        {
            get
            {
                if (BasicData.MultiPlayer == false)
                {
                    return true;
                }
                return SaveRoot.CanStart;
            }
        }

        private async Task FinalAnalAsync()
        {
            _command.ManuelFinish = true;
            if (BasicData!.MultiPlayer == false)
                throw new BasicBlankException("Single player cannot figure out the turns");
            if (PlayerList.Any(items => items.TookTurn == false))
            {
                SingleInfo = PlayerList!.GetSelf();
                if (SingleInfo.TookTurn == false)
                    throw new BasicBlankException($"I think the player {SingleInfo.NickName} should have taken your turn before going through the last step");
                Check!.IsEnabled = true; //waiting for others to show they took their turns
                return;
            }
            if (PlayerList.Any(Items => Items.TimeToGetWord == 0))
                throw new BasicBlankException("Must have taken longer than 0 to get a word");
            if (PlayerList.All(Items => Items.TimeToGetWord == -1))
            {
                await UIPlatform.ShowMessageAsync("Nobody found any words.  Therefore; going to the next one");
                await NextOneAsync();
                return;
            }
            if (BasicData.Client == true)
            {
                Check!.IsEnabled = true;
                return; //has to wait for host.
            }
            SingleInfo = PlayerList.Where(items => items.TimeToGetWord > -1).OrderBy
                (Items => Items.TimeToGetWord).Take(1).Single();
            WhoTurn = SingleInfo.Id;
            await Network!.SendAllAsync("whowon", WhoTurn);
            await ClientResultsAsync(WhoTurn);
        }
        private async Task NextOneAsync()
        {
            _gameboard.RemoveTiles();
            if (_test.ImmediatelyEndGame == false)
                SaveRoot!.TileList.RemoveTiles(_model);
            if (BasicData.MultiPlayer == true)
                PlayerList!.TakeTurns();
            if (SaveRoot!.TileList.Count == 0 || _test.ImmediatelyEndGame == true)
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
            await _gameboard.ShowWordAsync(SingleInfo.CardUsed);
            if (_gameboard.CardsRemaining() == 0 || _test.ImmediatelyEndGame == true)
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
            if (BasicData.MultiPlayer == true)
            {
                SingleInfo = PlayerList!.GetWhoPlayer(); //i think this means the who turn has to be whoever gave up.
                SingleInfo.TookTurn = true;
                await UIPlatform.ShowMessageAsync($"{SingleInfo.NickName} took turn");
                SingleInfo.TimeToGetWord = -1;
                await FinalAnalAsync();
                return;
            }
            await NextOneAsync();
        }
        public async Task PlayWordAsync(int deck)
        {
            if (BasicData.MultiPlayer == false)
            {
                var thisCard = _gameboard.ObjectList.GetSpecificItem(deck);
                thisCard.Visible = false;
                if (_gameboard.CardsRemaining() == 0 || _test.ImmediatelyEndGame == true)
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

        //decided to have the main game class this time process the miscdata.
        async Task IMiscDataNM.MiscDataReceived(string status, string content)
        {
            switch (status)
            {
                case "firstoption":
                    await Aggregator.PublishAsync(new FirstOptionEventModel(content));
                    break;
                case "advancedsettings":
                    await Aggregator.PublishAsync(new AdvancedSettingsEventModel(content));
                    break;
                case "howmanycards":
                    await Aggregator.PublishAsync(new CardsChosenEventModel(int.Parse(content)));
                    break;
                case "giveup": //no more tilelist now.
                    SingleInfo = PlayerList!.GetSelf();
                    if (SingleInfo.TookTurn == false)
                        throw new BasicBlankException("Did not take turn");
                    SaveRoot!.PlayOrder.WhoTurn = int.Parse(content); //hopefully this works too.
                    await GiveUpAsync();
                    break;
                case "playword":
                    SingleInfo = PlayerList!.GetSelf();
                    if (SingleInfo.TookTurn == false)
                        throw new BasicBlankException("Did not take turn");
                    TempWord thisWord = await js.DeserializeObjectAsync<TempWord>(content);
                    SaveRoot!.PlayOrder.WhoTurn = thisWord.Player;
                    SingleInfo = PlayerList.GetWhoPlayer(); //hopefully this still works.
                    SingleInfo.TimeToGetWord = thisWord.TimeToGetWord;
                    SingleInfo.TileList = thisWord.TileList;
                    await PlayWordAsync(thisWord.CardUsed);
                    break;
                case "whowon":
                    await ClientResultsAsync(int.Parse(content));
                    break;
                default:
                    throw new BasicBlankException($"Nothing for status {status}  with the message of {content}");
            }
        }
    }
}