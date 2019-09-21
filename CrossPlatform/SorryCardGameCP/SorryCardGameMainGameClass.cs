using BasicGameFramework.Attributes;
using BasicGameFramework.BasicDrawables.Dictionary;
using BasicGameFramework.BasicEventModels;
using BasicGameFramework.DIContainers;
using BasicGameFramework.Extensions;
using BasicGameFramework.MultiplayerClasses.BasicGameClasses;
using BasicGameFramework.MultiplayerClasses.BasicPlayerClasses;
using BasicGameFramework.MultiplayerClasses.Extensions;
using BasicGameFramework.MultiplayerClasses.InterfaceMessages;
using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.Exceptions;
using System.Linq;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using js = CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.JsonSerializers.NewtonJsonStrings; //just in case i need those 2.
namespace SorryCardGameCP
{
    [SingletonGame]
    public class SorryCardGameMainGameClass : CardGameClass<SorryCardGameCardInformation, SorryCardGamePlayerItem, SorryCardGameSaveInfo>, IMiscDataNM, IChoosePieceNM
    {
        public SorryCardGameMainGameClass(IGamePackageResolver container) : base(container) { }

        internal SorryCardGameViewModel? ThisMod;
        public bool DidChooseColors { get; set; } //do here too.
        private bool _wasNew;
        private bool _didPlay;
        public int OtherTurn
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
        public override void Init() //decided to have all the code under init to prevent overflow issues.
        {
            base.Init();
            ThisMod = MainContainer.Resolve<SorryCardGameViewModel>();
        }

        public override async Task FinishGetSavedAsync()
        {
            LoadControls();
            SaveRoot!.LoadMod(ThisMod!);
            FirstCheckColors();
            ThisMod!.Pile1!.SavedDiscardPiles(SaveRoot!.OtherPileData!);
            await base.FinishGetSavedAsync();
            if (DidChooseColors == false)
                ThisMod.MainOptionsVisible = false;
        }
        public override async Task PopulateSaveRootAsync()
        {
            SaveRoot!.OtherPileData = ThisMod!.Pile2!.GetSavedPile();
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
            if (DidChooseColors == false)
            {
                ThisMod!.ColorChooser!.Visible = true;
                ThisMod.NotifyColorChange();
                if (ThisTest!.NoAnimations == false)
                    await Delay!.DelaySeconds(.5);
                await ComputerChooseColorsAsync();
                return;
            }
            if (ThisTest!.NoAnimations == false)
                await Delay!.DelaySeconds(.5);
            if (SaveRoot!.GameStatus == EnumGameStatus.HasDontBeSorry)
                throw new BasicBlankException("The computer should never have a don't be sorry");
            await EndTurnAsync();
        }
        public async Task ComputerChooseColorsAsync()
        {
            if (DidChooseColors == true)
                return;
            EnumColorChoices ThisColor = ThisMod!.ColorChooser!.ItemToChoose();
            await ChoseColorAsync(ThisColor);
        }
        protected override Task StartSetUpAsync(bool isBeginning)
        {
            LoadControls();
            SaveRoot!.LoadMod(ThisMod!);
            SaveRoot.ImmediatelyStartTurn = true;
            SaveRoot.UpTo = 0;
            PlayerList!.ForEach(thisPlayer =>
            {
                thisPlayer.Color = EnumColorChoices.None;
                if (ThisTest!.DoubleCheck == false)
                    thisPlayer.HowManyAtHome = 0;
                else
                    thisPlayer.HowManyAtHome = 1;
            });
            ThisMod!.Pile2!.ClearCards();
            SaveRoot.WasTie = false;
            ThisMod.MainOptionsVisible = false; //has to manually be done because sometimes does not work otherwise.
            return base.StartSetUpAsync(isBeginning);
        }
        private void FirstCheckColors() //hopefully this is enough.
        {
            PlayerList!.ForEach(thisPlayer => thisPlayer.LoadCommand(this)); //might as well have it here too.
            CalculateColors();
            if (DidChooseColors == false)
            {
                ThisMod!.ColorChooser!.PlayerList = PlayerList;
                ThisMod.ColorChooser.LoadColors();
                ThisMod.ColorChooser.Visible = true;
            }
        }
        public override bool CanMakeMainOptionsVisibleAtBeginning => DidChooseColors;
        protected override Task LastPartOfSetUpBeforeBindingsAsync()
        {
            FirstCheckColors();
            return base.LastPartOfSetUpBeforeBindingsAsync();
        }
        public override async Task ContinueTurnAsync()
        {
            if (DidChooseColors == false)
                SaveRoot!.Instructions = "Choose a color";
            else if (SaveRoot!.GameStatus == EnumGameStatus.ChoosePlayerToSorry)
                SaveRoot.Instructions = "Choose a player to sorry";
            else if (SaveRoot.GameStatus == EnumGameStatus.Regular)
                SaveRoot.Instructions = "Please play a card";
            else if (SaveRoot.GameStatus == EnumGameStatus.HasDontBeSorry)
                SaveRoot.Instructions = "Waiting to see if player has don't be sorry";
            else if (SaveRoot.GameStatus == EnumGameStatus.WaitForSorry21)
                SaveRoot.Instructions = "Waiting to see if a player has a sorry at 21";
            if (DidChooseColors == false)
            {
                ThisMod!.ColorChooser!.Visible = true;
                ThisMod.NotifyColorChange(); //try this too.
            }
            if (SaveRoot.GameStatus != EnumGameStatus.WaitForSorry21 && SaveRoot.GameStatus != EnumGameStatus.HasDontBeSorry)
            {
                ThisMod!.PlayerHand1!.AutoSelect = BasicGameFramework.DrawableListsViewModels.HandViewModel<SorryCardGameCardInformation>.EnumAutoType.SelectAsMany;
                await base.ContinueTurnAsync();
                return;
            }
            if (SaveRoot.GameStatus == EnumGameStatus.HasDontBeSorry)
            {
                if (SaveRoot.LastSorry == EnumSorry.At21)
                {
                    await PossibleStartTimerAsync(WhoTurn);
                    return;
                }
                if (OtherTurn == 0)
                    throw new BasicBlankException("Can't have otherturn of 0 when the status is don't be sorry");
                if (OtherTurn == WhoTurn)
                    throw new BasicBlankException("Can't have otherturn the same as the players turn for don't be sorry");
                await PossibleStartTimerAsync(OtherTurn);
                return;
            }
            if (SaveRoot.GameStatus == EnumGameStatus.WaitForSorry21)
            {
                var tempPlayer = PlayerList!.GetSelf();
                if (tempPlayer.PlayerCategory != EnumPlayerCategory.Self)
                    throw new BasicBlankException("Not self");
                if (tempPlayer.OtherTurn == false)
                {
                    await ShowHumanCanPlayAsync();
                    return;
                }
                if (tempPlayer.PlayerCategory == EnumPlayerCategory.Computer)
                {
                    await ComputerTurnAsync();
                    return;
                }
                ThisCheck!.IsEnabled = true;
                return;
            }
            throw new BasicBlankException("Don't know what to do now");
        }

        private async Task PossibleStartTimerAsync(int player)
        {
            await SaveStateAsync();
            var tempPlayer = PlayerList![player];
            if (tempPlayer.PlayerCategory == EnumPlayerCategory.Computer)
                throw new BasicBlankException("Computer should not be playing");
            if (tempPlayer.PlayerCategory == EnumPlayerCategory.OtherHuman)
            {
                ThisCheck!.IsEnabled = true;
                return;
            }
            await ShowHumanCanPlayAsync();
        }
        async Task IMiscDataNM.MiscDataReceived(string status, string content)
        {
            switch (status)
            {
                case "sorrycard":
                    var firstSend = await GetSentPlayAsync(content);
                    if (SaveRoot!.GameStatus == EnumGameStatus.Regular || SaveRoot.GameStatus == EnumGameStatus.ChoosePlayerToSorry)
                    {
                        EnableMessages();
                        return;
                    }
                    var tempPlayer1 = PlayerList![firstSend.Player];
                    if (tempPlayer1.OtherTurn == true)
                    {
                        EnableMessages();
                        return;
                    }
                    var thisCard = tempPlayer1.MainHandList.GetSpecificItem(firstSend.Deck);
                    await PlaySorryCardAsync(thisCard, firstSend.Player);
                    return;
                case "sorryplayer":
                    await SorryPlayerAsync(int.Parse(content));
                    return;
                case "regularplay":
                    CustomBasicList<int> thisList = await js.DeserializeObjectAsync<CustomBasicList<int>>(content);
                    var newCol = thisList.GetNewObjectListFromDeckList(DeckList!);
                    await PlaySeveralCards(newCol);
                    return;
                case "timeout":
                    if (SaveRoot!.GameStatus == EnumGameStatus.Regular || SaveRoot.GameStatus == EnumGameStatus.ChoosePlayerToSorry)
                    {
                        EnableMessages();
                        return;
                    }
                    var finPlayer = PlayerList![int.Parse(content)];
                    if (finPlayer.OtherTurn == true)
                    {
                        EnableMessages();
                        return;
                    }
                    await NoSorryAsync(finPlayer.Id);
                    return;
                default:
                    throw new BasicBlankException($"Nothing for status {status}  with the message of {content}");
            }
        }
        public override async Task StartNewTurnAsync()
        {
            await base.StartNewTurnAsync();
            if (DidChooseColors == false && ThisMod!.ColorChooser!.Visible == false)
            {
                ThisMod.ColorChooser.Visible = true;
                ThisMod.ColorChooser.LoadColors();
            }
            PlayerList!.ForEach(thisPlayer => thisPlayer.OtherTurn = true); //has to manually set to false.
            SaveRoot!.GameStatus = EnumGameStatus.Regular;
            SaveRoot.LastSorry = EnumSorry.None;
            OtherTurn = 0;
            await base.StartNewTurnAsync();
            SingleInfo = PlayerList.GetWhoPlayer();
            if (DidChooseColors == true)
            {
                await StartDrawingAsync();
                return;
            }
            await ContinueTurnAsync(); //most of the time, continue turn.  can change to what is needed
        }
        public override async Task EndTurnAsync()
        {
            SingleInfo = PlayerList!.GetWhoPlayer();
            SingleInfo.MainHandList.UnhighlightObjects(); //i think this is best.
            if (SaveRoot!.UpTo > 21)
            {
                await BustAsync();
            }
            else if (SaveRoot.UpTo == 21 && SaveRoot.GameStatus == EnumGameStatus.Regular)
            {
                SingleInfo.HowManyAtHome++;
                if (ThisData!.MultiPlayer == true)
                {
                    SaveRoot.GameStatus = EnumGameStatus.WaitForSorry21;
                    PlayerList.ForConditionalItems(items => items.Id != SingleInfo.Id, thisPlayer =>
                    {
                        thisPlayer.OtherTurn = false;
                    });
                    SingleInfo.OtherTurn = true;
                    ThisMod!.CommandContainer!.ManuelFinish = true; //this too i think.
                    await ContinueTurnAsync();
                    return;
                }
                else
                {
                    await ClearPointsAsync(); //because the computer will never decide whether it has sorry21s.
                }
            }
            else if (SaveRoot.UpTo == 21)
            {
                throw new BasicBlankException("Don't know what to do.  Could be a never ending loop.  Should have cleared the points first");
            }

            if (PlayerList.Any(items => items.HowManyAtHome == 4))
            {
                try
                {
                    SingleInfo = PlayerList.Single(items => items.HowManyAtHome == 4);
                }
                catch
                {
                    throw new BasicBlankException("Possible tie.  Rethink");
                }
                SaveRoot.GameStatus = EnumGameStatus.Regular; //i think this is needed too.
                await ShowWinAsync();
                return;
            }
            ThisMod!.CommandContainer!.ManuelFinish = true; //because it could be somebody else's turn.
            WhoTurn = await PlayerList.CalculateWhoTurnAsync(true);
            await StartNewTurnAsync();
        }
        private async Task BustAsync()
        {
            await ThisMod!.ShowGameMessageAsync($"{SingleInfo!.NickName} has busted.  Everybody else gets one home");
            PlayerList!.ForConditionalItems(items => items.Id != SingleInfo.Id, thisPlayer =>
            {
                thisPlayer.HowManyAtHome++;
            });
            SaveRoot!.UpTo = 0;
            await ClearPointsAsync();
        }
        public async Task ClearPointsAsync()
        {
            if (ThisMod!.Pile2!.PileEmpty())
                return;
            var newList = ThisMod.Pile2.DiscardList().ToRegularDeckDict();
            newList.Add(ThisMod.Pile2.GetCardInfo());
            newList.Reverse();
            await newList.ForEachAsync(async thisCard =>
            {
                ThisMod.Pile2.RemoveCardFromPile(thisCard);
                await AnimatePlayAsync(thisCard);
                if (ThisTest!.NoAnimations == false)
                    await Delay!.DelaySeconds(.25);
            });
            ThisMod.Pile2.ClearCards();
            SaveRoot!.UpTo = 0;
        }
        private async Task StartDrawingAsync()
        {
            if (SingleInfo!.MainHandList.Count >= 5)
            {
                await ContinueTurnAsync();
                return; //because you already have at least 5 cards.
            }
            PlayerDraws = WhoTurn;
            _wasNew = SingleInfo.MainHandList.Count == 0;
            LeftToDraw = 5 - SingleInfo.MainHandList.Count;
            if (LeftToDraw == 1)
            {
                LeftToDraw = 0;
                PlayerDraws = 0;
            }
            await DrawAsync();
        }
        private async Task Draw2CardsAsync()
        {
            _didPlay = true;
            LeftToDraw = 2;
            _wasNew = false;
            await DrawAsync();
        }
        protected override async Task AfterDrawingAsync()
        {
            if (_wasNew == true)
                SingleInfo!.MainHandList.UnhighlightObjects();
            if (SingleInfo!.PlayerCategory == EnumPlayerCategory.Self)
                SortCards();
            if (_didPlay == false)
            {
                await base.AfterDrawingAsync();
                return;
            }
            _didPlay = false;
            await EndTurnAsync();
        }
        private void EnableMessages()
        {
            int thisPlayer = PlayerList!.GetSelf().Id;
            if (thisPlayer == WhoTurn)
                return; //because your turn.
            ThisCheck!.IsEnabled = true;
        }
        private async Task<SorryPlay> GetSentPlayAsync(string message)
        {
            return await js.DeserializeObjectAsync<SorryPlay>(message);
        }
        async Task IChoosePieceNM.ChoosePieceReceivedAsync(string data)
        {
            EnumColorChoices thisColor = await js.DeserializeObjectAsync<EnumColorChoices>(data);
            await ChoseColorAsync(thisColor);
        }
        public async Task ChoseColorAsync(EnumColorChoices thisColor)
        {
            if (SingleInfo!.CanSendMessage(ThisData!) == true)
                await ThisNet!.SendAllAsync("chosepiece", thisColor);
            ThisMod!.ColorChooser!.ChooseItem(thisColor);
            if (ThisTest!.NoAnimations == false)
                await Delay!.DelaySeconds(.5);
            SingleInfo!.Color = thisColor;
            ThisMod!.CommandContainer!.ManuelFinish = true;
            CalculateColors();
            ThisMod.ColorChooser.Visible = false;
            ThisMod.NotifyColorChange();
            if (DidChooseColors == false)
            {
                await EndTurnAsync();
                return;
            }
            await AfterChoosingColorsAsync();
        }
        private async Task AfterChoosingColorsAsync()
        {
            ThisMod!.MainOptionsVisible = true; //try this too.
            WhoTurn = WhoStarts;
            await StartNewTurnAsync();
        }
        private void CalculateColors()
        {
            DidChooseColors = false;
            foreach (var thisPlayer in PlayerList!)
            {
                if (thisPlayer.DidChooseColor == false && thisPlayer.InGame == true)
                    return;
            }
            DidChooseColors = true;
        }
        public async Task SorryPlayerAsync(int player)
        {
            if (SaveRoot!.GameStatus != EnumGameStatus.ChoosePlayerToSorry)
                throw new BasicBlankException("Can't sorry a player since its choosing who to sorry");
            OtherTurn = player;
            var tempPlayer = PlayerList![OtherTurn];
            if (tempPlayer.HowManyAtHome == 0)
                throw new BasicBlankException("Had none at home. Therefore; can't sorry a player");
            tempPlayer.HowManyAtHome--;
            SaveRoot.LastSorry = EnumSorry.Regular;
            ThisMod!.CommandContainer!.ManuelFinish = true; //here too.
            if (ThisData!.MultiPlayer == false)
            {
                if (SingleInfo!.PlayerCategory == EnumPlayerCategory.Computer)
                    throw new BasicBlankException("The computer can't sorry a player");
                await EndTurnAsync();
                return;
            }
            SaveRoot.GameStatus = EnumGameStatus.HasDontBeSorry;
            PlayerList.ForEach(thisPlayer => thisPlayer.OtherTurn = true);
            tempPlayer.OtherTurn = false;
            await ContinueTurnAsync();
        }
        public async Task NoSorryAsync(int player)
        {
            if (SaveRoot!.GameStatus == EnumGameStatus.HasDontBeSorry)
            {
                await ClearPointsAsync();
                await EndTurnAsync();
                return;
            }
            if (SaveRoot.GameStatus != EnumGameStatus.WaitForSorry21)
                throw new BasicBlankException("Don't know of any other cases where it needs a NoSorry routine");
            var tempPlayer = PlayerList![player];
            tempPlayer.OtherTurn = true;
            if (!PlayerList.Any(items => items.OtherTurn == false))
            {
                await ClearPointsAsync();
                await EndTurnAsync();
                return;
            }
            await ContinueTurnAsync();
        }
        private async Task PlaySingleCardAsync(SorryCardGameCardInformation thisCard)
        {
            thisCard.Drew = false;
            thisCard.IsSelected = false;
            if (thisCard.Category == EnumCategory.Regular)
            {
                SaveRoot!.UpTo += thisCard.Value;
            }
            else if (thisCard.Category == EnumCategory.Slide)
            {
                SaveRoot!.UpTo = thisCard.Value;
            }
            else if (thisCard.Category == EnumCategory.Switch)
            {
                PlayerList!.ChangeReverse();
            }
            if (SingleInfo!.MainHandList.ObjectExist(thisCard.Deck))
            {
                SingleInfo.MainHandList.RemoveObjectByDeck(thisCard.Deck);
            }
            else if (OtherTurn > 0)
            {
                var tempPlayer = PlayerList!.GetOtherPlayer();
                if (tempPlayer.MainHandList.ObjectExist(thisCard.Deck))
                    tempPlayer.MainHandList.RemoveObjectByDeck(thisCard.Deck);
            }
            await ThisE.AnimateCardAsync(thisCard, EnumAnimcationDirection.StartDownToCard, "otherpile", finalAction: () => ThisMod!.Pile2!.AddCard(thisCard));
        }
        public async Task PlaySorryCardAsync(SorryCardGameCardInformation thisCard, int player)
        {
            if (thisCard.Sorry != EnumSorry.At21 && thisCard.Sorry != EnumSorry.Dont)
                throw new BasicBlankException("Can't play a sorry card except for a sorry at 21 or don't be sorry");
            if (thisCard.Sorry == EnumSorry.Dont)
            {
                if (SaveRoot!.GameStatus != EnumGameStatus.HasDontBeSorry)
                    throw new BasicBlankException("Don't be sorry can't be played unless the status is don't be sorry");
            }
            if (thisCard.Sorry == EnumSorry.At21)
            {
                if (SaveRoot!.GameStatus != EnumGameStatus.WaitForSorry21)
                    throw new BasicBlankException("Can't play a sorry at 21 because nobody is at 21");
            }
            SorryCardGamePlayerItem tempPlayer;
            ThisMod!.CommandContainer!.ManuelFinish = true; //because you can't go automatically now.
            if (thisCard.Sorry == EnumSorry.At21)
            {
                if (ThisData!.MultiPlayer == false)
                    throw new BasicBlankException("Can't be at 21 for single player games currently");
                SaveRoot!.GameStatus = EnumGameStatus.HasDontBeSorry;
                OtherTurn = player;
                SaveRoot.LastSorry = EnumSorry.At21;
                SingleInfo!.HowManyAtHome--;
                tempPlayer = PlayerList!.GetOtherPlayer();
                tempPlayer.HowManyAtHome++;
                await PlaySingleCardAsync(thisCard);
                PlayerList.ForEach(thisPlayer => thisPlayer.OtherTurn = true);
                SingleInfo.OtherTurn = false;
                await ContinueTurnAsync();
                return;
            }
            if (thisCard.Sorry == EnumSorry.Dont)
            {
                if (SaveRoot!.LastSorry == EnumSorry.Regular)
                {
                    if (player != OtherTurn)
                        throw new BasicBlankException("Player must be the player who was the sorry was played on");
                    tempPlayer = PlayerList!.GetOtherPlayer();
                    tempPlayer.HowManyAtHome++;
                    SingleInfo = PlayerList.GetWhoPlayer();
                    await PlaySingleCardAsync(thisCard);
                    await EndTurnAsync();
                    return;
                }
                if (player != WhoTurn)
                    throw new BasicBlankException("The current player whose turn it is must be the one who plays the don't be sorry");
                SingleInfo = PlayerList!.GetWhoPlayer();
                SingleInfo.HowManyAtHome++;
                tempPlayer = PlayerList.GetOtherPlayer();
                tempPlayer.HowManyAtHome--;
                await PlaySingleCardAsync(thisCard);
                await ClearPointsAsync();
                await EndTurnAsync();
                return;
            }
            throw new BasicBlankException("Don't know what to do from here");
        }
        public async Task PlaySeveralCards(IDeckDict<SorryCardGameCardInformation> thisList)
        {
            if (SaveRoot!.GameStatus == EnumGameStatus.ChoosePlayerToSorry)
                throw new BasicBlankException("Can't choose player to sorry and play the cards");
            ThisMod!.CommandContainer!.ManuelFinish = true; //just in case.
            if (thisList.Count == 1)
            {
                var tempCard = thisList.First();
                await PlaySingleCardAsync(tempCard);
                if (tempCard.Sorry == EnumSorry.Regular)
                {
                    if (PlayerList.Any(items => items.Id != WhoTurn && items.HowManyAtHome > 0))
                    {
                        SaveRoot.GameStatus = EnumGameStatus.ChoosePlayerToSorry;
                        await ContinueTurnAsync();
                        return;
                    }
                }
                if (SaveRoot.GameStatus == EnumGameStatus.Regular)
                {
                    if (tempCard.Category == EnumCategory.Take2)
                    {
                        if (SingleInfo!.MainHandList.Count <= 5)
                        {
                            await Draw2CardsAsync();
                            return;
                        }
                    }
                    await EndTurnAsync();
                    return;
                }
                if (SaveRoot.GameStatus == EnumGameStatus.HasDontBeSorry)
                    throw new BasicBlankException("Must run a different process for don't be sorry");
                if (SaveRoot.GameStatus == EnumGameStatus.WaitForSorry21)
                    throw new BasicBlankException("Must run a different process for wait for sorry 21");
                throw new BasicBlankException("Wrong");
            }
            thisList = thisList.OrderByDescending(items => items.Category).ThenBy(items => items.Value).ToRegularDeckDict();
            await thisList.ForEachAsync(async thisCard =>
            {
                await PlaySingleCardAsync(thisCard);
                if (ThisTest!.NoAnimations == false)
                    await Delay!.DelaySeconds(.3);
            });
            await EndTurnAsync();
        }
        private bool IsSingleValid(SorryCardGameCardInformation thisCard)
        {
            if (thisCard.Category == EnumCategory.Sorry)
            {
                if (thisCard.Sorry == EnumSorry.Regular)
                {
                    return PlayerList.Any(items => items.Id != WhoTurn && items.HowManyAtHome > 0);
                }
                if (thisCard.Sorry == EnumSorry.At21 || thisCard.Sorry == EnumSorry.Dont)
                    return false;
            }
            if (thisCard.Category == EnumCategory.Take2)
                return SingleInfo!.MainHandList.Count < 7;
            return thisCard.Category != EnumCategory.Play2;
        }
        public bool IsValidMove(IDeckDict<SorryCardGameCardInformation> thisList)
        {
            if (thisList.Count > 1)
            {
                if (thisList.Count != 3)
                    return false;
                if (thisList.Count(items => items.Category == EnumCategory.Play2) != 1)
                    return false;
                if (thisList.Count(items => items.Category == EnumCategory.Regular) != 2)
                    return false;
                return true;
            }
            var thisCard = thisList.Single();
            bool rets = IsSingleValid(thisCard);
            if (rets == true)
                return true;
            if (!SingleInfo!.MainHandList.Any(items => IsSingleValid(items)))
                return true;
            return false;
        }
    }
}