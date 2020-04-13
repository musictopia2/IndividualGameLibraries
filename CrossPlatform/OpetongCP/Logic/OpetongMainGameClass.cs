using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.BasicDrawables.Dictionary;
using BasicGameFrameworkLibrary.BasicGameDataClasses;
using BasicGameFrameworkLibrary.CommandClasses;
using BasicGameFrameworkLibrary.CommonInterfaces;
using BasicGameFrameworkLibrary.DIContainers;
using BasicGameFrameworkLibrary.Extensions; //most likely will be used.
using BasicGameFrameworkLibrary.MultiplayerClasses.BasicGameClasses;
using BasicGameFrameworkLibrary.MultiplayerClasses.BasicPlayerClasses;
using BasicGameFrameworkLibrary.MultiplayerClasses.Extensions;
using BasicGameFrameworkLibrary.MultiplayerClasses.InterfaceMessages;
using BasicGameFrameworkLibrary.MultiplayerClasses.SavedGameClasses;
using BasicGameFrameworkLibrary.RegularDeckOfCards;
using BasicGameFrameworkLibrary.SpecializedGameTypes.RummyClasses;
using BasicGameFrameworkLibrary.TestUtilities;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.Messenging;
using OpetongCP.Data;
using System.Linq;
using System.Threading.Tasks; //most of the time, i will be using asyncs.

namespace OpetongCP.Logic
{
    [SingletonGame]
    public class OpetongMainGameClass : CardGameClass<RegularRummyCard, OpetongPlayerItem, OpetongSaveInfo>, IMiscDataNM
    {


        private readonly OpetongVMData _model;
        private readonly CommandContainer _command; //most of the time, needs this.  if not needed, take out.
        private readonly OpetongGameContainer _gameContainer; //if we don't need it, take it out.

        public OpetongMainGameClass(IGamePackageResolver mainContainer,
            IEventAggregator aggregator,
            BasicData basicData,
            TestOptions test,
            OpetongVMData currentMod,
            IMultiplayerSaveState state,
            IAsyncDelayer delay,
            ICardInfo<RegularRummyCard> cardInfo,
            CommandContainer command,
            OpetongGameContainer gameContainer)
            : base(mainContainer, aggregator, basicData, test, currentMod, state, delay, cardInfo, command, gameContainer)
        {
            _model = currentMod;
            _command = command;
            _gameContainer = gameContainer;
            _gameContainer.DrawFromPoolAsync = DrawFromPoolAsync;
        }
        
        public override Task FinishGetSavedAsync()
        {
            LoadControls();
            //_model!.TempSets!.ClearBoard();
            _model.MainSets!.ClearBoard(); //i think because its all gone.
            _model.Pool1!.LoadSavedGame(SaveRoot!.PoolList);
            int x = SaveRoot.SetList.Count; //the first time, actually load manually.
            x.Times(items =>
            {
                RummySet thisSet = new RummySet(_gameContainer);
                _model.MainSets.CreateNewSet(thisSet);
            });
            PlayerList!.ForEach(thisPlayer =>
            {
                if (thisPlayer.AdditionalCards.Count > 0)
                {
                    thisPlayer.MainHandList.AddRange(thisPlayer.AdditionalCards); //later sorts anyways.
                    thisPlayer.AdditionalCards.Clear(); //i think.
                }
            });
            SingleInfo = PlayerList.GetSelf(); //hopefully won't cause problems.
            SortCards(); //has to be this way this time.
            Aggregator.Subscribe(SingleInfo); //i think
            _model.MainSets.LoadSets(SaveRoot.SetList);
            return base.FinishGetSavedAsync();
        }
        public override async Task PopulateSaveRootAsync()
        {
            SaveRoot!.SetList = _model.MainSets.SavedSets();
            OpetongPlayerItem self = PlayerList!.GetSelf();
            self.AdditionalCards = _model.TempSets.ListAllObjects();
            SaveRoot.PoolList = _model.Pool1.ObjectList.ToRegularDeckDict();
            await base.PopulateSaveRootAsync();
        }
        private void LoadControls()
        {
            if (IsLoaded == true)
                return;

            IsLoaded = true; //i think needs to be here.
        }
        protected override async Task ComputerTurnAsync()
        {
            //if there is nothing, then just won't do anything.
            await Task.CompletedTask;
        }
        protected override Task LastPartOfSetUpBeforeBindingsAsync()
        {
            _model!.Deck1!.OriginalList(_gameContainer.DeckList!); //try this.
            if (IsLoaded == false)
            {
                LoadControls();
                SingleInfo = PlayerList!.GetSelf();
                Aggregator.Subscribe(SingleInfo);
            }
            _model.MainSets!.ClearBoard();
            SaveRoot!.SetList.Clear(); //i think this too.
            SaveRoot.FirstTurn = true;
            var tempCol = _model.Deck1.DrawSeveralCards(8);
            PlayerList!.ForEach(thisPlayer =>
            {
                thisPlayer.MainHandList.Clear();
                thisPlayer.SetsPlayed = 0;
                thisPlayer.TotalScore = 0;
            });
            _model.Pool1!.NewGame(tempCol);
            SaveRoot.ImmediatelyStartTurn = true;
            return base.LastPartOfSetUpBeforeBindingsAsync();
        }
        protected override void LinkHand()
        {
            SingleInfo = PlayerList!.GetSelf();
            _model!.PlayerHand1!.HandList = SingleInfo.MainHandList;
            PrepSort();
        }
        protected override Task StartSetUpAsync(bool isBeginning)
        {
            LoadControls();
            return base.StartSetUpAsync(isBeginning);
        }

        async Task IMiscDataNM.MiscDataReceived(string status, string content)
        {
            switch (status) //can't do switch because we don't know what the cases are ahead of time.
            {
                case "pool":
                    await DrawFromPoolAsync(int.Parse(content));
                    return;
                case "newset":
                    var thisList = await content.GetObjectsFromDataAsync(SingleInfo!.MainHandList); //hopefully this works too.
                    await PlaySetAsync(thisList);
                    return;
                default:
                    throw new BasicBlankException($"Nothing for status {status}  with the message of {content}");
            }
        }
        public override async Task StartNewTurnAsync()
        {
            await base.StartNewTurnAsync();
            SaveRoot!.WhichPart = 1;
            int nums = _model!.Pool1!.HowManyCardsNeeded;
            if (nums > 0)
            {
                var thisCol = _model.Deck1!.DrawSeveralCards(nums);
                _model.Pool1.ProcessNewCards(thisCol);
            }
            SingleInfo = PlayerList!.GetWhoPlayer();
            await ContinueTurnAsync(); //most of the time, continue turn.  can change to what is needed
        }
        public override async Task EndTurnAsync()
        {
            SingleInfo = PlayerList!.GetWhoPlayer();
            SingleInfo.MainHandList.UnhighlightObjects(); //i think this is best.
            _command.ManuelFinish = true; //because it could be somebody else's turn.
            _model.Instructions = "None";
            if (Test!.NoAnimations == false)
            {
                await Delay!.DelaySeconds(.5);
            }
            UnselectCards();
            WhoTurn = await PlayerList.CalculateWhoTurnAsync();
            await StartNewTurnAsync();
        }

        public override async Task ContinueTurnAsync()
        {
            if (SaveRoot!.FirstTurn)
                _model!.Instructions = "Make one move";
            else if (SaveRoot.WhichPart == 1)
                _model!.Instructions = "Make one of two moves";
            else
                _model!.Instructions = "Make your last move";
            await base.ContinueTurnAsync();
        }
        protected override async Task AfterDrawingAsync()
        {
            await ResumePlayAsync();
        }
        public async Task DrawFromPoolAsync(int deck)
        {
            if (SingleInfo!.CanSendMessage(BasicData!))
                await Network!.SendAllAsync("pool", deck);
            _model!.Pool1!.HideCard(deck);
            var thisCard = _gameContainer.DeckList!.GetSpecificItem(deck);
            thisCard.Drew = true;
            SingleInfo!.MainHandList.Add(thisCard);
            if (SingleInfo.PlayerCategory == EnumPlayerCategory.Self)
                SortCards();
            await ResumePlayAsync();
        }
        private void WhoWon()
        {
            SingleInfo = PlayerList.OrderByDescending(items => items.TotalScore).First();
        }
        private void UnselectCards()
        {
            if (SingleInfo!.PlayerCategory == EnumPlayerCategory.Self)
            {
                _model!.PlayerHand1!.EndTurn();
                _model.TempSets!.EndTurn();
            }
        }
        private async Task ResumePlayAsync()
        {
            if (SaveRoot!.FirstTurn)
            {
                SaveRoot.FirstTurn = false;
                await EndTurnAsync();
                return;
            }
            SaveRoot.WhichPart++;
            if (SaveRoot.WhichPart == 3)
            {
                await EndTurnAsync();
                return;
            }
            await ContinueTurnAsync();
        }
        private int ScoreOnBoard(int player)
        {
            var thisList = _model!.MainSets!.SetList;
            int output = 0;
            thisList.ForEach(thisSet => output += thisSet.CalculateScore(player));
            return output;
        }
        private void CalculateScores()
        {
            PlayerList!.ForEach(thisPlayer =>
            {
                thisPlayer.TotalScore = ScoreOnBoard(thisPlayer.Id) - thisPlayer.ObjectCount;
            });
        }
        private async Task GameOverAsync()
        {
            _model!.Instructions = "None";
            CalculateScores();
            WhoWon();
            await ShowWinAsync();
        }
        public async Task PlaySetAsync(IDeckDict<RegularRummyCard> thisCol)
        {
            RummySet thisSet = new RummySet(_gameContainer);
            thisSet.CreateNewSet(thisCol);
            _model!.MainSets!.CreateNewSet(thisSet);
            SingleInfo!.SetsPlayed++;
            if (SingleInfo.SetsPlayed == 4 || Test!.ImmediatelyEndGame)
            {
                await GameOverAsync();
                return;
            }
            await ResumePlayAsync();
        }
        private bool HasSet(DeckRegularDict<RegularRummyCard> thisCol)
        {
            var newCol = thisCol.ToRegularDeckDict();
            if (thisCol.Count < 2 || thisCol.Count > 4)
                return false;
            if (thisCol.Count == 2)
                return _gameContainer.Rummys!.IsNewRummy(newCol, 2, RummyProcesses<EnumSuitList, EnumColorList, RegularRummyCard>.EnumRummyType.Sets);
            bool rets;
            rets = _gameContainer.Rummys!.IsNewRummy(newCol, thisCol.Count, RummyProcesses<EnumSuitList, EnumColorList, RegularRummyCard>.EnumRummyType.Sets);
            if (rets == true)
                return true;
            rets = _gameContainer.Rummys.IsNewRummy(newCol, thisCol.Count, RummyProcesses<EnumSuitList, EnumColorList, RegularRummyCard>.EnumRummyType.Runs);
            if (rets == true)
                return true;
            rets = _gameContainer.Rummys.IsNewRummy(newCol, thisCol.Count, RummyProcesses<EnumSuitList, EnumColorList, RegularRummyCard>.EnumRummyType.Colors);
            return rets;
        }
        public int FindValidSet()
        {
            for (int x = 1; x <= 3; x++)
            {
                var thisCollection = _model!.TempSets!.ObjectList(x).ToRegularDeckDict();
                if (HasSet(thisCollection))
                    return x;
            }
            return 0;
        }

    }
}