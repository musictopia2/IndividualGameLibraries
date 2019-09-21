using BasicGameFramework.BasicDrawables.Dictionary;
using BasicGameFramework.BasicGameDataClasses;
using BasicGameFramework.CommandClasses; //often times we will need commands.
using BasicGameFramework.DIContainers;
using BasicGameFramework.DrawableListsViewModels;
using BasicGameFramework.Extensions;
using BasicGameFramework.MainViewModels;
using BasicGameFramework.RegularDeckOfCards;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.MVVMHelpers.Interfaces;
using System.Linq;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace BladesOfSteelCP
{
    public class BladesOfSteelViewModel : BasicCardGamesVM<RegularSimpleCard, BladesOfSteelPlayerItem, BladesOfSteelMainGameClass>
    {
        private string _OtherPlayer = "";
        public string OtherPlayer
        {
            get { return _OtherPlayer; }
            set
            {
                if (SetProperty(ref _OtherPlayer, value))
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
        private bool _IsFaceOff;
        public bool IsFaceOff
        {
            get { return _IsFaceOff; }
            set
            {
                if (SetProperty(ref _IsFaceOff, value))
                {
                    //can decide what to do when property changes
                    YourFaceOffCard!.Visible = value;
                    OpponentFaceOffCard!.Visible = value;
                    OnPropertyChanged(nameof(CommandsVisible));
                }
            }
        }
        public bool CommandsVisible => !IsFaceOff;
        public BladesOfSteelViewModel(ISimpleUI tempUI, IGamePackageResolver tempC, BasicData thisData) : base(tempUI, tempC, thisData) { }
        protected override bool CanEnableDeck()
        {
            return IsFaceOff;
        }
        protected override bool CanEnablePile1()
        {
            if (IsFaceOff == true)
                return false;
            return !MainDefense1!.HasCards;
        }
        public override bool CanEnableAlways()
        {
            return true;
        }
        protected override async Task ProcessDiscardClickedAsync()
        {
            if (MainGame!.SingleInfo!.MainHandList.Count == 0 && MainGame.SingleInfo.DefenseList.Count == 0)
                throw new BasicBlankException("There are no cards from hand or defense list.  Therfore; should have disabled the pile");
            if (MainGame.SingleInfo.DefensePile!.HandList.HasSelectedObject() == true)
            {
                if (MainGame.SingleInfo.DefenseList.All(items => items.IsSelected == false))
                {
                    await ShowGameMessageAsync("If you choose one card from defense, you must choose all cards");
                    return;
                }
                if (ThisData!.MultiPlayer == true)
                    await ThisNet!.SendAllAsync("throwawaydefense");
                await MainGame!.ThrowAwayDefenseCardsAsync();
                return;
            }
            if (ThisData!.MultiPlayer == true)
                await ThisNet!.SendAllAsync("throwawayallcardsfromhand");
            await MainGame!.ThrowAwayAllCardsFromHandAsync();
        }
        public BasicGameCommand? PassCommand { get; set; }
        public MainDefenseCP? MainDefense1;
        public PlayerAttackCP? YourAttackPile;
        public PlayerDefenseCP? YourDefensePile;
        public PlayerAttackCP? OpponentAttackPile;
        public PlayerDefenseCP? OpponentDefensePile;
        public PileViewModel<RegularSimpleCard>? YourFaceOffCard;
        public PileViewModel<RegularSimpleCard>? OpponentFaceOffCard;
        public override bool CanEndTurn()
        {
            return MainDefense1!.HasCards;
        }
        protected override void EndInit()
        {
            base.EndInit(); //must do this too.
            Deck1!.NeverAutoDisable = false;
            PlayerHand1!.Maximum = 6;
            PlayerHand1.AutoSelect = HandViewModel<RegularSimpleCard>.EnumAutoType.SelectAsMany;
            MainDefense1 = new MainDefenseCP(this);
            MainDefense1.SendEnableProcesses(this, () =>
            {
                if (IsFaceOff == true)
                    return false;
                return MainGame!.SaveRoot!.PlayOrder.OtherTurn > 0;
            });
            PassCommand = new BasicGameCommand(this, async items =>
            {
                if (ThisData!.MultiPlayer == true)
                    await ThisNet!.SendAllAsync("passdefense");
                await MainGame!.PassDefenseAsync();
            }, items => MainGame!.SaveRoot!.PlayOrder.OtherTurn > 0, this, CommandContainer!);
            MainDefense1.BoardClickedAsync += MainDefense1_BoardClickedAsync;
            YourFaceOffCard = new PileViewModel<RegularSimpleCard>(ThisE!, this);
            YourFaceOffCard.IsEnabled = false;
            YourFaceOffCard.Text = "Your";
            OpponentFaceOffCard = new PileViewModel<RegularSimpleCard>(ThisE!, this);
            OpponentFaceOffCard.IsEnabled = false;
            OpponentFaceOffCard.Text = "Opponent";
        }
        internal void LoadAttackCommands()
        {
            YourAttackPile!.BoardClickedAsync -= YourAttackPile_BoardClickedAsync;
            YourDefensePile!.BoardClickedAsync -= YourDefensePile_BoardClickedAsync;
            YourDefensePile.ObjectClickedAsync -= YourDefensePile_ObjectClickedAsync;
            YourAttackPile.SendEnableProcesses(this, () => CanEnablePiles(false));
            YourDefensePile.SendEnableProcesses(this, () => CanEnablePiles(true));
            YourAttackPile.BoardClickedAsync += YourAttackPile_BoardClickedAsync;
            YourDefensePile.BoardClickedAsync += YourDefensePile_BoardClickedAsync;
            YourDefensePile.ObjectClickedAsync += YourDefensePile_ObjectClickedAsync;
        }
        private async Task MainDefense1_BoardClickedAsync()
        {
            if (MainGame!.OtherTurn == 0)
                throw new BasicBlankException("Should not have allowed the click for main defense because not the other turn to defend themselves");
            if (MainGame!.SingleInfo!.MainHandList.HasSelectedObject() == false && MainGame.SingleInfo.DefenseList.HasSelectedObject() == false)
            {
                await ShowGameMessageAsync("Must choose at least a card from defense or at least a card from hand");
                return;
            }
            if (MainGame.SingleInfo.MainHandList.HasSelectedObject() && MainGame.SingleInfo.DefenseList.HasSelectedObject())
            {
                await ShowGameMessageAsync("Cannot choose cards from both from hand and from defense piles");
                return;
            }
            bool fromHand;
            DeckRegularDict<RegularSimpleCard> tempDefenseList;
            if (MainGame.SingleInfo.MainHandList.HasSelectedObject())
            {
                fromHand = true;
                tempDefenseList = MainGame.SingleInfo.MainHandList.GetSelectedItems();
            }
            else
            {
                fromHand = false;
                tempDefenseList = MainGame.SingleInfo.DefenseList.GetSelectedItems();
                if (tempDefenseList.Any(items => items.Color == EnumColorList.Red))
                    throw new BasicBlankException("No red attack cards should have even been put to defense pile");
            }
            if (await MainGame.CanPlayDefenseCardsAsync(tempDefenseList) == false)
                return;
            if (MainDefense1!.CanAddDefenseCards(tempDefenseList) == false)
            {
                await ShowGameMessageAsync("This defense is not enough to prevent a goal from the attacker");
                return;
            }
            if (ThisData!.MultiPlayer == true)
            {
                var deckList = tempDefenseList.GetDeckListFromObjectList();
                if (fromHand == true)
                    await ThisNet!.SendAllAsync("defensehand", deckList);
                else
                    await ThisNet!.SendAllAsync("defenseboard", deckList);
            }
            tempDefenseList.UnselectAllObjects();
            await MainGame.PlayDefenseCardsAsync(tempDefenseList, fromHand);
        }
        private Task YourDefensePile_ObjectClickedAsync(RegularSimpleCard thisObject, int index)
        {
            if (MainGame!.SingleInfo!.MainHandList.HasSelectedObject() == true)
                return Task.CompletedTask;
            thisObject.IsSelected = !thisObject.IsSelected;
            return Task.CompletedTask;
        }
        private async Task YourDefensePile_BoardClickedAsync()
        {
            if (MainGame!.OtherTurn > 0)
                return;
            if (MainGame!.SingleInfo!.MainHandList.HasSelectedObject() == false)
                return;
            if (MainGame.SingleInfo.DefenseList.HasSelectedObject() == true)
            {
                await ShowGameMessageAsync("Cannot choose any defense cards on the board to add more defense cards");
                return;
            }
            var tempDefenseList = MainGame.SingleInfo.MainHandList.GetSelectedItems();
            if (await MainGame.CanPlayDefenseCardsAsync(tempDefenseList) == false)
            {
                return;
            }
            if (ThisData!.MultiPlayer == true)
                await ThisNet!.SendAllAsync("addtodefense", tempDefenseList.GetDeckListFromObjectList());
            tempDefenseList.UnselectAllObjects();
            await MainGame.AddCardsToDefensePileAsync(tempDefenseList); //hopefully this is correct.
        }
        private async Task YourAttackPile_BoardClickedAsync()
        {
            var tempAttackList = MainGame!.SingleInfo!.MainHandList.GetSelectedItems();
            if (await MainGame.CanPlayAttackCardsAsync(tempAttackList) == false)
                return;
            tempAttackList.UnselectAllObjects();
            if (ThisData!.MultiPlayer == true)
                await ThisNet!.SendAllAsync("attack", tempAttackList.GetDeckListFromObjectList());
            await MainGame.PlayAttackCardsAsync(tempAttackList);
        }
        private bool CanEnablePiles(bool isDefense)
        {
            if (IsFaceOff == true)
                return false;
            if (MainGame!.SaveRoot!.PlayOrder.OtherTurn > 0)
            {
                return isDefense;
            }
            return !MainDefense1!.HasCards;
        }
    }
}