using BasicGameFramework.Attributes;
using BasicGameFramework.BasicDrawables.Dictionary;
using BasicGameFramework.DIContainers;
using BasicGameFramework.Extensions;
using BasicGameFramework.MultiplayerClasses.BasicGameClasses;
using BasicGameFramework.MultiplayerClasses.Extensions;
using BasicGameFramework.MultiplayerClasses.InterfaceMessages;
using BasicGameFramework.MultiplayerClasses.InterfacesForHelpers;
using BasicGameFramework.RegularDeckOfCards;
using BasicGameFramework.SpecializedGameTypes.TrickClasses;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using CommonBasicStandardLibraries.Exceptions;
using System.Linq;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace SnagCardGameCP
{
    [SingletonGame]
    public class SnagCardGameMainGameClass : TrickGameClass<EnumSuitList, SnagCardGameCardInformation,
        SnagCardGamePlayerItem, SnagCardGameSaveInfo>, IMiscDataNM, IStartNewGame
    {
        private IAdvancedTrickProcesses? _aTrick;
        public new TrickViewModel? TrickArea1;
        public SnagCardGameMainGameClass(IGamePackageResolver container) : base(container) { }
        internal SnagCardGameViewModel? ThisMod;
        public override void Init() //decided to have all the code under init to prevent overflow issues.
        {
            base.Init();
            ThisMod = MainContainer.Resolve<SnagCardGameViewModel>();
        }
        public override async Task FinishGetSavedAsync()
        {
            LoadControls();
            LoadVM();
            if (TrickArea1 == null)
                TrickArea1 = MainContainer.Resolve<TrickViewModel>();
            ShowBasicControls(); //i think most of the time, will not show the others.  if i am wrong, rethink.
            ThisMod!.Bar1!.LoadSavedGame();
            await base.FinishGetSavedAsync();
            _aTrick!.LoadGame();
        }
        private void LoadControls()
        {
            if (IsLoaded == true)
                return;
            LoadTrickAreas();
            ThisMod!.LoadControls(); //i think here is the best place for it.
            _aTrick = MainContainer.Resolve<IAdvancedTrickProcesses>(); //decided to be here.
            IsLoaded = true; //i think needs to be here.
        }
        internal async Task TakeCardAsync(int Deck)
        {
            if (Deck == 0)
                throw new BasicBlankException("The deck cannot be 0 when taking card");
            if (WhoTurn == 0)
                throw new BasicBlankException("The turn cannot be 0 when taking card");
            if (SingleInfo!.CanSendMessage(ThisData!) == true)
                await ThisNet!.SendAllAsync("cardchosen", Deck);
            TrickArea1!.SelectCard(Deck);
            if (ThisTest!.NoAnimations == false)
                await Delay!.DelaySeconds(.5);
            var thisCard = DeckList!.GetSpecificItem(Deck);
            thisCard.Player = WhoTurn;
            SaveRoot!.CardList.Add(thisCard);
            TrickArea1.RemoveCard(Deck);
            SingleInfo!.CardsWon++;
            int lefts = TrickArea1.HowManyCardsLeft;
            if (lefts == 1)
            {
                int final = TrickArea1.GetLastCard;
                thisCard = DeckList.GetSpecificItem(final);
                if (WhoTurn == 1)
                    thisCard.Player = 2;
                else
                    thisCard.Player = 1;
                var thisPlayer = PlayerList![thisCard.Player];
                thisPlayer.CardsWon++;
                SaveRoot.CardList.Add(thisCard);
                if (ThisMod!.Bar1!.HandList.Count > 0)
                {
                    WhoTurn = thisCard.Player;
                    SingleInfo = PlayerList.GetWhoPlayer();
                    await StartNewTrickAsync(); //hopefully this simple
                    return;
                }
                await EndRoundAsync();
                return;
            }
            await EndTurnAsync();
        }
        public override Task PopulateSaveRootAsync()
        {
            SaveRoot!.BarList = ThisMod!.Bar1!.HandList.ToRegularDeckDict();
            return base.PopulateSaveRootAsync();
        }
        protected override async Task ComputerTurnAsync()
        {
            if (ThisTest!.NoAnimations == false)
                await Delay!.DelaySeconds(.25);
            if (SaveRoot!.GameStatus == EnumStatusList.Normal)
            {
                await PlayCardAsync(ComputerAI.CardToPlay(this));
                return;
            }
            await TakeCardAsync(ComputerAI.CardToTake(this));
        }
        protected override Task StartSetUpAsync(bool isBeginning)
        {
            LoadControls();
            LoadVM();
            if (isBeginning == false)
                ShowBasicControls();
            if (TrickArea1 == null)
                TrickArea1 = MainContainer.Resolve<TrickViewModel>();
            SaveRoot!.CardList.Clear();
            PlayerList!.ForEach(thisPlayer =>
            {
                thisPlayer.CardsWon = 0;
            });
            ResetCards();
            return base.StartSetUpAsync(isBeginning);
        }
        private void ResetCards()
        {
            SaveRoot!.GameStatus = EnumStatusList.Normal;
            SaveRoot.FirstCardPlayed = false;
        }
        protected override Task LastPartOfSetUpBeforeBindingsAsync()
        {
            TrickArea1!.ClearBoard();
            ThisMod!.Bar1!.LoadBarCards(CardInfo!.DummyHand);
            return base.LastPartOfSetUpBeforeBindingsAsync();
        }
        async Task IMiscDataNM.MiscDataReceived(string status, string content)
        {
            switch (status)
            {
                case "cardchosen":
                    await TakeCardAsync(int.Parse(content));
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
            if (SaveRoot!.FirstCardPlayed == false)
                SaveRoot.FirstCardPlayed = true;
            ThisMod!.CommandContainer!.ManuelFinish = true; //because it could be somebody else's turn.
            WhoTurn = await PlayerList.CalculateWhoTurnAsync();
            this.ShowTurn();
            await SaveStateAsync();
            await ContinueTurnAsync();
        }
        public override Task ContinueTurnAsync()
        {
            if (SaveRoot!.FirstCardPlayed == false)
                ThisMod!.Instructions = "Choose a card from the bar to start the trick";
            else if (SaveRoot.GameStatus == EnumStatusList.Normal)
                ThisMod!.Instructions = "Choose a card from the hand";
            else
                ThisMod!.Instructions = "Choose a card to take";
            return base.ContinueTurnAsync();
        }
        private int WhoWonTrick(DeckObservableDict<SnagCardGameCardInformation> thisCol)
        {
            int begins = thisCol.First().Player;
            EnumCardValueList highestNumber = thisCol.OrderByDescending(items => items.Value).First().Value;
            SnagCardGameCardInformation thisCard;
            int x;
            if (thisCol.Count(items => items.Value == highestNumber) == 1)
            {
                for (x = 1; x <= 3; x++) //no ties
                {
                    thisCard = thisCol[x - 1];
                    if (thisCard.Value == highestNumber && (x == 1 || x == 3))
                        return begins;
                    else if (thisCard.Value == highestNumber)
                    {
                        if (begins == 1)
                            return 2;
                        else
                            return 1;
                    }
                }
            }
            for (x = 3; x >= 1; x += -1) //ties
            {
                thisCard = thisCol[x - 1];
                if ((thisCard.Value == highestNumber) & ((x == 1) | (x == 3)))
                    return begins;
                else if (thisCard.Value == highestNumber)
                {
                    if (begins == 1)
                        return 2;
                    else
                        return 1;
                }
            }
            throw new BasicBlankException("Could not find who won");
        }
        public override async Task EndTrickAsync()
        {
            var trickList = SaveRoot!.TrickList;
            int wins = WhoWonTrick(trickList);
            WhoTurn = wins;
            SaveRoot.GameStatus = EnumStatusList.ChooseCards;
            await StartNewTurnAsync();
        }
        private void ShowBasicControls()
        {
            ThisMod!.PlayerHand1!.Visible = true;
            ThisMod.Opponent1!.Visible = false;
            ThisMod.Human1!.Visible = false;
        }
        private async Task StartNewTrickAsync()
        {
            _aTrick!.ClearBoard();
            ResetCards();
            ThisMod!.CommandContainer!.ManuelFinish = true; //because it could be somebody else's turn.
            await StartNewTurnAsync(); //hopefully this simple.
        }
        Task IStartNewGame.ResetAsync()
        {
            ShowBasicControls();
            PlayerList!.ForEach(thisPlayer =>
            {
                thisPlayer.CurrentPoints = 0;
                thisPlayer.TotalPoints = 0;
            });
            return Task.CompletedTask;
        }
        private void Scoring()
        {
            DeckRegularDict<SnagCardGameCardInformation> firstCol = SaveRoot!.CardList.Where(items => items.Player == 1).ToRegularDeckDict();
            DeckRegularDict<SnagCardGameCardInformation> secondCol = SaveRoot.CardList.Where(items => items.Player == 2).ToRegularDeckDict();
            int wins = WhoWonRound(firstCol, secondCol, out int suits);
            int points = HowManyPoints(suits);
            var firstPlayer = PlayerList.First();
            var secondPlayer = PlayerList.Last();
            if (wins == 1)
            {
                firstPlayer.CurrentPoints = points;
                secondPlayer.CurrentPoints = 0;
                firstPlayer.TotalPoints += points;
            }
            else
            {
                secondPlayer.CurrentPoints = points;
                secondPlayer.TotalPoints += points;
                firstPlayer.CurrentPoints = 0;
            }
            DeckRegularDict<SnagCardGameCardInformation> humanList = new DeckRegularDict<SnagCardGameCardInformation>();
            DeckRegularDict<SnagCardGameCardInformation> opponentList = new DeckRegularDict<SnagCardGameCardInformation>();
            firstCol.ForEach(thisCard =>
            {
                thisCard.IsSelected = false;
                if (SelfPlayer == 1)
                    humanList.Add(thisCard);
                else
                    opponentList.Add(thisCard);
            });
            secondCol.ForEach(thisCard =>
            {
                thisCard.IsSelected = false;
                if (SelfPlayer == 1)
                    opponentList.Add(thisCard);
                else
                    humanList.Add(thisCard);
            });
            ThisMod!.Human1!.HandList.ReplaceRange(humanList);
            ThisMod.Opponent1!.HandList.ReplaceRange(opponentList);
            ThisMod.PlayerHand1!.Visible = false;
            ThisMod.Human1.Visible = true;
            ThisMod.Opponent1.Visible = true;
            TrickArea1!.Visible = false; //i think this too.
        }
        private int HowManyPoints(int numberSuits)
        {
            int output = 0;
            numberSuits.Times(x =>
            {
                output += x;
            });
            return output;
        }
        private void SortCards(DeckRegularDict<SnagCardGameCardInformation> firstCol, DeckRegularDict<SnagCardGameCardInformation> secondCol)
        {
            var temps = firstCol.GroupOrderDescending(items => items.Suit).ToCustomBasicList();
            EnumSuitList newSuit;
            DeckRegularDict<SnagCardGameCardInformation> newCol = new DeckRegularDict<SnagCardGameCardInformation>();
            temps.ForEach(thisItem =>
            {
                newSuit = thisItem.Key;
                newCol.AddRange(firstCol.Where(items => items.Suit == newSuit));
            });
            firstCol.ReplaceRange(newCol);
            newCol.Clear();
            temps = secondCol.GroupOrderDescending(items => items.Suit).ToCustomBasicList();
            temps.ForEach(ThisItem =>
            {
                newSuit = ThisItem.Key;
                newCol.AddRange(secondCol.Where(items => items.Suit == newSuit));
            });
            secondCol.ReplaceRange(newCol);
        }
        private int WhoWonRound(DeckRegularDict<SnagCardGameCardInformation> firstCol, DeckRegularDict<SnagCardGameCardInformation> secondCol, out int numberOfSuits)
        {
            var firstTemp = firstCol.GroupOrderDescending(items => items.Suit).ToCustomBasicList();
            var secondTemp = secondCol.GroupOrderDescending(items => items.Suit).ToCustomBasicList();
            int x = 0;
            do
            {
                if (firstTemp.Count > secondTemp.Count && x == secondTemp.Count)
                {
                    numberOfSuits = firstTemp[x].Count(); //hopefully this simple.
                    SortCards(firstCol, secondCol);
                    return 1;
                }
                if (secondTemp.Count > firstTemp.Count && x == firstTemp.Count)
                {
                    numberOfSuits = secondTemp[x].Count();
                    SortCards(firstCol, secondCol);
                    return 2;
                }
                if (firstTemp[x].Count() > secondTemp[x].Count())
                {
                    numberOfSuits = firstTemp[x].Count();
                    SortCards(firstCol, secondCol);
                    return 1;
                }
                if (secondTemp[x].Count() > firstTemp[x].Count())
                {
                    numberOfSuits = secondTemp[x].Count();
                    SortCards(firstCol, secondCol);
                    return 2;
                }
                x++;
            } while (true);
        }
        public override async Task EndRoundAsync()
        {
            ThisMod!.Instructions = "None";
            Scoring();
            if (CanEndGame() == true)
            {
                await ShowWinAsync();
                return;
            }
            this.RoundOverNext();
        }
        private bool CanEndGame()
        {
            SnagCardGamePlayerItem tempPlayer = PlayerList.Where(items => items.TotalPoints >= 50).OrderByDescending(Items => Items.TotalPoints).FirstOrDefault();
            if (tempPlayer == null)
                return false;
            SingleInfo = tempPlayer;
            return true;
        }
    }
}