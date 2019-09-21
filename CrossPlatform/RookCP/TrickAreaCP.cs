using BasicGameFramework.Attributes;
using BasicGameFramework.BasicDrawables.Dictionary;
using BasicGameFramework.ColorCards;
using BasicGameFramework.Extensions;
using BasicGameFramework.SpecializedGameTypes.TrickClasses;
using BasicGameFramework.ViewModelInterfaces;
using CommonBasicStandardLibraries.Exceptions;
using System.Linq;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace RookCP
{
    [SingletonGame]
    public class TrickAreaCP : PossibleDummyTrickViewModel<EnumColorTypes, RookCardInformation, RookPlayerItem, RookSaveInfo>, IAdvancedTrickProcesses
    {
        private readonly RookMainGameClass _mainGame;
        public TrickAreaCP(IBasicGameVM thisMod) : base(thisMod)
        {
            _mainGame = thisMod.MainContainer!.Resolve<RookMainGameClass>();
            if (_mainGame.PlayerList.Count() == 4)
                UseDummy = false;
            else
                UseDummy = true;
        }

        protected override bool UseDummy { get; set; }

        protected override int GetCardIndex()
        {
            var thisView = ViewList.Single(items => items.Player == _mainGame.WhoTurn && items.PossibleDummy == _mainGame.SaveRoot!.DummyPlay);
            return ViewList!.IndexOf(thisView); //hopefully this simple.
        }

        protected override void PopulateNewCard(RookCardInformation oldCard, ref RookCardInformation newCard)
        {
            newCard.IsDummy = oldCard.IsDummy;
        }

        protected override void PopulateOldCard(RookCardInformation oldCard)
        {
            oldCard.IsDummy = _mainGame.SaveRoot!.DummyPlay;
        }

        protected override async Task ProcessCardClickAsync(RookCardInformation thisCard)
        {
            int index = CardList.IndexOf(thisCard);
            if (index == 1 || index == 2)
                return;
            if (_mainGame.SaveRoot!.DummyPlay && index == 0)
            {
                await DummyClickAsync();
                return;
            }
            if (index == 3 && _mainGame.SaveRoot.DummyPlay == false)
            {
                await DummyClickAsync();
                return;
            }
            await MainGame.CardClickedAsync(); //hopefully this simple (?)
        }
        private async Task DummyClickAsync()
        {
            if (_mainGame.PlayerList.Count() == 3)
                return; //because somebody else is playing it.
            await MainGame.CardClickedAsync(); //hopefully this simple (?)
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
            _mainGame.SaveRoot!.DummyPlay = isDummy;
            WinCard = thisCard;
            await AnimateWinAsync(); // i think
        }

        public void ClearBoard()
        {
            DeckRegularDict<RookCardInformation> tempList = new DeckRegularDict<RookCardInformation>();
            int x;
            int self = MainGame.SelfPlayer;
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
                else if (x == 3 && self == _mainGame.SaveRoot!.WonSoFar)
                {
                    thisCard.Visible = true;
                }
                else if (x == 4 && self != _mainGame.SaveRoot!.WonSoFar)
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
            int self = MainGame.SelfPlayer;
            if (self == _mainGame.SaveRoot!.WonSoFar)
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
            if (_mainGame.SaveRoot!.TrickList.Count == 0)
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
            tempTurn = MainGame.WhoTurn;
            DeckRegularDict<RookCardInformation> otherList = new DeckRegularDict<RookCardInformation>();
            bool tempDummyPlay = _mainGame.SaveRoot.DummyPlay;
            tempList.ForEach(thisCard =>
            {
                if (thisCard.Player == 0)
                    throw new BasicBlankException("The Player Cannot Be 0");
                MainGame.WhoTurn = thisCard.Player;
                MainGame.SingleInfo = MainGame.PlayerList!.GetWhoPlayer();
                _mainGame.SaveRoot.DummyPlay = thisCard.IsDummy;
                index = GetCardIndex();
                lastCard = MainGame.GetBrandNewCard(thisCard.Deck);
                lastCard.Player = thisCard.Player;
                lastCard.IsDummy = thisCard.IsDummy;
                TradeCard(index, lastCard);
                otherList.Add(lastCard); //i think
            });
            OrderList.ReplaceRange(otherList); //i think we have to do it this way this tiem.
            MainGame.WhoTurn = tempTurn;
            _mainGame.SaveRoot.DummyPlay = tempDummyPlay;
        }

        public Task AnimateWinAsync(int wins)
        {
            throw new BasicBlankException("This time, needs to use the one with dummy player.");
        }
    }
}