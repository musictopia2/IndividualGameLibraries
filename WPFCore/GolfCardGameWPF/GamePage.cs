using BasicControlsAndWindowsCore.Controls.NavigationControls;
using BasicGameFrameworkLibrary.BasicGameDataClasses;
using BasicGameFrameworkLibrary.CommonInterfaces;
using BasicGamingUIWPFLibrary.Shells;
using GolfCardGameCP.ViewModels;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
//should not need the view models though.  if i am wrong, rethink.
//i think this is the most common things i like to do
namespace GolfCardGameWPF
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
                Name = nameof(GolfCardGameShellViewModel.FirstScreen)
            };
            AddMain(parent);
            return Task.CompletedTask;
        }
    }
}
