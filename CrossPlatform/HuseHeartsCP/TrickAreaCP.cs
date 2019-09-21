using BasicGameFramework.Attributes;
using BasicGameFramework.BasicDrawables.Dictionary;
using BasicGameFramework.Extensions;
using BasicGameFramework.MultiplayerClasses.BasicPlayerClasses;
using BasicGameFramework.RegularDeckOfCards;
using BasicGameFramework.SpecializedGameTypes.TrickClasses;
using BasicGameFramework.ViewModelInterfaces;
using CommonBasicStandardLibraries.Exceptions;
using System.Linq;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;
namespace HuseHeartsCP
{
    [SingletonGame]
    public class TrickAreaCP : PossibleDummyTrickViewModel<EnumSuitList, HuseHeartsCardInformation,
        HuseHeartsPlayerItem, HuseHeartsSaveInfo>, ITrickPlay, IAdvancedTrickProcesses
    {
        public TrickAreaCP(IBasicGameVM thisMod) : base(thisMod) { }
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
            int self = MainGame.SelfPlayer;
            HuseHeartsSaveInfo saveRoot = Resolve<HuseHeartsSaveInfo>(); //since you can't unit test now anyways because of config.
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
            tempTurn = MainGame.WhoTurn;
            DeckRegularDict<HuseHeartsCardInformation> otherList = new DeckRegularDict<HuseHeartsCardInformation>();
            tempList.ForEach(thisCard =>
            {
                if (thisCard.Player == 0)
                    throw new BasicBlankException("The Player Cannot Be 0");
                MainGame.WhoTurn = thisCard.Player;
                MainGame.SingleInfo = MainGame.PlayerList!.GetWhoPlayer();
                index = GetCardIndex();
                lastCard = MainGame.GetBrandNewCard(thisCard.Deck);
                lastCard.Player = thisCard.Player;
                TradeCard(index, lastCard);
                otherList.Add(lastCard); //i think
            });
            OrderList.ReplaceRange(otherList); //i think we have to do it this way this tiem.
            MainGame.WhoTurn = tempTurn;
        }
        protected override int GetCardIndex()
        {
            if (MainGame.SingleInfo!.PlayerCategory == EnumPlayerCategory.Self)
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
                await MainGame.CardClickedAsync(); //hopefully this simple (?)
        }
    }
}