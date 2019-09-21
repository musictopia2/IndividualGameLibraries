using BasicGameFramework.BasicGameDataClasses;
using BasicGameFramework.ChooserClasses;
using BasicGameFramework.ColorCards;
using BasicGameFramework.CommandClasses; //often times we will need commands.
using BasicGameFramework.DIContainers;
using BasicGameFramework.GameGraphicsCP.GamePieces;
using BasicGameFramework.MainViewModels;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.Misc;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.MVVMHelpers.Interfaces;
using System.Linq;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace UnoCP
{
    public class UnoViewModel : BasicCardGamesVM<UnoCardInformation, UnoPlayerItem, UnoMainGameClass>
    {
        public UnoViewModel(ISimpleUI tempUI, IGamePackageResolver tempC, BasicData thisData) : base(tempUI, tempC, thisData) { }
        protected override bool CanEnableDeck()
        {
            if (MainGame!.SaveRoot!.GameStatus != EnumGameStatus.NormalPlay)
                return false;
            if (MainGame.AlreadyDrew == true)
                return false;
            return CanDraw();
        }

        protected override bool CanEnablePile1()
        {
            return MainGame!.SaveRoot!.GameStatus == EnumGameStatus.NormalPlay;
        }

        protected override async Task ProcessDiscardClickedAsync()
        {
            int deck = PlayerHand1!.ObjectSelected();
            if (deck == 0)
            {
                await ShowGameMessageAsync("You must select a card first");
                return;
            }
            if (MainGame!.CanPlay(deck) == false)
            {
                await ShowGameMessageAsync("Illegal Move");
                PlayerHand1.UnselectAllObjects();
                return;
            }
            await MainGame.ProcessPlayAsync(deck);
        }
        public override bool CanEnableAlways()
        {
            return true;
        }
        public BasicGameCommand? UnoCommand { get; set; }
        public CustomStopWatchCP? Stops;
        protected override void EndInit()
        {
            base.EndInit(); //must do this too.
            UnoCommand = new BasicGameCommand(this, async items =>
            {
                UnoProcessing = false;
                Stops!.PauseTimer();
                await MainGame!.ProcessUnoAsync(true);

            }, Items => UnoVisible, this, CommandContainer!);
            Stops = new CustomStopWatchCP();
            Stops.MaxTime = 3000;
            Stops.TimeUp += Stops_TimeUp;
            ColorVM = new SimpleEnumPickerVM<EnumColorTypes, CheckerChoiceCP<EnumColorTypes>, ColorListChooser<EnumColorTypes>>(this);
            ColorVM.ItemClickedAsync += ColorVM_ItemClickedAsync;
            ColorVM.AutoSelectCategory = EnumAutoSelectCategory.AutoEvent;
            Deck1!.NeverAutoDisable = true;
            CommandContainer!.ExecutingChanged += CommandContainer_ExecutingChanged;
        }
        public SimpleEnumPickerVM<EnumColorTypes, CheckerChoiceCP<EnumColorTypes>, ColorListChooser<EnumColorTypes>>? ColorVM;
        private string _NextPlayer = "";
        public string NextPlayer
        {
            get
            {
                return _NextPlayer;
            }

            set
            {
                if (SetProperty(ref _NextPlayer, value) == true)
                {
                }
            }
        }

        private bool _UnoVisible;
        public bool UnoVisible
        {
            get
            {
                return _UnoVisible;
            }

            set
            {
                if (SetProperty(ref _UnoVisible, value) == true)
                {
                }
            }
        }

        private bool _ColorVisible;
        public bool ColorVisible
        {
            get
            {
                return _ColorVisible;
            }

            set
            {
                if (SetProperty(ref _ColorVisible, value) == true)
                {
                    if (value == true)
                        MainOptionsVisible = false;
                    else
                        MainOptionsVisible = true;
                    ColorVM!.Visible = value; // try this way.
                    ColorVM.IsEnabled = value; // needs this as well.  sometimes it can show but not visible (like on board games).
                }
            }
        }
        private bool CanDraw()
        {
            return !MainGame!.SingleInfo!.MainHandList.Any(Items => MainGame.CanPlay(Items.Deck));
        }
        public override bool CanEndTurn()
        {
            if (MainGame!.SaveRoot!.GameStatus != EnumGameStatus.NormalPlay)
                return false;
            if (CanDraw() == false)
                return false;// because you are able to play something.  if you have something to play, you must play it.  even if its what you drew.
            return MainGame.AlreadyDrew;
        }
        private void CommandContainer_ExecutingChanged()
        {
            if (CommandContainer!.IsExecuting == true)
            {
                ColorVisible = false;
                UnoVisible = false;
                return;
            }
            if (MainGame!.SaveRoot!.GameStatus == EnumGameStatus.ChooseColors)
            {
                ColorVisible = true;
                return;
            }
            if (MainGame.SaveRoot.GameStatus == EnumGameStatus.WaitingForUno)
            {
                UnoVisible = true;
                UnoCommand!.ReportCanExecuteChange(); //i think this is needed too.
                if (UnoProcessing == true)
                    return;
                Stops!.StartTimer();
                return;
            }
        }
        private bool UnoProcessing;
        private async Task ColorVM_ItemClickedAsync(EnumColorTypes thisPiece)
        {
            await MainGame!.ColorChosenAsync(thisPiece);
        }
        private async void Stops_TimeUp()
        {
            if (MainGame!.SaveRoot!.GameStatus != EnumGameStatus.WaitingForUno)
                throw new BasicBlankException("Not waiting for uno anymore.  Rethink");
            CommandContainer!.ManuelFinish = true;
            CommandContainer.IsExecuting = true; //now its executing.
            UnoProcessing = false;
            await MainGame.ProcessUnoAsync(false);
        }
    }
}