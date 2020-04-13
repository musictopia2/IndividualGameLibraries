using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.BasicDrawables.Dictionary;
using BasicGameFrameworkLibrary.BasicGameDataClasses;
using BasicGameFrameworkLibrary.CommandClasses;
using BasicGameFrameworkLibrary.CommonInterfaces;
using BasicGameFrameworkLibrary.DIContainers;
using BasicGameFrameworkLibrary.DrawableListsObservable;
using BasicGameFrameworkLibrary.Extensions; //most likely will be used.
using BasicGameFrameworkLibrary.MiscProcesses;
using BasicGameFrameworkLibrary.MultiplayerClasses.BasicGameClasses;
using BasicGameFrameworkLibrary.MultiplayerClasses.BasicPlayerClasses;
using BasicGameFrameworkLibrary.MultiplayerClasses.Extensions;
using BasicGameFrameworkLibrary.MultiplayerClasses.InterfaceMessages;
using BasicGameFrameworkLibrary.MultiplayerClasses.InterfacesForHelpers;
using BasicGameFrameworkLibrary.MultiplayerClasses.SavedGameClasses;
using BasicGameFrameworkLibrary.RegularDeckOfCards;
using BasicGameFrameworkLibrary.SpecializedGameTypes.TrickClasses;
using BasicGameFrameworkLibrary.TestUtilities;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.Messenging;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using SkuckCardGameCP.Cards;
using SkuckCardGameCP.Data;
using System.Linq;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using js = CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.JsonSerializers.NewtonJsonStrings; //just in case i need those 2.

namespace SkuckCardGameCP.Logic
{
    [SingletonGame]
    public class SkuckCardGameMainGameClass
        : TrickGameClass<EnumSuitList, SkuckCardGameCardInformation, SkuckCardGamePlayerItem, SkuckCardGameSaveInfo>
        , IMiscDataNM, IStartNewGame
    {


        private readonly SkuckCardGameVMData _model;
        private readonly CommandContainer _command; //most of the time, needs this.  if not needed, take out.
        private readonly SkuckCardGameGameContainer _gameContainer; //if we don't need it, take it out.
        private readonly IAdvancedTrickProcesses _aTrick;
        private readonly IBidProcesses _bidProcesses;
        private readonly IPlayChoiceProcesses _choiceProcesses;
        private readonly ITrumpProcesses _trumpProcesses;

        public SkuckCardGameMainGameClass(IGamePackageResolver mainContainer,
            IEventAggregator aggregator,
            BasicData basicData,
            TestOptions test,
            SkuckCardGameVMData currentMod,
            IMultiplayerSaveState state,
            IAsyncDelayer delay,
            ICardInfo<SkuckCardGameCardInformation> cardInfo,
            CommandContainer command,
            SkuckCardGameGameContainer gameContainer,
            ITrickData trickData,
            ITrickPlay trickPlay,
            IAdvancedTrickProcesses aTrick,
            IBidProcesses bidProcesses,
            IPlayChoiceProcesses choiceProcesses,
            ITrumpProcesses trumpProcesses
            )
            : base(mainContainer, aggregator, basicData, test, currentMod, state, delay, cardInfo, command, gameContainer, trickData, trickPlay)
        {
            _model = currentMod;
            _command = command;
            _gameContainer = gameContainer;
            _aTrick = aTrick;
            _bidProcesses = bidProcesses;
            _choiceProcesses = choiceProcesses;
            _trumpProcesses = trumpProcesses;
            _gameContainer.ComputerTurnAsync = ComputerTurnAsync;
            _gameContainer.StartNewTrickAsync = StartNewTrickAsync;
            _gameContainer.ShowHumanCanPlayAsync = ShowHumanCanPlayAsync;
        }

        public override Task PopulateSaveRootAsync()
        {
            PlayerList!.ForEach(thisPlayer =>
            {
                thisPlayer.SavedTemp = thisPlayer!.TempHand!.CardList.ToRegularDeckDict();
            });
            return base.PopulateSaveRootAsync();
        }
        private void LoadPlayerControls()
        {
            PlayerList!.ForEach(thisPlayer =>
            {

                if (thisPlayer.TempHand == null)
                {
                    thisPlayer.TempHand = new PlayerBoardObservable<SkuckCardGameCardInformation>(_command);
                    thisPlayer.TempHand.Game = PlayerBoardObservable<SkuckCardGameCardInformation>.EnumGameList.Skuck;
                    thisPlayer.TempHand.IsSelf = thisPlayer.PlayerCategory == EnumPlayerCategory.Self; //hopefully this works.

                }
                if (thisPlayer.SavedTemp.Count != 0)
                {
                    thisPlayer.TempHand.CardList.ReplaceRange(thisPlayer.SavedTemp);
                }

            });
        }
        //protected override async Task ShowHumanCanPlayAsync()
        //{
        //    await base.ShowHumanCanPlayAsync();
        //    GlobalDelegates.ManuelRepaintEnumControls.Invoke();
        //    if (BasicData.IsXamarinForms && SaveRoot.WhatStatus == EnumStatusList.ChooseTrump)
        //    {
                
        //        //await UIPlatform.ShowMessageAsync("Check 1");
        //        //_model.Suit1.ReportCanExecuteChange();
        //    }
        //}
        //public override async Task ContinueTrickAsync()
        //{
        //    await base.ContinueTrickAsync();
        //    GlobalDelegates.ManuelRepaintEnumControls.Invoke();
        //}
        public override async Task FinishGetSavedAsync()
        {
            LoadControls();
            LoadVM();
            LoadPlayerControls();
            _model.Bid1!.UnselectAll();
            _model.Suit1!.UnselectAll();
            PlayerList!.ForEach(thisPlayer =>
            {
                if (thisPlayer.PlayerCategory == EnumPlayerCategory.Self)
                    thisPlayer.BidVisible = true;
                else
                    thisPlayer.BidVisible = false;
            });
            await base.FinishGetSavedAsync();
            _aTrick!.LoadGame();
            //GlobalDelegates.ManuelRepaintEnumControls.Invoke();

        }
        private void LoadControls()
        {
            if (IsLoaded == true)
                return;

            IsLoaded = true; //i think needs to be here.
        }
        protected override Task LastPartOfSetUpBeforeBindingsAsync()
        {
            PlayerList!.ForEach(thisPlayer =>
            {
                var thisList = _model!.Deck1!.DrawSeveralCards(16);
                thisPlayer.TempHand!.ClearBoard(thisList);
            });
            EvalulateStrength();
            WhoTurn = WhoChoosesTrump();
            return base.LastPartOfSetUpBeforeBindingsAsync();
        }
        protected override void LoadVM()
        {
            base.LoadVM();
            SaveRoot!.LoadMod(_model!);
        }
        protected override async Task ComputerTurnAsync()
        {
            if (Test!.NoAnimations == false)
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
                _model!.BidAmount = _model.Bid1!.NumberToChoose();
                await _bidProcesses.ProcessBidAmountAsync(id);
                return;
            }
            if (SaveRoot.WhatStatus == EnumStatusList.ChooseTrump)
            {
                SaveRoot.TrumpSuit = _model!.Suit1!.ItemToChoose();
                await _trumpProcesses.TrumpChosenAsync();
                return;
            }
            int ask1 = _gameContainer.Random.GetRandomNumber(2);
            if (ask1 == 1)
                await _choiceProcesses.ChooseToPlayAsync();
            else
                await _choiceProcesses.ChooseToPassAsync();
        }
        protected override async Task StartSetUpAsync(bool isBeginning)
        {
            LoadControls();
            PlayerList!.ForEach(thisPlayer => thisPlayer.SavedTemp.Clear());
            LoadPlayerControls();
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
            _model.BidAmount = -1;
            _model.Bid1!.UnselectAll();
            _model.Suit1!.UnselectAll();
            await base.StartSetUpAsync(isBeginning);
            //GlobalDelegates.ManuelRepaintEnumControls.Invoke();
        }

        async Task IMiscDataNM.MiscDataReceived(string status, string content)
        {
            switch (status) //can't do switch because we don't know what the cases are ahead of time.
            {
                case "choosetoplay":
                    await _choiceProcesses.ChooseToPlayAsync();
                    break;
                case "choosetopass":
                    await _choiceProcesses.ChooseToPassAsync();
                    break;
                case "trump":
                    SaveRoot!.TrumpSuit = await js.DeserializeObjectAsync<EnumSuitList>(content);
                    await _trumpProcesses.TrumpChosenAsync();
                    break;
                case "bid":
                    int plays = PlayerList.Where(items => items.PlayerCategory == EnumPlayerCategory.OtherHuman).Single().Id;
                    _model!.BidAmount = int.Parse(content);
                    await _bidProcesses.ProcessBidAmountAsync(plays);
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
            SingleInfo.MainHandList.UnhighlightObjects(); //i think this is best.
            _command.ManuelFinish = true; //because it could be somebody else's turn.
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
            _model!.PlayerHand1!.EndTurn();
            WhoTurn = wins; //most of the time, whoever wins leads again.
            await StartNewTrickAsync();
        }
        private async Task StartNewTrickAsync()
        {
            _aTrick!.ClearBoard();
            _command!.ManuelFinish = true; //because it could be somebody else's turn.
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
            //hopefully its already hooked up to the view model (?)
            //if (SaveRoot!.WhatStatus != EnumStatusList.NormalPlay)
            //    TrickArea1!.Visible = false; //try this.
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
                if (BasicData!.MultiPlayer == false)
                    throw new BasicBlankException("If you already bidded, the computer player should have bidded then moved on");
                await SaveStateAsync();
                Check!.IsEnabled = true;
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
            if (_gameContainer.Test.DoubleCheck)
            {
                return PlayerList.GetSelf().Id; //both shows it for testing.
            }
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
            await this.RoundOverNextAsync();
        }


    }
}
