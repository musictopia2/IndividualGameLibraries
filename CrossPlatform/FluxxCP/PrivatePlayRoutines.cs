using BasicGameFramework.BasicDrawables.Dictionary;
using BasicGameFramework.Extensions;
using BasicGameFramework.MultiplayerClasses.Extensions;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.Exceptions;
using System.Linq;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace FluxxCP
{
    public partial class FluxxMainGameClass
    {
        private bool _doDrawTemporary = false;
        private bool _allNewCards;

        private async Task ScrambleKeepersAsync()
        {
            if (PlayerList.Count(items => items.KeeperList.Count > 0) < 2)
            {
                await AnalyzeQueAsync();
                return;
            }
            if (ThisData!.MultiPlayer == true && SingleInfo!.CanSendMessage(ThisData) == false)
            {
                ThisCheck!.IsEnabled = true;
                return;
            }
            PlayerList!.ScrambleKeepers();
            if (ThisData.MultiPlayer)
            {
                var thisList = PlayerList.Where(items => items.KeeperList.Count > 0).Select(temps => temps.KeeperList.Select(fins => (int)fins.Deck).ToCustomBasicList().ToCustomBasicList());
                await ThisNet!.SendAllAsync("scramblekeepers", thisList.ToCustomBasicList());
            }
            await AnalyzeQueAsync();
        }
        private async Task LastCardsForEverybodyGetsOneAsync()
        {
            int extras = ThisGlobal!.IncreaseAmount();
            int maxs = extras + 1;
            foreach (var thisPlayer in PlayerList!)
            {
                if (ThisGlobal.EverybodyGetsOneList.Count(items => items.Player == thisPlayer.Id) < maxs)
                {
                    var tempList = ThisGlobal.TempActionHandList.Select(items => new PreviousCard { Deck = items, Player = thisPlayer.Id }).ToCustomBasicList();
                    ThisGlobal.EverybodyGetsOneList.AddRange(tempList);
                    ThisGlobal.TempActionHandList.Clear();
                    await FinalEverybodyGetsOneAsync();
                    return;
                }
            }
            throw new BasicBlankException("Wrong");
        }
        private async Task FinalEverybodyGetsOneAsync()
        {
            ThisGlobal!.EverybodyGetsOneList.ForEach(thisTemp =>
            {
                var thisPlayer = PlayerList![thisTemp.Player];
                var thisCard = DeckList!.GetSpecificItem(thisTemp.Deck);
                thisCard.Drew = false;
                thisCard.IsSelected = false;
                thisPlayer.MainHandList.Add(thisCard);
            });
            ThisGlobal.EverybodyGetsOneList.Clear();
            SingleInfo = PlayerList!.GetSelf();
            SortCards();
            SingleInfo = PlayerList.GetWhoPlayer();
            ThisGlobal.CurrentAction = null;
            await AnalyzeQueAsync();
        }
        private void AnalyzeRules()
        {
            int extraAmount = ThisGlobal!.IncreaseAmount();
            int possibleBonus = extraAmount + 1;
            if (SaveRoot!.RuleList.Any(items => items.Deck == EnumRuleText.PoorBonus))
            {
                if (SaveRoot.DrawBonus < possibleBonus && ThisGlobal.HasFewestKeepers())
                    SaveRoot.DrawBonus = possibleBonus;
            }
            if (SaveRoot.RuleList.Any(items => items.Deck == EnumRuleText.RichBonus))
            {
                if (SaveRoot.PlayBonus < possibleBonus && ThisGlobal.HasMostKeepers())
                    SaveRoot.PlayBonus = possibleBonus;
            }
            RuleCard thisRule;
            thisRule = SaveRoot.RuleList.Where(items => items.Category == EnumRuleCategory.Draw).SingleOrDefault();
            if (thisRule == null)
            {
                SaveRoot.DrawRules = possibleBonus;
            }
            else
            {
                if (thisRule.HowMany == 0)
                    throw new BasicBlankException("Cannot be 0");
                SaveRoot.DrawRules = extraAmount + thisRule.HowMany;
            }
            thisRule = SaveRoot.RuleList.Where(items => items.Category == EnumRuleCategory.Play).SingleOrDefault();
            if (thisRule == null)
            {
                SaveRoot.PlayLimit = possibleBonus;
            }
            else if (thisRule.HowMany == -1)
            {
                SaveRoot.PlayLimit = -1; //means unlimited.
            }
            else
            {
                if (thisRule.HowMany == 0)
                    throw new BasicBlankException("Cannot be 0");
                SaveRoot.PlayLimit = extraAmount + thisRule.HowMany;
            }
            thisRule = SaveRoot.RuleList.Where(items => items.Category == EnumRuleCategory.Hand).SingleOrDefault();
            if (thisRule == null)
            {
                SaveRoot.HandLimit = -1; //means unlimited
            }
            else
            {
                SaveRoot.HandLimit = extraAmount + thisRule.HowMany;
            }
            thisRule = SaveRoot.RuleList.Where(items => items.Category == EnumRuleCategory.Keeper).SingleOrDefault();
            if (thisRule == null)
            {
                SaveRoot.KeeperLimit = -1; //means unlimited
            }
            else
            {
                SaveRoot.KeeperLimit = extraAmount + thisRule.HowMany;
            }
            if (SaveRoot.PlayLimit == -1)
                SaveRoot.PlaysLeft = SingleInfo!.MainHandList.Count;
            else
                SaveRoot.PlaysLeft = SaveRoot.PlayLimit + SaveRoot.PlayBonus - SaveRoot.CardsPlayed;
            if (SaveRoot.PlaysLeft < 0)
                SaveRoot.PlaysLeft = 0;
            if (SaveRoot.PlaysLeft > SingleInfo!.MainHandList.Count)
                SaveRoot.PlaysLeft = SingleInfo.MainHandList.Count;
            LeftToDraw = ExtraCardsToDraw();
            if (LeftToDraw > 0)
                SaveRoot.CardsDrawn += LeftToDraw;
        }
        private int ExtraCardsToDraw()
        {
            int howMany = SaveRoot!.PreviousBonus + SaveRoot.DrawRules + SaveRoot.DrawBonus - SaveRoot.CardsDrawn;
            if (howMany <= 0)
                return 0;
            return howMany;
        }
        public async Task AnalyzeQueAsync()
        {
            if (ThisGlobal!.QuePlayList.Count == 0 || ThisGlobal.TempActionHandList.Count > 0 || ThisGlobal.CurrentAction != null)
            {
                if (ThisGlobal.TempActionHandList.Count == 0 && ThisGlobal.CurrentAction == null)
                {
                    await SaveRoot!.SavedActionData.TempDiscardList.ForEachAsync(async thisDiscard =>
                    {
                        var thisCard = DeckList!.GetSpecificItem(thisDiscard);
                        await AnimatePlayAsync(thisCard);
                    });
                    SaveRoot.SavedActionData.TempDiscardList.Clear();
                }
                SaveRoot!.DoAnalyze = false;
                await ContinueTurnAsync();
                return;
            }
            AnalyzeRules();
            if (LeftToDraw > 0)
            {
                await DrawAsync();
                return;
            }
            var tempList = PlayerList!.AllPlayersExceptForCurrent();
            if (tempList.Any(items => items.ObeyedRulesWhenNotYourTurn() == false))
            {
                SaveRoot!.DoAnalyze = true;
                await ContinueTurnAsync();
                return;
            }
            OtherTurn = 0;
            SingleInfo = PlayerList.GetWhoPlayer();
            if (NeedsToRemoveGoal())
            {
                SaveRoot!.DoAnalyze = false;
                await ContinueTurnAsync();
                return;
            }
            int wins = WhoWonGame();
            if (wins > 0)
            {
                SingleInfo = PlayerList[wins];
                await ShowWinAsync();
                return;
            }
            SaveRoot!.DoAnalyze = true;
            await SaveStateAsync();
            await PlayCardAsync(ThisGlobal.QuePlayList.First().Deck);
        }
        public async Task SendPlayAsync(int deck)
        {
            if (SingleInfo!.CanSendMessage(ThisData!))
                await ThisNet!.SendAllAsync("playcard", deck);
        }
        public async Task PlayCardAsync(int deck)
        {
            SingleInfo = PlayerList!.GetWhoPlayer();
            await PlayCardAsync(DeckList!.GetSpecificItem(deck));
        }
        public async Task PlayCardAsync(FluxxCardInformation thisCard)
        {
            await ThisMod!.ShowPlayCardAsync(thisCard);
            thisCard.IsSelected = false;
            thisCard.Drew = false;
            bool doAgain = false;
            SaveRoot!.DoAnalyze = false;
            if (ThisGlobal!.EverybodyGetsOneList.Count > 0)
                throw new BasicBlankException("Everybody gets one was not finished.  That must be finished before playing another card");
            if (ThisGlobal.TempActionHandList.Count > 0)
                throw new BasicBlankException("There are cards left from the temporary action list.  Must finish choosing the order to play the cards.  Then they will play in the order");
            if (ThisGlobal.CurrentAction != null)
            {
                if (ThisGlobal.CurrentAction.Deck == EnumActionMain.LetsDoThatAgain)
                {
                    doAgain = true;
                    ThisMod.Pile1!.RemoveCardFromPile(thisCard);
                }
            }
            ThisGlobal.CurrentAction = null;
            if (ThisGlobal.QuePlayList.Count == 0 && doAgain == false)
            {
                SaveRoot.CardsPlayed++;
                SingleInfo = PlayerList!.GetWhoPlayer();
                RemoveFromHandOnly(thisCard);
            }
            else if (doAgain == false)
            {
                ThisGlobal.QuePlayList.RemoveFirstItem();
            }
            SingleInfo = PlayerList!.GetWhoPlayer();
            switch (thisCard.CardType)
            {
                case EnumCardType.Rule:
                    await PlayRuleAsync((RuleCard)thisCard);
                    break;
                case EnumCardType.Keeper:
                    await PlayKeeperAsync((KeeperCard)thisCard);
                    break;
                case EnumCardType.Goal:
                    await PlayGoalAsync((GoalCard)thisCard);
                    break;
                case EnumCardType.Action:
                    await PlayActionAsync((ActionCard)thisCard);
                    break;
                default:
                    throw new BasicBlankException("Can't figure out which one to play for card type");
            }
        }
        public async Task DrawUsedAsync(int deck)
        {
            if (SingleInfo!.CanSendMessage(ThisData!))
                await ThisNet!.SendAllAsync("drawuse", deck);
            ThisGlobal!.TempActionHandList.RemoveSpecificItem(deck);
            var thisCard = DeckList!.GetSpecificItem(deck);
            ThisGlobal.QuePlayList.Add(thisCard);
            if (ThisGlobal.TempActionHandList.Count == 1)
            {
                if (ThisGlobal.CurrentAction!.Deck == EnumActionMain.Draw3Play2OfThem)
                {
                    SaveRoot!.SavedActionData.TempDiscardList.Add(ThisGlobal.TempActionHandList.Single());
                }
                else
                {
                    thisCard = DeckList.GetSpecificItem(ThisGlobal.TempActionHandList.Single());
                    ThisGlobal.QuePlayList.Add(thisCard);
                }
                ThisGlobal.TempActionHandList.Clear();
                ThisGlobal.CurrentAction = null;
                await AnalyzeQueAsync();
                return;
            }
            await ContinueTurnAsync();
        }
        private async Task PlayActionAsync(ActionCard thisCard)
        {
            await AnimatePlayAsync(thisCard);
            if (thisCard.Deck == EnumActionMain.LetsSimplify || thisCard.Deck == EnumActionMain.TrashANewRule)
            {
                if (SaveRoot!.RuleList.Count == 1)
                {
                    await AnalyzeQueAsync();
                    return;
                }
            }
            else if (thisCard.Deck == EnumActionMain.UseWhatYouTake || thisCard.Deck == EnumActionMain.Taxation)
            {
                var tempList = PlayerList!.AllPlayersExceptForCurrent();
                if (!tempList.Any(items => items.MainHandList.Count > 0))
                {
                    await AnalyzeQueAsync();
                    return;
                }
            }
            else if (thisCard.Deck == EnumActionMain.LetsDoThatAgain)
            {
                var tempList = ThisMod!.Pile1!.DiscardList();
                if (!tempList.Any(items => items.CanDoCardAgain() == true))
                {
                    await AnalyzeQueAsync();
                    return;
                }
            }
            else if (thisCard.Deck == EnumActionMain.TrashAKeeper)
            {
                if (!PlayerList.Any(items => items.KeeperList.Count > 0))
                {
                    await AnalyzeQueAsync();
                    return;
                }
            }
            else if (thisCard.Deck == EnumActionMain.ExchangeKeepers)
            {
                if (SingleInfo!.KeeperList.Count == 0)
                {
                    await AnalyzeQueAsync();
                    return;
                }
            }
            if (thisCard.Deck == EnumActionMain.ExchangeKeepers || thisCard.Deck == EnumActionMain.StealAKeeper)
            {
                var tempList = PlayerList!.AllPlayersExceptForCurrent();
                if (!tempList.Any(items => items.KeeperList.Count > 0))
                {
                    await AnalyzeQueAsync();
                    return;
                }
            }
            ThisGlobal!.CurrentAction = thisCard;
            if (ThisGlobal.CurrentAction.Category != EnumActionScreen.None)
            {
                if (ThisGlobal.CurrentAction.Category == EnumActionScreen.OtherPlayer)
                {
                    var tempList = PlayerList!.AllPlayersExceptForCurrent();
                    if (tempList.Any(items => items.MainHandList.Count > 0))
                    {
                        do
                        {
                            OtherTurn = await PlayerList.CalculateOtherTurnAsync();
                            var tempPlayer = PlayerList.GetOtherPlayer(); //i think.
                            if (tempPlayer.MainHandList.Count > 0)
                                break;
                            if (OtherTurn == 0)
                                throw new BasicBlankException("There should have been at least one player with cards for taxation");
                        } while (true);
                        await ContinueTurnAsync(); //because of taxation
                        return;
                    }
                }
                if (ThisGlobal.CurrentAction.Deck == EnumActionMain.EverybodyGets1 || ThisGlobal.CurrentAction.Deck == EnumActionMain.Draw2AndUseEm || ThisGlobal.CurrentAction.Deck == EnumActionMain.Draw3Play2OfThem)
                {
                    await DrawTemporaryCardsAsync();
                    return;
                }
                await AnalyzeQueAsync();
                return;
            }
            ThisGlobal.CurrentAction = null;
            switch (thisCard.Deck)
            {
                case EnumActionMain.TakeAnotherTurn:
                    SaveRoot!.AnotherTurn = true;
                    break;
                case EnumActionMain.ScrambleKeepers:
                    await ScrambleKeepersAsync();
                    break;
                case EnumActionMain.RulesReset:
                    await ResetRulesAsync();
                    break;
                case EnumActionMain.EmptyTheTrash:
                    await EmptyTrashAsync();
                    break;
                case EnumActionMain.DiscardDraw:
                    await DrawDiscardAsync();
                    break;
                case EnumActionMain.NoLimits:
                    await NoLimitsAsync();
                    break;
                case EnumActionMain.Jackpot:
                    LeftToDraw = ThisGlobal.IncreaseAmount() + 3;
                    await DrawAsync();
                    break;
                default:
                    break;
            }
            if (thisCard.Deck != EnumActionMain.EmptyTheTrash && thisCard.Deck != EnumActionMain.DiscardDraw && thisCard.Deck != EnumActionMain.Jackpot && thisCard.Deck != EnumActionMain.ScrambleKeepers)
                await AnalyzeQueAsync();
        }
        private async Task DrawTemporaryCardsAsync()
        {
            _doDrawTemporary = true;
            if (ThisGlobal!.TempActionHandList.Count > 0)
                throw new BasicBlankException("There are already cards left to choose from.  Should have not Drawn Temporary Cards");
            int extras = ThisGlobal.IncreaseAmount();
            switch (ThisGlobal.CurrentAction!.Deck)
            {
                case EnumActionMain.EverybodyGets1:
                    if (extras == 0)
                        LeftToDraw = PlayerList.Count();
                    else
                        LeftToDraw = PlayerList.Count() * 2;
                    break;
                case EnumActionMain.Draw2AndUseEm:
                    LeftToDraw = 2 + extras;
                    break;
                case EnumActionMain.Draw3Play2OfThem:
                    LeftToDraw = 3 + extras;
                    break;
                default:
                    break;
            }
            await DrawAsync();
        }
        private async Task NoLimitsAsync()
        {
            var tempList = SaveRoot!.RuleList.Where(items => items.Category == EnumRuleCategory.Hand || items.Category == EnumRuleCategory.Keeper).ToCustomBasicList();
            await tempList.ForEachAsync(async thisRule =>
            {
                await DiscardRuleAsync(thisRule);
                if (ThisTest!.NoAnimations == false)
                    await Delay!.DelaySeconds(.1);
            });
            RefreshRules();
        }
        private async Task DrawDiscardAsync()
        {
            LeftToDraw = SingleInfo!.MainHandList.Count;
            var tempList = SingleInfo.MainHandList.ToRegularDeckDict();
            await tempList.ForEachAsync(async thisCard => await DiscardFromHandAsync(thisCard));
            _allNewCards = true;
            await DrawAsync();
        }
        private async Task EmptyTrashAsync()
        {
            await ThisMod!.ShowGameMessageAsync("Empty the trash was played.  Therefore; the cards are being reshuffled");
            if (ThisData!.MultiPlayer && SingleInfo!.CanSendMessage(ThisData) == false)
            {
                ThisCheck!.IsEnabled = true;
                return;
            }
            var thisList = ThisMod.Pile1!.DiscardList();
            thisList.AddRange(ThisMod.Deck1!.DeckList());
            thisList.ShuffleList();
            if (ThisData.MultiPlayer)
                await ThisNet!.SendAllAsync("emptytrash", thisList.GetDeckListFromObjectList());
            await FinishEmptyTrashAsync(thisList);
        }
        public async Task FinishEmptyTrashAsync(IEnumerableDeck<FluxxCardInformation> cardList)
        {
            ThisMod!.Deck1!.OriginalList(cardList);
            ThisMod.Pile1!.CardsReshuffled();
            await AnalyzeQueAsync();
        }
        private async Task ResetRulesAsync()
        {
            if (SaveRoot!.RuleList.Count == 0)
            {
                SaveRoot.RuleList.Clear(); //try this way.
                SaveRoot.RuleList.Add((RuleCard)DeckList!.GetSpecificItem(1));
            }
            else
            {
                var tempList = SaveRoot.RuleList.Where(items => items.Category != EnumRuleCategory.Basic).ToRegularDeckDict();
                await tempList.ForEachAsync(async thisCard =>
                {
                    await DiscardRuleAsync(thisCard);
                    if (ThisTest!.NoAnimations == false)
                        await Delay!.DelaySeconds(.2);
                });
            }
            RefreshRules();
        }
        private async Task PlayRuleAsync(RuleCard thisCard)
        {
            if (thisCard.Deck == EnumRuleText.ReverseOrder)
            {
                if (PlayerList.Count() == 2)
                {
                    SaveRoot!.AnotherTurn = true;
                    await AnimatePlayAsync(thisCard);
                    await AnalyzeQueAsync();
                    return;
                }
            }
            if (thisCard.Category != EnumRuleCategory.Basic && thisCard.Category != EnumRuleCategory.None)
            {
                var thisCat = thisCard.Category;
                SaveRoot!.RuleList.RemoveAllOnly(items => items.Category == thisCat);
            }
            SaveRoot!.RuleList.Add(thisCard);
            RefreshRules();
            if (ThisGlobal!.QuePlayList.Count == 0)
            {
                AnalyzeRules();
                if (LeftToDraw > 0)
                {
                    SaveRoot.DoAnalyze = true;
                    await DrawAsync();
                    return;
                }
            }
            await AnalyzeQueAsync();
        }
        private async Task PlayGoalAsync(GoalCard thisCard)
        {
            bool isDouble = ThisGlobal!.HasDoubleAgenda();
            if (SaveRoot!.GoalList.Count == 3)
                throw new BasicBlankException("Needs to remove a goal before it can play a goal");
            if (SaveRoot.GoalList.Count == 2 && isDouble == false)
                throw new BasicBlankException("Cannot play another goal because 2 goals already and not double agenda");
            if (isDouble == false && SaveRoot.GoalList.Count == 1)
                await DiscardGoalAsync(SaveRoot.GoalList.Single());
            SaveRoot.GoalList.Add(thisCard);
            await AnalyzeQueAsync();
        }
        private async Task PlayKeeperAsync(KeeperCard thisCard)
        {
            if (OtherTurn > 0)
                throw new BasicBlankException("Can't be otherturn for playing a keeper.  If its playing first card random; will set OtherTurn to 0 first");
            SingleInfo = PlayerList!.GetWhoPlayer();
            SingleInfo.KeeperList.Add(thisCard);
            await AnalyzeQueAsync(); //should not be a need to analyze keeper anymore because observable.
        }
        public bool NeedsToRemoveGoal()
        {
            if (SaveRoot!.GoalList.Count == 3)
                return true;
            if (SaveRoot.GoalList.Count > 3)
                throw new BasicBlankException("Too many goals");
            return SaveRoot.GoalList.Count == 2 && ThisGlobal!.HasDoubleAgenda() == false;
        }
        public EnumEndTurnStatus StatusEndRegularTurn()
        {
            if (NeedsToRemoveGoal())
                return EnumEndTurnStatus.Goal;
            if (SaveRoot!.CardsPlayed < SaveRoot.PlayLimit && SaveRoot.PlayLimit > -1 && SingleInfo!.MainHandList.Count > 0)
                return EnumEndTurnStatus.Play;
            if (SaveRoot.KeeperLimit > -1)
            {
                if (SingleInfo!.KeeperList.Count > SaveRoot.KeeperLimit)
                    return EnumEndTurnStatus.Keeper;
            }
            if (SaveRoot.HandLimit > -1)
            {
                if (SingleInfo!.MainHandList.Count > SaveRoot.HandLimit)
                    return EnumEndTurnStatus.Hand;
            }
            return EnumEndTurnStatus.Successful; //this means you obeyed all the rules and can end your turn.
        }
        public bool IsFirstPlayRandom()
        {
            if (SaveRoot!.CardsPlayed > 0)
                return false;
            if (SaveRoot.PlayLimit == 1)
                return false;
            if (SingleInfo!.MainHandList.Count == 0)
                return false; //because the main player has 0 cards
            return SaveRoot.RuleList.Any(items => items.Deck == EnumRuleText.FirstPlayRandom);
        }
        private EnumActionScreen ScreenToLoad()
        {
            if (IsFirstPlayRandom())
                return EnumActionScreen.ActionScreen;
            if (OtherTurn > 0 && SingleInfo!.ObeyedRulesWhenNotYourTurn() == false)
                return EnumActionScreen.None; //because somebody needs to remove the necessary cards before it can be continued
            if (ThisGlobal!.CurrentAction == null)
                return EnumActionScreen.None;
            if (ThisGlobal.CurrentAction.Category == EnumActionScreen.OtherPlayer)
                return EnumActionScreen.None;
            return ThisGlobal.CurrentAction.Category;
        }
        public async Task DoAgainProcessPart1Async(int selectedIndex)
        {
            SingleInfo = PlayerList!.GetWhoPlayer();
            if (SingleInfo.CanSendMessage(ThisData!))
                await ThisNet!.SendAllAsync("doagain", selectedIndex);
        }
        public async Task RotateHandAsync(EnumDirection direction)
        {
            SingleInfo = PlayerList!.GetWhoPlayer();
            if (SingleInfo.CanSendMessage(ThisData!))
                await ThisNet!.SendAllAsync("rotatehands", direction);
            DeckRegularDict<FluxxCardInformation> oldList;
            DeckRegularDict<FluxxCardInformation> newList;
            int oldTurn = WhoTurn;
            bool oldReverse = SaveRoot!.PlayOrder.IsReversed;
            FluxxPlayerItem oldPlayer;
            SaveRoot.PlayOrder.IsReversed = direction == EnumDirection.Right;
            oldList = SingleInfo.MainHandList.ToRegularDeckDict();
            int count = PlayerList.Count();
            await count.TimesAsync(async x =>
            {
                oldPlayer = SingleInfo;
                WhoTurn = await PlayerList.CalculateWhoTurnAsync();
                SingleInfo = PlayerList.GetWhoPlayer();
                if (oldPlayer.NickName == SingleInfo.NickName)
                    throw new BasicBlankException("Its the same player.  A problem");
                newList = SingleInfo.MainHandList.ToRegularDeckDict();
                SingleInfo.MainHandList.ReplaceRange(oldList);
                if (newList.Count == SingleInfo.MainHandList.Count && SingleInfo.MainHandList.Count > 0 && newList.First().Deck == SingleInfo.MainHandList.First().Deck)
                    throw new BasicBlankException("Did not rotate the cards");
                oldList = newList;
            });
            SaveRoot.PlayOrder.IsReversed = oldReverse;
            WhoTurn = oldTurn;
            SingleInfo = PlayerList.GetSelf();
            SortCards();
            SingleInfo = PlayerList.GetWhoPlayer();
            ThisGlobal!.CurrentAction = null;
            await AnalyzeQueAsync();
        }
        public async Task TradeHandAsync(int selectedIndex)
        {
            if (SingleInfo!.CanSendMessage(ThisData!))
                await ThisNet!.SendAllAsync("tradehands", selectedIndex);
            int player = ThisMod!.Action1!.GetPlayerIndex(selectedIndex);
            if (player == WhoTurn)
                throw new BasicBlankException("Cannot pick yourself");
            ThisMod.Action1.ResetPlayers();
            SingleInfo = PlayerList!.GetWhoPlayer();
            var oldList = SingleInfo.MainHandList.ToRegularDeckDict();
            var thisPlayer = PlayerList[player];
            var newList = thisPlayer.MainHandList.ToRegularDeckDict();
            SingleInfo.MainHandList.ReplaceRange(newList);
            thisPlayer.MainHandList.ReplaceRange(oldList);
            int myID = PlayerList.GetSelf().Id;
            if (player == myID || WhoTurn == myID)
            {
                SingleInfo = PlayerList.GetSelf();
                SortCards();
            }
            SingleInfo = PlayerList.GetWhoPlayer();
            ThisGlobal!.CurrentAction = null;
            await AnalyzeQueAsync();
        }
        public async Task EverybodyGetsOneAsync(CustomBasicList<int> thisList, int selectedIndex)
        {
            if (SingleInfo!.CanSendMessage(ThisData!))
            {
                CustomBasicList<int> sendList = new CustomBasicList<int>();
                sendList.AddRange(thisList);
                sendList.Add(selectedIndex);
                await ThisNet!.SendAllAsync("everybodygetsone", sendList);
            }
            int player = ThisMod!.Action1!.GetPlayerIndex(selectedIndex);
            var newList = thisList.Select(items => new PreviousCard { Deck = items, Player = player }).ToCustomBasicList();
            ThisGlobal!.EverybodyGetsOneList.AddRange(newList);
            int oldCount = ThisGlobal.TempActionHandList.Count;
            ThisGlobal.TempActionHandList.RemoveGivenList(thisList, System.Collections.Specialized.NotifyCollectionChangedAction.Remove);
            ThisMod.Action1.ResetPlayers();
            if (ThisGlobal.TempActionHandList.Count == oldCount)
                throw new BasicBlankException("Did not remove from temphand");
            if (ThisMod.Action1.CanLoadEverybodyGetsOne() == false)
            {
                await LastCardsForEverybodyGetsOneAsync();
                return;
            }
            await AnalyzeQueAsync();
        }
    }
}