using BasicGameFrameworkLibrary.BasicDrawables.Dictionary;
using BasicGameFrameworkLibrary.CommandClasses;
using BasicGameFrameworkLibrary.DIContainers;
using BasicGameFrameworkLibrary.RegularDeckOfCards;
using BasicGameFrameworkLibrary.SolitaireClasses.Cards;
using BasicGameFrameworkLibrary.SolitaireClasses.ClockClasses;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.Messenging;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace ClockSolitaireCP.Logic
{
    public class ClockBoard : ClockObservable
    {
        public ClockBoard(IClockVM thisMod,
            ClockSolitaireMainGameClass mainGame,
            CommandContainer command,
            IGamePackageResolver resolver,
            IEventAggregator aggregator
            ) : base(thisMod, command, resolver)
        {
            _mainGame = mainGame;
            _aggregator = aggregator;
            ShowCenter = true;
            LoadBoard();
        }
        public int CardsLeft
        {
            get => _mainGame.SaveRoot.CardsLeft;
            private set => _mainGame.SaveRoot.CardsLeft = value;
        }
        private int PreviousOne
        {
            get => _mainGame.SaveRoot.PreviousOne;
            set => _mainGame.SaveRoot.PreviousOne = value;
        }
        private bool _wasSaved;
        private readonly ClockSolitaireMainGameClass _mainGame;
        private readonly IEventAggregator _aggregator;

        public override void LoadSavedClocks(CustomBasicList<ClockInfo> thisList)
        {
            _wasSaved = true;
            if (_mainGame.SaveRoot.CurrentCard == 0)
                CurrentCard = null;
            else
            {
                CurrentCard = new SolitaireCard();
                CurrentCard.Populate(_mainGame.SaveRoot.CurrentCard); //to clone.
            }
            base.LoadSavedClocks(thisList);
            SendMessage(PreviousOne, EnumCardMessageCategory.Known);
        }
        public void SendSavedMessage()
        {
            if (_wasSaved == false)
            {
                SendMessage(12, EnumCardMessageCategory.Known);
                return;
            }
            SendMessage(PreviousOne, EnumCardMessageCategory.Known);
        }

        public void SaveGame()
        {
            if (CurrentCard == null)
                _mainGame.SaveRoot.CurrentCard = 0;
            else
                _mainGame.SaveRoot.CurrentCard = CurrentCard.Deck;
            _mainGame.SaveRoot.SavedClocks = ClockList!.ToCustomBasicList();
        }


        public void NewGame(DeckRegularDict<SolitaireCard> thisCol)
        {
            CardsLeft = 51; //because one is given.
            if (thisCol.Count != 52)
                throw new BasicBlankException("Must have 52 cards");
            ClearBoard();
            int y = 0;
            4.Times(x =>
            {
                ClockList!.ForEach(thisClock =>
                {
                    y++;
                    var thisCard = thisCol[y - 1]; //because 0 based
                    if (y == 52)
                    {
                        CurrentCard = thisCard;
                        CurrentCard.IsUnknown = false;
                    }
                    else
                        thisClock.CardList.Add(thisCard);
                });
            });
            EnablePiles();
            PreviousOne = 12;
            SendMessage(12, EnumCardMessageCategory.Known);
        }
        public bool HasWon() => CardsLeft == 0;
        public bool IsGameOver()
        {
            if (CurrentCard!.Value != EnumCardValueList.King)
                return false;
            return HasCard(12) == false;
        }
        public bool IsValidMove(int whichOne) => (int)CurrentCard!.Value == whichOne + 1;
        public void MakeMove(int whichOne)
        {
            SendMessage(PreviousOne, EnumCardMessageCategory.Hidden);
            PreviousOne = whichOne;
            CardsLeft--;
            CurrentCard = GetLastCard(whichOne);
            RemoveCardFromPile(whichOne);
            SendMessage(whichOne, EnumCardMessageCategory.Known);
        }
        protected override Task ClickCurrentCardProcessAsync()
        {
            return OnClockClickedAsync(PreviousOne);
        }
        private void SendMessage(int index, EnumCardMessageCategory category)
        {
            CurrentCardEventModel thisMessage = new CurrentCardEventModel();
            thisMessage.ThisClock = ClockList![index];
            thisMessage.ThisCategory = category;
            _aggregator.Publish(thisMessage);
        }
    }
}