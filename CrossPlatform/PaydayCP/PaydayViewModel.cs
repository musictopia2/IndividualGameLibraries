using BasicGameFramework.BasicGameDataClasses;
using BasicGameFramework.ChooserClasses;
using BasicGameFramework.CommandClasses; //often times we will need commands.
using BasicGameFramework.DIContainers;
using BasicGameFramework.DrawableListsViewModels;
using BasicGameFramework.GameGraphicsCP.GamePieces;
using BasicGameFramework.MainViewModels;
using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.MVVMHelpers.Interfaces;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using static BasicGameFramework.ChooserClasses.ListViewPicker;
namespace PaydayCP
{
    public class PaydayViewModel : BoardDiceGameVM<EnumColorChoice, PawnPiecesCP<EnumColorChoice>,
        PaydayPlayerItem, PaydayMainGameClass, int>
    {
        private EnumStatus _GameStatus;
        public EnumStatus GameStatus
        {
            get { return _GameStatus; }
            set
            {
                if (SetProperty(ref _GameStatus, value))
                {
                    //can decide what to do when property changes
                }
            }
        }
        private string _MonthLabel = "";
        public string MonthLabel
        {
            get
            {
                return _MonthLabel;
            }
            set
            {
                if (SetProperty(ref _MonthLabel, value) == true)
                {
                }
            }
        }
        private string _OtherLabel = "";
        public string OtherLabel
        {
            get
            {
                return _OtherLabel;
            }
            set
            {
                if (SetProperty(ref _OtherLabel, value) == true)
                {
                }
            }
        }
        private string _PopUpChosen = "";
        public string PopUpChosen
        {
            get { return _PopUpChosen; }
            set
            {
                if (SetProperty(ref _PopUpChosen, value))
                {
                    //can decide what to do when property changes
                }

            }
        }
        public PaydayViewModel(ISimpleUI tempUI, IGamePackageResolver tempC, BasicData thisData) : base(tempUI, tempC, thisData) { }
        protected override bool CanEnableDice()
        {
            return false;
        }
        protected override bool CanRollDice()
        {
            if (MainGame!.DidChooseColors == false)
                return false;
            EnumStatus GameStatus = MainGame!.SaveRoot!.GameStatus;
            return GameStatus == EnumStatus.Starts || GameStatus == EnumStatus.RollCharity ||
                GameStatus == EnumStatus.RollLottery || GameStatus == EnumStatus.RollRadio;
        }
        public override bool CanEndTurn()
        {
            return false;
        }
        protected override void FinishCup()
        {
            ThisCup!.HowManyDice = 1; //most board games are only one dice.  can increase if necessary
            ThisCup.Visible = true;
        }
        public PileViewModel<DealCard>? DealPile;
        public PileViewModel<MailCard>? MailPile;
        public HandViewModel<DealCard>? CurrentDealList; //this has events.
        public HandViewModel<MailCard>? CurrentMailList;
        public ListViewPicker? PopUpList;
        internal void AddPopupLists(CustomBasicList<string> ThisList)
        {
            PopUpChosen = "";
            PopUpList!.LoadTextList(ThisList); //i think.
            PopUpList.UnselectAll(); //just in case.
            PopUpList.Visible = true;
            if (ThisList.Count == 1)
            {
                PopUpChosen = ThisList.Single();// obviously should show it selected because no choice.
                PopUpList.SelectSpecificItem(1); //because there is only one.
            }
        }
        public BasicGameCommand<int>? SpaceCommand { get; set; }
        public BasicGameCommand? SubmitPopUpChoiceCommand { get; set; }
        public BasicGameCommand? DealCommand { get; set; }
        public void SetDealCommand()
        {
            DealCommand = new BasicGameCommand(this, async items =>
            {
                if (CurrentDealList!.ObjectSelected() == 0)
                    return;
                await MainGame!.BuyerSelectedAsync(CurrentDealList.ObjectSelected());
            }, items =>
            {
                return MainGame!.SaveRoot!.GameStatus == EnumStatus.ChooseBuy;
            }, this, CommandContainer!);
        }
        internal void HookUpEvents()
        {
            CurrentDealList!.ObjectClickedAsync += CurrentDealList_ObjectClickedAsync;
        }
        protected override void EndInit()
        {
            base.EndInit(); //must do this too.
            PopUpList = new ListViewPicker(this);
            PopUpList.IndexMethod = EnumIndexMethod.OneBased;
            PopUpList.ItemSelectedAsync += PopUpList_ItemSelectedAsync;
            PopUpList.SendEnableProcesses(this, () =>
            {
                return MainGame!.SaveRoot!.GameStatus == EnumStatus.DealOrBuy ||
                    MainGame.SaveRoot.GameStatus == EnumStatus.ChoosePlayer ||
                    MainGame.SaveRoot.GameStatus == EnumStatus.ChooseDeal ||
                    MainGame.SaveRoot.GameStatus == EnumStatus.ChooseLottery;
            });
            SpaceCommand = new BasicGameCommand<int>(this, async items =>
            {
                await MainGame!.GameBoard1!.AnimateMoveAsync(items);
            }, items =>
            {
                if (MainGame!.DidChooseColors == false)
                    return false;
                return MainGame.SaveRoot!.GameStatus == EnumStatus.MakeMove;
            }, this, CommandContainer!);
            SubmitPopUpChoiceCommand = new BasicGameCommand(this, async items =>
            {
                await MainGame!.ResumeAfterPopUpAsync();
            }, items =>
            {
                return PopUpChosen != ""; //i think
            }, this, CommandContainer!);
            if (ThisData!.IsXamarinForms)
                SetDealCommand();
        }
        private async Task CurrentDealList_ObjectClickedAsync(DealCard thisObject, int index)
        {
            await MainGame!.BuyerSelectedAsync(thisObject.Deck);
        }
        private Task PopUpList_ItemSelectedAsync(int selectedIndex, string selectedText)
        {
            PopUpChosen = selectedText;
            return Task.CompletedTask;
        }
    }
}