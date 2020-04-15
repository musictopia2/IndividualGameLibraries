using BasicGameFrameworkLibrary.BasicGameDataClasses;
using BasicGameFrameworkLibrary.CommonInterfaces;
using BasicGameFrameworkLibrary.StandardImplementations.XamarinForms.Interfaces;
using BasicGamingUIXFLibrary.Shells;
using BasicXFControlsAndPages.MVVMFramework.Controls;
using PickelCardGameCP.ViewModels;
using System.Threading.Tasks; //most of the time, i will be using asyncs.

namespace PickelCardGameXF
{
    public class GamePage : MultiplayerBasicShellView
    {
        public GamePage(
            IGamePlatform customPlatform,
            IGameInfo gameData,
            BasicData basicData,
            IStartUp start,
            IStandardScreen screen) : base(customPlatform, gameData, basicData, start, screen)
        {
            //TODO:  may need to think about if we need load or update (?)
        }



        protected override Task PopulateUIAsync()
        {
            ParentSingleUIContainer parent = new ParentSingleUIContainer(nameof(PickelCardGameShellViewModel.BidScreen));
            AddMain(parent);
            return Task.CompletedTask;
        }
    }
}
