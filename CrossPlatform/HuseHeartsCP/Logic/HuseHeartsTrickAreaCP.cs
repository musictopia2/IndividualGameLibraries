using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.BasicDrawables.Dictionary;
using BasicGameFrameworkLibrary.Extensions;
using BasicGameFrameworkLibrary.MultiplayerClasses.BasicPlayerClasses;
using BasicGameFrameworkLibrary.RegularDeckOfCards;
using BasicGameFrameworkLibrary.SpecializedGameTypes.TrickClasses;
using CommonBasicStandardLibraries.Exceptions;
using HuseHeartsCP.Cards;
using HuseHeartsCP.Data;
using System.Linq;
using System.Threading.Tasks;

namespace HuseHeartsCP.Logic
{
    [SingletonGame]
    public class HuseHeartsTrickAreaCP : PossibleDummyTrickObservable<EnumSuitList, HuseHeartsCardInformation,
        HuseHeartsPlayerItem, HuseHeartsSaveInfo>, ITrickPlay, IAdvancedTrickProcesses
    {
        private readonly HuseHeartsGameContainer _gameContainer;

        public HuseHeartsTrickAreaCP(HuseHeartsGameContainer gameContainer) : base(gameContainer)
        {
            _gameContainer = gameContainer;
        }

        protected override bool UseDummy { get; set; } = true;
        public async Task AnimateWinAsync(int wins)
        {
            WinCard = GetWinningCard(wins);
            await base.AnimateWinAsync();
        }
        private HuseHeartsCardInformation GetWinningCard(int wins)
        {
            var TempList = OrderList.ToRegularDeckDict();
            TempList.RemoveLastItem();
            return TempList.Single(Items => Items.Player == wins);
        }
        public bool FromDummy => OrderList.Count == 2;
        public void ClearBoard()
        {
            DeckRegularDict<HuseHeartsCardInformation> tempList = new DeckRegularDict<HuseHeartsCardInformation>();
            int x;
            int self = _gameContainer.SelfPlayer;
            HuseHeartsSaveInfo saveRoot = _gameContainer.Resolver.Resolve<HuseHeartsSaveInfo>(); //since you can't unit test now anyways because of config.
            for (x = 1; x <= 4; x++)
            {
                HuseHeartsCardInformation thisCard = new HuseHeartsCardInformation();
                thisCard.Populate(x);
                thisCard.Deck += 1000; //this was the workaround.
                thisCard.IsUnknown = true;
                if (x <= 2)
                {
                    thisCard.Visible = true;
                    ViewList![x - 1].Visible = true;
                }
                else if (x == 3 && self == saveRoot.WhoLeadsTrick)
                {
                    thisCard.Visible = true;
                    ViewList![x - 1].Visible = true;
                }
                else if (x == 4 && self != saveRoot.WhoLeadsTrick)
                {
                    thisCard.Visible = true;
                    ViewList![x - 1].Visible = true;
                }
                else
                {
                    thisCard.Visible = false;
                    ViewList![x - 1].Visible = false;
                }
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
            HuseHeartsCardInformation lastCard;
            tempTurn = _gameContainer.WhoTurn;
            DeckRegularDict<HuseHeartsCardInformation> otherList = new DeckRegularDict<HuseHeartsCardInformation>();
            //not sure if we need a clear method (?)
            tempList.ForEach(thisCard =>
            {
                if (thisCard.Player == 0)
                    throw new BasicBlankException("The Player Cannot Be 0");
                _gameContainer.WhoTurn = thisCard.Player;
                _gameContainer.SingleInfo = _gameContainer.PlayerList!.GetWhoPlayer();
                index = GetCardIndex();
                lastCard = _gameContainer.GetBrandNewCard(thisCard.Deck);
                lastCard.Player = thisCard.Player;
                TradeCard(index, lastCard);
                otherList.Add(lastCard); //i think
            });
            OrderList.ReplaceRange(otherList); //i think we have to do it this way this tiem.
            _gameContainer.WhoTurn = tempTurn;
        }
        protected override int GetCardIndex()
        {
            if (_gameContainer.SingleInfo!.PlayerCategory == EnumPlayerCategory.Self)
            {
                if (OrderList.Count == 2)
                    return 3;
                return 0;
            }
            if (OrderList.Count == 2)
                return 2;
            return 1;
        }
        protected override void PopulateNewCard(HuseHeartsCardInformation oldCard, ref HuseHeartsCardInformation newCard) { }
        protected override void PopulateOldCard(HuseHeartsCardInformation oldCard) { }
        protected override async Task ProcessCardClickAsync(HuseHeartsCardInformation thisCard)
        {
            int tndex = CardList.IndexOf(thisCard);
            if (tndex == 0 || tndex == 3)
                await _gameContainer.CardClickedAsync!.Invoke(); //hopefully this simple (?)
        }

    }
}
