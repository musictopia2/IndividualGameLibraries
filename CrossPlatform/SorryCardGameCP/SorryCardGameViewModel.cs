using BasicGameFramework.BasicGameDataClasses;
using BasicGameFramework.ChooserClasses;
using BasicGameFramework.CommandClasses; //often times we will need commands.
using BasicGameFramework.DIContainers;
using BasicGameFramework.DrawableListsViewModels;
using BasicGameFramework.Extensions;
using BasicGameFramework.GameGraphicsCP.GamePieces;
using BasicGameFramework.MainViewModels;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.Misc;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.MVVMHelpers.Interfaces;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace SorryCardGameCP
{
    public class SorryCardGameViewModel : BasicCardGamesVM<SorryCardGameCardInformation, SorryCardGamePlayerItem, SorryCardGameMainGameClass>
    {
        private int _UpTo;
        public int UpTo
        {
            get { return _UpTo; }
            set
            {
                if (SetProperty(ref _UpTo, value))
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
        public SorryCardGameViewModel(ISimpleUI tempUI, IGamePackageResolver tempC, BasicData thisData) : base(tempUI, tempC, thisData) { }
        protected override bool CanEnableDeck()
        {
            return false;
        }
        protected override bool CanEnablePile1()
        {
            return false;
        }
        protected override async Task ProcessDiscardClickedAsync()
        {
            await Task.CompletedTask;
        }
        public override bool CanEnableAlways()
        {
            return true;
        }
        public PileViewModel<SorryCardGameCardInformation>? Pile2;
        public BoardGamesColorPicker<EnumColorChoices, PawnPiecesCP<EnumColorChoices>, SorryCardGamePlayerItem>? ColorChooser { get; set; }
        public bool ColorVisible => ColorChooser!.Visible;
        public CustomStopWatchCP? Stops;
        public void NotifyColorChange()
        {
            OnPropertyChanged(nameof(ColorVisible));
        }
        protected override void EndInit()
        {
            base.EndInit(); //must do this too.
            Deck1!.NeverAutoDisable = true; //if you want reshuffling, use this.  otherwise, comment or delete.
            Pile2 = new PileViewModel<SorryCardGameCardInformation>(ThisE!, this);
            Pile2.Text = "Play Pile";
            Pile2.FirstLoad(new SorryCardGameCardInformation());
            Pile2.Visible = true;
            Pile2.SendEnableProcesses(this, () =>
            {
                if (MainGame!.DidChooseColors == false)
                    return false;
                return MainGame.SaveRoot!.GameStatus == EnumGameStatus.Regular;
            });
            Pile2.PileClickedAsync += Pile2_PileClickedAsync;
            PlayerHand1!.Maximum = 8;
            ColorChooser = new BoardGamesColorPicker<EnumColorChoices, PawnPiecesCP<EnumColorChoices>, SorryCardGamePlayerItem>(this);
            ColorChooser.AutoSelectCategory = EnumAutoSelectCategory.AutoEvent; //i meant autoevent.
            ColorChooser.ItemClickedAsync += ColorChooser_ItemClickedAsync;
            CommandContainer!.ExecutingChanged += CommandContainer_ExecutingChanged;
            Stops = new CustomStopWatchCP();
            Stops.MaxTime = 7000;
            Stops.TimeUp += Stops_TimeUp;
        }
        private async void Stops_TimeUp()
        {
            await StopTimerAsync();
        }
        private async Task StopTimerAsync()
        {
            CommandContainer!.ManuelFinish = true;
            CommandContainer.IsExecuting = true;
            int myPlayer = MainGame!.PlayerList!.GetSelf().Id;
            if (ThisData!.MultiPlayer)
                await ThisNet!.SendAllAsync("timeout", myPlayer);
            await MainGame.NoSorryAsync(myPlayer);
        }
        private void CommandContainer_ExecutingChanged()
        {
            if (MainGame!.DidChooseColors == false)
            {
                MainOptionsVisible = false;
                ColorChooser!.Visible = true;
                NotifyColorChange();
                return;
            }
            if (CommandContainer!.IsExecuting)
                return;
            if (MainGame!.SaveRoot!.GameStatus == EnumGameStatus.HasDontBeSorry || MainGame.SaveRoot.GameStatus == EnumGameStatus.WaitForSorry21)
            {
                PlayerHand1!.AutoSelect = HandViewModel<SorryCardGameCardInformation>.EnumAutoType.None;
                Stops!.StartTimer();
                return;
            }
        }
        private async Task ColorChooser_ItemClickedAsync(EnumColorChoices thisPiece)
        {
            await MainGame!.ChoseColorAsync(thisPiece); //hopefully this simple.
        }
        private async Task Pile2_PileClickedAsync()
        {
            var thisList = PlayerHand1!.ListSelectedObjects();
            if (thisList.Count == 0)
            {
                await ShowGameMessageAsync("Must choose at least one card to play");
                return;
            }
            bool rets = MainGame!.IsValidMove(thisList);
            if (rets == false)
            {
                await ShowGameMessageAsync("Illegal Move");
                return;
            }
            if (ThisData!.MultiPlayer)
                await ThisNet!.SendAllAsync("regularplay", thisList.GetDeckListFromObjectList());
            await MainGame.PlaySeveralCards(thisList);
        }
        private async Task PlaySorryCardAsync(SorryCardGameCardInformation thisCard)
        {
            int myID = MainGame!.PlayerList!.GetSelf().Id;
            if (ThisData!.MultiPlayer)
            {
                SorryPlay thisPlay = new SorryPlay();
                thisPlay.Deck = thisCard.Deck;
                thisPlay.Player = myID;
                await ThisNet!.SendAllAsync("sorrycard", thisPlay);
            }
            await MainGame.PlaySorryCardAsync(thisCard, myID);
        }
        protected override async Task ProcessHandClickedAsync(SorryCardGameCardInformation thisCard, int index)
        {
            if (MainGame!.SaveRoot!.GameStatus == EnumGameStatus.WaitForSorry21)
            {
                Stops!.PauseTimer();
                if (thisCard.Sorry == EnumSorry.At21)
                {
                    await PlaySorryCardAsync(thisCard);
                    return;
                }
                await ShowGameMessageAsync("Illegal Move");
                await StopTimerAsync();
                return;
            }
            if (MainGame.SaveRoot.GameStatus == EnumGameStatus.HasDontBeSorry)
            {
                Stops!.PauseTimer();
                if (thisCard.Sorry == EnumSorry.Dont)
                {
                    await PlaySorryCardAsync(thisCard);
                    return;
                }
                await ShowGameMessageAsync("Illegal Move");
                await StopTimerAsync();
                return;
            }
            throw new BasicBlankException("If the game status is not wait for sorry21 or has don't be sorry; then can't choose just one card to play");
        }
    }
}