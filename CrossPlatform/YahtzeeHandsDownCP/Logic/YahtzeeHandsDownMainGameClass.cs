using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.BasicDrawables.Dictionary;
using BasicGameFrameworkLibrary.BasicGameDataClasses;
using BasicGameFrameworkLibrary.CommandClasses;
using BasicGameFrameworkLibrary.CommonInterfaces;
using BasicGameFrameworkLibrary.DIContainers;
using BasicGameFrameworkLibrary.Extensions; //most likely will be used.
using BasicGameFrameworkLibrary.MultiplayerClasses.BasicGameClasses;
using BasicGameFrameworkLibrary.MultiplayerClasses.InterfaceMessages;
using BasicGameFrameworkLibrary.MultiplayerClasses.SavedGameClasses;
using BasicGameFrameworkLibrary.TestUtilities;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.Messenging;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using System.Linq;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using YahtzeeHandsDownCP.Cards;
using YahtzeeHandsDownCP.Data;
using js = CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.JsonSerializers.NewtonJsonStrings; //just in case i need those 2.

namespace YahtzeeHandsDownCP.Logic
{
    [SingletonGame]
    public class YahtzeeHandsDownMainGameClass : CardGameClass<YahtzeeHandsDownCardInformation, YahtzeeHandsDownPlayerItem, YahtzeeHandsDownSaveInfo>, IMiscDataNM
    {


        private readonly YahtzeeHandsDownVMData _model;
        private readonly CommandContainer _command; //most of the time, needs this.  if not needed, take out.
        private readonly YahtzeeHandsDownGameContainer _gameContainer; //if we don't need it, take it out.

        public YahtzeeHandsDownMainGameClass(IGamePackageResolver mainContainer,
            IEventAggregator aggregator,
            BasicData basicData,
            TestOptions test,
            YahtzeeHandsDownVMData currentMod,
            IMultiplayerSaveState state,
            IAsyncDelayer delay,
            ICardInfo<YahtzeeHandsDownCardInformation> cardInfo,
            CommandContainer command,
            YahtzeeHandsDownGameContainer gameContainer)
            : base(mainContainer, aggregator, basicData, test, currentMod, state, delay, cardInfo, command, gameContainer)
        {
            _model = currentMod;
            _command = command;
            _gameContainer = gameContainer;
        }
        private bool _wasStarted;
        private readonly CalculateYahtzeeCombinationClass _yatz = new CalculateYahtzeeCombinationClass();
        private DeckRegularDict<ChanceCardInfo> _chanceList = new DeckRegularDict<ChanceCardInfo>();

        public override Task FinishGetSavedAsync()
        {
            LoadControls();
            HookUpCombo();
            var firstList = SaveRoot!.ChanceList.Select(items =>
            {
                var temps = new ChanceCardInfo();
                temps.Populate(items);
                return temps;
            });
            _chanceList = new DeckRegularDict<ChanceCardInfo>(firstList); //hopefully this works too.
            return base.FinishGetSavedAsync();
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
        protected override Task StartSetUpAsync(bool isBeginning)
        {
            LoadControls();
            PlayerList!.ForEach(thisPlayer =>
            {
                thisPlayer.WonLastRound = "";
                thisPlayer.ScoreRound = 0;
                thisPlayer.TotalScore = 0;
            });
            DeckRegularDict<ComboCardInfo> tempList = new DeckRegularDict<ComboCardInfo>();
            6.Times(x =>
            {
                ComboCardInfo thisCard = new ComboCardInfo();
                thisCard.Populate(x);
                if (Test!.DoubleCheck == false || x == 6)
                    tempList.Add(thisCard);
            });

            _model!.ComboHandList!.HandList.ReplaceRange(tempList);
            _gameContainer.AlreadyDrew = false;
            SaveRoot!.ExtraTurns = 0;
            SaveRoot.FirstPlayerWentOut = 0;
            _chanceList.Clear();
            12.Times(x =>
            {
                ChanceCardInfo thisCard = new ChanceCardInfo();
                thisCard.Populate(x);
                _chanceList.Add(thisCard);
            });
            _chanceList.ShuffleList();
            return base.StartSetUpAsync(isBeginning);
        }
        public override Task PopulateSaveRootAsync()
        {
            SaveRoot!.Combos = _model!.ComboHandList!.HandList.GetDeckListFromObjectList();
            SaveRoot.ChanceList = _chanceList.GetDeckListFromObjectList();
            return base.PopulateSaveRootAsync();
        }
        protected override Task LastPartOfSetUpBeforeBindingsAsync()
        {
            if (Test!.DoubleCheck)
            {
                var thisList = _model!.Deck1!.DrawSeveralCards(72);
                _model.Pile1!.AddSeveralCards(thisList);
            }
            return base.LastPartOfSetUpBeforeBindingsAsync();
        }
        private void HookUpCombo()
        {
            var tempList = SaveRoot!.Combos.Select(items =>
            {
                var newItem = new ComboCardInfo();
                newItem.Populate(items);
                return newItem;
            });
            _model!.ComboHandList!.HandList.ReplaceRange(tempList);
        }
        async Task IMiscDataNM.MiscDataReceived(string status, string content)
        {
            switch (status) //can't do switch because we don't know what the cases are ahead of time.
            {
                case "replacecards":
                    var thisList = await js.DeserializeObjectAsync<CustomBasicList<int>>(content);
                    var nextList = thisList.GetNewObjectListFromDeckList(_gameContainer.DeckList!);
                    await ReplaceCardsAsync(nextList);
                    return;
                case "wentout":
                    var thisItem = await js.DeserializeObjectAsync<YahtzeeResults>(content);
                    await PlayerGoesOutAsync(thisItem);
                    return;
                default:
                    throw new BasicBlankException($"Nothing for status {status}  with the message of {content}");
            }
        }
        public override async Task StartNewTurnAsync()
        {
            await base.StartNewTurnAsync();
            SingleInfo = PlayerList!.GetWhoPlayer();
            if (PlayerList.Count() == 2 && SaveRoot!.FirstPlayerWentOut > 0)
                SaveRoot.ExtraTurns++;
            if (SingleInfo.MainHandList.Count == 0)
            {
                LeftToDraw = 5;
                _wasStarted = true;
                await DrawAsync();
                return;
            }
            _wasStarted = false;
            await ContinueTurnAsync(); //most of the time, continue turn.  can change to what is needed
        }
        public override async Task EndTurnAsync()
        {
            SingleInfo = PlayerList!.GetWhoPlayer();
            SingleInfo.MainHandList.UnhighlightObjects(); //i think this is best.
            _command.ManuelFinish = true; //because it could be somebody else's turn.
            if (SaveRoot!.ExtraTurns == 4)
            {
                await SecondPlayerDidNotGoOutAsync();
                return;
            }
            WhoTurn = await PlayerList.CalculateWhoTurnAsync(true);
            await StartNewTurnAsync();
        }
        protected override async Task AfterDrawingAsync()
        {
            if (SingleInfo!.MainHandList.Count != 5)
                throw new BasicBlankException("Must have only 5 cards left");
            if (_wasStarted == false)
            {
                _gameContainer.AlreadyDrew = true;
            }
            else
            {
                _gameContainer.AlreadyDrew = false;
                _wasStarted = false;
            }
            var tempPlayer = PlayerList!.GetSelf();
            if (tempPlayer.MainHandList.Any(items => items.IsUnknown == true))
                throw new BasicBlankException("Can't have any unknown cards in hand after drawing");
            int lefts = _model!.Deck1!.CardsLeft();
            if (Test!.DoubleCheck)
            {
                var thisList = _model.Deck1.DrawSeveralCards(lefts - 3);
                _model.Pile1!.AddSeveralCards(thisList);
            }

            await base.AfterDrawingAsync();
        }
        public async Task ReplaceCardsAsync(IDeckDict<YahtzeeHandsDownCardInformation> thisList)
        {
            LeftToDraw = thisList.Count;
            PlayerDraws = WhoTurn;
            await thisList.ForEachAsync(async thisCard =>
            {
                thisCard.Drew = false;
                thisCard.IsSelected = false;
                SingleInfo!.MainHandList.RemoveObjectByDeck(thisCard.Deck);
                await AnimatePlayAsync(thisCard);
                if (Test!.NoAnimations == false)
                    await Delay!.DelaySeconds(.25);
            });
            await DrawAsync();
        }
        public CustomBasicList<YahtzeeResults> GetResults()
        {
            var tempComboList = _model!.ComboHandList!.HandList.ToRegularDeckDict();
            var otherList = SingleInfo!.MainHandList.GetInterfaceList();
            return _yatz.GetResults(tempComboList, otherList);
        }
        private async Task SecondPlayerDidNotGoOutAsync()
        {
            if (PlayerList.Count() > 2)
                throw new BasicBlankException("If there are more than 2 players; then a second person must go out");
            await ScoreRoundAsync();
        }
        private async Task FinishPartRoundAsync()
        {
            if (_model!.ComboHandList!.HandList.Count == 0)
            {
                SingleInfo = PlayerList.OrderByDescending(items => items.TotalScore).First();
                await ShowWinAsync();
                return;
            }
            PlayerList!.ForEach(thisPlayer =>
            {
                thisPlayer.InGame = true;
                thisPlayer.Results = new YahtzeeResults();
            });
            SaveRoot!.ExtraTurns = 0;
            SaveRoot.FirstPlayerWentOut = 0;
            int tempGoes = WhoStarts;
            WhoTurn = WhoStarts;
            WhoTurn = await PlayerList.CalculateWhoTurnAsync();
            if (WhoTurn == tempGoes)
                throw new BasicBlankException("The same player is going first.  Find out what happened");
            if (WhoTurn == 0)
                throw new BasicBlankException("WhoTurn cannot be 0 at the end of yahtzee hands down.  Rethink");
            await StartNewTurnAsync();
        }
        public async Task PlayerGoesOutAsync(YahtzeeResults thisResults)
        {
            SingleInfo!.Results = thisResults;
            var tempList = _model!.ComboHandList!.HandList.ToRegularDeckDict();
            var thisCombo = tempList.Single(items => items.Points == thisResults.Points);
            _model.ComboHandList.HandList.ReplaceAllWithGivenItem(thisCombo); //hopefully this simple.
            bool rets;
            if (SaveRoot!.FirstPlayerWentOut == 0)
            {
                rets = true;
                SingleInfo.InGame = false;
                await UIPlatform.ShowMessageAsync($"{SingleInfo.NickName} first went out");
                SaveRoot.FirstPlayerWentOut = WhoTurn;
            }
            else
            {
                rets = false;
                if (Test!.NoAnimations == false)
                    await Delay!.DelaySeconds(2);
            }
            _model.ComboHandList.HandList.ReplaceRange(tempList);
            if (rets == true)
            {
                await EndTurnAsync();
                return;
            }
            await ScoreRoundAsync();
        }
        private async Task ScoreRoundAsync()
        {
            var thisList = PlayerList.Where(items => items.Results.Points > 0).ToCustomBasicList();
            if (thisList.Count > 2)
                throw new BasicBlankException("Can only have 2 to compare with for scoring");
            var tempList = thisList.Select(items => items.Results).ToCustomBasicList();
            var whoWon = _yatz.WhoWon(tempList);
            var thisPlayer = thisList[whoWon - 1];
            await UIPlatform.ShowMessageAsync($"{thisPlayer.NickName} has won the hand");
            thisPlayer.WonLastRound = "Yes";
            thisPlayer.ScoreRound = thisPlayer.Results.Points;
            thisPlayer.TotalScore += thisPlayer.Results.Points;
            var thisCombo = _model!.ComboHandList!.HandList.Single(items => items.Points == thisPlayer.Results.Points);
            _model.ComboHandList.HandList.RemoveSpecificItem(thisCombo);
            async Task AnimateRemovalOfCardsAsync()
            {
                var finList = thisPlayer.MainHandList.ToRegularDeckDict();
                await finList.ForEachAsync(async thisCard =>
                {
                    thisPlayer.MainHandList.RemoveObjectByDeck(thisCard.Deck);
                    await AnimatePlayAsync(thisCard);
                    if (Test!.NoAnimations == false)
                        await Delay!.DelaySeconds(.5);
                });
            };
            await AnimateRemovalOfCardsAsync();
            var pList = PlayerList.ToCustomBasicList();
            pList.RemoveSpecificItem(thisPlayer);
            pList.ForEach(tempPlayer =>
            {
                tempPlayer.WonLastRound = "No";
                tempPlayer.ScoreRound = 0;
            });
            if (tempList.Count > 1)
            {
                if (whoWon == 1)
                    thisPlayer = thisList.Last();
                else
                    thisPlayer = thisList.First();
                var thisChance = _chanceList.First();
                _chanceList.RemoveSpecificItem(thisChance);
                _model.ChancePile!.AddCard(thisChance);
                _model.ChancePile.Visible = true;
                thisPlayer.ScoreRound += thisChance.Points;
                thisPlayer.TotalScore += thisChance.Points;
                await AnimateRemovalOfCardsAsync();
                _model.ChancePile.Visible = false;
            }
            await FinishPartRoundAsync();
        }
    }
}
