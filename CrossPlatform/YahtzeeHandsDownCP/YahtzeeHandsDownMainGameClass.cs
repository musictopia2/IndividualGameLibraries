using BasicGameFramework.Attributes;
using BasicGameFramework.BasicDrawables.Dictionary;
using BasicGameFramework.DIContainers;
using BasicGameFramework.Extensions;
using BasicGameFramework.MultiplayerClasses.BasicGameClasses;
using BasicGameFramework.MultiplayerClasses.InterfaceMessages;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.Exceptions;
using System.Linq;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using js = CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.JsonSerializers.NewtonJsonStrings; //just in case i need those 2.
namespace YahtzeeHandsDownCP
{
    [SingletonGame]
    public class YahtzeeHandsDownMainGameClass : CardGameClass<YahtzeeHandsDownCardInformation, YahtzeeHandsDownPlayerItem, YahtzeeHandsDownSaveInfo>, IMiscDataNM
    {
        private bool _wasStarted;
        private readonly CalculateYahtzeeCombinationClass _yatz = new CalculateYahtzeeCombinationClass();
        private DeckRegularDict<ChanceCardInfo> _chanceList = new DeckRegularDict<ChanceCardInfo>();
        public YahtzeeHandsDownMainGameClass(IGamePackageResolver container) : base(container) { }
        private YahtzeeHandsDownViewModel? _thisMod;
        public override void Init() //decided to have all the code under init to prevent overflow issues.
        {
            base.Init();
            _thisMod = MainContainer.Resolve<YahtzeeHandsDownViewModel>();
        }
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
                if (ThisTest!.DoubleCheck == false || x == 6)
                    tempList.Add(thisCard);
            });

            _thisMod!.ComboHandList!.HandList.ReplaceRange(tempList);
            AlreadyDrew = false;
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
            SaveRoot!.Combos = _thisMod!.ComboHandList!.HandList.GetDeckListFromObjectList();
            SaveRoot.ChanceList = _chanceList.GetDeckListFromObjectList();
            return base.PopulateSaveRootAsync();
        }
        protected override Task LastPartOfSetUpBeforeBindingsAsync()
        {
            if (ThisTest!.DoubleCheck)
            {
                var thisList = _thisMod!.Deck1!.DrawSeveralCards(72);
                _thisMod.Pile1!.AddSeveralCards(thisList);
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
            _thisMod!.ComboHandList!.HandList.ReplaceRange(tempList);
        }
        async Task IMiscDataNM.MiscDataReceived(string status, string content)
        {
            switch (status)
            {
                case "replacecards":
                    var thisList = await js.DeserializeObjectAsync<CustomBasicList<int>>(content);
                    var nextList = thisList.GetNewObjectListFromDeckList(DeckList!);
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
            _thisMod!.CommandContainer!.ManuelFinish = true; //because it could be somebody else's turn.
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
                AlreadyDrew = true;
            }
            else
            {
                AlreadyDrew = false;
                _wasStarted = false;
            }
            var tempPlayer = PlayerList!.GetSelf();
            if (tempPlayer.MainHandList.Any(items => items.IsUnknown == true))
                throw new BasicBlankException("Can't have any unknown cards in hand after drawing");
            int lefts = _thisMod!.Deck1!.CardsLeft();
            if (ThisTest!.DoubleCheck)
            {
                var thisList = _thisMod.Deck1.DrawSeveralCards(lefts - 3);
                _thisMod.Pile1!.AddSeveralCards(thisList);
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
                if (ThisTest!.NoAnimations == false)
                    await Delay!.DelaySeconds(.25);
            });
            await DrawAsync();
        }
        public CustomBasicList<YahtzeeResults> GetResults()
        {
            var tempComboList = _thisMod!.ComboHandList!.HandList.ToRegularDeckDict();
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
            if (_thisMod!.ComboHandList!.HandList.Count == 0)
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
            var tempList = _thisMod!.ComboHandList!.HandList.ToRegularDeckDict();
            var thisCombo = tempList.Single(items => items.Points == thisResults.Points);
            _thisMod.ComboHandList.HandList.ReplaceAllWithGivenItem(thisCombo); //hopefully this simple.
            bool rets;
            if (SaveRoot!.FirstPlayerWentOut == 0)
            {
                rets = true;
                SingleInfo.InGame = false;
                await _thisMod.ShowGameMessageAsync($"{SingleInfo.NickName} first went out");
                SaveRoot.FirstPlayerWentOut = WhoTurn;
            }
            else
            {
                rets = false;
                if (ThisTest!.NoAnimations == false)
                    await Delay!.DelaySeconds(2);
            }
            _thisMod.ComboHandList.HandList.ReplaceRange(tempList);
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
            await _thisMod!.ShowGameMessageAsync($"{thisPlayer.NickName} has won the hand");
            thisPlayer.WonLastRound = "Yes";
            thisPlayer.ScoreRound = thisPlayer.Results.Points;
            thisPlayer.TotalScore += thisPlayer.Results.Points;
            var thisCombo = _thisMod!.ComboHandList!.HandList.Single(items => items.Points == thisPlayer.Results.Points);
            _thisMod.ComboHandList.HandList.RemoveSpecificItem(thisCombo);
            async Task AnimateRemovalOfCardsAsync()
            {
                var finList = thisPlayer.MainHandList.ToRegularDeckDict();
                await finList.ForEachAsync(async thisCard =>
                {
                    thisPlayer.MainHandList.RemoveObjectByDeck(thisCard.Deck);
                    await AnimatePlayAsync(thisCard);
                    if (ThisTest!.NoAnimations == false)
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
                _thisMod.ChancePile!.AddCard(thisChance);
                _thisMod.ChancePile.Visible = true;
                thisPlayer.ScoreRound += thisChance.Points;
                thisPlayer.TotalScore += thisChance.Points;
                await AnimateRemovalOfCardsAsync();
                _thisMod.ChancePile.Visible = false;
            }
            await FinishPartRoundAsync();
        }
    }
}