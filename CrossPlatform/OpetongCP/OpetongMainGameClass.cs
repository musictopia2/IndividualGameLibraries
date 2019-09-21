using BasicGameFramework.Attributes;
using BasicGameFramework.BasicDrawables.Dictionary;
using BasicGameFramework.DIContainers;
using BasicGameFramework.Extensions;
using BasicGameFramework.MultiplayerClasses.BasicGameClasses;
using BasicGameFramework.MultiplayerClasses.BasicPlayerClasses;
using BasicGameFramework.MultiplayerClasses.Extensions;
using BasicGameFramework.MultiplayerClasses.InterfaceMessages;
using BasicGameFramework.RegularDeckOfCards;
using BasicGameFramework.SpecializedGameTypes.RummyClasses;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using CommonBasicStandardLibraries.Exceptions;
using System.Linq;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace OpetongCP
{
    [SingletonGame]
    public class OpetongMainGameClass : CardGameClass<RegularRummyCard, OpetongPlayerItem, OpetongSaveInfo>, IMiscDataNM
    {
        public OpetongMainGameClass(IGamePackageResolver container) : base(container) { }

        internal OpetongViewModel? ThisMod;
        internal RummyProcesses<EnumSuitList, EnumColorList, RegularRummyCard>? Rummys;

        public override void Init() //decided to have all the code under init to prevent overflow issues.
        {
            base.Init();
            ThisMod = MainContainer.Resolve<OpetongViewModel>(); //risk doing here.  if it pays off, then do this instead.
            Rummys = new RummyProcesses<EnumSuitList, EnumColorList, RegularRummyCard>();
            Rummys.HasSecond = true;
            Rummys.HasWild = false;
            Rummys.LowNumber = 1;
            Rummys.HighNumber = 13;
            Rummys.NeedMatch = false;
        }

        public override Task FinishGetSavedAsync()
        {
            LoadControls();
            ThisMod!.TempSets!.ClearBoard();
            ThisMod.MainSets!.ClearBoard(); //i think because its all gone.
            ThisMod.Pool1!.LoadSavedGame(SaveRoot!.PoolList);
            int x = SaveRoot.SetList.Count; //the first time, actually load manually.
            x.Times(items =>
            {
                RummySet thisSet = new RummySet(ThisMod);
                ThisMod.MainSets.CreateNewSet(thisSet);
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
            ThisE.Subscribe(SingleInfo); //i think
            ThisMod.MainSets.LoadSets(SaveRoot.SetList);
            return base.FinishGetSavedAsync();
        }
        public override async Task PopulateSaveRootAsync()
        {
            SaveRoot!.SetList = ThisMod!.MainSets!.SavedSets();
            OpetongPlayerItem self = PlayerList!.GetSelf();
            self.AdditionalCards = ThisMod.TempSets!.ListAllObjects();
            SaveRoot.PoolList = ThisMod.Pool1!.ObjectList.ToRegularDeckDict();
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
            await Task.CompletedTask;
        }
        protected override Task LastPartOfSetUpBeforeBindingsAsync()
        {
            ThisMod!.Deck1!.OriginalList(DeckList!); //try this.
            if (IsLoaded == false)
            {
                LoadControls();
                SingleInfo = PlayerList!.GetSelf();
                ThisE.Subscribe(SingleInfo);
            }
            ThisMod.MainSets!.ClearBoard();
            SaveRoot!.SetList.Clear(); //i think this too.
            ThisMod.TempSets!.ClearBoard();
            SaveRoot.FirstTurn = true;
            var tempCol = ThisMod.Deck1.DrawSeveralCards(8);
            PlayerList!.ForEach(thisPlayer =>
            {
                thisPlayer.MainHandList.Clear();
                thisPlayer.SetsPlayed = 0;
                thisPlayer.TotalScore = 0;
            });
            ThisMod.Pool1!.NewGame(tempCol);
            SaveRoot.ImmediatelyStartTurn = true;
            return base.LastPartOfSetUpBeforeBindingsAsync();
        }
        protected override void LinkHand()
        {
            SingleInfo = PlayerList!.GetSelf();
            ThisMod!.PlayerHand1!.HandList = SingleInfo.MainHandList;
            PrepSort();
        }
        protected override Task StartSetUpAsync(bool isBeginning)
        {
            LoadControls();
            return base.StartSetUpAsync(isBeginning);
        }

        async Task IMiscDataNM.MiscDataReceived(string status, string content)
        {
            switch (status)
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
        public override async Task EndTurnAsync()
        {
            SingleInfo = PlayerList!.GetWhoPlayer();
            ThisMod!.CommandContainer!.ManuelFinish = true; //because it could be somebody else's turn.
            ThisMod.Instructions = "None";
            if (ThisTest!.NoAnimations == false)
                await Delay!.DelaySeconds(.5);
            UnselectCards();
            WhoTurn = await PlayerList.CalculateWhoTurnAsync();
            await StartNewTurnAsync();
        }
        public async override Task StartNewTurnAsync()
        {
            await base.StartNewTurnAsync();
            SaveRoot!.WhichPart = 1;
            int nums = ThisMod!.Pool1!.HowManyCardsNeeded;
            if (nums > 0)
            {
                var thisCol = ThisMod.Deck1!.DrawSeveralCards(nums);
                ThisMod.Pool1.ProcessNewCards(thisCol);
            }
            SingleInfo = PlayerList!.GetWhoPlayer();
            await ContinueTurnAsync();
        }
        public override async Task ContinueTurnAsync()
        {
            if (SaveRoot!.FirstTurn)
                ThisMod!.Instructions = "Make one move";
            else if (SaveRoot.WhichPart == 1)
                ThisMod!.Instructions = "Make one of two moves";
            else
                ThisMod!.Instructions = "Make your last move";
            await base.ContinueTurnAsync();
        }
        protected override async Task AfterDrawingAsync()
        {
            await ResumePlayAsync();
        }
        public async Task DrawFromPoolAsync(int deck)
        {
            if (SingleInfo!.CanSendMessage(ThisData!))
                await ThisNet!.SendAllAsync("pool", deck);
            ThisMod!.Pool1!.HideCard(deck);
            var thisCard = DeckList!.GetSpecificItem(deck);
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
                ThisMod!.PlayerHand1!.EndTurn();
                ThisMod.TempSets!.EndTurn();
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
            var thisList = ThisMod!.MainSets!.SetList;
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
            ThisMod!.Instructions = "None";
            CalculateScores();
            WhoWon();
            await ShowWinAsync();
        }
        public async Task PlaySetAsync(IDeckDict<RegularRummyCard> thisCol)
        {
            RummySet thisSet = new RummySet(ThisMod!);
            thisSet.CreateNewSet(thisCol);
            ThisMod!.MainSets!.CreateNewSet(thisSet);
            SingleInfo!.SetsPlayed++;
            if (SingleInfo.SetsPlayed == 4)
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
                return Rummys!.IsNewRummy(newCol, 2, RummyProcesses<EnumSuitList, EnumColorList, RegularRummyCard>.EnumRummyType.Sets);
            bool rets;
            rets = Rummys!.IsNewRummy(newCol, thisCol.Count, RummyProcesses<EnumSuitList, EnumColorList, RegularRummyCard>.EnumRummyType.Sets);
            if (rets == true)
                return true;
            rets = Rummys.IsNewRummy(newCol, thisCol.Count, RummyProcesses<EnumSuitList, EnumColorList, RegularRummyCard>.EnumRummyType.Runs);
            if (rets == true)
                return true;
            rets = Rummys.IsNewRummy(newCol, thisCol.Count, RummyProcesses<EnumSuitList, EnumColorList, RegularRummyCard>.EnumRummyType.Colors);
            return rets;
        }
        public int FindValidSet()
        {
            for (int x = 1; x <= 3; x++)
            {
                var thisCollection = ThisMod!.TempSets!.ObjectList(x).ToRegularDeckDict();
                if (HasSet(thisCollection))
                    return x;
            }
            return 0;
        }
    }

    //most did not even use this one. FinishStartAsync
}