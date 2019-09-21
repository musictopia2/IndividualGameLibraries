using BasicGameFramework.Attributes;
using BasicGameFramework.BasicDrawables.Dictionary;
using BasicGameFramework.CommonInterfaces;
using BasicGameFramework.Extensions;
using BasicGameFramework.RegularDeckOfCards;
using BasicGameFramework.SpecializedGameTypes.RummyClasses;
using BasicGameFramework.ViewModelInterfaces;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.Exceptions;
using System.Linq;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace CribbagePatienceCP
{
    [SingletonGame]
    public class CribbagePatienceGameClass : RegularDeckOfCardsGameClass<CribbageCard>
    {
        public CribbagePatienceSaveInfo SaveRoot;

        private readonly ISaveSinglePlayerClass _thisState;

        internal readonly CribbagePatienceViewModel ThisMod;

        internal bool GameGoing;
        private RummyProcesses<EnumSuitList, EnumColorList, CribbageCard>? _Rummys;

        internal CustomBasicList<CribbageCombos> ComboList = new CustomBasicList<CribbageCombos>();
        public CribbagePatienceGameClass(ISoloCardGameVM<CribbageCard> thisMod) : base(thisMod)
        {
            _thisState = thisMod.MainContainer!.Resolve<ISaveSinglePlayerClass>();
            ThisMod = (CribbagePatienceViewModel)thisMod;
            SaveRoot = new CribbagePatienceSaveInfo();
            SaveRoot.LoadMod(ThisMod); //unless told otherwise since it is single player.
        }
        public override Task NewGameAsync()
        {
            if (_Rummys == null)
            {
                _Rummys = new RummyProcesses<EnumSuitList, EnumColorList, CribbageCard>();
                _Rummys.HasSecond = false;
                _Rummys.HasWild = false;
                _Rummys.LowNumber = 1;
                _Rummys.HighNumber = 13;
                _Rummys.NeedMatch = false;
                _Rummys.UseAll = true;
                PopulateLists(); //i think this should be done here too.
            }
            GameGoing = true;
            return base.NewGameAsync();
        }
        public override async Task<bool> CanOpenSavedSinglePlayerGameAsync()
        {
            return await _thisState.CanOpenSavedSinglePlayerGameAsync();
        }

        public override async Task OpenSavedGameAsync()
        {
            DeckList.OrderedObjects(); //i think
            SaveRoot = await _thisState.RetrieveSinglePlayerGameAsync<CribbagePatienceSaveInfo>();
            if (SaveRoot.DeckList.Count > 0)
            {
                var newList = SaveRoot.DeckList.GetNewObjectListFromDeckList(DeckList);
                ThisMod.DeckPile!.OriginalList(newList);
                ThisMod.DeckPile.Visible = true;
            }
            //anything else that is needed to open the saved game will be here.
            ThisMod.StartPile!.SavedDiscardPiles(SaveRoot.StartCard!);
            SaveRoot.LoadMod(ThisMod);
            ThisMod.Scores!.Reload();
            var thisCategory = CalculateCurrentHandCategory();
            if (thisCategory == EnumHandCategory.Crib)
            {
                ThisMod.Hand1!.Visible = false;
                ThisMod.Hand1.Text = "None"; //to stop from having problems on tablets.
                ThisMod.TempCrib!.Visible = false;
                if (ThisMod.DeckPile!.IsEndOfDeck())
                {
                    GameGoing = false;
                    ThisMod.NewGameVisible = true;
                }
                return;
            }
            ThisMod.Hand1!.Visible = true;
            ThisMod.Hand1.HandList.ReplaceRange(GetCards(thisCategory));
            ThisMod.Hand1.HandList.Sort();
            ThisMod.Hand1.Text = GetText(thisCategory);
            if (thisCategory == EnumHandCategory.Hand2)
            {
                ThisMod.TempCrib!.HandList.ReplaceRange(GetCards(EnumHandCategory.Crib));
                ThisMod.TempCrib.Visible = true;
            }
        }
        private bool saveBusy;

        public async Task SaveStateAsync()
        {
            if (saveBusy)
                return;
            saveBusy = true;
            SaveRoot.DeckList = ThisMod.DeckPile!.GetCardIntegers();
            SaveRoot.StartCard = ThisMod.StartPile!.GetSavedPile();
            await _thisState.SaveSimpleSinglePlayerGameAsync(SaveRoot); //i think
            saveBusy = false;
        }

        public async Task ShowWinAsync()
        {
            await ThisMod.ShowGameMessageAsync("Congratulations, you won");
            ThisMod.NewGameVisible = true;
            GameGoing = false;
            await _thisState.DeleteSinglePlayerGameAsync(); //i think.
        }

        protected override void AfterShuffle()
        {
            ThisMod.Scores!.NewGame();
            NewRound();
        }
        public void NewRound() //autoresume can't work here unfortunately.  because the override for aftershuffle is not async.
        {
            ThisMod.TempCrib!.Visible = false;
            ThisMod.StartPile!.ClearCards();
            ThisMod.HandScores!.ForEach(thisPile => thisPile.NewRound());
            if (ThisMod.DeckPile!.IsEndOfDeck())
                throw new BasicBlankException("Can't goto a new round because there are no cards in the deck");
            var tempList = ThisMod.HandScores.Where(items => items.HandCategory != EnumHandCategory.Crib).ToCustomBasicList();
            tempList.ForEach(thisPile =>
            {
                var finalList = ThisMod.DeckPile.DrawSeveralCards(6);
                thisPile.TempList.AddRange(finalList);
                if (thisPile.HandCategory == EnumHandCategory.Hand1)
                {
                    ThisMod.Hand1!.HandList.ReplaceRange(finalList);
                    ThisMod.Hand1.Text = thisPile.HandData();
                }
            });
            ThisMod.Hand1!.Visible = true;
            ThisMod.Hand1.HandList.Sort();
        }

        #region "Custom Processes"

        internal CribbageCard StartCard()
        {
            return ThisMod.StartPile!.GetCardInfo();
        }

        private void PopulateLists()
        {
            ComboList.Add(new CribbageCombos() { CardsToUse = 2, Description = "Fifteens", Points = 2, NumberNeeded = 15 });
            ComboList.Add(new CribbageCombos() { CardsToUse = 5, Description = "Full House", IsFullHouse = true, Group = EnumScoreGroup.ScorePairRuns, Points = 5 });
            ComboList.Add(new CribbageCombos() { CardsToUse = 2, Description = "Pairs", Group = EnumScoreGroup.ScorePairRuns, Points = 2, NumberForKind = 2 });
            ComboList.Add(new CribbageCombos() { CardsToUse = 3, Description = "Three of a kind", Group = EnumScoreGroup.ScorePairRuns, NumberForKind = 3, Points = 6 });
            ComboList.Add(new CribbageCombos() { CardsToUse = 4, Description = "Four Of a Kind", Group = EnumScoreGroup.ScorePairRuns, NumberForKind = 4, Points = 12 });
            ComboList.Add(new CribbageCombos() { CardsToUse = 3, Description = "Run of three", Group = EnumScoreGroup.ScorePairRuns, NumberInStraight = 3, Points = 3 });
            ComboList.Add(new CribbageCombos() { CardsToUse = 4, Description = "Run of four", Group = EnumScoreGroup.ScorePairRuns, NumberInStraight = 4, Points = 4 });
            ComboList.Add(new CribbageCombos() { CardsToUse = 5, Description = "Run of five", Group = EnumScoreGroup.ScorePairRuns, NumberInStraight = 5, Points = 5 });
            ComboList.Add(new CribbageCombos() { CardsToUse = 4, Description = "Double run of three", Group = EnumScoreGroup.ScorePairRuns, NumberInStraight = 3, NumberForKind = 2, Points = 8 });
            ComboList.Add(new CribbageCombos() { CardsToUse = 5, Description = "Double run of four", Group = EnumScoreGroup.ScorePairRuns, NumberInStraight = 4, NumberForKind = 2, Points = 10 });
            ComboList.Add(new CribbageCombos() { CardsToUse = 5, Description = "Triple run of three", Group = EnumScoreGroup.ScorePairRuns, NumberForKind = 3, NumberInStraight = 3, Points = 15 });
            ComboList.Add(new CribbageCombos() { CardsToUse = 5, Description = "Quadruple run of three", Group = EnumScoreGroup.ScorePairRuns, NumberForKind = 2, NumberInStraight = 3, DoublePairNeeded = true, Points = 16 });
            ComboList.Add(new CribbageCombos() { CardsToUse = 4, Description = "Four cards of same suit", IsFlush = true, Group = EnumScoreGroup.ScoreFlush, Points = 4 });
            ComboList.Add(new CribbageCombos() { CardsToUse = 5, Description = "Five cards of same suit", IsFlush = true, Group = EnumScoreGroup.ScoreFlush, Points = 5 });
            ComboList.Add(new CribbageCombos() { CardsToUse = 1, Description = "Jack of same suit", Points = 1, JackStatus = EnumJackType.Nob });
            ComboList.Add(new CribbageCombos() { CardsToUse = 1, Description = "Same suit as starter jack", Points = 2, JackStatus = EnumJackType.Heels });
        }
        public DeckRegularDict<CribbageCard> GetCards(EnumHandCategory thisCategory)
        {
            return ThisMod.HandScores.Where(items => items.HandCategory == thisCategory).Select(items => items.TempList).Single();
        }
        public string GetText(EnumHandCategory thisCategory) => ThisMod.HandScores.Where(items => items.HandCategory == thisCategory).Select(items => items.HandData()).Single();

        public EnumHandCategory CalculateCurrentHandCategory()
        {
            if (ThisMod.HandScores!.Count != 3)
                throw new BasicBlankException("Must have a list of 3 hands.  Otherwise, will be unable to calculate the category");
            var thisScore = ThisMod.HandScores.Single(items => items.HandCategory == EnumHandCategory.Crib);
            if (thisScore.TempList.Count == 4)
                return EnumHandCategory.Crib;
            thisScore = ThisMod.HandScores.Single(items => items.HandCategory == EnumHandCategory.Hand1);
            if (thisScore.TempList.Count == 6)
                return EnumHandCategory.Hand1;
            return EnumHandCategory.Hand2;
        }
        public void AddTempCardsToCrib(DeckRegularDict<CribbageCard> thisList)
        {
            var thisScore = ThisMod.HandScores.Single(items => items.HandCategory == EnumHandCategory.Crib);
            thisList.UnhighlightObjects();
            thisScore.TempList.AddRange(thisList);
        }
        public ScoreHandCP GetScoreHand(EnumHandCategory thisCategory)
        {
            return ThisMod.HandScores.Single(items => items.HandCategory == thisCategory);
        }

        public (int Row, int Column) GetRowColumn(EnumHandCategory ThisCategory)
        {
            var ThisHand = GetScoreHand(ThisCategory);
            return ThisHand.GetRowColumn();
        }

        public void RemoveTempCards(IDeckDict<CribbageCard> ThisList)
        {
            var thisScore = ThisMod.HandScores.Single(items => items.HandCategory == EnumHandCategory.Hand1);
            // hand1 comes first
            if (thisScore.TempList.Count == 6)
            {
                thisScore.TempList.RemoveGivenList(ThisList);
                return;
            }
            thisScore = ThisMod.HandScores.Single(items => items.HandCategory == EnumHandCategory.Hand2);
            if (thisScore.TempList.Count == 6)
            {
                thisScore.TempList.RemoveGivenList(ThisList);
                return;
            }
            throw new BasicBlankException("Error removing temp cards.  Think about what message to display.");
        }

        public void CardsToCrib(DeckRegularDict<CribbageCard> thisList)
        {
            if (thisList.Count != 2)
                throw new BasicBlankException("Must have 2 cards for crib");
            var cribList = GetCards(EnumHandCategory.Crib);
            if (cribList.Count == 4)
                throw new BasicBlankException("There are already 4 cards to the crib");
            var startList = cribList.ToRegularDeckDict();
            AddTempCardsToCrib(thisList);
            if (startList.Count == 0)
            {
                var tempList = GetCards(EnumHandCategory.Hand1);
                if (tempList.Count != 4)
                    throw new BasicBlankException("Should now be 4 cards left for the cribbage list for hand 1");
                var newList = GetCards(EnumHandCategory.Hand2);
                ThisMod.Hand1!.HandList.ReplaceRange(newList);
                ThisMod.Hand1.Text = GetText(EnumHandCategory.Hand2);
                ThisMod.TempCrib!.Visible = true;
                ThisMod.TempCrib.HandList.ReplaceRange(tempList);
                ThisMod.Hand1.HandList.Sort(); //has to be here now since no multiplayer.
                return;
            }
            if (startList.Count == 2)
            {
                ThisMod.TempCrib!.Visible = false;
                ThisMod.Hand1!.Visible = false;
                var tempList = GetCards(EnumHandCategory.Hand2);
                if (tempList.Count != 4)
                    throw new BasicBlankException("Should now be 4 cards left for the cribbage list for hand 2");
                ThisMod.StartPile!.AddCard(ThisMod.DeckPile!.DrawCard());
                ResultsFromHandOrCrib();
                return;
            }
            throw new BasicBlankException("Must have 0 or 2 cards left");
        }

        private void ResultsFromHandOrCrib()
        {
            ThisMod.HandScores!.ForEach(thisScore =>
            {
                bool fromCrib = thisScore.HandCategory == EnumHandCategory.Crib;
                thisScore.Scores = ListCribbageCombos(thisScore.TempList, fromCrib);
                thisScore.CardList.ReplaceRange(thisScore.TempList);
            });
            ThisMod.Scores!.AddScore(ThisMod.HandScores.Sum(items => items.TotalScore()));
        }

        private CustomBasicCollection<CribbageCombos> ListCribbageCombos(IDeckDict<CribbageCard> thisCol, bool fromCrib)
        {
            CustomBasicCollection<CribbageCombos> output = new CustomBasicCollection<CribbageCombos>();
            var mostSuits = thisCol.GroupOrderDescending(items => items.Suit);
            bool hadFourFlush;
            bool hadFiveFlush;
            var startCard = StartCard();
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
                hadFiveFlush = false; //originally was true.
            }
            else
            {
                hadFourFlush = false;
                hadFiveFlush = false;
            }
            bool hadMultiMove = false;
            int pairss = 0;
            bool hadLongerRun = false;
            bool hadStraight = false;
            bool hadKind = false;
            int fifs;
            CribbageCombos newCombo;
            ComboList.ForEach(thisCombo =>
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

                else if (thisCombo.JackStatus == EnumJackType.Nob)
                {
                    if (thisCol.Any(items => items.Value == EnumCardValueList.Jack && items.Suit == startCard.Suit && startCard.Value != EnumCardValueList.Jack))
                        output.Add(thisCombo);
                }
                else if (thisCombo.JackStatus == EnumJackType.Heels)
                {
                    if (thisCol.Any(items => items.Suit == startCard.Suit && startCard.Value == EnumCardValueList.Jack))
                        output.Add(thisCombo);
                }
                else if (thisCombo.IsFullHouse)
                {
                    if (HadFullHouse(thisCol))
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

                else if (thisCombo.NumberInStraight == 4 && hadLongerRun == true)
                {
                    output.Add(thisCombo);
                }
                else if (thisCombo.NumberInStraight > 0)
                {
                    hadStraight = HadProperStraight(thisCol, thisCombo);
                    if (hadStraight == true)
                        output.Add(thisCombo);
                }
                else if (thisCombo.NumberForKind > 2)
                {
                    hadKind = HadProperKind(thisCol, thisCombo);
                    if (hadKind == true)
                        output.Add(thisCombo);
                }
                else if (thisCombo.NumberForKind == 2)
                {
                    pairss = HowManyPairs(thisCol);
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

            });
            if (output.Any(items => items.Points == 8))
            {
                if (output.Any(items => items.Points == 3 && items.NumberInStraight == 3))
                    output.RemoveAllOnly(items => items.Points == 3 && items.NumberInStraight == 3); //because you got the double run of 3.
            }
            return output;
        }


        #endregion

        #region "Rummy Functions"
        private int HowManyPairs(IDeckDict<CribbageCard> thisCol)
        {
            var newCol = thisCol.ToRegularDeckDict();
            int output = 0;
            CribbageCard newCard;
            DeckRegularDict<CribbageCard> finals = new DeckRegularDict<CribbageCard>();
            newCol.Add(StartCard());
            newCol.RemoveAllOnly(items => items.HasUsed);
            var whatNewRummy = _Rummys!.WhatNewRummy(newCol, 2, RummyProcesses<EnumSuitList, EnumColorList, CribbageCard>.EnumRummyType.Sets, false);
            if (whatNewRummy.Count == 0)
                return 0;
            if (whatNewRummy.Count > 2)
            {
                finals.AddRange(newCol);
                newCard = whatNewRummy.First();
                finals.KeepConditionalItems(items => items.Value != newCard.Value);
                newCol.Clear();
                newCol.AddRange(finals);
                whatNewRummy = _Rummys.WhatNewRummy(newCol, 2, RummyProcesses<EnumSuitList, EnumColorList, CribbageCard>.EnumRummyType.Sets, false);
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
            whatNewRummy = _Rummys.WhatNewRummy(newCol, 2, RummyProcesses<EnumSuitList, EnumColorList, CribbageCard>.EnumRummyType.Sets, false);
            if (whatNewRummy.Count == 0)
                return output;
            if (whatNewRummy.Count > 2)
                return output;
            return output + 1;
        }
        public int Find15Combos(IDeckDict<CribbageCard> thisCol)
        {
            DeckRegularDict<CribbageCard> newCol = new DeckRegularDict<CribbageCard>();
            newCol.AddRange(thisCol);
            newCol.Add(StartCard());
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
        private bool IsMultiMove(CribbageCombos thisCombo, IDeckDict<CribbageCard> thisCol)
        {
            var newCol = thisCol.ToRegularDeckDict();
            newCol.Add(StartCard());
            var whatNewRummy = _Rummys!.WhatNewRummy(newCol, thisCombo.NumberInStraight, RummyProcesses<EnumSuitList, EnumColorList, CribbageCard>.EnumRummyType.Runs, false);
            if (whatNewRummy.Count == 0)
                return false;
            if (whatNewRummy.Count > thisCombo.NumberInStraight)
                return false;
            whatNewRummy.ForEach(thisCard => thisCard.HasUsed = true);
            EnumCardValueList firstNumber = whatNewRummy.First().Value;
            EnumCardValueList secondNumber = whatNewRummy.Last().Value;
            var lastTemp = newCol.ToRegularDeckDict();
            lastTemp.KeepConditionalItems(items => items.Value >= firstNumber || items.Value <= secondNumber);
            newCol.Clear();
            newCol.AddRange(lastTemp);
            whatNewRummy = _Rummys.WhatNewRummy(newCol, thisCombo.NumberForKind, RummyProcesses<EnumSuitList, EnumColorList, CribbageCard>.EnumRummyType.Sets, false);
            if (whatNewRummy.Count == 0 || whatNewRummy.Count > thisCombo.NumberForKind)
            {
                thisCol.ForEach(thisCard => thisCard.HasUsed = false);
                StartCard().HasUsed = false;
                return false;
            }
            if (thisCombo.DoublePairNeeded == true)
            {
                lastTemp.KeepConditionalItems(items => items.Value != whatNewRummy.First().Value);
                newCol.Clear();
                newCol.AddRange(lastTemp);
                whatNewRummy = _Rummys.WhatNewRummy(newCol, thisCombo.NumberForKind, RummyProcesses<EnumSuitList, EnumColorList, CribbageCard>.EnumRummyType.Sets, false);
                if (whatNewRummy.Count == 0 || whatNewRummy.Count > thisCombo.NumberForKind)
                {
                    thisCol.ForEach(thisCard => thisCard.HasUsed = false);
                    StartCard().HasUsed = false;
                    return false;
                }
                whatNewRummy.ForEach(thisCard => thisCard.HasUsed = true);
            }
            return true;
        }
        public bool HadProperStraight(IDeckDict<CribbageCard> thisCol, CribbageCombos thisCombo)
        {
            var newCol = thisCol.ToRegularDeckDict();
            DeckRegularDict<CribbageCard> finals = new DeckRegularDict<CribbageCard>();

            newCol.Add(StartCard());
            finals.AddRange(newCol);
            finals.KeepConditionalItems(items => items.HasUsed == false);
            newCol.Clear();
            newCol.AddRange(finals);

            var whatNewRummy = _Rummys!.WhatNewRummy(newCol, thisCombo.NumberInStraight, RummyProcesses<EnumSuitList, EnumColorList, CribbageCard>.EnumRummyType.Runs, false);
            if (whatNewRummy.Count == 0)
                return false;
            if (whatNewRummy.Count > thisCombo.NumberInStraight)
            {
                return false;
            }
            whatNewRummy.ForEach(thisCard => thisCard.HasUsed = true);
            return true;
        }

        public bool HadProperKind(IDeckDict<CribbageCard> thisCol, CribbageCombos thisCombo)
        {
            var newCol = thisCol.ToRegularDeckDict();
            DeckRegularDict<CribbageCard> finals = new DeckRegularDict<CribbageCard>();
            newCol.Add(StartCard());
            finals.AddRange(newCol);
            finals.KeepConditionalItems(items => items.HasUsed == false);
            newCol.Clear();
            newCol.AddRange(finals);
            var whatNewRummy = _Rummys!.WhatNewRummy(newCol, thisCombo.NumberForKind, RummyProcesses<EnumSuitList, EnumColorList, CribbageCard>.EnumRummyType.Sets, false);
            if (whatNewRummy.Count == 0)
                return false;
            if (whatNewRummy.Count > thisCombo.NumberForKind)
                return false;
            whatNewRummy.ForEach(thisCard => thisCard.HasUsed = true);
            return true;
        }
        private bool HadFullHouse(IDeckDict<CribbageCard> thisCol)
        {
            var newCol = thisCol.Where(items => items.HasUsed == false).ToRegularDeckDict();
            var whatNewRummy = _Rummys!.WhatNewRummy(newCol, 3, RummyProcesses<EnumSuitList, EnumColorList, CribbageCard>.EnumRummyType.Sets, false);
            if (whatNewRummy.Count == 0 || whatNewRummy.Count > 3)
                return false;
            whatNewRummy.ForEach(thisCard => thisCard.HasUsed = true);
            newCol.RemoveAllOnly(items => items.HasUsed == true);
            whatNewRummy = _Rummys.WhatNewRummy(newCol, 2, RummyProcesses<EnumSuitList, EnumColorList, CribbageCard>.EnumRummyType.Sets, false);
            if (whatNewRummy.Count == 0 || whatNewRummy.Count > 2)
            {
                thisCol.ForEach(thisCard => thisCard.HasUsed = false);
                return false;
            }
            whatNewRummy.ForEach(thisCard => thisCard.HasUsed = true);
            return true;
        }

        #endregion
    }
}
