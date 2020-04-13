using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.BasicGameDataClasses;
using BasicGameFrameworkLibrary.CommandClasses;
using BasicGameFrameworkLibrary.DIContainers;
using BasicGameFrameworkLibrary.DrawableListsObservable;
using BasicGameFrameworkLibrary.Extensions;
using BasicGameFrameworkLibrary.MultiplayerClasses.MainViewModels;
using BasicGameFrameworkLibrary.RegularDeckOfCards;
using BasicGameFrameworkLibrary.TestUtilities;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using OpetongCP.Data;
using OpetongCP.Logic;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace OpetongCP.ViewModels
{
    [InstanceGame]
    public class OpetongMainViewModel : BasicCardGamesVM<RegularRummyCard>
    {
        private readonly OpetongMainGameClass _mainGame;
        private readonly OpetongVMData _model;
        public OpetongMainViewModel(CommandContainer commandContainer,
            OpetongMainGameClass mainGame,
            OpetongVMData viewModel,
            BasicData basicData,
            TestOptions test,
            IGamePackageResolver resolver
            )
            : base(commandContainer, mainGame, viewModel, basicData, test, resolver)
        {
            _mainGame = mainGame;
            _model = viewModel;

            var player = _mainGame.PlayerList.GetSelf();
            _mainGame.Aggregator.Subscribe(player); //maybe for tempsets, has to be here now.

            _model.Deck1.NeverAutoDisable = true;
            _model.TempSets.Init(this);
            _model.TempSets.ClearBoard(); //try this too.
            _model.MainSets.SendEnableProcesses(this, () => false);
            _model.PlayerHand1.AutoSelect = HandObservable<RegularRummyCard>.EnumAutoType.SelectAsMany;
            _model.TempSets.SetClickedAsync += TempSets_SetClickedAsync;
        }
        protected override Task TryCloseAsync()
        {
            _model.TempSets.SetClickedAsync -= TempSets_SetClickedAsync;
            return base.TryCloseAsync();
        }
        private bool _isProcessing;
        private Task TempSets_SetClickedAsync(int index)
        {
            if (_isProcessing == true)
                return Task.CompletedTask;
            _isProcessing = true;
            var tempList = _model.PlayerHand1!.ListSelectedObjects(true);
            _model.TempSets!.AddCards(index, tempList);
            _isProcessing = false;
            return Task.CompletedTask;
        }

        //anything else needed is here.
        protected override bool CanEnableDeck()
        {
            return true;
        }

        protected override bool CanEnablePile1()
        {
            return false; //otherwise, can't compile.
        }

        protected override Task ProcessDiscardClickedAsync()
        {
            throw new BasicBlankException("Discard should never be used this time.");
        }
        public override bool CanEnableAlways()
        {
            return true;
        }
        private string _instructions = "";
        [VM]
        public string Instructions
        {
            get { return _instructions; }
            set
            {
                if (SetProperty(ref _instructions, value))
                {
                    //can decide what to do when property changes
                }

            }
        }
        [Command(EnumCommandCategory.Game)]
        public async Task PlaySetAsync()
        {
            int nums = _mainGame!.FindValidSet();
            if (nums == 0)
            {
                await UIPlatform.ShowMessageAsync("Sorry, there is no valid set here");
                return;
            }
            var thisCol = _model.TempSets.ObjectList(nums).ToRegularDeckDict();
            _model.TempSets.ClearBoard(nums);
            if (_mainGame.BasicData!.MultiPlayer)
            {
                var tempCol = thisCol.GetDeckListFromObjectList();
                await _mainGame.Network!.SendAllAsync("newset", tempCol);
            }
            await _mainGame.PlaySetAsync(thisCol);
        }

    }
}