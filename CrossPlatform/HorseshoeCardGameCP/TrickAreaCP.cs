using BasicGameFramework.Attributes;
using BasicGameFramework.BasicDrawables.Dictionary;
using BasicGameFramework.Extensions;
using BasicGameFramework.RegularDeckOfCards;
using BasicGameFramework.SpecializedGameTypes.TrickClasses;
using BasicGameFramework.ViewModelInterfaces;
using CommonBasicStandardLibraries.Exceptions;
using System.Linq;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace HorseshoeCardGameCP
{
    [SingletonGame]
    public class TrickAreaCP : PossibleDummyTrickViewModel<EnumSuitList, HorseshoeCardGameCardInformation,
        HorseshoeCardGamePlayerItem, HorseshoeCardGameSaveInfo>, ITrickPlay, IAdvancedTrickProcesses
    {
        readonly HorseshoeCardGameMainGameClass _mainGame;
        public TrickAreaCP(IBasicGameVM thisMod) : base(thisMod)
        {
            _mainGame = thisMod.MainContainer!.Resolve<HorseshoeCardGameMainGameClass>();
        }
        protected override bool UseDummy { get; set; } = true;
        protected override int GetCardIndex()
        {
            var thisSingle = ViewList.Where(items => items.Player == _mainGame.WhoTurn && items.PossibleDummy == _mainGame.SaveRoot!.FirstCardPlayed).Single();
            return ViewList!.IndexOf(thisSingle);
        }
        protected override void PopulateNewCard(HorseshoeCardGameCardInformation oldCard, ref HorseshoeCardGameCardInformation newCard) { }
        protected override void PopulateOldCard(HorseshoeCardGameCardInformation oldCard) { }
        protected override string FirstHumanText()
        {
            return "Your First Card:";
        }
        protected override string DummyHumanText()
        {
            return "Your Last Card:";
        }
        protected override string FirstOpponentText()
        {
            return "Opponent First Card:";
        }
        protected override string DummyOpponentText()
        {
            return "Opponent Last Card:";
        }
        protected override async Task ProcessCardClickAsync(HorseshoeCardGameCardInformation thisCard)
        {
            int index = CardList.IndexOf(thisCard);
            if (index == 0 && _mainGame.SaveRoot!.FirstCardPlayed == false)
            {
                await _mainGame.CardClickedAsync();
                return;
            }
            if (index == 3 && _mainGame.SaveRoot!.FirstCardPlayed == true)
            {
                await _mainGame.CardClickedAsync();
                return;
            }
        }
        public async Task AnimateWinAsync(int wins)
        {
            WinCard = GetWinningCard(wins);
            await AnimateWinAsync();
        }
        private HorseshoeCardGameCardInformation GetWinningCard(int wins)
        {
            var tempList = OrderList.ToRegularDeckDict();
            return tempList.First(Items => Items.Player == wins);
        }
        protected override int GetMaxCount()
        {
            return 4;
        }
        internal bool DidPlay2Cards => _mainGame.SaveRoot!.TrickList.Count == 2;
        public void ClearBoard()
        {
            DeckRegularDict<HorseshoeCardGameCardInformation> tempList = new DeckRegularDict<HorseshoeCardGameCardInformation>();
            for (int x = 1; x <= 4; x++)
            {
                HorseshoeCardGameCardInformation thisCard = new HorseshoeCardGameCardInformation();
                thisCard.Populate(x);
                thisCard.Deck += 1000; //this was the workaround.
                thisCard.IsUnknown = true;
                thisCard.Visible = true;
                tempList.Add(thisCard); //hopefully this simple.
            }
            OrderList.Clear();
            CardList.ReplaceRange(tempList); // hopefully its that simple.
            Visible = true; // now it is visible.
        }
        public void LoadGame()
        {
            var tempList = OrderList.ToRegularDeckDict();
            ClearBoard();
            if (tempList.Count == 0)
                return;
            int index;
            int tempTurn;
            bool previousFirst;
            previousFirst = _mainGame.SaveRoot!.FirstCardPlayed;
            HorseshoeCardGameCardInformation lastCard;
            tempTurn = _mainGame.WhoTurn;
            DeckRegularDict<HorseshoeCardGameCardInformation> otherList = new DeckRegularDict<HorseshoeCardGameCardInformation>();
            int x = 0;
            tempList.ForEach(thisCard =>
            {
                if (thisCard.Player == 0)
                    throw new BasicBlankException("The Player Cannot Be 0");
                _mainGame.WhoTurn = thisCard.Player;
                _mainGame.SingleInfo = _mainGame.PlayerList!.GetWhoPlayer();
                x++;
                if (x == 1 || x == 2)
                    _mainGame.SaveRoot.FirstCardPlayed = false;
                else
                    _mainGame.SaveRoot.FirstCardPlayed = true;
                index = GetCardIndex();
                lastCard = _mainGame.GetBrandNewCard(thisCard.Deck);
                lastCard.Player = thisCard.Player;
                TradeCard(index, lastCard);
                otherList.Add(lastCard); //i think
            });
            OrderList.ReplaceRange(otherList); //i think we have to do it this way this time.
            _mainGame.SaveRoot.FirstCardPlayed = previousFirst;
            _mainGame.WhoTurn = tempTurn;
        }
    }
}