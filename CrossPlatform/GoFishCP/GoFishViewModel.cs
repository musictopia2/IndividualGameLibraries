using BasicGameFramework.BasicGameDataClasses;
using BasicGameFramework.ChooserClasses;
using BasicGameFramework.CommandClasses; //often times we will need commands.
using BasicGameFramework.DIContainers;
using BasicGameFramework.DrawableListsViewModels;
using BasicGameFramework.MainViewModels;
using BasicGameFramework.MultiplayerClasses.BasicPlayerClasses;
using BasicGameFramework.RegularDeckOfCards;
using CommonBasicStandardLibraries.MVVMHelpers.Interfaces;
using System.Linq;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace GoFishCP
{
    public class GoFishViewModel : BasicCardGamesVM<RegularSimpleCard, GoFishPlayerItem, GoFishMainGameClass>
    {
        private EnumCardValueList _CardYouAsked = EnumCardValueList.None;

        public EnumCardValueList CardYouAsked
        {
            get { return _CardYouAsked; }
            set
            {
                if (SetProperty(ref _CardYouAsked, value))
                {
                    //can decide what to do when property changes
                }

            }
        }
        public GoFishChooserCP? AskList;
        public GoFishViewModel(ISimpleUI tempUI, IGamePackageResolver tempC, BasicData thisData) : base(tempUI, tempC, thisData) { }
        protected override bool CanEnableDeck()
        {
            return false;
        }
        protected override bool CanEnablePile1()
        {
            return true;
        }
        protected override async Task ProcessDiscardClickedAsync()
        {
            var thisList = PlayerHand1!.ListSelectedObjects();
            if (thisList.Count != 2)
            {
                await ShowGameMessageAsync("Must select 2 cards to throw away");
                return;
            }
            if (MainGame!.IsValidMove(thisList) == false)
            {
                await ShowGameMessageAsync("Illegal Move");
                return;
            }
            if (ThisData!.MultiPlayer == true)
            {
                SendPair thisPair = new SendPair();
                thisPair.Card1 = thisList.First().Deck;
                thisPair.Card2 = thisList.Last().Deck;
                await ThisNet!.SendAllAsync("processplay", thisPair);
            }
            await MainGame.ProcessPlayAsync(thisList.First().Deck, thisList.Last().Deck);
        }
        public override bool CanEndTurn()
        {
            return MainGame!.SaveRoot!.RemovePairs == true || MainGame.SaveRoot.NumberAsked == true;
        }
        public override bool CanEnableAlways()
        {
            return true;
        }
        public void LoadAskList() //done.
        {
            AskList!.Visible = true; //i think.
            if (MainGame!.SingleInfo!.PlayerCategory == EnumPlayerCategory.Self)
                AskList.LoadFromHandCardValues(MainGame.SingleInfo);
            else
                AskList.LoadEntireList(); //i think
            CardYouAsked = EnumCardValueList.None;
            AskList.UnselectAll(); //just in case.
        }
        public BasicGameCommand? AskCommand { get; set; }
        protected override void EndInit()
        {
            base.EndInit(); //must do this too.
            AskList = new GoFishChooserCP(this);
            AskList.ItemClickedAsync += AskList_ItemClickedAsync;
            AskList.AutoSelectCategory = EnumAutoSelectCategory.AutoEvent; //i think.
            AskList.SendEnableProcesses(this, () =>
            {
                return MainGame!.SaveRoot!.RemovePairs == false && MainGame.SaveRoot.NumberAsked == false;
            });
            PlayerHand1!.AutoSelect = HandViewModel<RegularSimpleCard>.EnumAutoType.SelectAsMany;
            AskCommand = new BasicGameCommand(this, async Items =>
            {
                if (ThisData!.MultiPlayer == true)
                    await ThisNet!.SendAllAsync("numbertoask", CardYouAsked);
                await MainGame!.NumberToAskAsync(CardYouAsked);
            }, items =>
            {
                if (MainGame!.SaveRoot!.RemovePairs == true || MainGame.SaveRoot.NumberAsked == true)
                    return false;
                return CardYouAsked != EnumCardValueList.None;
            }, this, CommandContainer!);
        }
        private Task AskList_ItemClickedAsync(EnumCardValueList thisPiece)
        {
            CardYouAsked = thisPiece;
            AskList!.SelectSpecificItem(thisPiece); //try this.
            return Task.CompletedTask;
        }
    }
}