using BasicGameFramework.BasicDrawables.Dictionary;
using BasicGameFramework.BasicGameDataClasses;
using BasicGameFramework.CommandClasses; //often times we will need commands.
using BasicGameFramework.DIContainers;
using BasicGameFramework.DrawableListsViewModels;
using BasicGameFramework.MainViewModels;
using BasicGameFramework.RegularDeckOfCards;
using CommonBasicStandardLibraries.MVVMHelpers.Interfaces;
using System.Linq;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace GolfCardGameCP
{
    public class GolfCardGameViewModel : BasicCardGamesVM<RegularSimpleCard, GolfCardGamePlayerItem, GolfCardGameMainGameClass>
    {
        private int _Round;

        public int Round
        {
            get { return _Round; }
            set
            {
                if (SetProperty(ref _Round, value))
                {
                    //can decide what to do when property changes
                }

            }
        }
        private string _Instructions = "";

        public string Instructions
        {
            get { return _Instructions; }
            set
            {
                if (SetProperty(ref _Instructions, value))
                {
                    //can decide what to do when property changes
                }

            }
        }
        private bool _ChooseFirstCardsVisible;

        public bool ChooseFirstCardsVisible
        {
            get { return _ChooseFirstCardsVisible; }
            set
            {
                if (SetProperty(ref _ChooseFirstCardsVisible, value))
                {
                    //can decide what to do when property changes
                }

            }
        }
        private bool _KnockedVisible;

        public bool KnockedVisible
        {
            get { return _KnockedVisible; }
            set
            {
                if (SetProperty(ref _KnockedVisible, value))
                {
                    //can decide what to do when property changes
                }

            }
        }
        public PileViewModel<RegularSimpleCard>? Pile2;
        public GolfCardGameViewModel(ISimpleUI tempUI, IGamePackageResolver tempC, BasicData thisData) : base(tempUI, tempC, thisData) { }
        protected override bool CanEnableDeck()
        {
            return !MainGame!.AlreadyDrew;
        }

        protected override bool CanEnablePile1()
        {
            return !MainGame!.AlreadyDrew;
        }

        protected override async Task ProcessDiscardClickedAsync()
        {
            await MainGame!.PickupFromDiscardAsync();
        }
        public override bool CanEnableAlways()
        {
            return true;
        }
        public BasicGameCommand? ChooseFirstCardsCommand { get; set; }
        public BasicGameCommand? KnockedCommand { get; set; }
        public HiddenCards? HiddenCards1;
        public Beginnings? Beginnings1;
        public GolfHand? GolfHand1;
        protected override void EndInit()
        {
            base.EndInit(); //must do this too.
            Deck1!.NeverAutoDisable = true; //if you want reshuffling, use this.  otherwise, comment or delete.
            GolfHand1 = new GolfHand(this);
            HiddenCards1 = new HiddenCards(this);
            Beginnings1 = new Beginnings(this);
            Pile2 = new PileViewModel<RegularSimpleCard>(ThisE!, this);
            Pile2.PileClickedAsync += Pile2_PileClickedAsync;
            GolfHand1.SendEnableProcesses(this, () => MainGame!.AlreadyDrew);
            HiddenCards1.SendEnableProcesses(this, () => MainGame!.AlreadyDrew);
            Pile2.SendEnableProcesses(this, () => MainGame!.AlreadyDrew);
            Pile2.CurrentOnly = true;
            MainGame!.OtherPile = Pile2;
            Pile2.Text = "Current";
            ChooseFirstCardsCommand = new BasicGameCommand(this, async x =>
            {
                if (Beginnings1.CanContinue == false)
                {
                    await ShowGameMessageAsync("Sorry, must select 2 and only 2 cards to put into your hand");
                    return;
                }
                Beginnings1.GetSelectInfo(out DeckRegularDict<RegularSimpleCard> selectList, out DeckRegularDict<RegularSimpleCard> unselectList);
                if (ThisData!.MultiPlayer == true)
                {
                    SendBeginning thisB = new SendBeginning();
                    thisB.SelectList = selectList;
                    thisB.UnsSelectList = unselectList;
                    thisB.Player = MainGame.PlayerList.Where(items => items.PlayerCategory == BasicGameFramework.MultiplayerClasses.BasicPlayerClasses.EnumPlayerCategory.Self).Single().Id;
                    await ThisNet!.SendAllAsync("selectbeginning", thisB);
                }

                int Player = MainGame.PlayerList.Single(items => items.PlayerCategory == BasicGameFramework.MultiplayerClasses.BasicPlayerClasses.EnumPlayerCategory.Self).Id;
                await MainGame.SelectBeginningAsync(Player, selectList, unselectList);
            }, items => ChooseFirstCardsVisible, this, CommandContainer!);
            KnockedCommand = new BasicGameCommand(this, async items =>
            {
                if (MainGame.AlreadyDrew == true)
                    return;
                if (ThisData!.MultiPlayer == true)
                    await ThisNet!.SendAllAsync("knock");
                await MainGame.KnockAsync();
            }, x =>
            {
                if (KnockedVisible == false)
                    return false;
                return !MainGame.PlayerList.Any(items => items.Knocked == true);
            }, this, CommandContainer!);
        }
        private async Task Pile2_PileClickedAsync()
        {
            var tempCard = Pile2!.GetCardInfo();
            if (MainGame!.PreviousCard == tempCard.Deck)
            {
                await ShowGameMessageAsync("Sorry; you cannot throwaway the same card you picked up from the discard pile");
                return;
            }
            await MainGame.SendDiscardMessageAsync(tempCard.Deck);
            await MainGame.DiscardAsync(tempCard.Deck);
        }
    }
}