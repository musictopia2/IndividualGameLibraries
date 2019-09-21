using BasicGameFramework.Attributes;
using BasicGameFramework.BasicDrawables.Dictionary;
using BasicGameFramework.DIContainers;
using BasicGameFramework.Extensions;
using BasicGameFramework.MultiplayerClasses.BasicGameClasses;
using BasicGameFramework.MultiplayerClasses.BasicPlayerClasses;
using BasicGameFramework.MultiplayerClasses.InterfaceMessages;
using BasicGameFramework.RegularDeckOfCards;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.Exceptions;
using System.Linq;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace BladesOfSteelCP
{
    [SingletonGame]
    public class BladesOfSteelMainGameClass : CardGameClass<RegularSimpleCard, BladesOfSteelPlayerItem, BladesOfSteelSaveInfo>, IMiscDataNM
    {
        public BladesOfSteelMainGameClass(IGamePackageResolver container) : base(container) { }
        internal BladesOfSteelViewModel? ThisMod;
        private bool _firstDraw;
        private bool _drewCard;
        private ComputerAI? _ai;
        public override void Init() //decided to have all the code under init to prevent overflow issues.
        {
            base.Init();
            ThisMod = MainContainer.Resolve<BladesOfSteelViewModel>();
        }
        #region "public functions"
        public EnumAttackGroup GetAttackStage(ICustomBasicList<RegularSimpleCard> thisList)
        {
            if (thisList.Any(items => items.Color == EnumColorList.Black))
                throw new BasicBlankException("Cannot get the attack stage if black cards are present");
            if (thisList.Count < 2)
                throw new BasicBlankException("Must have at least 2 cards for attack");
            if (thisList.Count == 2)
            {
                if (thisList.All(items => items.Value == EnumCardValueList.Nine))
                    return EnumAttackGroup.GreatOne;
                if (thisList.HasDuplicates(items => items.Value))
                    return EnumAttackGroup.OneTimer;
                if (thisList.Any(items => items.Value == EnumCardValueList.HighAce))
                {
                    bool suitMatch = thisList.HasDuplicates(items => items.Suit);
                    if (suitMatch == true)
                        return EnumAttackGroup.BreakAway;
                }
                return EnumAttackGroup.HighCard;
            }
            if (thisList.Count > 3)
                throw new BasicBlankException("Can only attack with 3 cards at the most");
            int counts = thisList.DistinctCount(items => items.Suit);
            if (counts == 1)
                return EnumAttackGroup.Flush;
            return EnumAttackGroup.HighCard;
        }
        public EnumDefenseGroup GetDefenseStage(ICustomBasicList<RegularSimpleCard> thisList)
        {
            if (thisList.Any(items => items.Color == EnumColorList.Red))
                throw new BasicBlankException("Cannot get the defense stage if there are red cards present");
            if (thisList.Count == 0)
                throw new BasicBlankException("Must have at least one card for defense");
            if (thisList.Count > 3)
                throw new BasicBlankException("Cannot defend with more than 3 cards");
            if (thisList.Count == 1)
            {
                if (thisList.Single().Value == EnumCardValueList.HighAce)
                    return EnumDefenseGroup.StarGoalie;
                return EnumDefenseGroup.HighCard;
            }
            int counts = thisList.DistinctCount(items => items.Suit);
            if (thisList.Count == 2)
            {
                if (thisList.HasDuplicates(items => items.Value) == true) //this will imply this time.
                    return EnumDefenseGroup.StarDefense;
                return EnumDefenseGroup.HighCard;
            }
            if (counts == 1)
                return EnumDefenseGroup.Flush;
            return EnumDefenseGroup.HighCard;
        }
        public async Task<bool> CanPlayAttackCardsAsync(IDeckDict<RegularSimpleCard> thisList)
        {
            if (thisList.Count < 2)
            {
                await ThisMod!.ShowGameMessageAsync("Must have at least 2 cards for attacking");
                return false;
            }
            if (thisList.Any(items => items.Color == EnumColorList.Black))
            {
                await ThisMod!.ShowGameMessageAsync("Cannot attack with black (defense) cards");
                return false;
            }
            if (thisList.Count > 3)
            {
                await ThisMod!.ShowGameMessageAsync("Cannot attack with more than 3 cards");
                return false;
            }
            return true;
        }
        public async Task<bool> CanPlayDefenseCardsAsync(IDeckDict<RegularSimpleCard> thisList)
        {
            if (thisList.Count == 0)
            {
                await ThisMod!.ShowGameMessageAsync("Must choose at least one card");
                return false;
            }
            if (thisList.Count > 3)
            {
                await ThisMod!.ShowGameMessageAsync("Cannot play more than 3 cards for defense");
                return false;
            }
            if (thisList.Any(items => items.Color == EnumColorList.Red))
            {
                await ThisMod!.ShowGameMessageAsync("Cannot use attack (red) cards for defense");
                return false;
            }
            return true;
        }
        public async Task PlayAttackCardsAsync(IDeckDict<RegularSimpleCard> thisList)
        {
            SingleInfo = PlayerList!.GetWhoPlayer();
            SingleInfo.MainHandList.RemoveGivenList(thisList); //hopefully this simple.
            thisList.UnhighlightObjects();
            if (SingleInfo.AttackList.Count > 0)
                throw new BasicBlankException("There are already attack cards played.  There should not have been able to play attack cards");
            SingleInfo.AttackList.AddRange(thisList);
            if (WhoTurn == 1)
                OtherTurn = 2;
            else
                OtherTurn = 1;
            await ContinueTurnAsync();
        }
        public async Task PlayDefenseCardsAsync(IDeckDict<RegularSimpleCard> defenseList, bool fromHand)
        {
            if (fromHand == true)
            {
                SingleInfo!.MainHandList.RemoveSelectedItems(defenseList); //well see if this works or not (?)
            }
            else
                SingleInfo!.DefenseList.RemoveSelectedItems(defenseList);
            ThisMod!.MainDefense1!.PopulateObjects(defenseList);
            OtherTurn = 0;
            await ContinueTurnAsync();
        }
        public async Task AddCardsToDefensePileAsync(IDeckDict<RegularSimpleCard> defenseList)
        {
            SingleInfo!.MainHandList!.RemoveGivenList(defenseList); //maybe works both ways (?)
            SingleInfo.DefensePile!.PopulateObjects(defenseList);
            await EndTurnAsync();
        }
        public async Task PassDefenseAsync()
        {
            SingleInfo = PlayerList!.GetWhoPlayer();
            await ThisMod!.ShowGameMessageAsync($"{SingleInfo.NickName} has scored a goal");
            SingleInfo.Score++;
            SingleInfo.AttackList.Clear();
            if (SaveRoot!.WasTie == true)
            {
                await ShowWinAsync(); //this one won period now.
                return;
            }
            await EndTurnAsync();
        }
        public async Task ThrowAwayDefenseCardsAsync()
        {
            if (SingleInfo!.DefenseList.Count == 0)
                throw new BasicBlankException("There are no defense cards to throw away");
            var thisList = SingleInfo.DefenseList.ToRegularDeckDict();
            await thisList.ForEachAsync(async thisCard =>
            {
                SingleInfo.DefenseList.RemoveObjectByDeck(thisCard.Deck);
                await AnimatePlayAsync(thisCard);
                if (ThisTest!.NoAnimations == false)
                    await Delay!.DelaySeconds(.1);
            });
            if (SingleInfo.DefenseList.Count > 0)
                throw new BasicBlankException("All the defense cards should be gone");
            await EndTurnAsync(); //i think.
        }
        public async Task ThrowAwayAllCardsFromHandAsync()
        {
            var thisList = SingleInfo!.MainHandList.ToRegularDeckDict();
            await thisList.ForEachAsync(async thisCard =>
            {
                SingleInfo.MainHandList.RemoveObjectByDeck(thisCard.Deck);
                await AnimatePlayAsync(thisCard);
                if (ThisTest!.NoAnimations == false)
                    await Delay!.DelaySeconds(.1);
            });
            if (SingleInfo.MainHandList.Count > 0)
                throw new BasicBlankException("Should have 0 cards left after discarding all your cards");
            await EndTurnAsync();
        }
        #endregion
        internal int OtherTurn
        {
            get
            {
                return SaveRoot!.PlayOrder.OtherTurn;
            }
            set
            {
                SaveRoot!.PlayOrder.OtherTurn = value;
            }
        }
        protected override bool ShowNewCardDrawn(BladesOfSteelPlayerItem tempPlayer)
        {
            return _drewCard;
        }
        protected override void SortAfterDrawing()
        {
            if (_firstDraw == false)
            {
                base.SortAfterDrawing();
                return;
            }
            SingleInfo = PlayerList!.GetSelf();
            if (SingleInfo.MainHandList.Count == 6)
                SortCards();
        }
        protected override async Task AfterDrawingAsync()
        {
            if (_firstDraw == false)
            {
                if (SingleInfo!.MainHandList.Count != 6)
                    throw new BasicBlankException("Should have 6 cards in hand after drawing");
                await base.AfterDrawingAsync();
                return;
            }
            if (PlayerList.First().MainHandList.Count == 6 && PlayerList.Last().MainHandList.Count == 6)
            {
                _firstDraw = false;
            }
            await EndTurnAsync();
        }
        protected override void LinkHand()
        {
            ThisMod!.PlayerHand1!.HandList = SingleInfo!.MainHandList; //hopefully this is it.
            PrepSort();
        }
        public override async Task DrawAsync()
        {
            if (SaveRoot!.IsFaceOff == true)
            {
                if (SingleInfo!.FaceOff != null)
                    throw new BasicBlankException("Player already has a faceoff card.  Therefore; cannot get another faceoff card");
                if (SingleInfo.PlayerCategory == EnumPlayerCategory.OtherHuman)
                {
                    ThisCheck!.IsEnabled = true;
                    return; //has to wait for other player to send the faceoff card info
                }
                int deck = DeckList!.ToRegularDeckDict().GetRandomItem().Deck;
                RegularSimpleCard thisCard = new RegularSimpleCard();
                thisCard.Populate(deck);
                if (ThisData!.MultiPlayer == true)
                    await ThisNet!.SendAllAsync("faceoff", deck);
                await FaceOffCardAsync(thisCard);
                return;
            }
            await base.DrawAsync();
        }
        public async Task FaceOffCardAsync(RegularSimpleCard thisCard)
        {
            SingleInfo!.FaceOff = thisCard;
            if (SingleInfo.PlayerCategory == EnumPlayerCategory.Self)
                ThisMod!.YourFaceOffCard!.AddCard(thisCard);
            else
                ThisMod!.OpponentFaceOffCard!.AddCard(thisCard);
            if (PlayerList.Any(items => items.FaceOff == null))
            {
                await EndTurnAsync();
                return;
            }
            await AnalyzeFaceOffAsync();
        }
        private async Task AnalyzeFaceOffAsync()
        {
            int tempTurn = WhoWonFaceOff();
            if (tempTurn == 0)
            {
                await ThisMod!.ShowGameMessageAsync("There was a tie during the faceoff.  Therefore; the faceoff is being done again");
                ClearFaceOff();
                await EndTurnAsync();
                return;
            }
            WhoTurn = tempTurn;
            SingleInfo = PlayerList!.GetWhoPlayer();
            await ThisMod!.ShowGameMessageAsync($"{SingleInfo.NickName} has won the face off");
            ClearFaceOff();
            SaveRoot!.IsFaceOff = false;
            await StartNewTurnAsync();
        }
        private int WhoWonFaceOff()
        {
            if (ThisTest!.DoubleCheck == true)
                return PlayerList.First(items => items.PlayerCategory == EnumPlayerCategory.Self).Id;
            if (PlayerList.First().FaceOff!.Value > PlayerList.Last().FaceOff!.Value)
                return 1;
            if (PlayerList.Last().FaceOff!.Value > PlayerList.First().FaceOff!.Value)
                return 2;
            return 0;
        }
        private void ClearFaceOff()
        {
            PlayerList.First().FaceOff = null;
            PlayerList.Last().FaceOff = null;
        }
        private async Task StartDrawingAsync()
        {
            if (SaveRoot!.IsFaceOff == true)
            {
                await ContinueTurnAsync();
                return;
            }
            SingleInfo = PlayerList!.GetWhoPlayer();
            if (_firstDraw == true)
            {
                if (SingleInfo.MainHandList.Count == 6)
                    throw new BasicBlankException("Cannot have 6 cards because its first draw");
                LeftToDraw = 0;
                await DrawAsync();
                return;
            }
            if (SingleInfo.MainHandList.Count == 6)
            {
                await ContinueTurnAsync();
                return;
            }
            if (SingleInfo.MainHandList.Count > 6)
                throw new BasicBlankException("Cannot have more than 6 cards in hand");
            _drewCard = SingleInfo.MainHandList.Count > 0;
            LeftToDraw = 6 - SingleInfo.MainHandList.Count;
            if (LeftToDraw > ThisMod!.Deck1!.CardsLeft())
            {
                await StartEndAsync();
                return;
            }
            PlayerDraws = WhoTurn;
            if (LeftToDraw == 1)
            {
                LeftToDraw = 0;
                PlayerDraws = 0; //because it will do the part automatically;
            }
            await DrawAsync();
        }
        protected override void GetPlayerToContinueTurn()
        {
            if (SaveRoot!.PlayOrder.OtherTurn == 0)
            {
                ThisMod!.OtherPlayer = "None";
                if (SaveRoot.IsFaceOff == true)
                    ThisMod.Instructions = "Face-Off.  Click the deck to draw a card at random.  Whoever draws a higher number goes first for the game.  If there is a tie; then repeat.";
                else if (ThisMod.MainDefense1!.HasCards == true)
                {
                    ThisMod.Instructions = "Look at the results to see that the goal was blocked and end turn.";
                }
                else
                    ThisMod.Instructions = "Either throw away all 6 of your cards, choose to attack or choose cards for defense.";
                base.GetPlayerToContinueTurn();
                return;
            }
            SingleInfo = PlayerList!.GetOtherPlayer();
            ThisMod!.OtherPlayer = SingleInfo.NickName;
            ThisMod.Instructions = "Either choose cards for defense or choose to pass to allow the goal to go through";
        }
        private int CalculateWin
        {
            get
            {
                if (PlayerList.First().Score == PlayerList.Last().Score)
                    return 0;
                if (PlayerList.First().Score > PlayerList.Last().Score)
                    return 1;
                return 2;
            }
        }
        private async Task StartEndAsync()
        {
            int whoWon = CalculateWin;
            if (whoWon == 0)
            {
                await ThisMod!.ShowGameMessageAsync("There was a tie.  Therefore; all the cards are being reshuffled.  Then a faceoff will happen again to see who goes first and the cards are passed out.  Whoever gets the first point in this round wins");
                SaveRoot!.WasTie = true;
                if (ThisData!.MultiPlayer == true && ThisData.Client == true)
                {
                    ThisCheck!.IsEnabled = true; //to wait to hear back from host again.
                    return;
                }
                WhoTurn = WhoStarts;
                WhoTurn = await PlayerList!.CalculateWhoTurnAsync();
                await SetUpGameAsync(false); //hopefully this works.
                return;
            }
            WhoTurn = whoWon;
            SingleInfo = PlayerList!.GetWhoPlayer();
            SaveRoot!.WasTie = false;
            await ShowWinAsync();
        }
        public override Task FinishGetSavedAsync()
        {
            LoadControls();
            LoadPlayerControls();
            LinkRestPiles();
            SaveRoot!.LoadMod(ThisMod!);
            return base.FinishGetSavedAsync();
        }
        private void LoadPlayerControls()
        {
            PlayerList!.ForEach(thisPlayer =>
            {
                thisPlayer.DefensePile = new PlayerDefenseCP(ThisMod!);
                thisPlayer.DefensePile.LoadBoard(thisPlayer);
                thisPlayer.AttackPile = new PlayerAttackCP(ThisMod!);
                thisPlayer.AttackPile.LoadBoard(thisPlayer);
                if (thisPlayer.PlayerCategory == EnumPlayerCategory.Self)
                {
                    ThisMod!.YourAttackPile = thisPlayer.AttackPile;
                    ThisMod.YourDefensePile = thisPlayer.DefensePile;
                    ThisMod.LoadAttackCommands();
                }
                else
                {
                    ThisMod!.OpponentAttackPile = thisPlayer.AttackPile;
                    ThisMod.OpponentDefensePile = thisPlayer.DefensePile;
                }
                thisPlayer.DefensePile.Visible = true;
                thisPlayer.AttackPile.Visible = true;
            });
        }
        private void LoadControls()
        {
            if (IsLoaded == true)
                return;
            ThisMod!.MainDefense1!.Visible = true;
            if (ThisData!.MultiPlayer == false)
                _ai = MainContainer.Resolve<ComputerAI>();
            IsLoaded = true; //i think needs to be here.
        }
        protected override async Task ComputerTurnAsync()
        {
            if (ThisTest!.NoAnimations == false)
                await Delay!.DelaySeconds(1);
            if (SaveRoot!.IsFaceOff == true)
            {
                await DrawAsync();
                return;
            }
            if (ThisTest.DoubleCheck == true)
                return; //decided to make it stuck when double checking.  not during faceoffs though.
            if (ThisMod!.MainDefense1!.HasCards == true)
            {
                await EndTurnAsync();
                return;
            }
            if (OtherTurn > 0)
            {
                var (defenseStep, cardList1) = _ai!.CardsForDefense();
                if (defenseStep == EnumDefenseStep.Pass)
                {
                    await PassDefenseAsync();
                    return;
                }
                if (defenseStep == EnumDefenseStep.Hand)
                    await PlayDefenseCardsAsync(cardList1, true);
                else
                    await PlayDefenseCardsAsync(cardList1, false);
                return;
            }
            var (firstStep, cardList2) = _ai!.GetFirstMove();
            if (firstStep == EnumFirstStep.ThrowAwayAllCards)
            {
                await ThrowAwayAllCardsFromHandAsync();
                return;
            }
            if (firstStep == EnumFirstStep.PlayDefense)
            {
                if (SingleInfo!.DefenseList.Count + cardList2.Count > 3)
                {
                    await ThrowAwayDefenseCardsAsync();
                    return;
                }
                await AddCardsToDefensePileAsync(cardList2);
                return;
            }
            await PlayAttackCardsAsync(cardList2);
        }
        protected override Task StartSetUpAsync(bool isBeginning)
        {
            if (IsLoaded == false)
                LoadPlayerControls();
            LoadControls();
            SaveRoot!.LoadMod(ThisMod!);
            if (ThisMod!.MainDefense1!.HasCards == true)
                throw new BasicBlankException("Cannot start setup when there are cards in defense");
            PlayerList!.ForEach(thisPlayer =>
            {
                thisPlayer.MainHandList.Clear();
                thisPlayer.DefenseList.Clear();
                thisPlayer.AttackList.Clear();
                thisPlayer.Score = 0;
                thisPlayer.FaceOff = null;
            });
            SaveRoot.IsFaceOff = true;
            return base.StartSetUpAsync(isBeginning);
        }
        protected override Task LastPartOfSetUpBeforeBindingsAsync()
        {
            LinkRestPiles();
            ThisMod!.Pile1!.ClearCards(); //i think this is where it should have been at.
            return base.LastPartOfSetUpBeforeBindingsAsync();
        }
        private void LinkRestPiles()
        {
            PlayerList!.ForEach(thisPlayer =>
            {
                thisPlayer.DefensePile!.HandList = thisPlayer.DefenseList;
                thisPlayer.AttackPile!.HandList = thisPlayer.AttackList;
            });
            if (ThisMod!.OpponentFaceOffCard != null)
            {
                ThisMod.OpponentFaceOffCard.ClearCards();
                ThisMod.YourFaceOffCard!.ClearCards();
            }
            _firstDraw = true;
        }
        async Task IMiscDataNM.MiscDataReceived(string status, string content)
        {
            switch (status)
            {
                case "passdefense":
                    await PassDefenseAsync();
                    return;
                case "faceoff":
                    RegularSimpleCard thisCard = new RegularSimpleCard();
                    thisCard.Populate(int.Parse(content));
                    await FaceOffCardAsync(thisCard);
                    return;
                case "throwawaydefense":
                    await ThrowAwayDefenseCardsAsync();
                    return;
                case "throwawayallcardsfromhand":
                    await ThrowAwayAllCardsFromHandAsync();
                    return;
                case "defensehand":
                    var thisList = await content.GetNewObjectListFromDeckListAsync(DeckList!);
                    await PlayDefenseCardsAsync(thisList, true);
                    return;
                case "defenseboard":
                    var thisList2 = await content.GetNewObjectListFromDeckListAsync(DeckList!);
                    await PlayDefenseCardsAsync(thisList2, false);
                    return;
                case "attack":
                    var thisList3 = await content.GetNewObjectListFromDeckListAsync(DeckList!);
                    await PlayAttackCardsAsync(thisList3);
                    return;
                case "addtodefense":
                    var thisList4 = await content.GetNewObjectListFromDeckListAsync(DeckList!);
                    await AddCardsToDefensePileAsync(thisList4);
                    return;
                default:
                    throw new BasicBlankException($"Nothing for status {status}  with the message of {content}");
            }
        }
        public override async Task StartNewTurnAsync()
        {
            await base.StartNewTurnAsync();
            await StartDrawingAsync();
        }
        public override async Task EndTurnAsync()
        {
            SingleInfo = PlayerList!.GetWhoPlayer();
            SingleInfo.MainHandList.UnhighlightObjects(); //i think this is best.
            OtherTurn = 0; //easier to do this way now. since other turn is used quite a bit.
            if (ThisMod!.MainDefense1!.HasCards == true)
            {
                DeckRegularDict<RegularSimpleCard> thisList = ThisMod.MainDefense1.HandList.ToRegularDeckDict();
                await thisList.ForEachAsync(async thisCard =>
                {
                    ThisMod.MainDefense1.HandList.RemoveSpecificItem(thisCard);
                    await AnimatePlayAsync(thisCard); //i think.
                    if (ThisTest!.NoAnimations == false)
                        await Delay!.DelaySeconds(.1);
                });

                if (SingleInfo.AttackList.Count == 0)
                    throw new BasicBlankException("The main defense cannot have any cards because there are no cards for attack");
                thisList = SingleInfo.AttackList.ToRegularDeckDict();
                await thisList.ForEachAsync(async thisCard =>
                {
                    SingleInfo.AttackList.RemoveObjectByDeck(thisCard.Deck);
                    await AnimatePlayAsync(thisCard); //i think.
                    if (ThisTest!.NoAnimations == false)
                        await Delay!.DelaySeconds(.1);
                });
                if (SingleInfo.AttackList.Count > 0)
                    throw new BasicBlankException("Must have 0 cards left for attack");
            }
            ThisMod.CommandContainer!.ManuelFinish = true; //because it could be somebody else's turn.
            WhoTurn = await PlayerList.CalculateWhoTurnAsync();
            await StartNewTurnAsync();
        }
    }
}