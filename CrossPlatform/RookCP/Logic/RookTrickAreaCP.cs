using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.BasicDrawables.Dictionary;
using BasicGameFrameworkLibrary.ColorCards;
using BasicGameFrameworkLibrary.Extensions;
using BasicGameFrameworkLibrary.SpecializedGameTypes.TrickClasses;
using CommonBasicStandardLibraries.Exceptions;
using RookCP.Cards;
using RookCP.Data;
using System.Linq;
using System.Threading.Tasks;

namespace RookCP.Logic
{
    [SingletonGame]
    [AutoReset]
    public class RookTrickAreaCP : PossibleDummyTrickObservable<EnumColorTypes, RookCardInformation, RookPlayerItem, RookSaveInfo>, IAdvancedTrickProcesses
    {
        private readonly RookGameContainer _gameContainer;

        public RookTrickAreaCP(RookGameContainer gameContainer) : base(gameContainer)
        {
            _gameContainer = gameContainer;
            if (_gameContainer.PlayerList!.Count() == 4)
                UseDummy = false;
            else
                UseDummy = true;
        }

        protected override bool UseDummy { get; set; }

        protected override int GetCardIndex()
        {
            var thisView = ViewList.Single(items => items.Player == _gameContainer.WhoTurn && items.PossibleDummy == _gameContainer.SaveRoot!.DummyPlay);
            return ViewList!.IndexOf(thisView); //hopefully this simple.
        }

        protected override void PopulateNewCard(RookCardInformation oldCard, ref RookCardInformation newCard)
        {
            newCard.IsDummy = oldCard.IsDummy;
        }

        protected override void PopulateOldCard(RookCardInformation oldCard)
        {
            oldCard.IsDummy = _gameContainer.SaveRoot!.DummyPlay;
        }

        protected override async Task ProcessCardClickAsync(RookCardInformation thisCard)
        {
            int index = CardList.IndexOf(thisCard);
            if (index == 1 || index == 2)
                return;
            if (_gameContainer.SaveRoot!.DummyPlay && index == 0)
            {
                await DummyClickAsync();
                return;
            }
            if (index == 3 && _gameContainer.SaveRoot.DummyPlay == false)
            {
                await DummyClickAsync();
                return;
            }
            await _gameContainer.CardClickedAsync!.Invoke(); //hopefully this simple (?)
        }
        private async Task DummyClickAsync()
        {
            if (_gameContainer.PlayerList.Count() == 3)
                return; //because somebody else is playing it.
            await _gameContainer.CardClickedAsync!.Invoke(); //hopefully this simple (?)
        }


        private RookCardInformation GetWinningCard(int wins, bool isDummy)
        {
            return (from items in OrderList
                    where items.Player == wins && items.IsDummy == isDummy
                    select items).Single(); // i think
        }

        public async Task AnimateWinAsync(int wins, bool isDummy)
        {
            var thisCard = GetWinningCard(wins, isDummy);
            _gameContainer.SaveRoot!.DummyPlay = isDummy;
            WinCard = thisCard;
            await AnimateWinAsync(); // i think
        }

        public void ClearBoard()
        {
            DeckRegularDict<RookCardInformation> tempList = new DeckRegularDict<RookCardInformation>();
            int x;
            int self = _gameContainer.SelfPlayer;
            for (x = 1; x <= 4; x++)
            {
                RookCardInformation thisCard = new RookCardInformation();
                thisCard.Populate(x);
                thisCard.Deck += 1000; //this was the workaround.
                thisCard.IsUnknown = true;

                if (x <= 2)
                {
                    thisCard.Visible = true;
                }
                else if (x == 3 && self == _gameContainer.SaveRoot!.WonSoFar)
                {
                    thisCard.Visible = true;
                }
                else if (x == 4 && self != _gameContainer.SaveRoot!.WonSoFar)
                {
                    thisCard.Visible = true;
                }
                else
                {
                    thisCard.Visible = false;
                }
                tempList.Add(thisCard); //hopefully this simple.
            }
            OrderList.Clear();
            CardList.ReplaceRange(tempList); // hopefully its that simple.
            Visible = true; // now it is visible.
        }
        public void NewRound()
        {
            int self = _gameContainer.SelfPlayer;
            if (self == _gameContainer.SaveRoot!.WonSoFar)
            {
                ViewList![2].Visible = true;
                ViewList[3].Visible = false;
            }
            else
            {
                ViewList![2].Visible = false;
                ViewList[3].Visible = true;
            }
            ClearBoard();
        }

        public void LoadGame()
        {
            ViewList = GetCoordinateList();
            if (_gameContainer.SaveRoot!.TrickList.Count == 0)
            {
                NewRound();
                return;
            }

            var tempList = OrderList.ToRegularDeckDict();
            ClearBoard();
            if (tempList.Count == 0)
                throw new BasicBlankException("Rethinking may be required.");
            int index;
            int tempTurn;
            RookCardInformation lastCard;
            tempTurn = _gameContainer.WhoTurn;
            DeckRegularDict<RookCardInformation> otherList = new DeckRegularDict<RookCardInformation>();
            bool tempDummyPlay = _gameContainer.SaveRoot.DummyPlay;
            tempList.ForEach(thisCard =>
            {
                if (thisCard.Player == 0)
                    throw new BasicBlankException("The Player Cannot Be 0");
                _gameContainer.WhoTurn = thisCard.Player;
                _gameContainer.SingleInfo = _gameContainer.PlayerList!.GetWhoPlayer();
                _gameContainer.SaveRoot.DummyPlay = thisCard.IsDummy;
                index = GetCardIndex();
                lastCard = _gameContainer.GetBrandNewCard(thisCard.Deck);
                lastCard.Player = thisCard.Player;
                lastCard.IsDummy = thisCard.IsDummy;
                TradeCard(index, lastCard);
                otherList.Add(lastCard); //i think
            });
            OrderList.ReplaceRange(otherList); //i think we have to do it this way this tiem.
            _gameContainer.WhoTurn = tempTurn;
            _gameContainer.SaveRoot.DummyPlay = tempDummyPlay;
        }

        public Task AnimateWinAsync(int wins)
        {
            throw new BasicBlankException("This time, needs to use the one with dummy player.");
        }

    }
}