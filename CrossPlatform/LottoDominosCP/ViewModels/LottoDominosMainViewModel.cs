using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.BasicGameDataClasses;
using BasicGameFrameworkLibrary.CommandClasses;
using BasicGameFrameworkLibrary.DIContainers;
using BasicGameFrameworkLibrary.MultiplayerClasses.MainViewModels;
using BasicGameFrameworkLibrary.TestUtilities;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.Messenging;
using LottoDominosCP.EventModels;
using LottoDominosCP.Logic;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace LottoDominosCP.ViewModels
{
    [InstanceGame]
    public class LottoDominosMainViewModel : BasicMultiplayerMainVM, IHandleAsync<ChangeGameStatusEventModel>
    {
        private readonly LottoDominosMainGameClass _mainGame; //if we don't need, delete.
        private readonly IGamePackageResolver _resolver;

        public LottoDominosMainViewModel(CommandContainer commandContainer,
            LottoDominosMainGameClass mainGame,
            IViewModelData viewModel,
            BasicData basicData,
            TestOptions test,
            IGamePackageResolver resolver
            )
            : base(commandContainer, mainGame, viewModel, basicData, test, resolver)
        {
            _mainGame = mainGame;
            _resolver = resolver;
        }
        //anything else needed is here.

        public ChooseNumberViewModel? ChooseScreen { get; set; }
        public MainBoardViewModel? BoardScreen { get; set; } //you have to do properties for screens.

        protected override async Task ActivateAsync()
        {
            if (_mainGame.SaveRoot.GameStatus == Data.EnumStatus.ChooseNumber)
            {
                ChooseScreen = _resolver.Resolve<ChooseNumberViewModel>();
                await LoadScreenAsync(ChooseScreen);
                return;
            }
            if (_mainGame.SaveRoot.GameStatus == Data.EnumStatus.NormalPlay)
            {
                BoardScreen = _resolver.Resolve<MainBoardViewModel>();
                await LoadScreenAsync(BoardScreen); //hopefully no need for load event.
                return;
            }
            throw new BasicBlankException("Rethink because no status found");
        }

        async Task IHandleAsync<ChangeGameStatusEventModel>.HandleAsync(ChangeGameStatusEventModel message)
        {
            if (_mainGame.SaveRoot.GameStatus != Data.EnumStatus.NormalPlay)
            {
                throw new BasicBlankException("Only normal play is supported to change status.  Otherwise, rethink");
            }
            if (ChooseScreen == null)
            {
                return;
                //throw new BasicBlankException("The choose screen was never opened.  Rethink");
            }
            await CloseSpecificChildAsync(ChooseScreen);
            ChooseScreen = null;
            BoardScreen = _resolver.Resolve<MainBoardViewModel>();
            await LoadScreenAsync(BoardScreen); //hopefully no need to even have load this time.
        }
    }
}