using BasicGameFramework.BasicEventModels;
using BasicGameFramework.BasicGameDataClasses;
using BasicGameFramework.CommandClasses;
using BasicGameFramework.DIContainers;
using BasicGameFramework.DrawableListsViewModels;
using BasicGameFramework.MainViewModels;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.MVVMHelpers.Interfaces;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace DominosMexicanTrainCP
{
    public class DominosMexicanTrainViewModel : DominoGamesVM<MexicanDomino, DominosMexicanTrainPlayerItem, DominosMexicanTrainMainGameClass>
    {
        public TrainStationBoardProcesses? TrainStation1;
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
        public void AfterLoadControls() //needs players for this part
        {
            PrivateTrain1!.Text = "Your Train"; //hopefully this simple now.
        }
        public DominosMexicanTrainViewModel(ISimpleUI tempUI, IGamePackageResolver tempC, BasicData thisData) : base(tempUI, tempC, thisData) { }
        public BasicGameCommand<int>? TrainStationCommand { get; set; }
        public BasicGameCommand? LongestTrainCommand { get; set; }
        public HandViewModel<MexicanDomino>? PrivateTrain1;
        public override bool CanEndTurn()
        {
            bool sats = MainGame!.ForceSatisfy();
            if (BoneYard!.IsEnabled == false && sats == false)
                return true;
            if (BoneYard.HasDrawn() == false)
                return false;
            return !sats;
        }
        protected override void EndInit()
        {
            base.EndInit(); //best to keep this to be safe.
            PlayerHand1!.AutoSelect = HandViewModel<MexicanDomino>.EnumAutoType.None;
            TrainStationCommand = new BasicGameCommand<int>(this, async index =>
            {
                int decks = DominoSelected(out bool train);
                if (decks == 0)
                {
                    await ShowGameMessageAsync("Sorry, must have one domino selected to put on the pile");
                    return;
                }
                MexicanDomino thisDomino;
                if (train)
                    thisDomino = PrivateTrain1!.HandList.GetSpecificItem(decks);
                else
                    thisDomino = PlayerHand1.HandList.GetSpecificItem(decks);
                if (TrainStation1!.CanPlacePiece(thisDomino, index) == false)
                {
                    await ShowGameMessageAsync("Illegal Move");
                    return;
                }
                if (ThisData!.MultiPlayer)
                {
                    SendPlay output = new SendPlay();
                    output.Deck = decks;
                    output.Section = index;
                    await ThisNet!.SendAllAsync("dominoplayed", output);
                }
                if (train)
                {
                    MainGame!.SingleInfo!.LongestTrainList.RemoveObjectByDeck(decks);
                    PrivateTrain1!.HandList.RemoveObjectByDeck(decks);
                }
                else
                    MainGame!.SingleInfo!.MainHandList.RemoveObjectByDeck(decks);
                UpdateCount();
                await TrainStation1.AnimateShowSelectedDominoAsync(index, thisDomino);
            }, items => true, this, CommandContainer!);
            LongestTrainCommand = new BasicGameCommand(this, items =>
            {
                PlayerHand1.HandList.AddRange(MainGame!.SingleInfo!.LongestTrainList);
                PlayerHand1.HandList.Sort(); //maybe i have to sort first.
                int locals = TrainStation1!.DominoNeeded(MainGame.WhoTurn); //try that way.
                LongestTrainClass temps = new LongestTrainClass();
                var output = temps.GetTrainList(MainGame.SingleInfo.MainHandList, locals);
                MainGame.SingleInfo.LongestTrainList.ReplaceRange(output);
                PrivateTrain1!.HandList.ReplaceRange(output);
                UpdateCount();
            }, items => true, this, CommandContainer!);
            //i think you can't do on your turn because causes too many problems.
            PrivateTrain1 = new HandViewModel<MexicanDomino>(this);
            PrivateTrain1.AutoSelect = HandViewModel<MexicanDomino>.EnumAutoType.None;
            PrivateTrain1.Visible = true;
            PrivateTrain1.BoardClickedAsync += PrivateTrain1_BoardClickedAsync;
            PrivateTrain1.ObjectClickedAsync += PrivateTrain1_ObjectClickedAsync;
            TrainStation1 = MainContainer!.Resolve<TrainStationBoardProcesses>();
            CommandContainer!.ExecutingChanged += CommandContainer_ExecutingChanged;
        }
        internal void UpdateCount()
        {
            UpdateCountEventModel thisC = new UpdateCountEventModel();
            thisC.ObjectCount = MainGame!.SingleInfo!.LongestTrainList.Count;
            ThisE!.Publish(thisC);
        }
        private void CommandContainer_ExecutingChanged()
        {
            EndTurnCommand.ReportCanExecuteChange(); //because of boneyard.
        }
        private async Task PrivateTrain1_ObjectClickedAsync(MexicanDomino ThisObject, int Index)
        {
            int decks = PlayerHand1!.ObjectSelected();
            if (decks > 0)
            {
                PutBackHand(decks);
                return;
            }
            if (ThisObject.IsSelected == true)
            {
                ThisObject.IsSelected = false;
                return;
            }
            PrivateTrain1!.UnselectAllObjects();
            ThisObject.IsSelected = true;
            await Task.CompletedTask;
        }
        private async Task PrivateTrain1_BoardClickedAsync()
        {
            int decks = PlayerHand1!.ObjectSelected();
            if (decks > 0)
                PutBackHand(decks);
            await Task.CompletedTask;
        }
        protected override async Task HandClicked(MexicanDomino thisDomino, int index)
        {
            int decks = PrivateTrain1!.ObjectSelected();
            if (decks > 0)
            {
                PutBackTrain(decks);
                return;
            }
            if (thisDomino.IsSelected == true)
            {
                thisDomino.IsSelected = false;
                return;
            }
            PlayerHand1!.UnselectAllObjects();
            thisDomino.IsSelected = true;
            MainGame!.SingleInfo = MainGame.PlayerList!.GetWhoPlayer();
            await Task.CompletedTask;
        }
        protected override async Task PlayerBoardClickedAsync()
        {
            int decks = PrivateTrain1!.ObjectSelected();
            if (decks > 0)
                PutBackTrain(decks);
            await Task.CompletedTask;
        }
        private void PutBackTrain(int decks)
        {
            var thisDomino = MainGame!.SingleInfo!.LongestTrainList.GetSpecificItem(decks);
            MainGame.SingleInfo.LongestTrainList.RemoveObjectByDeck(decks);
            PrivateTrain1!.HandList.RemoveObjectByDeck(decks);
            MainGame.SingleInfo.MainHandList.Add(thisDomino);
            thisDomino.IsSelected = false;
            MainGame.SingleInfo.MainHandList.Sort();
            UpdateCount();
        }
        private void PutBackHand(int decks)
        {
            var thisDomino = MainGame!.SingleInfo!.MainHandList.GetSpecificItem(decks);
            PrivateTrain1!.HandList.Add(thisDomino);
            MainGame.SingleInfo.LongestTrainList.Add(thisDomino);
            thisDomino.IsSelected = false;
            MainGame.SingleInfo.MainHandList.RemoveObjectByDeck(decks);
            UpdateCount();
        }
        protected override bool CanEnableBoneYard()
        {
            bool sats = MainGame!.ForceSatisfy();
            if (BoneYard!.HasDrawn())
                return false;
            return !sats;
        }
        private int DominoSelected(out bool isTrain)
        {
            int hands = PlayerHand1!.ObjectSelected();
            isTrain = false;
            int trains = PrivateTrain1!.ObjectSelected();
            if (hands > 0 && trains > 0)
                throw new BasicBlankException("Can only have one selected from the train or hand but not both");
            if (hands > 0)
                return hands;
            if (trains == 0)
                return 0;
            isTrain = true;
            return trains;
        }
    }
}