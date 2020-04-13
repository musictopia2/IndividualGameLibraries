using BasicGameFrameworkLibrary.BasicGameDataClasses;
using BasicGameFrameworkLibrary.CommandClasses;
using BasicGameFrameworkLibrary.CommonInterfaces;
using BasicGameFrameworkLibrary.DIContainers;
using BasicGameFrameworkLibrary.ViewModels;
using CommonBasicStandardLibraries.Messenging;
using PyramidSolitaireCP.EventModels;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace PyramidSolitaireCP.ViewModels
{
    public class PyramidSolitaireShellViewModel : SinglePlayerShellViewModel,
        IHandleAsync<PossibleNewGameEventModel>
    {
        public PyramidSolitaireShellViewModel(IGamePackageResolver mainContainer, CommandContainer container, IGameInfo GameData, ISaveSinglePlayerClass saves) : base(mainContainer, container, GameData, saves)
        {
        }

        protected override bool AlwaysNewGame => false; //most games allow new game always.

        protected override IMainScreen GetMainViewModel()
        {
            var model = MainContainer.Resolve<PyramidSolitaireMainViewModel>();
            return model;
        }

        Task IHandleAsync<PossibleNewGameEventModel>.HandleAsync(PossibleNewGameEventModel message)
        {
            return ShowNewGameAsync();
        }
    }
}