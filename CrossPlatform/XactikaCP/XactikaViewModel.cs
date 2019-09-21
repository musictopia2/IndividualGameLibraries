using BasicGameFramework.BasicGameDataClasses;
using BasicGameFramework.ChooserClasses;
using BasicGameFramework.CommandClasses; //often times we will need commands.
using BasicGameFramework.DIContainers;
using BasicGameFramework.MainViewModels;
using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.MVVMHelpers.Interfaces;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using static BasicGameFramework.ChooserClasses.ListViewPicker;
namespace XactikaCP
{
    public class XactikaViewModel : TrickGamesVM<EnumShapes, XactikaCardInformation, XactikaPlayerItem, XactikaMainGameClass>
    {
        public ChooseShapeViewModel? ShapeChoose1;
        private EnumShapes _ShapeChosen;

        public EnumShapes ShapeChosen
        {
            get { return _ShapeChosen; }
            set
            {
                if (SetProperty(ref _ShapeChosen, value))
                {
                    //can decide what to do when property changes
                }

            }
        }
        private string _GameModeText = "";

        public string GameModeText
        {
            get { return _GameModeText; }
            set
            {
                if (SetProperty(ref _GameModeText, value))
                {
                    //can decide what to do when property changes
                }

            }
        }
        private int _RoundNumber;

        public int RoundNumber
        {
            get { return _RoundNumber; }
            set
            {
                if (SetProperty(ref _RoundNumber, value))
                {
                    //can decide what to do when property changes
                }

            }
        }
        private EnumGameMode _ModeChosen;

        public EnumGameMode ModeChosen
        {
            get { return _ModeChosen; }
            set
            {
                if (SetProperty(ref _ModeChosen, value))
                {
                    //can decide what to do when property changes
                }

            }
        }
        public bool ModeVisible
        {
            get
            {
                return MainGame!.SaveRoot!.GameStatus == EnumStatusList.ChooseGameType;
            }
        }
        public bool BidVisible
        {
            get
            {
                return MainGame!.SaveRoot!.GameStatus == EnumStatusList.Bidding;
            }
        }
        public bool ShapeVisible => MainGame!.SaveRoot!.GameStatus == EnumStatusList.CallShape;
        private int _BidChosen = -1;

        public int BidChosen
        {
            get { return _BidChosen; }
            set
            {
                if (SetProperty(ref _BidChosen, value))
                {
                    //can decide what to do when property changes
                }

            }
        }
        public NumberPicker? Bid1;
        public BasicGameCommand? BidCommand { get; set; }

        public ListViewPicker? ModeChoose1;
        public BasicGameCommand? ModeCommand { get; set; }
        public BasicGameCommand? ChooseShapeNumberCommand { get; set; }
        public XactikaViewModel(ISimpleUI tempUI, IGamePackageResolver tempC, BasicData thisData) : base(tempUI, tempC, thisData) { }
        protected override bool CanEnableDeck()
        {
            return false; //otherwise, can't compile.
        }
        protected override bool CanEnablePile1()
        {
            return false; //otherwise, can't compile.
        }
        protected override async Task ProcessDiscardClickedAsync()
        {
            await Task.CompletedTask;
        }
        protected override void EndInit()
        {
            base.EndInit(); //must do this too.  most trick taking games does not reshuffle.
            ModeChoose1 = new ListViewPicker(this);
            ModeChoose1.ItemSelectedAsync += ModeChooser1_ItemSelectedAsync;
            ModeChoose1.SendEnableProcesses(this, () => MainGame!.SaveRoot!.GameStatus == EnumStatusList.ChooseGameType);
            Bid1 = new NumberPicker(this);
            Bid1.ChangedNumberValueAsync += Bid1_ChangedNumberValueAsync;
            Bid1.SendEnableProcesses(this, () => MainGame!.SaveRoot!.GameStatus == EnumStatusList.Bidding);
            PlayerHand1!.Maximum = 8;
            ShapeChoose1 = new ChooseShapeViewModel(this);
            ShapeChoose1.SendEnableProcesses(this, () => MainGame!.SaveRoot!.GameStatus == EnumStatusList.CallShape);
            ModeChoose1.IndexMethod = EnumIndexMethod.OneBased;
            CustomBasicList<string> tempList = new CustomBasicList<string> { "To Win", "To Lose", "Bid" };
            ModeChoose1.LoadTextList(tempList);
            ModeChoose1.Visible = false; //start with false.
            LoadCommands();
        }
        private void LoadCommands()
        {
            ChooseShapeNumberCommand = new BasicGameCommand(this, async items =>
            {
                if (ThisData!.MultiPlayer)
                    await ThisNet!.SendAllAsync("shapeused", ShapeChosen);
                await MainGame!.ShapeChosenAsync(ShapeChosen);
            }, items =>
            {
                if (MainGame!.SaveRoot!.GameStatus != EnumStatusList.CallShape)
                    return false;
                return ShapeChosen != EnumShapes.None;
            }, this, CommandContainer!);
            ModeCommand = new BasicGameCommand(this, async items =>
            {
                if (ThisData!.MultiPlayer)
                    await ThisNet!.SendAllAsync("gameoptionchosen", ModeChosen);
                await MainGame!.ProcessGameOptionChosenAsync(ModeChosen, true);
            }, items =>
            {
                if (MainGame!.SaveRoot!.GameStatus != EnumStatusList.ChooseGameType)
                    return false;
                return ModeChosen != EnumGameMode.None;
            }, this, CommandContainer!);
            BidCommand = new BasicGameCommand(this, async items =>
            {
                if (ThisData!.MultiPlayer)
                    await ThisNet!.SendAllAsync("bid", BidChosen);
                await MainGame!.ProcessBidAsync();
            }, items =>
            {
                if (MainGame!.SaveRoot!.GameStatus != EnumStatusList.Bidding)
                    return false;
                return BidChosen != -1;
            }, this, CommandContainer!);
        }
        public void VisibleChanges()
        {
            OnPropertyChanged(nameof(ShapeVisible));
            OnPropertyChanged(nameof(ModeVisible));
            if (ModeChoose1 != null)
                ModeChoose1.Visible = ModeVisible;
            OnPropertyChanged(nameof(BidVisible));
            if (Bid1 != null)
                Bid1.Visible = BidVisible;
            MainOptionsVisible = MainGame!.SaveRoot!.GameStatus != EnumStatusList.ChooseGameType;
            MainGame.TrickArea1!.Visible = MainGame.SaveRoot.GameStatus != EnumStatusList.Bidding;
            if (BidVisible == true)
                ShapeChoose1!.Visible = false;
            else
                ShapeChoose1!.Visible = true; //for now.
        }
        private Task Bid1_ChangedNumberValueAsync(int chosen)
        {
            BidChosen = chosen;
            return Task.CompletedTask;
        }
        private Task ModeChooser1_ItemSelectedAsync(int selectedIndex, string selectedText)
        {
            MainGame!.SaveRoot!.GameMode = (EnumGameMode)selectedIndex;
            return Task.CompletedTask;
        }
        public override bool CanEnableAlways()
        {
            return true;
        }
    }
}