using BasicControlsAndWindowsCore.Controls.NavigationControls;
using BasicGameFrameworkLibrary.BasicGameDataClasses;
using BasicGameFrameworkLibrary.CommonInterfaces;
using BasicGamingUIWPFLibrary.Shells;
using FluxxCP.ViewModels;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace FluxxWPF
{
    public class GamePage : MultiplayerBasicShellView
    {
        public GamePage(IGameInfo gameData,
            BasicData basicData,
            IStartUp start) : base(gameData, basicData, start)
        {
        }


        protected override Task PopulateUIAsync()
        {
            //if any exceptions to the shell, do here or override other things.
            ParentSingleUIContainer parent = new ParentSingleUIContainer()
            {
                Name = nameof(FluxxShellViewModel.ActionScreen)
            };
            AddMain(parent);
            parent = new ParentSingleUIContainer()
            {
                Name = nameof(FluxxShellViewModel.KeeperScreen)
            };
            AddMain(parent);
            return Task.CompletedTask;
        }
    }
}
