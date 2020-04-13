using BasicControlsAndWindowsCore.Controls.NavigationControls;
using BasicGameFrameworkLibrary.BasicGameDataClasses;
using BasicGameFrameworkLibrary.CommonInterfaces;
using BasicGamingUIWPFLibrary.Shells;
using PickelCardGameCP.ViewModels;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace PickelCardGameWPF
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
            ParentSingleUIContainer parent = new ParentSingleUIContainer()
            {
                Name = nameof(PickelCardGameShellViewModel.BidScreen)
            };
            AddMain(parent);
            //if any exceptions to the shell, do here or override other things.
            return Task.CompletedTask;
        }
    }
}
