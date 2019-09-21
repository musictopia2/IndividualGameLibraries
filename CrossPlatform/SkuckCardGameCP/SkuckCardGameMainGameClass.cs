using System;
using System.Text;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using System.Linq;
using CommonBasicStandardLibraries.BasicDataSettingsAndProcesses;
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;
using CommonBasicStandardLibraries.CollectionClasses;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using fs = CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.JsonSerializers.FileHelpers;
using js = CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.JsonSerializers.NewtonJsonStrings; //just in case i need those 2.
using BasicGameFramework.Attributes;
using BasicGameFramework.CommonInterfaces;
using BasicGameFramework.MultiplayerClasses.BasicGameClasses;
using BasicGameFramework.MultiplayerClasses.InterfaceMessages;
using BasicGameFramework.DIContainers;
using BasicGameFramework.Extensions;
using BasicGameFramework.MultiplayerClasses.Extensions;
using BasicGameFramework.NetworkingClasses.Extensions;
using BasicGameFramework.MultiplayerClasses.BasicPlayerClasses;
using BasicGameFramework.RegularDeckOfCards;
using BasicGameFramework.SpecializedGameTypes.TrickClasses;
using BasicGameFramework.MultiplayerClasses.InterfacesForHelpers;
using BasicGameFramework.BasicDrawables.Dictionary;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.RandomGenerator;

namespace SkuckCardGameCP
{
    [SingletonGame]
    public class SkuckCardGameMainGameClass : TrickGameClass<EnumSuitList, SkuckCardGameCardInformation,
        SkuckCardGamePlayerItem, SkuckCardGameSaveInfo>, IMiscDataNM, IStartNewGame
    {
        private IAdvancedTrickProcesses? _aTrick;
        public SkuckCardGameMainGameClass(IGamePackageResolver container) : base(container) { }

        private SkuckCardGameViewModel? _thisMod;
        protected override bool CanEnableTrickAreas => SaveRoot!.WhatStatus == EnumStatusList.NormalPlay;
        public override void Init() //decided to have all the code under init to prevent overflow issues.
        {
			base.Init();
            _thisMod = MainContainer.Resolve<SkuckCardGameViewModel>();
        }
        public override Task PopulateSaveRootAsync()
        {
            PlayerList!.ForEach(thisPlayer =>
            {
                thisPlayer.SavedTemp = thisPlayer!.TempHand!.CardList.ToRegularDeckDict();
            });
            return base.PopulateSaveRootAsync();
        }
        public override async Task FinishGetSavedAsync()
        {
            LoadControls();
            LoadVM();
            _thisMod!.LoadPlayerControls(); //i think
            _thisMod.Bid1!.UnselectAll();
            _thisMod.Suit1!.UnselectAll();
            PlayerList!.ForEach(thisPlayer =>
            {
                if (thisPlayer.PlayerCategory == EnumPlayerCategory.Self)
                    thisPlayer.BidVisible = true;
                else
                    thisPlayer.BidVisible = false;
            });
            await base.FinishGetSavedAsync();
            _aTrick!.LoadGame();
        }
        protected override Task LastPartOfSetUpBeforeBindingsAsync()
        {
            PlayerList!.ForEach(thisPlayer =>
            {
                var thisList = _thisMod!.Deck1!.DrawSeveralCards(16);
                thisPlayer.TempHand!.ClearBoard(thisList);
            });
            EvalulateStrength();
            WhoTurn = WhoChoosesTrump();
            return base.LastPartOfSetUpBeforeBindingsAsync();
        }
        protected override void LoadVM()
        {
            base.LoadVM();
            SaveRoot!.LoadMod(_thisMod!);
        }
        private void LoadControls()
        {
            if (IsLoaded == true)
                return;
            LoadTrickAreas();
            _aTrick = MainContainer.Resolve<IAdvancedTrickProcesses>(); //decided to be here.
            IsLoaded = true; //i think needs to be here.
        }
        protected override async Task ComputerTurnAsync()
        {
            if (ThisTest!.NoAnimations == false)
                await Delay!.DelaySeconds(.75);
            if (SaveRoot!.WhatStatus == EnumStatusList.NormalPlay)
            {
                var moveList = SingleInfo!.MainHandList.Where(Items => IsValidMove(Items.Deck)).Select(Items => Items.Deck).ToCustomBasicList();
                var otherList = SingleInfo.TempHand!.ValidCardList;
                otherList.KeepConditionalItems(Items => IsValidMove(Items.Deck));
                var finList = otherList.Select(Items => Items.Deck).ToCustomBasicList();
                moveList.AddRange(finList);
                if (moveList.Count == 0)
                    throw new BasicBlankException("There must be at least one move for the computer");
                await PlayCardAsync(moveList.GetRandomItem());
                return;
            }
            if (SaveRoot.WhatStatus == EnumStatusList.WaitForOtherPlayers)
            {
                int id = PlayerList.Where(items => items.PlayerCategory == EnumPlayerCategory.Computer).Single().Id;
                _thisMod!.BidAmount = _thisMod.Bid1!.NumberToChoose();
                await ProcessBidAmountAsync(id);
                return;
            }
            if (SaveRoot.WhatStatus == EnumStatusList.ChooseTrump)
            {
                SaveRoot.TrumpSuit = _thisMod!.Suit1!.ItemToChoose();
                await TrumpChosenAsync();
                return;
            }
            if (_rs == null)
                _rs = MainContainer.Resolve<RandomGenerator>();
            int Ask1 = _rs.GetRandomNumber(2);
            if (Ask1 == 1)
                await ChooseToPlayAsync();
            else
                await ChooseToPassAsync();
        }
        private RandomGenerator? _rs;
        protected override Task StartSetUpAsync(bool isBeginning)
        {
            LoadControls();
            PlayerList!.ForEach(thisPlayer => thisPlayer.SavedTemp.Clear());
            _thisMod!.LoadPlayerControls();
            LoadVM();
            SaveRoot!.WhatStatus = EnumStatusList.ChooseTrump;
            SaveRoot.RoundNumber++;
            PlayerList.ForEach(thisPlayer =>
            {
                thisPlayer.BidAmount = 0;
                thisPlayer.BidVisible = false;
                thisPlayer.TricksWon = 0;
                thisPlayer.StrengthHand = 0;
                thisPlayer.TieBreaker = "0";
            });
            SaveRoot.TrumpSuit = EnumSuitList.None;
            _thisMod.BidAmount = -1;
            _thisMod.Bid1!.UnselectAll();
            _thisMod.Suit1!.UnselectAll();
            return base.StartSetUpAsync(isBeginning);
        }
        async Task IMiscDataNM.MiscDataReceived(string status, string content)
        {
            switch (status)
            {
                case "choosetoplay":
                    await ChooseToPlayAsync();
                    break;
                case "choosetopass":
                    await ChooseToPassAsync();
                    break;
                case "trump":
                    SaveRoot!.TrumpSuit = await js.DeserializeObjectAsync<EnumSuitList>(content);
                    await TrumpChosenAsync();
                    break;
                case "bid":
                    int plays = PlayerList.Where(items => items.PlayerCategory == EnumPlayerCategory.OtherHuman).Single().Id;
                    _thisMod!.BidAmount = int.Parse(content);
                    await ProcessBidAmountAsync(plays);
                    break;
                default:
                    throw new BasicBlankException($"Nothing for status {status}  with the message of {content}");
            }
        }
        public override async Task StartNewTurnAsync()
        {
            await base.StartNewTurnAsync();

            await ContinueTurnAsync(); //most of the time, continue turn.  can change to what is needed
        }
        public override async Task EndTurnAsync()
        {
            SingleInfo = PlayerList!.GetWhoPlayer();
            SingleInfo.MainHandList.UnhighlightObjects();
            _thisMod!.CommandContainer!.ManuelFinish = true; //because it could be somebody else's turn.
            WhoTurn = await PlayerList.CalculateWhoTurnAsync();
            this.ShowTurn();
            await SaveStateAsync();
            await ContinueTurnAsync();
        }
        private int WhoWonTrick(DeckObservableDict<SkuckCardGameCardInformation> thisCol)
        {
            if (thisCol.Count != 2)
                throw new BasicBlankException("The trick list must have 2 cards");
            var leadCard = thisCol.First();
            int begins = leadCard.Player;
            var otherCard = thisCol.Last();
            if (otherCard.Suit == SaveRoot!.TrumpSuit && leadCard.Suit != SaveRoot.TrumpSuit && CanConsiderTrump(otherCard) == true)
                return WhoTurn;
            if (leadCard.Suit == SaveRoot.TrumpSuit && otherCard.Suit != SaveRoot.TrumpSuit && CanConsiderTrump(otherCard) == true)
                return begins;
            if (leadCard.Suit == otherCard.Suit)
            {
                if (otherCard.Value > leadCard.Value)
                    return WhoTurn;
            }
            return begins;
        }
        public override async Task EndTrickAsync()
        {
            var trickList = SaveRoot!.TrickList;
            int wins = WhoWonTrick(trickList);
            SkuckCardGamePlayerItem thisPlayer = PlayerList![wins];
            thisPlayer.TricksWon += HowManyWins(trickList);
            await _aTrick!.AnimateWinAsync(wins);
            //has to get self this time.
            SingleInfo = PlayerList.GetSelf();
            if (SingleInfo.MainHandList.Count == 0 && SingleInfo.TempHand!.IsFinished == true)
            {
                await EndRoundAsync();
                return; //most of the time its in rounds.
            }
            _thisMod!.PlayerHand1!.EndTurn();
            WhoTurn = wins; //most of the time, whoever wins leads again.
            await StartNewTrickAsync();
        }
        private async Task StartNewTrickAsync()
        {
            _aTrick!.ClearBoard();
            _thisMod!.CommandContainer!.ManuelFinish = true; //because it could be somebody else's turn.
            await StartNewTurnAsync(); //hopefully this simple.
        }

        Task IStartNewGame.ResetAsync()
        {
            PlayerList!.ForEach(thisPlayer =>
            {
                thisPlayer.TotalScore = 0; //hopefully just these 2.
                thisPlayer.PerfectRounds = 0;
            });
            SaveRoot!.RoundNumber = 0;
            return Task.CompletedTask;
        }
        public override async Task ContinueTurnAsync()
        {
            if (SaveRoot!.WhatStatus != EnumStatusList.NormalPlay)
                TrickArea1!.Visible = false; //try this.
            if (SaveRoot!.WhatStatus != EnumStatusList.ChooseBid)
            {
                await base.ContinueTurnAsync();
                PlayerList!.ForEach(thisItem => thisItem.TempHand!.ReportCanExecuteChange()); //just in case it got hosed.
                return;
            }
            SingleInfo = PlayerList!.GetSelf();
            WhoTurn = SingleInfo.Id;
            this.ShowTurn();
            if (SingleInfo.BidAmount > 0)
            {
                if (ThisData!.MultiPlayer == false)
                    throw new BasicBlankException("If you already bidded, the computer player should have bidded then moved on");
                await SaveStateAsync();
                ThisCheck!.IsEnabled = true;
                return; //to wait for the other player.
            }
            await base.ContinueTurnAsync();
        }
        private void EvalulateStrength()
        {
            DeckRegularDict<SkuckCardGameCardInformation> firstCollection = new DeckRegularDict<SkuckCardGameCardInformation>();
            DeckRegularDict<SkuckCardGameCardInformation> secondCollection = new DeckRegularDict<SkuckCardGameCardInformation>();
            DeckRegularDict<SkuckCardGameCardInformation> tempList;
            PlayerList!.ForEach(tempPlayer =>
            {
                tempPlayer.MainHandList.ForEach(thisCard =>
                {
                    if (tempPlayer.Id == 1)
                        firstCollection.Add(thisCard);
                    else
                        secondCollection.Add(thisCard);
                });
                tempList = tempPlayer.TempHand!.KnownList;
                tempList.ForEach(ThisCard =>
                {
                    if (tempPlayer.Id == 1)
                        firstCollection.Add(ThisCard);
                    else
                        secondCollection.Add(ThisCard);
                });
            });
            if (firstCollection.Count != 18 || secondCollection.Count != 18)
                throw new BasicBlankException("Both players has 18 cards to begin with to evaluate strength hand");
            int firstPoints = 0;
            int secondPoints = 0;
            firstCollection.ForEach(thisCard =>
            {
                if (thisCard.Value == EnumCardValueList.HighAce)
                    firstPoints += 5;
                else if (thisCard.Value == EnumCardValueList.King)
                    firstPoints += 4;
                else if (thisCard.Value == EnumCardValueList.Queen)
                    firstPoints += 3;
                else if (thisCard.Value == EnumCardValueList.Jack)
                    firstPoints += 2;
                else if (thisCard.Value == EnumCardValueList.Ten)
                    firstPoints += 1;
            });
            secondCollection.ForEach(thisCard =>
            {
                if (thisCard.Value == EnumCardValueList.HighAce)
                    secondPoints += 5;
                else if (thisCard.Value == EnumCardValueList.King)
                    secondPoints += 4;
                else if (thisCard.Value == EnumCardValueList.Queen)
                    secondPoints += 3;
                else if (thisCard.Value == EnumCardValueList.Jack)
                    secondPoints += 2;
                else if (thisCard.Value == EnumCardValueList.Ten)
                    secondPoints += 1;
            });
            PlayerList.First().StrengthHand = firstPoints;
            PlayerList.Last().StrengthHand = secondPoints;
            if (firstPoints != secondPoints)
            {
                PlayerList.ForEach(ThisPlayer => ThisPlayer.TieBreaker = "");
                return;
            }
            int firstNumber;
            int secondNumber;
            string whatText;
            int whos;
            for (int x = 2; x <= 14; x++)
            {
                firstNumber = firstCollection.Count(Items => Items.Value == x.ToEnum<EnumCardValueList>());
                secondNumber = secondCollection.Count(Items => Items.Value == x.ToEnum<EnumCardValueList>());
                if (firstNumber != secondNumber)
                {
                    if (firstNumber > secondNumber)
                        whos = 1;
                    else
                        whos = 2;
                    if (whos == 1)
                        whatText = $"{x},y";
                    else
                        whatText = $"{x},n";
                    PlayerList.First().TieBreaker = whatText;
                    if (whos == 1)
                        whatText = $"{x},n";
                    else
                        whatText = $"{x},y";
                    PlayerList.Last().TieBreaker = whatText;
                }
            }
        }
        private int WhoChoosesTrump()
        {
            int firstNumber;
            int secondNumber;
            firstNumber = PlayerList.First().StrengthHand;
            secondNumber = PlayerList.Last().StrengthHand;
            if (firstNumber < secondNumber)
                return 1;
            else if (secondNumber > firstNumber)
                return 2;
            string ThisText = PlayerList.First().TieBreaker;
            if (ThisText.Contains("y"))
                return 1;
            return 2;
        }
        private async Task GameOverAsync()
        {
            int wins = WhoWonGame();
            if (wins == 0)
                await ShowTieAsync();
            else
            {
                SingleInfo = PlayerList![wins];
                await ShowWinAsync();
            }
        }
        private int WhoWonGame()
        {
            if (PlayerList.Count() != 2)
                throw new BasicBlankException("2 Player Game Only.  Rethink");
            var firstPlayer = PlayerList.First();
            var secondPlayer = PlayerList.Last();
            if (firstPlayer.TotalScore > secondPlayer.TotalScore)
                return 1;
            if (secondPlayer.TotalScore > firstPlayer.TotalScore)
                return 2;
            if (firstPlayer.PerfectRounds == secondPlayer.PerfectRounds)
                return 0;
            if (firstPlayer.PerfectRounds > secondPlayer.PerfectRounds)
                return 1;
            return 2;
        }
        private bool CanConsiderTrump(SkuckCardGameCardInformation thisCard)
        {
            if (SaveRoot!.TrumpSuit != EnumSuitList.Clubs)
                return true;
            if (thisCard.Suit != EnumSuitList.Clubs)
                return true;
            if (thisCard.Value == EnumCardValueList.Jack)
                return false;
            return true;
        }
        public async Task ChooseToPlayAsync()
        {
            SaveRoot!.WhatStatus = EnumStatusList.NormalPlay;
            await StartNewTrickAsync();
        }
        public async Task ChooseToPassAsync()
        {
            if (WhoTurn == 1)
                WhoTurn = 2;
            else
                WhoTurn = 1;
            SaveRoot!.WhatStatus = EnumStatusList.NormalPlay;
            await StartNewTrickAsync();
        }
        private bool HasException()
        {
            if (ThisTest!.DoubleCheck == true)
                return true;
            int diffs = PlayerList.First().StrengthHand - PlayerList.Last().StrengthHand;
            if (Math.Abs(diffs) >= 12)
                return true;
            return false;
        }
        private int HowManyWins(IDeckDict<SkuckCardGameCardInformation> trickList)
        {
            if (trickList.Any(items => items.Suit == EnumSuitList.Clubs && items.Value == EnumCardValueList.Jack))
                return 2;
            return 1;
        }
        private void CalculateScore()
        {
            int nums;
            PlayerList!.ForEach(thisPlayer =>
            {
                nums = thisPlayer.TricksWon - thisPlayer.BidAmount;
                if (nums == 0)
                    thisPlayer.PerfectRounds++;
                else if (nums > 0)
                    nums *= -1;
                thisPlayer.TotalScore += nums;
            });
        }
        public override async Task EndRoundAsync()
        {
            CalculateScore();
            PlayerList!.ForEach(thisPlayer => thisPlayer.BidVisible = true);
            if (SaveRoot!.RoundNumber == 4)
            {
                await GameOverAsync();
                return;
            }
            this.RoundOverNext();
        }
        public async Task TrumpChosenAsync()
        {
            if (SaveRoot!.TrumpSuit == EnumSuitList.None)
                throw new BasicBlankException("Suit must have been chosen before you can run the method to show the trump actually chosen");
            if (SingleInfo!.PlayerCategory != EnumPlayerCategory.Self)
            {
                if (ThisTest!.NoAnimations == false)
                {
                    _thisMod!.Suit1!.SelectSpecificItem(SaveRoot.TrumpSuit);
                    await Delay!.DelaySeconds(1);
                }
            }
            SaveRoot.WhatStatus = EnumStatusList.ChooseBid;
            await ShowHumanCanPlayAsync(); //because both players can do at the same time.
        }
        public async Task ProcessBidAmountAsync(int player)
        {
            if (_thisMod!.BidAmount == -1)
                throw new BasicBlankException("Did not choose a bid amount");
            var thisPlayer = PlayerList![player];
            thisPlayer.BidAmount = _thisMod.BidAmount;
            if (thisPlayer.PlayerCategory == EnumPlayerCategory.Self)
            {
                thisPlayer.BidVisible = true;
                SaveRoot!.WhatStatus = EnumStatusList.WaitForOtherPlayers;
                if (ThisData!.MultiPlayer == false)
                {
                    await ComputerTurnAsync();
                    return;
                }
                ThisCheck!.IsEnabled = true; //wait for other players.
                return;
            }
            if (HasException() == true)
            {
                _thisMod!.CommandContainer!.ManuelFinish = true; //just in case.
                SaveRoot!.WhatStatus = EnumStatusList.ChoosePlay;
                _thisMod.CommandContainer.IsExecuting = true; //make sure its executing no matter what as well.
                if (PlayerList.First().StrengthHand > PlayerList.Last().StrengthHand)
                    WhoTurn = 2;
                else
                    WhoTurn = 1; //to double check.
                SingleInfo = PlayerList.GetWhoPlayer();
                await StartNewTurnAsync(); //hopefully this works.
                return;
            }
            SaveRoot!.WhatStatus = EnumStatusList.NormalPlay;
            if (WhoTurn == 1)
                WhoTurn = 2;
            else
                WhoTurn = 1;
            SingleInfo = PlayerList.GetWhoPlayer();
            await StartNewTrickAsync();
        }
    }
}