using System;
using System.Text;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using System.Linq;
using CommonBasicStandardLibraries.BasicDataSettingsAndProcesses;
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;
using CommonBasicStandardLibraries.CollectionClasses;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using fs = CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.JsonSerializers.FileHelpers;
using js = CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.JsonSerializers.NewtonJsonStrings; //just in case i need those 2.
using BasicGameFrameworkLibrary.ViewModels;
using BasicGameFrameworkLibrary.CommonInterfaces;
using BasicGameFrameworkLibrary.DIContainers;
using BasicGameFrameworkLibrary.CommandClasses;
using BasicGameFrameworkLibrary.BasicGameDataClasses;
using CommonBasicStandardLibraries.MVVMFramework.ViewModels;
using CommonBasicStandardLibraries.Messenging;
using BuncoDiceGameCP.EventModels;
namespace BuncoDiceGameCP.ViewModels
{
    public class BuncoDiceGameShellViewModel : SinglePlayerShellViewModel, 
        IHandleAsync<ChoseNewRoundEventModel>,
        IHandleAsync<EndGameEventModel>,
        IHandleAsync<NewRoundEventModel>
    {
        public BuncoDiceGameShellViewModel(IGamePackageResolver mainContainer, CommandContainer container, IGameInfo GameData, ISaveSinglePlayerClass saves) : base(mainContainer, container, GameData, saves)
        {
        }

        protected override bool AlwaysNewGame => false; //most games allow new game always.
        protected override bool AutoStartNewGame => true;
        public IScreen? TempScreen { get; set; }


        protected override IMainScreen GetMainViewModel()
        {
            var model = MainContainer.Resolve<BuncoDiceGameMainViewModel>();
            return model;
        }

        async Task IHandleAsync<ChoseNewRoundEventModel>.HandleAsync(ChoseNewRoundEventModel message)
        {
            if (TempScreen == null)
            {
                throw new BasicBlankException("No screen was set up to show you chose new round.  Rethink");
            }
            await CloseSpecificChildAsync(TempScreen);
            TempScreen = null;
        }

        Task IHandleAsync<EndGameEventModel>.HandleAsync(EndGameEventModel message)
        {
            if (TempScreen != null)
            {
                throw new BasicBlankException("The screen was never closed out.  Rethink");
            }
            TempScreen = MainContainer.Resolve<EndGameViewModel>();
            return LoadScreenAsync(TempScreen);
        }
        protected override async Task GameOverScreenAsync()
        {
            if (TempScreen == null)
            {
                throw new BasicBlankException("Must have the end screen first.  Rethink");
            }
            await CloseSpecificChildAsync(TempScreen);
            TempScreen = null;
        }
        Task IHandleAsync<NewRoundEventModel>.HandleAsync(NewRoundEventModel message)
        {
            if (TempScreen != null)
            {
                throw new BasicBlankException("The screen was never closed out.  Rethink");
            }
            TempScreen = MainContainer.Resolve<BuncoNewRoundViewModel>();
            return LoadScreenAsync(TempScreen);
        }
    }
}
