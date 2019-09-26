using BasicGameFramework.Attributes;
using BasicGameFramework.BasicDrawables.Dictionary;
using BasicGameFramework.DIContainers;
using BasicGameFramework.Extensions;
using BasicGameFramework.MultiplayerClasses.BasicGameClasses;
using BasicGameFramework.MultiplayerClasses.BasicPlayerClasses;
using BasicGameFramework.MultiplayerClasses.Extensions;
using BasicGameFramework.MultiplayerClasses.InterfaceMessages;
using BasicGameFramework.MultiplayerClasses.InterfacesForHelpers;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.Exceptions;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using js = CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.JsonSerializers.NewtonJsonStrings; //just in case i need those 2.
namespace MonasteryCardGameCP
{
    [SingletonGame]
    public class MonasteryCardGameMainGameClass : CardGameClass<MonasteryCardInfo, MonasteryCardGamePlayerItem, MonasteryCardGameSaveInfo>, IMiscDataNM, IStartNewGame
    {
        public MonasteryCardGameMainGameClass(IGamePackageResolver container) : base(container) { }
        internal MonasteryCardGameViewModel? ThisMod;
        internal RummyClass? Rummys; //this has to be loaded later because of the deck class;
        internal MissionList? CurrentMission;
        internal CustomBasicList<MissionList>? MissionInfo;
        public override void Init() //decided to have all the code under init to prevent overflow issues.
        {
            base.Init();
            ThisMod = MainContainer.Resolve<MonasteryCardGameViewModel>();
        }
        public override async Task FinishGetSavedAsync()
        {
            ThisMod!.MainSets!.ClearBoard(); //i think because its all gone.
            ThisMod!.TempSets!.ClearBoard(); //try this too.
            LoadControls();
            int x = SaveRoot!.SetList.Count; //the first time, actually load manually.
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
            SingleInfo = PlayerList.GetWhoPlayer();
            if (SaveRoot.Mission == 0)
                CurrentMission = null;
            else
                CurrentMission = MissionInfo![SaveRoot.Mission - 1]; //because 0 based;
            PopulateMissions();  //i think.
            await base.FinishGetSavedAsync();
            CreateRummys();
        }
        private void LoadControls()
        {
            if (IsLoaded == true)
                return;
            CreateSets();
            IsLoaded = true; //i think needs to be here.
        }
        private void CreateRummys()
        {
            if (Rummys != null)
                return;
            Rummys = new RummyClass(this);
        }
        private void CreateSets()
        {
            MissionInfo = new CustomBasicList<MissionList>();
            MissionList thisMission;
            thisMission = new MissionList();
            thisMission.Description = "2 sets of 3 in color";
            SetInfo newSets;
            int x;
            for (x = 1; x <= 2; x++)
            {
                newSets = new SetInfo();
                newSets.HowMany = 3;
                newSets.SetType = EnumWhatSets.KindColor;
                thisMission.MissionSets.Add(newSets);
            }
            MissionInfo.Add(thisMission);
            thisMission = new MissionList();
            thisMission.Description = "3 sets of 3";
            for (x = 1; x <= 3; x++)
            {
                newSets = new SetInfo();
                newSets.HowMany = 3;
                newSets.SetType = EnumWhatSets.RegularKinds;
                thisMission.MissionSets.Add(newSets);
            }
            MissionInfo.Add(thisMission);
            thisMission = new MissionList();
            thisMission.Description = "1 set of 4, 1 run of 4";
            for (x = 1; x <= 2; x++)
            {
                newSets = new SetInfo();
                if (x == 2)
                {
                    newSets.HowMany = 4;
                    newSets.SetType = EnumWhatSets.RegularKinds;
                }
                else
                {
                    newSets.HowMany = 4;
                    newSets.SetType = EnumWhatSets.RegularRuns;
                }
                thisMission.MissionSets.Add(newSets);
            }
            MissionInfo.Add(thisMission);
            for (x = 5; x <= 8; x++)
            {
                if (x != 7)
                {
                    thisMission = new MissionList();
                    newSets = new SetInfo();
                    if (x == 5)
                    {
                        thisMission.Description = "1 run of 5 in suit";
                        newSets.SetType = EnumWhatSets.SuitRuns;
                    }
                    else if (x == 6)
                    {
                        thisMission.Description = "1 run of 6 in color";
                        newSets.SetType = EnumWhatSets.RunColors;
                    }
                    else
                    {
                        thisMission.Description = "1 run of 8";
                        newSets.SetType = EnumWhatSets.RegularRuns;
                    }
                    newSets.HowMany = x;
                    thisMission.MissionSets.Add(newSets);
                    MissionInfo.Add(thisMission);
                }
            }
            thisMission = new MissionList();
            thisMission.Description = "1 double run of three";
            newSets = new SetInfo();
            newSets.HowMany = 3;
            newSets.SetType = EnumWhatSets.DoubleRun;
            thisMission.MissionSets.Add(newSets);
            MissionInfo.Add(thisMission);
            thisMission = new MissionList();
            thisMission.Description = "7 cards of the same suit";
            newSets = new SetInfo();
            newSets.HowMany = 7;
            newSets.SetType = EnumWhatSets.RegularSuits;
            thisMission.MissionSets.Add(newSets);
            MissionInfo.Add(thisMission);
            thisMission = new MissionList();
            thisMission.Description = "9 cards even or odd (all)";
            newSets = new SetInfo();
            newSets.HowMany = 9;
            newSets.SetType = EnumWhatSets.EvenOdd;
            thisMission.MissionSets.Add(newSets);
            MissionInfo.Add(thisMission);
        }
        public override Task PopulateSaveRootAsync()
        {
            SaveRoot!.SetList = ThisMod!.MainSets!.SavedSets();
            MonasteryCardGamePlayerItem self = PlayerList!.GetSelf();
            self.AdditionalCards = ThisMod.TempSets!.ListAllObjects();
            return base.PopulateSaveRootAsync();
        }
        protected override Task StartSetUpAsync(bool isBeginning)
        {
            LoadControls();
            if (isBeginning)
                PlayerList!.ForEach(thisPlayer => thisPlayer.UpdateIndexes());
            PlayerList!.ForEach(thisPlayer => thisPlayer.FinishedCurrentMission = false);
            return base.StartSetUpAsync(isBeginning);
        }
        protected override Task LastPartOfSetUpBeforeBindingsAsync()
        {
            if (IsLoaded == false)
            {
                LoadControls();
                CreateRummys();
                SingleInfo = PlayerList!.GetSelf();
                ThisE.Subscribe(SingleInfo);
            }
            ThisMod!.MainSets!.ClearBoard();
            SaveRoot!.SetList.Clear(); //i think this too.
            ThisMod!.TempSets!.ClearBoard();
            SingleInfo = PlayerList!.GetWhoPlayer();
            PopulateMissions();
            return base.LastPartOfSetUpBeforeBindingsAsync();
        }
        async Task IMiscDataNM.MiscDataReceived(string status, string content)
        {
            switch (status)
            {
                case "finished":
                    await CreateSetsAsync(content);
                    if (ThisTest!.NoAnimations == false)
                        await Delay!.DelayMilli(500);
                    await FinishedAsync();
                    return;
                case "expandset":
                    var tempData = await js.DeserializeObjectAsync<SendExpandSet>(content);
                    var tempList = await tempData.CardData.GetObjectsFromDataAsync(SingleInfo!.MainHandList);
                    await ExpandSetAsync(tempList, tempData.SetNumber, tempData.Position);
                    return;
                default:
                    throw new BasicBlankException($"Nothing for status {status}  with the message of {content}");
            }
        }
        public override async Task StartNewTurnAsync()
        {
            await base.StartNewTurnAsync();
            PopulateMissions();
            if (SingleInfo!.ObjectCount == 8)
                throw new BasicBlankException("Its impossible to have only 8 cards at the start of your turn");
            await ContinueTurnAsync(); //most of the time, continue turn.  can change to what is needed
        }
        public override async Task EndTurnAsync()
        {
            SingleInfo = PlayerList!.GetWhoPlayer();
            SingleInfo.MainHandList.UnhighlightObjects(); //i think this is best.
            ThisMod!.CommandContainer!.ManuelFinish = true; //because it could be somebody else's turn.
            WhoTurn = await PlayerList.CalculateWhoTurnAsync();
            await StartNewTurnAsync();
        }
        private async Task CreateSetsAsync(string message)
        {
            var firstTemp = await js.DeserializeObjectAsync<CustomBasicList<string>>(message);
            int x = 0;
            foreach (var thisFirst in firstTemp)
            {
                x++;
                var thisSend = await js.DeserializeObjectAsync<SendNewSet>(thisFirst);
                var thisCol = await thisSend.CardData.GetObjectsFromDataAsync(SingleInfo!.MainHandList);
                if (x == 1)
                {
                    ThisMod!.MissionChosen = thisSend.MissionCompleted;
                    ProcessCurrentMission();
                }
                CreateNewSet(thisCol, thisSend.Index);
            }
        }
        public bool CanProcessDiscard(out bool pickUp, out int index, out int deck, out string message)
        {
            message = "";
            index = 0;
            deck = 0;
            if (AlreadyDrew == false && PreviousCard == 0)
                pickUp = true;
            else
                pickUp = false;


            if (pickUp == true)
            {

                if (ThisMod!.PlayerHand1!.HowManySelectedObjects > 0 || ThisMod.TempSets!.HowManySelectedObjects > 0)
                {
                    message = "There is at least one card selected.  Unselect all the cards before picking up from discard";
                    return false;
                }
                return true;
            }
            var thisCol = ThisMod!.PlayerHand1!.ListSelectedObjects();
            var otherCol = ThisMod.TempSets!.ListSelectedObjects();
            if (thisCol.Count == 0 && otherCol.Count == 0)
            {
                message = "Sorry, you must select a card to discard";
                return false;
            }
            if (thisCol.Count + otherCol.Count > 1)
            {
                message = "Sorry, you can only select one card to discard";
                return false;
            }

            if (thisCol.Count == 0)
            {
                index = ThisMod.TempSets.PileForSelectedObject;
                deck = ThisMod.TempSets.DeckForSelectedObjected(index);
            }
            else
            {
                deck = ThisMod.PlayerHand1.ObjectSelected();
            }
            var thisCard = DeckList!.GetSpecificItem(deck);
            if (thisCard.Deck == PreviousCard && SingleInfo!.ObjectCount > 1)
            {
                message = "Sorry, you cannot discard the one that was picked up since there is more than one card to choose from";
                return false;
            }
            return true;
        }
        private void RemoveCard(int deck)
        {
            if (SingleInfo!.PlayerCategory == EnumPlayerCategory.Computer)
                return; //i think computer player would have already removed their card.
            if (SingleInfo.PlayerCategory == EnumPlayerCategory.Self)
            {
                if (ThisMod!.TempSets!.HasObject(deck))
                {
                    ThisMod.TempSets.RemoveObject(deck);
                    return;
                }
            }
            SingleInfo.MainHandList.RemoveObjectByDeck(deck);
        }
        private void RemoveCards(DeckRegularDict<MonasteryCardInfo> thisCol)
        {
            if (SingleInfo!.PlayerCategory != EnumPlayerCategory.Self)
                return;
            DeckRegularDict<MonasteryCardInfo> output = new DeckRegularDict<MonasteryCardInfo>();
            thisCol.ForEach(thisCard =>
            {
                if (ThisMod!.TempSets!.HasObject(thisCard.Deck))
                    ThisMod.TempSets.RemoveObject(thisCard.Deck);
                else
                    output.Add(thisCard);
            });
            SingleInfo.MainHandList.RemoveGivenList(output, NotifyCollectionChangedAction.Remove);
        }
        private void UnselectCards()
        {
            if (SingleInfo!.PlayerCategory != EnumPlayerCategory.Self)
                return;
            ThisMod!.PlayerHand1!.EndTurn();
            ThisMod.TempSets!.EndTurn();
        }
        private bool CanEndGame()
        {
            if (PlayerList.Any(items => items.IndexList.Count == 0) == false)
                return false; //because nobody completed all the missions.
            if (PlayerList.Count(items => items.IndexList.Count == 0) > 1)
                throw new BasicBlankException("It should be impossible for both players to complete all 9 missions.");
            SingleInfo = PlayerList.Single(items => items.IndexList.Count == 0);
            return true;
        }
        private int GetMissionNumber => MissionInfo!.IndexOf(CurrentMission!);
        private bool DidAllPlayersComplete => PlayerList.All(items => items.FinishedCurrentMission == true);
        public MissionList GetMission => MissionInfo.Single(items => items.Description == ThisMod!.MissionChosen);
        public void ProcessCurrentMission()
        {
            CurrentMission = GetMission;
            int nums = GetMissionNumber;
            SingleInfo!.CompleteMissionIndex(nums);
            ThisMod!.CompleteMissions.ReplaceAllWithGivenItem(CurrentMission);
        }
        public void CreateNewSet(DeckRegularDict<MonasteryCardInfo> thisCol, int whichOne)
        {
            if (thisCol.Count == 0)
                throw new BasicBlankException("Did not send in any cards for the set");
            var thisSet = CurrentMission!.MissionSets[whichOne - 1]; //because 0 based.
            var newCol = thisSet.SetType switch
            {
                EnumWhatSets.RegularSuits => thisCol, //because no change is needed in this situation.
                EnumWhatSets.RegularKinds => Rummys!.KindList(thisCol, false),
                EnumWhatSets.RegularRuns => Rummys!.RunList(thisCol, EnumRunType.None),
                EnumWhatSets.SuitRuns => Rummys!.RunList(thisCol, EnumRunType.Suit),
                EnumWhatSets.DoubleRun => Rummys!.DoubleRunList(thisCol),
                EnumWhatSets.KindColor => Rummys!.KindList(thisCol, true),
                EnumWhatSets.RunColors => Rummys!.RunList(thisCol, EnumRunType.Color),
                EnumWhatSets.EvenOdd => Rummys!.EvenOddList(thisCol),
                _ => throw new BasicBlankException("None"),
            };
            RummySet thisRummy = new RummySet(ThisMod!);
            thisRummy.CreateSet(newCol, thisSet.SetType);
            if (thisRummy.HandList.Count != thisCol.Count)
                throw new BasicBlankException("The rummy hand list don't match the list being sent in");
            if (newCol.Count == 0)
                throw new BasicBlankException("There was no list to even create a set with.");
            ThisMod!.MainSets!.CreateNewSet(thisRummy);
        }
        public async Task ExpandSetAsync(DeckRegularDict<MonasteryCardInfo> thisCol, int whichOne, int position)
        {
            var thisRummy = ThisMod!.MainSets!.GetIndividualSet(whichOne);
            RemoveCards(thisCol);
            thisCol.ForEach(thisCard =>
            {
                thisCard.IsSelected = false;
                thisCard.Drew = false;
                thisRummy.AddCard(thisCard, position);
            });
            await FinishedAsync();
        }
        public async Task FinishedAsync()
        {
            if (SingleInfo!.ObjectCount == 0)
            {
                await EndRoundAsync();
                return;
            }
            if (CanEndGame())
            {
                await ShowWinAsync();
                return;
            }
            if (DidAllPlayersComplete)
            {
                await EndRoundAsync();
                return;
            }
            await ContinueTurnAsync();
        }
        public bool IsCardSelected()
        {
            return ThisMod!.PlayerHand1!.HowManySelectedObjects > 0 || ThisMod.TempSets!.HowManySelectedObjects > 0;
        }
        private void ResetSuccess()
        {
            MissionInfo!.ForEach(thisMission =>
            {
                thisMission.MissionSets.ForEach(thisSet => thisSet.DidSucceed = false);
            });
        }
        private void PopulateMissions()
        {
            var tempList = SingleInfo!.IndexList.ToCustomBasicList();
            CustomBasicList<MissionList> otherList = new CustomBasicList<MissionList>();
            tempList.ForEach(thisIndex =>
            {
                otherList.Add(MissionInfo![thisIndex]);
            });
            ThisMod!.PopulateMissions(otherList);
        }
        public bool DidCompleteMission(out CustomBasicList<InstructionInfo> tempList)
        {
            tempList = new CustomBasicList<InstructionInfo>();
            var thisMission = GetMission;
            DeckRegularDict<MonasteryCardInfo> thisCollection;
            IDeckDict<MonasteryCardInfo> tempCollection; //hopefully still works (?)
            for (int x = 1; x <= ThisMod!.TempSets!.HowManySets; x++)
            {
                tempCollection = ThisMod.TempSets.ObjectList(x);//i think.
                thisCollection = new DeckRegularDict<MonasteryCardInfo>();
                thisCollection.AddRange(tempCollection);

                for (int y = 1; y <= thisMission.MissionSets.Count; y++)
                {
                    var newSet = thisMission.MissionSets[y - 1]; //because 0 based.

                    if (newSet.DidSucceed == false)
                    {
                        newSet.DidSucceed = newSet.SetType switch
                        {
                            EnumWhatSets.RegularSuits => Rummys!.IsSuit(thisCollection, newSet.HowMany),
                            EnumWhatSets.RegularKinds => Rummys!.IsKind(thisCollection, false, newSet.HowMany),
                            EnumWhatSets.RegularRuns => Rummys!.IsRun(thisCollection, EnumRunType.None, newSet.HowMany),
                            EnumWhatSets.SuitRuns => Rummys!.IsRun(thisCollection, EnumRunType.Suit, newSet.HowMany),
                            EnumWhatSets.DoubleRun => Rummys!.IsDoubleRun(thisCollection),
                            EnumWhatSets.KindColor => Rummys!.IsKind(thisCollection, true, newSet.HowMany),
                            EnumWhatSets.RunColors => Rummys!.IsRun(thisCollection, EnumRunType.Color, newSet.HowMany),
                            EnumWhatSets.EvenOdd => Rummys!.IsEvenOdd(thisCollection),
                            _ => throw new BasicBlankException("None"),
                        };
                        if (newSet.DidSucceed)
                        {
                            InstructionInfo thisInfo = new InstructionInfo();
                            thisInfo.WhichOne = y;
                            thisInfo.SetNumber = x;
                            tempList.Add(thisInfo);
                            break;
                        }
                    }
                }
                if (tempList.Count == thisMission.MissionSets.Count)
                    break; //so no possibility of allow more than was allowed.
            }
            if (thisMission.MissionSets.Any(items => items.DidSucceed == false))
            {
                ResetSuccess();
                return false;
            }
            ResetSuccess();
            return true;
        }
        public override async Task DiscardAsync(MonasteryCardInfo thisCard)
        {
            SingleInfo = PlayerList!.GetWhoPlayer();
            RemoveCard(thisCard.Deck);
            await AnimatePlayAsync(thisCard);
            UnselectCards();
            if (SingleInfo.ObjectCount == 8)
                throw new BasicBlankException("Cannot have only 8 cards after discarding");
            if (SingleInfo.ObjectCount == 0)
            {
                await EndRoundAsync();
                return;
            }
            await EndTurnAsync();
        }
        protected override Task AfterPickupFromDiscardAsync()
        {
            if (SingleInfo!.ObjectCount == 9)
                throw new BasicBlankException("Its impossible to have 9 cards left after picking up from discard");
            return base.AfterPickupFromDiscardAsync();
        }
        protected override Task AfterDrawingAsync()
        {
            if (SingleInfo!.ObjectCount == 9)
                throw new BasicBlankException("Its impossible to have 9 cards left after drawing");
            return base.AfterDrawingAsync();
        }
        public override async Task EndRoundAsync()
        {
            if (CanEndGame())
            {
                await ShowWinAsync();
                return;
            }
            this.RoundOverNext();
        }
        private void SetPlayerMissions()
        {
            PlayerList!.ForEach(thisPlayer => thisPlayer.UpdateIndexes()); //hopefully this simple.
        }
        Task IStartNewGame.ResetAsync()
        {
            SetPlayerMissions(); //i think this is needed here too.
            //i think this is it (not sure though).
            return Task.CompletedTask;
        }
    }
}