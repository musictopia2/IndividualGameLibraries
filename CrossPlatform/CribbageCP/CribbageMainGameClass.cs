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
using BasicGameFramework.RegularDeckOfCards;
using BasicGameFramework.Extensions;
using BasicGameFramework.MultiplayerClasses.Extensions;
using BasicGameFramework.NetworkingClasses.Extensions;
using BasicGameFramework.MultiplayerClasses.BasicPlayerClasses;
using BasicGameFramework.SpecializedGameTypes.RummyClasses;
using BasicGameFramework.MultiplayerClasses.InterfacesForHelpers;
using BasicGameFramework.BasicDrawables.Dictionary;
namespace CribbageCP
{
    [SingletonGame]
    public class CribbageMainGameClass : CardGameClass<CribbageCard, CribbagePlayerItem, CribbageSaveInfo>, IMiscDataNM, IStartNewGame
    {
        public CribbageMainGameClass(IGamePackageResolver container) : base(container) { }

        private CribbageViewModel? _thisMod;
        public override void Init() //decided to have all the code under init to prevent overflow issues.
        {
			base.Init();
            _thisMod = MainContainer.Resolve<CribbageViewModel>();
        }
        internal async Task GameOverAsync()
        {
            await ShowWinAsync();
        }
        private void PopulateLists()
        {
            _comboList = new CustomBasicList<CribbageCombos>();
            for (int x = 1; x <= 17; x++)
            {
                CribbageCombos thiscombo = new CribbageCombos();
                switch (x)
                {
                    case 1:
                        {
                            thiscombo.WhenUsed = EnumPlayType.AllCombos;
                            thiscombo.NumberNeeded = 15;
                            thiscombo.CardsToUse = 2;
                            thiscombo.Description = "Fifteens";
                            thiscombo.WhatEqual = EnumCribbagEequals.ToEqual;
                            thiscombo.Points = 2;
                            break;
                        }

                    case 2:
                        {
                            thiscombo.WhenUsed = EnumPlayType.InPlay;
                            thiscombo.NumberNeeded = 31;
                            thiscombo.WhatEqual = EnumCribbagEequals.ToEqual;
                            thiscombo.Description = "Thirty-one";
                            thiscombo.Points = 2;
                            break;
                        }

                    case 3:
                        {
                            thiscombo.WhenUsed = EnumPlayType.InPlay;
                            thiscombo.NumberNeeded = 31;
                            thiscombo.WhatEqual = EnumCribbagEequals.ToLessThan;
                            thiscombo.Description = "Go or last card";
                            thiscombo.Points = 1;
                            break;
                        }

                    case 4:
                        {
                            thiscombo.WhenUsed = EnumPlayType.InPlay;
                            thiscombo.JackStarter = true;
                            thiscombo.Description = "Jacks as starter card";
                            thiscombo.Points = 2;
                            break;
                        }

                    case 17:
                        {
                            thiscombo.WhenUsed = EnumPlayType.AllCombos;
                            thiscombo.NumberForKind = 2;
                            thiscombo.Group = EnumScoreGroups.ScorePairRuns;
                            thiscombo.Description = "Pairs"; // however, can have more than one pair
                            thiscombo.Points = 2;
                            thiscombo.CardsToUse = 2;
                            break;
                        }

                    case 16:
                        {
                            thiscombo.WhenUsed = EnumPlayType.AllCombos;
                            thiscombo.NumberForKind = 3;
                            thiscombo.Group = EnumScoreGroups.ScorePairRuns;
                            thiscombo.Description = "Three of a kind";
                            thiscombo.Points = 6;
                            thiscombo.CardsToUse = 3;
                            break;
                        }

                    case 15:
                        {
                            thiscombo.WhenUsed = EnumPlayType.AllCombos;
                            thiscombo.NumberForKind = 4;
                            thiscombo.Group = EnumScoreGroups.ScorePairRuns;
                            thiscombo.Description = "Four of a kind";
                            thiscombo.Points = 12;
                            thiscombo.CardsToUse = 4;
                            break;
                        }

                    case 14:
                        {
                            thiscombo.WhenUsed = EnumPlayType.AllCombos;
                            thiscombo.NumberInStraight = 3;
                            thiscombo.Group = EnumScoreGroups.ScorePairRuns;
                            thiscombo.Description = "Run of three";
                            thiscombo.Points = 3;
                            thiscombo.CardsToUse = 3;
                            break;
                        }

                    case 13:
                        {
                            thiscombo.WhenUsed = EnumPlayType.AllCombos;
                            thiscombo.NumberInStraight = 4;
                            thiscombo.Group = EnumScoreGroups.ScorePairRuns;
                            thiscombo.Description = "Run of four";
                            thiscombo.Points = 4;
                            thiscombo.CardsToUse = 4;
                            break;
                        }

                    case 12:
                        {
                            thiscombo.WhenUsed = EnumPlayType.AllCombos;
                            thiscombo.NumberInStraight = 4;
                            thiscombo.Group = EnumScoreGroups.ScorePairRuns;
                            thiscombo.WhatEqual = EnumCribbagEequals.ToGreaterThan;
                            thiscombo.Description = "Longer Runs";
                            thiscombo.Points = 1;
                            break;
                        }

                    case 11:
                        {
                            thiscombo.WhenUsed = EnumPlayType.InHandAndCrib;
                            thiscombo.NumberInStraight = 3;
                            thiscombo.NumberForKind = 2;
                            thiscombo.Description = "Double run of three";
                            thiscombo.Group = EnumScoreGroups.ScorePairRuns;
                            thiscombo.Points = 8;
                            thiscombo.CardsToUse = 4;
                            break;
                        }

                    case 10:
                        {
                            thiscombo.WhenUsed = EnumPlayType.InHandAndCrib;
                            thiscombo.NumberInStraight = 4;
                            thiscombo.NumberForKind = 2;
                            thiscombo.CardsToUse = 5;
                            thiscombo.Description = "Double run of four";
                            thiscombo.Group = EnumScoreGroups.ScorePairRuns;
                            thiscombo.Points = 10;
                            break;
                        }

                    case 9:
                        {
                            thiscombo.WhenUsed = EnumPlayType.InHandAndCrib;
                            thiscombo.NumberInStraight = 3;
                            thiscombo.NumberForKind = 3;
                            thiscombo.CardsToUse = 5;
                            thiscombo.Description = "Triple run of three";
                            thiscombo.Group = EnumScoreGroups.ScorePairRuns;
                            thiscombo.Points = 15;
                            break;
                        }

                    case 8:
                        {
                            thiscombo.WhenUsed = EnumPlayType.InHandAndCrib;
                            thiscombo.NumberForKind = 2;
                            thiscombo.DoublePairNeeded = true;
                            thiscombo.NumberInStraight = 3;
                            thiscombo.CardsToUse = 5;
                            thiscombo.Description = "Quadruple run of three";
                            thiscombo.Group = EnumScoreGroups.ScorePairRuns;
                            thiscombo.Points = 16;
                            break;
                        }

                    case 7:
                        {
                            thiscombo.WhenUsed = EnumPlayType.InHand;
                            thiscombo.IsFlush = true;
                            thiscombo.CardsToUse = 4;
                            thiscombo.Description = "Four cards of same suit";
                            thiscombo.Group = EnumScoreGroups.ScoreFlush;
                            thiscombo.Points = 4;
                            break;
                        }

                    case 6:
                        {
                            thiscombo.WhenUsed = EnumPlayType.InHandAndCrib;
                            thiscombo.IsFlush = true;
                            thiscombo.CardsToUse = 5;
                            thiscombo.Description = "Five cards of same suit";
                            thiscombo.Group = EnumScoreGroups.ScoreFlush;
                            thiscombo.Points = 5;
                            break;
                        }

                    case 5:
                        {
                            thiscombo.JackSameSuitAsStarter = true;
                            thiscombo.WhenUsed = EnumPlayType.InHandAndCrib;
                            thiscombo.CardsToUse = 1;
                            thiscombo.Points = 1;
                            thiscombo.Description = "Jack of same suit";
                            break;
                        }
                    default:
                        break;

                }
                _comboList.Add(thiscombo);
            }
        }
        public override Task PopulateSaveRootAsync()
        {
            SaveRoot!.CribList = _thisMod!.CribFrame!.HandList.ToRegularDeckDict();
            SaveRoot.MainFrameList = _thisMod.MainFrame!.HandList.ToRegularDeckDict();
            return base.PopulateSaveRootAsync();
        }
        public override Task FinishGetSavedAsync()
        {
            LoadControls();
            SaveRoot!.LoadMod(_thisMod!);
            ShowDealer();
            HookUpControls();
            if (SaveRoot.MainFrameList.Count > 0)
                _thisMod!.TotalCount = SaveRoot.MainFrameList.Sum(items =>
                {
                    if (items.Value >= EnumCardValueList.Ten)
                        return 10;
                    else
                        return (int)items.Value;
                });
            if (SaveRoot.WhatStatus == EnumGameStatus.CardsForCrib)
                StartCardsCrib();
            return base.FinishGetSavedAsync();
        }
        public override async Task ContinueTurnAsync()
        {
            if (SaveRoot!.WhatStatus == EnumGameStatus.CardsForCrib)
            {
                await SaveStateAsync(); //i think.
                SingleInfo = PlayerList!.GetSelf();
                if (SingleInfo.ChoseCrib == true)
                {
                    if (ThisData!.MultiPlayer == false)
                        throw new BasicBlankException("Computer should have already chosen for crib.  Rethink");
                    ThisCheck!.IsEnabled = true;
                    return;
                }
                if (ThisData!.MultiPlayer)
                    ThisCheck!.IsEnabled = false; //try this too.
                _thisMod!.NormalTurn = "None"; //because both players at the same time has to choose cards for crib.
                await ShowHumanCanPlayAsync();
                return;
            }
            await base.ContinueTurnAsync();
        }
        private void HookUpControls()
        {
            _thisMod!.MainFrame!.HandList.ReplaceRange(SaveRoot!.MainFrameList);
            _thisMod.CribFrame!.HandList.ReplaceRange(SaveRoot.CribList);
        }
        protected override async Task LastPartOfSetUpBeforeBindingsAsync()
        {
            HookUpControls();
            await base.LastPartOfSetUpBeforeBindingsAsync();
        }
        private RummyProcesses<EnumSuitList, EnumColorList, CribbageCard>? _rummys;

        private CustomBasicList<CribbageCombos>? _comboList;
        private void LoadControls()
        {
            if (IsLoaded == true)
                return;
            _rummys = new RummyProcesses<EnumSuitList, EnumColorList, CribbageCard>();
            _rummys.HasSecond = false;
            _rummys.HasWild = false;
            _rummys.LowNumber = 1;
            _rummys.HighNumber = 13;
            _rummys.NeedMatch = false;
            _rummys.UseAll = true;
            _thisMod!.LoadControls();
            PopulateLists();
            IsLoaded = true; //i think needs to be here.
        }
        protected override async Task StartSetUpAsync(bool isBeginning)
        {
            LoadControls();
            if (SaveRoot!.Dealer == 0)
                SaveRoot!.Dealer = await PlayerList!.CalculateOldTurnAsync();
            else
            {
                int olds = WhoTurn;
                WhoTurn = SaveRoot.Dealer;
                SaveRoot!.Dealer = await PlayerList!.CalculateWhoTurnAsync();
                WhoTurn = olds; //since its okay later.
            }
            ShowDealer();
            SaveRoot.WhatStatus = EnumGameStatus.CardsForCrib;
            StartCardsCrib();
            SaveRoot.CribList.Clear();
            SaveRoot.MainFrameList.Clear();
            SaveRoot.MainList.Clear();
            PlayerList.ForEach(thisPlayer =>
            {
                thisPlayer.ScoreRound = 0;
                thisPlayer.ChoseCrib = false;
                thisPlayer.FinishedLooking = false;
                
            });
            SaveRoot.LoadMod(_thisMod!);
            await base.StartSetUpAsync(isBeginning);
        }
        private void StartCardsCrib()
        {
            ClearItems();
            _thisMod!.TotalScore = 0;
            _thisMod.ScoreBoard1!.ResetScores(); //i think.
        }
        private void ClearItems()
        {
            _thisMod!.CribFrame!.HandList.Clear();
            _thisMod.MainFrame!.HandList.Clear();
            SaveRoot!.MainList.Clear(); //i think.
        }
        private void ShowDealer()
        {
            SingleInfo = PlayerList![SaveRoot!.Dealer];
            _thisMod!.Dealer = SingleInfo.NickName;
        }
        protected override async Task ComputerTurnAsync()
        {
            if (ThisTest!.NoAnimations == false)
                await Delay!.DelaySeconds(.25);
            if (SaveRoot!.WhatStatus == EnumGameStatus.CardsForCrib)
            {
                int counts;
                if (PlayerList.Count() == 2)
                    counts = 2;
                else
                    counts = 1;
                var thisCol = SingleInfo!.MainHandList.GetRandomList(false, counts);
                await ProcessCribAsync(thisCol.ToRegularDeckDict());
                return;
            }
            if (SaveRoot.WhatStatus == EnumGameStatus.PlayCard)
            {
                var thisCol = ValidMoveList();
                if (thisCol.Count == 0)
                    throw new BasicBlankException("There has to be at least one valid move.");
                await PlayCardAsync(thisCol.GetRandomItem());
                return;
            }
        }
        async Task IMiscDataNM.MiscDataReceived(string status, string content)
        {
            switch (status)
            {
                case "cardsforcrib":
                    SendCrib thiss = await js.DeserializeObjectAsync<SendCrib>(content);
                    WhoTurn = thiss.Player;
                    SingleInfo = PlayerList!.GetWhoPlayer();
                    if (thiss.CardList.Count == 0)
                        throw new BasicBlankException("Player cannot send you 0 cards");
                    await ProcessCribAsync(thiss.CardList);
                    break;
                case "playcard":
                    await PlayCardAsync(int.Parse(content));
                    break;
                case "endview":
                    SingleInfo = PlayerList![int.Parse(content)];
                    SingleInfo.FinishedLooking = true;
                    await EndViewAsync();
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
            _thisMod!.CommandContainer!.ManuelFinish = true; //because it could be somebody else's turn.
            WhoTurn = await PlayerList.CalculateWhoTurnAsync();
            await StartNewTurnAsync();
        }
        protected override void SetHand()
        {
            _thisMod!.PlayerHand1!.HandList.ReplaceRange(SingleInfo!.MainHandList); //somehow this was needed this time.
        }
        public bool IsValidMove(CribbageCard thisCard)
        {
            int thisNumber = (int)thisCard.Value;
            if (thisNumber > 10)
                thisNumber = 10;
            return thisNumber + _thisMod!.TotalCount <= 31;
        }
        public DeckRegularDict<CribbageCard> ValidMoveList()
        {
            return SingleInfo!.MainHandList.Where(items => IsValidMove(items)).ToRegularDeckDict();
        }
        public bool WillEndRound => PlayerList.All(items => items.MainHandList.Count == 0);
        internal async Task NextStepAsync()
        {
            if (SaveRoot!.WhatStatus == EnumGameStatus.PlayCard)
            {
                if (WillEndRound == true)
                {
                    ResetCardsInPlay();
                    await StartCountingCardsAsync();
                    return;
                }
                if (SaveRoot.NewTurn > 0)
                {
                    WhoTurn = SaveRoot.NewTurn;
                    SaveRoot.NewTurn = 0;
                }
                if (SaveRoot.StartOver == true)
                {
                    if (SaveRoot.IsStart == true)
                    {
                        if (SaveRoot.IsCorrect == false)
                        {
                            WhoTurn = await PlayerList!.CalculateWhoTurnAsync();
                        }
                        SaveRoot.IsCorrect = false;
                        SaveRoot.IsStart = false;
                    }
                    await StartSectionAsync();
                    return;
                }
                await StartNewTurnAsync();
                return;
            }
            if (SaveRoot.WhatStatus == EnumGameStatus.GetResultsCrib || SaveRoot.WhatStatus == EnumGameStatus.GetResultsHand)
            {
                if (ThisData!.MultiPlayer == true)
                    ThisCheck!.IsEnabled = true;
                var tempPlayer = PlayerList!.GetSelf();
                if (tempPlayer.FinishedLooking == false)
                    await ShowHumanCanPlayAsync(); //i think
            }
        }
        public async Task StartCountingCardsAsync()
        {
            SaveRoot!.MainList.ForEach(thisCard =>
            {
                SingleInfo = PlayerList![thisCard.Player];
                SingleInfo.MainHandList.Add(thisCard);
            });
            SingleInfo = PlayerList!.GetSelf();
            SaveRoot.WhatStatus = EnumGameStatus.GetResultsHand;
            WhoTurn = WhoStarts;
            PlayerList.ForEach(thisPlayer => thisPlayer.InGame = true);
            await ResultsFromHandOrCribAsync();
        }
        public void ResetCardsInPlay()
        {
            _thisMod!.TotalCount = 0;
            _thisMod.ScoreBoard1!.ResetScores();
            _thisMod.TotalCount = 0;
            if (_thisMod.MainFrame!.HandList.Any(items => items.Player == 0))
                throw new BasicBlankException("The player cannot be 0");
            SaveRoot!.MainList.AddRange(_thisMod.MainFrame.HandList);
            _thisMod.MainFrame.HandList.Clear();
        }
        public async Task StartSectionAsync()
        {
            SaveRoot!.StartOver = false;
            ResetCardsInPlay();
            await StartNewTurnAsync();
        }
        public async Task EndViewAsync()
        {
            _thisMod!.CommandContainer!.ManuelFinish = true;
            if (ThisData!.MultiPlayer == false)
            {
                await FinishEndAsync();
                return;
            }
            var tempList = PlayerList!.GetAllComputerPlayers(); //hopefully this works too.
            tempList.ForEach(thisPlayer => thisPlayer.FinishedLooking = true);
            if (PlayerList.Any(items => items.FinishedLooking == false))
            {
                ThisCheck!.IsEnabled = true;
                return;
            }
            await FinishEndAsync();
        }
        private async Task FinishEndAsync()
        {
            PlayerList!.ForEach(items => items.FinishedLooking = false);
            if (SaveRoot!.WhatStatus == EnumGameStatus.GetResultsHand && WhoTurn == SaveRoot.Dealer)
            {
                SaveRoot.WhatStatus = EnumGameStatus.GetResultsCrib;
                await ResultsFromHandOrCribAsync();
                return;
            }
            if (SaveRoot.WhatStatus == EnumGameStatus.GetResultsHand)
            {
                WhoTurn = await PlayerList.CalculateWhoTurnAsync();
                await ResultsFromHandOrCribAsync();
                return;
            }
            if (SaveRoot.WhatStatus == EnumGameStatus.GetResultsCrib)
            {
                if (ThisData!.MultiPlayer == true && ThisData.Client == true)
                {
                    ThisCheck!.IsEnabled = true; //to wait to hear back from host again.
                    return;
                }
                WhoTurn = WhoStarts;
                WhoTurn = await PlayerList.CalculateWhoTurnAsync();
                await SetUpGameAsync(false); //hopefully this works.
                return;
            }
        }
        private async Task ResultsFromHandOrCribAsync()
        {
            SingleInfo = PlayerList!.GetWhoPlayer();
            this.ShowTurn();
            DeckRegularDict<CribbageCard> newList = new DeckRegularDict<CribbageCard>();
            CustomBasicList<CribbageCombos> thisCol;
            if (SaveRoot!.WhatStatus == EnumGameStatus.GetResultsHand)
            {
                newList.AddRange(SingleInfo.MainHandList);
                thisCol = ListCribbageCombos(newList, false);
                PrivateSortCards();
            }
            else
            {
                SingleInfo.MainHandList.Clear();
                _thisMod!.PlayerHand1!.HandList.Clear();
                newList.AddRange(_thisMod.CribFrame!.HandList);
                thisCol = ListCribbageCombos(newList, true);
                SortCards(_thisMod.CribFrame.HandList); //hopefully this works.
            }
            if (WhoTurn == 0)
                throw new BasicBlankException("The whoturn cannot be 0");
            await ProcessScoreAsync(thisCol);
        }
        public async Task ProcessScoreAsync(CustomBasicList<CribbageCombos> thisCol)
        {
            _thisMod!.ScoreBoard1!.ResetScores();
            _thisMod.TotalScore = 0;
            thisCol.ForEach(thisCribbage =>
            {
                _thisMod.ScoreBoard1.AddScore(thisCribbage.Description, thisCribbage.Points);
            });
            _thisMod.TotalScore = _thisMod.ScoreBoard1.TotalScore;
            _thisMod.ScoreBoard1.ShowScores();
            if (_thisMod.TotalScore > 0 && ThisTest!.NoAnimations == false)
                await Delay!.DelaySeconds(1.5);
            SingleInfo = PlayerList!.GetWhoPlayer();
            if (WhoTurn == 0)
                throw new BasicBlankException("The whoturn cannot be 0");
            await _thisMod.GameBoard1!.PegAsync(_thisMod.ScoreBoard1.TotalScore);
        }
        private void PrivateSortCards()
        {
            if (SingleInfo!.MainHandList.Count != 4 && PlayerList.Count() == 2)
                throw new BasicBlankException("There are only 2 players.  Therefore; should have 4 cards.  Find out what happened");
            _thisMod!.PlayerHand1!.HandList.ReplaceRange(SingleInfo.MainHandList); //for some strange reason, i am forced to put to other.  otherwise, it deletes the other.
        }
        private CustomBasicList<CribbageCombos> ListCribbageCombos(IDeckDict<CribbageCard> thisCol, bool fromCrib)
        {
            CustomBasicList<CribbageCombos> output = new CustomBasicList<CribbageCombos>();
            var mostSuits = thisCol.GroupOrderDescending(items => items.Suit);
            bool hadFourFlush;
            bool hadFiveFlush;
            var startCard = _thisMod!.Pile1!.GetCardInfo();
            startCard.HasUsed = false;
            thisCol.ForEach(thisCard => thisCard.HasUsed = false);
            if (mostSuits.First().Count() == 4 && startCard.Suit == mostSuits.First().Key)
            {
                hadFiveFlush = true;
                hadFourFlush = false;
            }
            else if (mostSuits.First().Count() == 4 && fromCrib == false)
            {
                hadFourFlush = true;
                hadFiveFlush = true;
            }
            else
            {
                hadFourFlush = false;
                hadFiveFlush = false;
            }
            bool hadMultiMove = false;
            int longs = 0;
            int pairss = 0;
            bool hadLongerRun = false;
            bool hadStraight = false;
            bool hadKind = false;
            int fifs;
            CribbageCombos newCombo;
            _comboList!.ForEach(thisCombo =>
            {
                if (thisCombo.WhenUsed == EnumPlayType.AllCombos | thisCombo.WhenUsed == EnumPlayType.InHand & fromCrib == false | thisCombo.WhenUsed == EnumPlayType.InHandAndCrib)
                {
                    if (thisCombo.NumberNeeded == 15)
                    {
                        fifs = Find15Combos(thisCol);
                        if (fifs > 0)
                        {
                            newCombo = new CribbageCombos();
                            newCombo.Description = "Fifteens";
                            newCombo.Points = fifs * 2;
                            output.Add(newCombo);
                        }
                    }
                    else if (thisCombo.IsFlush == true)
                    {
                        if (thisCombo.CardsToUse == 5 && hadFiveFlush == true)
                            output.Add(thisCombo);
                        else if (thisCombo.CardsToUse == 4 && hadFourFlush == true)
                            output.Add(thisCombo);
                    }
                    else if (thisCombo.JackSameSuitAsStarter)
                    {
                        if (thisCol.Any(items => items.Value == EnumCardValueList.Jack && items.Suit == startCard.Suit))
                            output.Add(thisCombo);
                    }
                    else if (thisCombo.NumberForKind > 0 && thisCombo.NumberInStraight > 0)
                    {
                        if (hadMultiMove == false)
                        {
                            hadMultiMove = IsMultiMove(thisCombo, thisCol);
                            if (hadMultiMove == true)
                                output.Add(thisCombo);
                        }
                    }
                    else if (thisCombo.NumberInStraight > 0 && thisCombo.WhatEqual == EnumCribbagEequals.ToGreaterThan)
                    {
                        longs = HowManyLongerRun(thisCol, false);
                        if (longs > 0)
                        {
                            newCombo = new CribbageCombos();
                            newCombo.Description = thisCombo.Description;
                            newCombo.Points = longs;
                            output.Add(newCombo);
                            hadLongerRun = true;
                        }
                    }
                    else if (thisCombo.NumberInStraight == 4 && hadLongerRun == true)
                    {
                        output.Add(thisCombo);
                    }
                    else if (thisCombo.NumberInStraight > 0)
                    {
                        hadStraight = HadProperStraight(thisCol, thisCombo, false);
                        if (hadStraight == true)
                            output.Add(thisCombo);
                    }
                    else if (thisCombo.NumberForKind > 2)
                    {
                        hadKind = HadProperKind(thisCol, thisCombo, false);
                        if (hadKind == true)
                            output.Add(thisCombo);
                    }
                    else if (thisCombo.NumberForKind == 2)
                    {
                        pairss = HowManyPairs(thisCol, false);
                        if (pairss > 0)
                        {
                            newCombo = new CribbageCombos();
                            newCombo.Description = thisCombo.Description;
                            newCombo.Points = pairss * thisCombo.Points;
                            output.Add(newCombo);
                        }
                    }
                    else
                    {
                        throw new BasicBlankException("Combo Not Supported.  Rethink");
                    }
                }

            });
            return output;
        }
        public CustomBasicList<CribbageCombos> ListComboPointsInPlay(int numberSoFar, out bool startOver, out int whoNext)
        {
            CustomBasicList<CribbageCombos> output = new CustomBasicList<CribbageCombos>();
            startOver = false;
            whoNext = 0; //i think
            if (_thisMod!.MainFrame!.HandList.Count == 0 && numberSoFar == 0)
            {
                if (_thisMod.Pile1!.GetCardInfo().Value == EnumCardValueList.Jack)
                {
                    var tempCombo = _comboList.Where(items => items.JackStarter == true).First();
                    output.Add(tempCombo);
                }
                return output;
            }
            int diffs = 31 - numberSoFar;
            whoNext = WhoHad(diffs);
            if (whoNext == 0)
                startOver = true;
            DeckRegularDict<CribbageCard> newCol = new DeckRegularDict<CribbageCard>();
            int y;
            foreach (var thisCombo in _comboList!)
            {
                if (thisCombo.CardsToUse > 0)
                {
                    newCol = new DeckRegularDict<CribbageCard>();
                    y = _thisMod.MainFrame.HandList.Count;
                    for (int x = 1; x <= thisCombo.CardsToUse; x++)
                    {
                        var thisCard = _thisMod.MainFrame.HandList[y - 1];
                        newCol.Add(thisCard);
                        y--;
                        if (y == 0)
                            break;
                    }
                }
                int manys;
                CribbageCombos newCombo;
                bool hadss;
                bool hadThree = false;
                bool hadFour = false;
                bool hadTwo = false;
                bool hadThreeRun = false;
                if (thisCombo.JackSameSuitAsStarter == false && (thisCombo.WhenUsed == EnumPlayType.AllCombos || thisCombo.WhenUsed == EnumPlayType.InPlay) && thisCombo.JackStarter == false)
                {
                    if (thisCombo.NumberNeeded == 31 && thisCombo.WhatEqual == EnumCribbagEequals.ToEqual)
                    {
                        if (numberSoFar == 31)
                            output.Add(thisCombo);
                    }
                    else if (thisCombo.WhatEqual == EnumCribbagEequals.ToLessThan)
                    {
                        if (startOver == true && numberSoFar < thisCombo.NumberNeeded)
                            output.Add(thisCombo);
                    }
                    else if (thisCombo.WhatEqual == EnumCribbagEequals.ToEqual && startOver == true && numberSoFar == thisCombo.NumberNeeded)
                    {
                        output.Add(thisCombo);
                    }
                    else if (thisCombo.NumberNeeded == 15)
                    {
                        if (numberSoFar == 15)
                            output.Add(thisCombo);
                    }
                    else if (thisCombo.WhatEqual == EnumCribbagEequals.ToGreaterThan)
                    {
                        manys = HowManyLongerRun(newCol, true);
                        if (manys > 0)
                        {
                            newCombo = new CribbageCombos();
                            newCombo.Description = thisCombo.Description;
                            newCombo.Points = manys;
                            output.Add(newCombo); //not sure.
                        }
                    }
                    else if (thisCombo.NumberInStraight > 0)
                    {
                        hadss = HadProperStraight(newCol, thisCombo, true);
                        if (hadThree == true)
                            hadss = false;
                        if (hadss == true)
                        {
                            if (thisCombo.NumberInStraight == 3)
                                hadThreeRun = true;
                            output.Add(thisCombo);
                            if (hadThreeRun == true && thisCombo.NumberInStraight > 3)
                                output.KeepConditionalItems(items => items.NumberInStraight != 3);
                        }
                    }
                    else if (thisCombo.NumberForKind > 0)
                    {
                        hadss = HadProperKind(newCol, thisCombo, true);
                        if (hadFour == true)
                            hadss = false;
                        else if (hadThree == true && thisCombo.NumberForKind < 4)
                            hadss = false;
                        if (hadss == true)
                        {
                            if (thisCombo.NumberForKind == 2)
                                hadTwo = true;
                            else if (thisCombo.NumberForKind == 3)
                                hadThree = true;
                            else if (thisCombo.NumberForKind == 4)
                                hadFour = true;
                            output.Add(thisCombo);
                            if (hadTwo == true && thisCombo.NumberForKind > 2)
                                output.KeepConditionalItems(items => items.NumberForKind != 2);
                            else if (hadThree == true && thisCombo.NumberForKind > 3)
                                output.KeepConditionalItems(items => items.NumberForKind != 3);
                        }
                    }
                    else
                    {
                        throw new BasicBlankException("No combo.  Rethink");
                    }
                }
            }
            return output;
        }
        public async Task PlayCardAsync(int Deck)
        {
            var thisCard = SingleInfo!.MainHandList.GetSpecificItem(Deck);
            await PlayCardAsync(thisCard);
        }
        public async Task PlayCardAsync(CribbageCard thisCard)
        {
            _thisMod!.CommandContainer!.ManuelFinish = true;
            SingleInfo = PlayerList!.GetWhoPlayer();
            if (SingleInfo.CanSendMessage(ThisData!) == true)
                await ThisNet!.SendAllAsync("playcard", thisCard.Deck);
            SingleInfo.MainHandList.RemoveObjectByDeck(thisCard.Deck);
            if (SingleInfo.PlayerCategory == EnumPlayerCategory.Self)
                _thisMod.PlayerHand1!.HandList.RemoveObjectByDeck(thisCard.Deck);
            thisCard.Player = WhoTurn;
            if (thisCard.Player == 0)
                throw new BasicBlankException("The player cannot be 0");
            thisCard.IsSelected = false;
            if (SingleInfo.MainHandList.Count == 0)
                SingleInfo.InGame = false;
            _thisMod.MainFrame!.HandList.Add(thisCard);
            if (thisCard.Value < EnumCardValueList.Ten)
                _thisMod.TotalCount += (int)thisCard.Value;
            else
                _thisMod.TotalCount += 10;
            var thisCol = ListComboPointsInPlay(_thisMod.TotalCount, out bool starts, out int nextTurn);
            SaveRoot!.StartOver = starts;
            if (nextTurn > 0)
                SaveRoot.NewTurn = nextTurn;
            else
                SaveRoot.NewTurn = await PlayerList.CalculateWhoTurnAsync(true);
            if (WhoTurn == 0)
                throw new BasicBlankException("The whoturn cannot be 0");
            await ProcessScoreAsync(thisCol);
        }
        #region "rummy functions"
        public int Find15Combos(IDeckDict<CribbageCard> thisCol)
        {
            DeckRegularDict<CribbageCard> newCol = new DeckRegularDict<CribbageCard>();
            newCol.AddRange(thisCol);
            newCol.Add(_thisMod!.Pile1!.GetCardInfo());
            int firstNumber;
            int secondNumber;
            int output = 0;
            CribbageCard secondCard;
            int x = 0;
            newCol.ForEach(firstCard =>
            {
                if (firstCard.Value >= EnumCardValueList.Ten)
                    firstNumber = 10;
                else
                    firstNumber = (int)firstCard.Value;
                var loopTo = newCol.Count - 1; // because 0 based
                for (int y = x + 1; y <= loopTo; y++)
                {
                    secondCard = newCol[y];
                    if (secondCard.Value >= EnumCardValueList.Ten)
                        secondNumber = 10;
                    else
                        secondNumber = (int)secondCard.Value;
                    if (firstNumber + secondNumber == 15)
                        output++;

                }
                x += 1;
            });
            return output;
        }
        private int HowManyPairs(IDeckDict<CribbageCard> thisCol, bool fromPlay)
        {
            var newCol = thisCol.ToRegularDeckDict();
            int output = 0;
            CribbageCard newCard;
            DeckRegularDict<CribbageCard> finals = new DeckRegularDict<CribbageCard>();
            if (fromPlay == false)
            {
                newCol.Add(_thisMod!.Pile1!.GetCardInfo());
                finals.AddRange(newCol);
                finals.KeepConditionalItems(items => items.HasUsed == false);
                newCol.Clear();
                newCol.AddRange(finals);
            }
            var whatNewRummy = _rummys!.WhatNewRummy(newCol, 2, RummyProcesses<EnumSuitList, EnumColorList, CribbageCard>.EnumRummyType.Sets, false);
            if (whatNewRummy.Count == 0)
                return 0;
            if (whatNewRummy.Count > 2)
            {
                finals.AddRange(newCol);
                newCard = whatNewRummy.First();
                finals.KeepConditionalItems(items => items.Value != newCard.Value);
                newCol.Clear();
                newCol.AddRange(finals);
                whatNewRummy = _rummys.WhatNewRummy(newCol, 2, RummyProcesses<EnumSuitList, EnumColorList, CribbageCard>.EnumRummyType.Sets, false);
                if (whatNewRummy.Count == 0)
                    return 0;
                finals.Clear();
            }
            output++;
            newCard = whatNewRummy.First();
            finals.Clear();
            finals.AddRange(newCol);
            finals.KeepConditionalItems(items => items.Value != newCard.Value); //i think
            newCol.Clear();
            newCol.AddRange(finals);
            whatNewRummy = _rummys.WhatNewRummy(newCol, 2, RummyProcesses<EnumSuitList, EnumColorList, CribbageCard>.EnumRummyType.Sets, false);
            if (whatNewRummy.Count == 0)
                return output;
            if (whatNewRummy.Count > 2)
                return output;
            return output + 1;
        }
        public int HowManyLongerRun(IDeckDict<CribbageCard> thisCol, bool fromPlay)
        {
            var newCol = thisCol.ToRegularDeckDict();
            DeckRegularDict<CribbageCard> finals = new DeckRegularDict<CribbageCard>();
            if (fromPlay == false)
            {
                newCol.Add(_thisMod!.Pile1!.GetCardInfo());
                finals.AddRange(newCol);
                finals.KeepConditionalItems(items => items.HasUsed == false);
                newCol.Clear();
                newCol.AddRange(finals);
            }
            var whatNewRummy = _rummys!.WhatNewRummy(newCol, 5, RummyProcesses<EnumSuitList, EnumColorList, CribbageCard>.EnumRummyType.Runs, false);
            if (whatNewRummy.Count == 0)
                return 0;
            return whatNewRummy.Count - 4;
        }
        public bool HadProperKind(IDeckDict<CribbageCard> thisCol, CribbageCombos thisCombo, bool fromPlay)
        {
            var newCol = thisCol.ToRegularDeckDict();
            DeckRegularDict<CribbageCard> finals = new DeckRegularDict<CribbageCard>();
            if (fromPlay == false)
            {
                newCol.Add(_thisMod!.Pile1!.GetCardInfo());
                finals.AddRange(newCol);
                finals.KeepConditionalItems(items => items.HasUsed == false);
                newCol.Clear();
                newCol.AddRange(finals);
            }
            var whatNewRummy = _rummys!.WhatNewRummy(newCol, thisCombo.NumberForKind, RummyProcesses<EnumSuitList, EnumColorList, CribbageCard>.EnumRummyType.Sets, false);
            if (whatNewRummy.Count == 0)
                return false;
            if (whatNewRummy.Count > thisCombo.NumberForKind)
                return false;
            whatNewRummy.ForEach(thisCard => thisCard.HasUsed = true);
            return true;
        }
        public bool HadProperStraight(IDeckDict<CribbageCard> thisCol, CribbageCombos thisCombo, bool fromPlay)
        {
            var newCol = thisCol.ToRegularDeckDict();
            DeckRegularDict<CribbageCard> finals = new DeckRegularDict<CribbageCard>();
            if (fromPlay == false)
            {
                newCol.Add(_thisMod!.Pile1!.GetCardInfo());
                finals.AddRange(newCol);
                finals.KeepConditionalItems(items => items.HasUsed == false);
                newCol.Clear();
                newCol.AddRange(finals);
            }
            var whatNewRummy = _rummys!.WhatNewRummy(newCol, thisCombo.NumberInStraight, RummyProcesses<EnumSuitList, EnumColorList, CribbageCard>.EnumRummyType.Runs, false);
            if (whatNewRummy.Count == 0)
                return false;
            if (whatNewRummy.Count > thisCombo.NumberInStraight)
            {
                if (fromPlay == true)
                    return false;
                else
                    if (thisCombo.NumberInStraight == 4)
                    return true;
                return false;
            }
            return true;
        }
        private bool IsMultiMove(CribbageCombos thisCombo, IDeckDict<CribbageCard> thisCol)
        {
            var newCol = thisCol.ToRegularDeckDict();
            newCol.Add(_thisMod!.Pile1!.GetCardInfo());
            var whatNewRummy = _rummys!.WhatNewRummy(newCol, thisCombo.NumberInStraight, RummyProcesses<EnumSuitList, EnumColorList, CribbageCard>.EnumRummyType.Runs, false);
            if (whatNewRummy.Count == 0)
                return false;
            if (whatNewRummy.Count > thisCombo.NumberInStraight)
                return false;
            newCol.ForEach(thisCard => thisCard.HasUsed = true);
            EnumCardValueList firstNumber = whatNewRummy.First().Value;
            EnumCardValueList secondNumber = whatNewRummy.Last().Value;
            var lastTemp = newCol.ToRegularDeckDict();
            lastTemp.KeepConditionalItems(items => items.Value >= firstNumber || items.Value <= secondNumber);
            newCol.Clear();
            newCol.AddRange(lastTemp);
            whatNewRummy = _rummys.WhatNewRummy(newCol, thisCombo.NumberForKind, RummyProcesses<EnumSuitList, EnumColorList, CribbageCard>.EnumRummyType.Sets, false);
            if (whatNewRummy.Count == 0 || whatNewRummy.Count > thisCombo.NumberForKind)
            {
                thisCol.ForEach(thisCard => thisCard.HasUsed = false);
                _thisMod.Pile1.GetCardInfo().HasUsed = false;
                return false;
            }
            if (thisCombo.DoublePairNeeded == true)
            {
                lastTemp.KeepConditionalItems(items => items.Value != whatNewRummy.First().Value);
                newCol.Clear();
                newCol.AddRange(lastTemp);
                whatNewRummy = _rummys.WhatNewRummy(newCol, thisCombo.NumberForKind, RummyProcesses<EnumSuitList, EnumColorList, CribbageCard>.EnumRummyType.Sets, false);
                if (whatNewRummy.Count == 0 || whatNewRummy.Count > thisCombo.NumberForKind)
                {
                    thisCol.ForEach(thisCard => thisCard.HasUsed = false);
                    _thisMod.Pile1.GetCardInfo().HasUsed = false;
                    return false;
                }
                whatNewRummy.ForEach(thisCard => thisCard.HasUsed = true);
            }
            return true;
        }
        #endregion
        private int WhoHad(int diffs)
        {
            if (WillEndRound == true)
                return 0;
            if (diffs == 0)
                return 0;
            int oldTurn = WhoTurn;
            int output;
            if (diffs > 9)
            {
                WhoTurn = PlayerList!.SimpleCalulcateWhoTurn(true);
                SingleInfo = PlayerList.GetWhoPlayer();
                if (SingleInfo.MainHandList.Count > 0)
                {
                    output = WhoTurn;
                    WhoTurn = oldTurn;
                    return output;
                }
            }
            int xx = 0;
            int minNumber;
            DeckRegularDict<CribbageCard> temps;
            do
            {
                xx++;
                WhoTurn = PlayerList!.SimpleCalulcateWhoTurn(true);
                SingleInfo = PlayerList.GetWhoPlayer(); //i think this was needed.  if i am wrong, rethink.
                if (SingleInfo.MainHandList.Count > 0)
                {
                    temps = SingleInfo.MainHandList.ToRegularDeckDict();
                    minNumber = (int)temps.OrderBy(items => items.Value).First().Value;
                    if (minNumber <= diffs)
                    {
                        output = WhoTurn;
                        WhoTurn = oldTurn;
                        return output;
                    }
                }
                if (oldTurn == WhoTurn)
                    return 0;
                if (xx > 7)
                    return 0;
            } while (true);
        }
        private async Task StartPlayingAsync()
        {
            SaveRoot!.IsStart = false;
            SaveRoot.WhatStatus = EnumGameStatus.PlayCard;
            WhoTurn = SaveRoot.Dealer;
            WhoTurn = await PlayerList!.CalculateWhoTurnAsync();
            WhoStarts = WhoTurn; //try this way.
            var thisCol = ListComboPointsInPlay(0, out _, out _);
            if (thisCol.Count > 0)
            {
                SaveRoot.StartOver = true;
                SaveRoot.IsStart = true;
                SaveRoot.IsCorrect = true;
                if (WhoTurn == 0)
                    throw new BasicBlankException("The whoturn cannot be 0");
                await ProcessScoreAsync(thisCol);
                return;
            }
            await StartSectionAsync();
        }
        public async Task ProcessCribAsync(DeckRegularDict<CribbageCard> thisCol)
        {
            _thisMod!.CommandContainer!.ManuelFinish = true;
            if (SingleInfo!.CanSendMessage(ThisData!) == true)
            {
                SendCrib thiss = new SendCrib();
                thiss.CardList = thisCol;
                thiss.Player = PlayerList!.GetSelf().Id;
                await ThisNet!.SendAllAsync("cardsforcrib", thiss);
            }
            thisCol.UnselectAllObjects();
            SingleInfo!.MainHandList.RemoveSelectedItems(thisCol); //hopefully its fine here.
            if (SingleInfo.PlayerCategory == EnumPlayerCategory.Self)
                _thisMod.PlayerHand1!.HandList.RemoveSelectedItems(thisCol);
            SingleInfo.ChoseCrib = true;
            _thisMod.CribFrame!.HandList.AddRange(thisCol);
            await SaveStateAsync(); //hopefully this works (?)
            if (PlayerList.All(items => items.ChoseCrib == true))
            {
                if (_thisMod.CribFrame.HandList.Count != 4 && PlayerList.Count() == 2)
                    throw new BasicBlankException("Must have 4 cards in crib before starting turn");
                await StartPlayingAsync();
                return;
            }
            if (ThisData!.MultiPlayer == false)
            {
                SingleInfo = PlayerList.FirstOrDefault(items => items.PlayerCategory == EnumPlayerCategory.Computer && items.ChoseCrib == false);
                if (SingleInfo == null)
                    throw new BasicBlankException("Everyone has chosen cards for crib.  Therefore, there is a problem");
                await ComputerTurnAsync();
                return;
            }
            ThisCheck!.IsEnabled = true;
        }
        Task IStartNewGame.ResetAsync()
        {
            PlayerList!.ForEach(thisPlayer =>
            {
                thisPlayer.IsSkunk = true; //has to reset that part too obviously.
                thisPlayer.FirstPosition = 0;
                thisPlayer.SecondPosition = 0;
                thisPlayer.TotalScore = 0;
            });
            return Task.CompletedTask;
        }
    }
}