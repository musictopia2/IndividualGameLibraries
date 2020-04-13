using BasicControlsAndWindowsCore.Controls.NavigationControls;
using BasicGameFrameworkLibrary.BasicGameDataClasses;
using BasicGameFrameworkLibrary.CommonInterfaces;
using BasicGamingUIWPFLibrary.Shells;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using XactikaCP.ViewModels;
namespace XactikaWPF
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
                Name = nameof(XactikaShellViewModel.ModeScreen)
            };
            AddMain(parent);
            return Task.CompletedTask;
        }
    }
}
