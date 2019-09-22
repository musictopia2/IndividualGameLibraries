using BasicGameFramework.Attributes;
using BasicGameFramework.BasicDrawables.Dictionary;
using BasicGameFramework.DIContainers;
using BasicGameFramework.Extensions;
using BasicGameFramework.MultiplayerClasses.BasicGameClasses;
using BasicGameFramework.MultiplayerClasses.BasicPlayerClasses;
using BasicGameFramework.MultiplayerClasses.Extensions;
using BasicGameFramework.MultiplayerClasses.InterfaceMessages;
using BasicGameFramework.MultiplayerClasses.InterfacesForHelpers;
using BasicGameFramework.RegularDeckOfCards;
using BasicGameFramework.SpecializedGameTypes.RummyClasses;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using CommonBasicStandardLibraries.Exceptions;
using System.Linq;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using js = CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.JsonSerializers.NewtonJsonStrings; //just in case i need those 2.

namespace Rummy500CP
{
    [SingletonGame]
    public class Rummy500MainGameClass : CardGameClass<RegularRummyCard, Rummy500PlayerItem, Rummy500SaveInfo>, IMiscDataNM, IStartNewGame
    {
        public Rummy500MainGameClass(IGamePackageResolver container) : base(container) { }

        private Rummy500ViewModel? _thisMod;
        private RummyProcesses<EnumSuitList, EnumColorList, RegularRummyCard>? _rummys;
        public override void Init() //decided to have all the code under init to prevent overflow issues.
        {
            base.Init();
            _thisMod = MainContainer.Resolve<Rummy500ViewModel>();
        }
        public override Task FinishGetSavedAsync()
        {
            LoadControls();
            _thisMod!.MainSets1!.ClearBoard(); //i think because its all gone.
            int x = SaveRoot!.SetList.Count; //the first time, actually load manually.
            x.Times(items =>
            {
                RummySet thisSet = new RummySet(_thisMod);
                _thisMod.MainSets1.CreateNewSet(thisSet);
            });
            _thisMod.MainSets1.LoadSets(SaveRoot.SetList);
            _thisMod.DiscardList1!.HandList.ReplaceRange(SaveRoot.DiscardList);
            return base.FinishGetSavedAsync();
        }
        public override Task PopulateSaveRootAsync()
        {
            SaveRoot!.SetList = _thisMod!.MainSets1!.SavedSets();
            SaveRoot.DiscardList = _thisMod.DiscardList1!.HandList.ToRegularDeckDict();
            return base.PopulateSaveRootAsync();
        }
        private void LoadControls()
        {
            if (IsLoaded == true)
                return;
            _rummys = new RummyProcesses<EnumSuitList, EnumColorList, RegularRummyCard>();
            _rummys.HasSecond = true;
            _rummys.HasWild = false;
            _rummys.LowNumber = 1;
            _rummys.HighNumber = 14; //try this way (?)
            _rummys.NeedMatch = true;
            IsLoaded = true; //i think needs to be here.
        }
        protected override Task StartSetUpAsync(bool isBeginning)
        {
            LoadControls();
            _thisMod!.DiscardList1!.ClearDiscardList();
            _thisMod.MainSets1!.ClearBoard();
            SaveRoot!.MoreThanOne = false;
            PlayerList!.ForEach(thisPlayer =>
            {
                thisPlayer.CurrentScore = 0;
                thisPlayer.CardsPlayed = 0;
                thisPlayer.PointsPlayed = 0;
            });
            return base.StartSetUpAsync(isBeginning);
        }
        async Task IMiscDataNM.MiscDataReceived(string status, string content)
        {
            switch (status)
            {
                case "pickupfromdiscard":
                    await PickupFromDiscardAsync(int.Parse(content));
                    break;
                case "addtoset":
                    SendAddSet sendAdd = await js.DeserializeObjectAsync<SendAddSet>(content);
                    await AddToSetAsync(sendAdd.Index, sendAdd.Deck, sendAdd.Position);
                    break;
                case "newset":
                    SendNewSet sendNew = await js.DeserializeObjectAsync<SendNewSet>(content);
                    var newList = sendNew.DeckList.GetNewObjectListFromDeckList(DeckList!);
                    await CreateNewSetAsync(newList, sendNew.SetType, sendNew.UseSecond);
                    break;
                default:
                    throw new BasicBlankException($"Nothing for status {status}  with the message of {content}");
            }
        }
        Task IStartNewGame.ResetAsync()
        {
            PlayerList!.ForEach(thisPlayer =>
            {
                thisPlayer.CurrentScore = 0;
                thisPlayer.TotalScore = 0;
            });
            return Task.CompletedTask;
        }
        public bool CanProcessDiscard(out bool pickUp, ref int deck, out string message)
        {
            message = "";
            if (AlreadyDrew == false && PreviousCard == 0)
                pickUp = true;
            else
                pickUp = false;
            var thisCol = _thisMod!.PlayerHand1!.ListSelectedObjects();
            if (pickUp == true)
            {
                if (thisCol.Count > 0)
                {
                    message = "There is at least one card selected.  Unselect all the cards before picking up from discard";
                    return false;
                }
                return true;
            }
            if (thisCol.Count == 0)
            {
                message = "Sorry, you must select a card to discard";
                return false;
            }
            if (thisCol.Count > 1)
            {
                message = "Sorry, you can only select one card to discard";
                return false;
            }
            if (SaveRoot!.MoreThanOne == true)
            {
                if (SingleInfo!.MainHandList.Any(items => items.Deck == PreviousCard))
                {
                    message = "Sorry, since you picked up more than one card from the discard pile, the one clicked must be used in a set";
                    return false;
                }
            }
            if (thisCol.First().Deck == PreviousCard)
            {
                message = "Sorry, you cannot discard the one that was picked up since there is more than one card to choose from";
                return false;
            }
            deck = thisCol.First().Deck;
            return true;
        }
        public override async Task DiscardAsync(RegularRummyCard thisCard)
        {
            SingleInfo!.MainHandList.RemoveObjectByDeck(thisCard.Deck);
            _thisMod!.DiscardList1!.AddToDiscard(thisCard);
            await EndTurnAsync();
        }
        public override async Task StartNewTurnAsync()
        {
            await base.StartNewTurnAsync();
            SaveRoot!.MoreThanOne = false;
            await ContinueTurnAsync(); //most of the time, continue turn.  can change to what is needed
        }
        public override async Task EndTurnAsync()
        {
            SingleInfo = PlayerList!.GetWhoPlayer();
            SingleInfo.MainHandList.UnhighlightObjects(); //i think this is best.
            _thisMod!.MainSets1!.EndTurn();
            _thisMod.CommandContainer!.ManuelFinish = true; //because it could be somebody else's turn.
            if (SingleInfo.PlayerCategory != EnumPlayerCategory.Computer)
            {
                if (SingleInfo.MainHandList.Count == 0 || _thisMod!.Deck1!.IsEndOfDeck() == true)
                {
                    await EndRoundAsync();
                    return;
                }
            }
            WhoTurn = await PlayerList.CalculateWhoTurnAsync();
            await StartNewTurnAsync();
        }
        protected override Task LastPartOfSetUpBeforeBindingsAsync()
        {
            _thisMod!.DiscardList1!.AddToDiscard(_thisMod.Deck1!.DrawCard());
            return base.LastPartOfSetUpBeforeBindingsAsync();
        }
        public override async Task EndRoundAsync()
        {
            CalculatePoints();
            if (IsGameOver() == true)
            {
                await ShowWinAsync();
                return;
            }
            this.RoundOverNext();
        }
        private void CalculatePoints()
        {
            PlayerList!.ForEach(thisPlayer =>
            {
                var thisPoint = CalculatePoints(thisPlayer.Id);
                int negs = thisPlayer.MainHandList.Sum(Items => Items.NegativePoints());
                thisPlayer.CurrentScore = thisPoint.Points + negs; //this will already be negative.
                thisPlayer.TotalScore += thisPlayer.CurrentScore;
            });
        }
        private PointInfo CalculatePoints(int player)
        {
            PointInfo output = new PointInfo();
            PointInfo temps;
            _thisMod!.MainSets1!.SetList.ForEach(thisSet =>
            {
                temps = thisSet.GetPointInfo(player);
                output.Points += temps.Points;
                output.NumberOfCards += temps.NumberOfCards;
            });
            return output;
        }
        private bool IsGameOver()
        {
            SingleInfo = PlayerList.Where(items => items.TotalScore >= 500).OrderByDescending(Items => Items.TotalScore).FirstOrDefault();
            return SingleInfo != null;
        }
        public async Task PickupFromDiscardAsync(int deck)
        {
            var thisList = _thisMod!.DiscardList1!.DiscardListSelected(deck);
            await PickupFromDiscardAsync(thisList);
        }
        public async Task PickupFromDiscardAsync(IDeckDict<RegularRummyCard> thisCol)
        {
            var thisCard = thisCol.First();
            PreviousCard = thisCard.Deck;
            _thisMod!.DiscardList1!.RemoveFromPoint(thisCard.Deck);
            thisCol.ForEach(tempCard =>
            {
                tempCard.Drew = true;
            });
            SingleInfo!.MainHandList.AddRange(thisCol);
            SaveRoot!.MoreThanOne = thisCol.Count > 1;
            if (SingleInfo.PlayerCategory == EnumPlayerCategory.Self)
                SortCards();
            AlreadyDrew = true; //this counts as already drawing too.
            await ContinueTurnAsync();
        }
        public DeckRegularDict<RegularRummyCard> AppendDiscardList(IDeckDict<RegularRummyCard> thisCol)
        {
            DeckRegularDict<RegularRummyCard> output = new DeckRegularDict<RegularRummyCard>();
            output.AddRange(thisCol);
            output.AddRange(SingleInfo!.MainHandList);
            return output;
        }
        public bool CardContainsRummy(int deck, IDeckDict<RegularRummyCard> newSet)
        {
            int x;
            var thisCard = DeckList!.GetSpecificItem(deck);
            int plays;
            foreach (var thisSet in _thisMod!.MainSets1!.SetList)
            {
                for (x = 1; x <= 2; x++)
                {
                    plays = thisSet.PositionToPlay(thisCard);
                    if (plays > 0)
                        return true;
                }
            }
            if (newSet.Count < 4)
                return false; //because still needs one for discard.
            int manys = newSet.Count(items => items.Value == thisCard.Value);
            if (manys >= 3)
                return true; //other cases are covered.
            DeckRegularDict<RegularRummyCard> mainList;
            do
            {
                mainList = newSet.ToRegularDeckDict();
                var tempCol = _rummys!.WhatNewRummy(mainList, 3, RummyProcesses<EnumSuitList, EnumColorList, RegularRummyCard>.EnumRummyType.Runs, false);
                if (tempCol.Count == 0)
                    return false;
                if (tempCol.Count == newSet.Count)
                    return false; //because nothing left to discard.
                if (tempCol.Any(Items => Items.Deck == deck))
                    return true;
                newSet.RemoveGivenList(tempCol, System.Collections.Specialized.NotifyCollectionChangedAction.Remove); //hopefully that works.
            } while (true);
        }
        public bool IsValidRummy(IDeckDict<RegularRummyCard> thisCol, out EnumWhatSets whatType, out bool useSecond)
        {
            whatType = EnumWhatSets.kinds;
            useSecond = false;
            if (thisCol.Count < 3)
                return false; //you have to have at least 3 cards for a rummy.
            bool rets;
            var newList = thisCol.ToRegularDeckDict();
            rets = _rummys!.IsNewRummy(newList, thisCol.Count, RummyProcesses<EnumSuitList, EnumColorList, RegularRummyCard>.EnumRummyType.Sets);
            if (rets == true)
            {
                return true;
            }
            rets = _rummys.IsNewRummy(newList, thisCol.Count, RummyProcesses<EnumSuitList, EnumColorList, RegularRummyCard>.EnumRummyType.Runs);
            if (rets == true)
            {
                whatType = EnumWhatSets.runs;
                useSecond = _rummys.UseSecond;
                return true;
            }
            return false;
        }
        public async Task CreateNewSetAsync(IDeckDict<RegularRummyCard> thisCol, EnumWhatSets setType, bool useSecond)
        {
            DeckRegularDict<RegularRummyCard> newCol = new DeckRegularDict<RegularRummyCard>();
            thisCol.ForEach(ThisCard => newCol.Add(SingleInfo!.MainHandList.GetSpecificItem(ThisCard.Deck)));
            SingleInfo!.MainHandList.RemoveGivenList(newCol, System.Collections.Specialized.NotifyCollectionChangedAction.Remove);
            RummySet thisSet = new RummySet(_thisMod!);
            thisSet.CreateNewSet(thisCol, setType, useSecond);
            _thisMod!.MainSets1!.CreateNewSet(thisSet);
            UpdatePoints();
            await ContinueTurnAsync();
        }
        public async Task AddToSetAsync(int whatSet, int deck, int position)
        {
            RummySet thisSet = _thisMod!.MainSets1!.GetIndividualSet(whatSet);
            SingleInfo!.MainHandList.RemoveObjectByDeck(deck);
            RegularRummyCard thisCard = DeckList!.GetSpecificItem(deck);
            thisSet.AddCard(thisCard, position);
            UpdatePoints();
            await ContinueTurnAsync();
        }
        private void UpdatePoints()
        {
            PointInfo thisPoint = CalculatePoints(WhoTurn);
            SingleInfo!.CardsPlayed = thisPoint.NumberOfCards;
            SingleInfo.PointsPlayed = thisPoint.Points;
        }
    }
}