using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.BasicGameDataClasses;
using BasicGameFrameworkLibrary.CommandClasses;
using BasicGameFrameworkLibrary.DIContainers;
using BasicGameFrameworkLibrary.MultiplayerClasses.MainViewModels;
using BasicGameFrameworkLibrary.TestUtilities;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using DominosMexicanTrainCP.Data;
using DominosMexicanTrainCP.Logic;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace DominosMexicanTrainCP.ViewModels
{
    [InstanceGame]
    public class DominosMexicanTrainMainViewModel : DominoGamesVM<MexicanDomino>
    {
        private readonly DominosMexicanTrainMainGameClass _mainGame; //if we don't need, delete.
        private readonly DominosMexicanTrainVMData _viewModel;
        private readonly TestOptions _test;

        public DominosMexicanTrainMainViewModel(
            CommandContainer commandContainer,
            DominosMexicanTrainMainGameClass mainGame,
            DominosMexicanTrainVMData viewModel,
            BasicData basicData,
            TestOptions test,

            IGamePackageResolver resolver) : base(commandContainer, mainGame, viewModel, basicData, test, resolver)
        {
            _mainGame = mainGame;
            _viewModel = viewModel;
            _test = test;
            GlobalClass.TrainClickedAsync = TrainClickedAsync;
            _viewModel.PrivateTrainObjectClickedAsync = PrivateTrain1_ObjectClickedAsync;
            _viewModel.PrivateTrainBoardClickedAsync = PrivateTrain1_BoardClickedAsync;
            commandContainer!.ExecutingChanged += CommandContainer_ExecutingChanged;
        }
        public override bool CanEndTurn()
        {
            if (_test.AllowAnyMove)
            {
                return false;
            }
            bool sats = _mainGame!.ForceSatisfy();
            if (_viewModel.BoneYard!.IsEnabled == false && sats == false)
                return true;
            if (_viewModel.BoneYard.HasDrawn() == false)
                return false;
            return !sats;
        }
        private void CommandContainer_ExecutingChanged()
        {
            NotifyOfCanExecuteChange(nameof(CanEndTurn));
        }

        private async Task PrivateTrain1_ObjectClickedAsync(MexicanDomino payLoad, int index)
        {
            int decks = _viewModel.PlayerHand1!.ObjectSelected();
            if (decks > 0)
            {
                PutBackHand(decks);
                return;
            }
            if (payLoad.IsSelected == true)
            {
                payLoad.IsSelected = false;
                return;
            }
            _viewModel.PrivateTrain1!.UnselectAllObjects();
            payLoad.IsSelected = true;
            await Task.CompletedTask;
        }
        private async Task PrivateTrain1_BoardClickedAsync()
        {
            int decks = _viewModel.PlayerHand1!.ObjectSelected();
            if (decks > 0)
                PutBackHand(decks);
            await Task.CompletedTask;
        }



        private async Task TrainClickedAsync(int index)
        {
            CommandContainer.StartExecuting();
            int decks = DominoSelected(out bool train);
            if (decks == 0)
            {
                await UIPlatform.ShowMessageAsync("Sorry, must have one domino selected to put on the pile");
                CommandContainer.StopExecuting();
                CommandContainer.ManualReport();
                return;
            }
            MexicanDomino thisDomino;
            if (train)
                thisDomino = _viewModel.PrivateTrain1!.HandList.GetSpecificItem(decks);
            else
                thisDomino = _viewModel.PlayerHand1.HandList.GetSpecificItem(decks);
            if (_viewModel.TrainStation1!.CanPlacePiece(thisDomino, index) == false)
            {
                await UIPlatform.ShowMessageAsync("Illegal Move"); 
                CommandContainer.StopExecuting();
                return;
            }
            if (_mainGame.BasicData!.MultiPlayer)
            {
                SendPlay output = new SendPlay();
                output.Deck = decks;
                output.Section = index;
                await _mainGame.Network!.SendAllAsync("dominoplayed", output);
            }
            if (train)
            {
                _mainGame!.SingleInfo!.LongestTrainList.RemoveObjectByDeck(decks);
                _viewModel.PrivateTrain1!.HandList.RemoveObjectByDeck(decks);
            }
            else
                _mainGame!.SingleInfo!.MainHandList.RemoveObjectByDeck(decks);
            _viewModel.UpdateCount(_mainGame.SingleInfo!);
            await _viewModel.TrainStation1.AnimateShowSelectedDominoAsync(index, thisDomino, _mainGame); //hopefully this simple.
            CommandContainer.StopExecuting(); //try this as well.
        }


        private int _upTo;
        [VM]
        public int UpTo
        {
            get { return _upTo; }
            set
            {
                if (SetProperty(ref _upTo, value))
                {
                    //can decide what to do when property changes
                }
            }
        }
        private void PutBackTrain(int decks)
        {
            var thisDomino = _mainGame!.SingleInfo!.LongestTrainList.GetSpecificItem(decks);
            _mainGame.SingleInfo.LongestTrainList.RemoveObjectByDeck(decks);
            _viewModel.PrivateTrain1!.HandList.RemoveObjectByDeck(decks);
            _mainGame.SingleInfo.MainHandList.Add(thisDomino);
            thisDomino.IsSelected = false;
            _mainGame.SingleInfo.MainHandList.Sort();
            _viewModel.UpdateCount(_mainGame.SingleInfo);
        }
        private void PutBackHand(int decks)
        {
            var thisDomino = _mainGame!.SingleInfo!.MainHandList.GetSpecificItem(decks);
            _viewModel.PrivateTrain1!.HandList.Add(thisDomino);
            _mainGame.SingleInfo.LongestTrainList.Add(thisDomino);
            thisDomino.IsSelected = false;
            _mainGame.SingleInfo.MainHandList.RemoveObjectByDeck(decks);
            _viewModel.UpdateCount(_mainGame.SingleInfo);
        }
        protected override bool CanEnableBoneYard()
        {
            if (_test.AllowAnyMove)
            {
                return false;
            }
            bool sats = _mainGame!.ForceSatisfy();
            if (_viewModel.BoneYard!.HasDrawn())
                return false;
            return !sats;
        }
        private int DominoSelected(out bool isTrain)
        {
            int hands = _viewModel.PlayerHand1!.ObjectSelected();
            isTrain = false;
            int trains = _viewModel.PrivateTrain1!.ObjectSelected();
            if (hands > 0 && trains > 0)
                throw new BasicBlankException("Can only have one selected from the train or hand but not both");
            if (hands > 0)
                return hands;
            if (trains == 0)
                return 0;
            isTrain = true;
            return trains;
        }
        protected override async Task HandClicked(MexicanDomino domino, int index)
        {
            int decks = _viewModel.PrivateTrain1!.ObjectSelected();
            if (decks > 0)
            {
                PutBackTrain(decks);
                return;
            }
            if (domino.IsSelected == true)
            {
                domino.IsSelected = false;
                return;
            }
            _viewModel.PlayerHand1!.UnselectAllObjects();
            domino.IsSelected = true;
            _mainGame!.SingleInfo = _mainGame.PlayerList!.GetWhoPlayer();
            await Task.CompletedTask;
        }
        protected override async Task PlayerBoardClickedAsync()
        {
            int decks = _viewModel.PrivateTrain1!.ObjectSelected();
            if (decks > 0)
                PutBackTrain(decks);
            await Task.CompletedTask;
        }
        [Command(EnumCommandCategory.Game)]
        public void LongestTrain()
        {
            _viewModel.PlayerHand1.HandList.AddRange(_mainGame!.SingleInfo!.LongestTrainList);
            _viewModel.PlayerHand1.HandList.Sort(); //maybe i have to sort first.
            int locals = _viewModel.TrainStation1!.DominoNeeded(_mainGame.WhoTurn); //try that way.
            LongestTrainClass temps = new LongestTrainClass();
            var output = temps.GetTrainList(_mainGame.SingleInfo.MainHandList, locals);
            _mainGame.SingleInfo.LongestTrainList.ReplaceRange(output);
            _viewModel.PrivateTrain1!.HandList.ReplaceRange(output);
            _viewModel.UpdateCount(_mainGame.SingleInfo);
        }
    }
}