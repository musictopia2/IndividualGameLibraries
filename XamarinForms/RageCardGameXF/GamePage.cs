using BasicGameFrameworkLibrary.BasicGameDataClasses;
using BasicGameFrameworkLibrary.CommonInterfaces;
using BasicGameFrameworkLibrary.StandardImplementations.XamarinForms.Interfaces;
using BasicGamingUIXFLibrary.Shells;
using BasicXFControlsAndPages.MVVMFramework.Controls;
using RageCardGameCP.ViewModels;
using System.Threading.Tasks; //most of the time, i will be using asyncs.

namespace RageCardGameXF
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

        }



        protected override Task PopulateUIAsync()
        {
            ParentSingleUIContainer parent = new ParentSingleUIContainer(nameof(RageCardGameShellViewModel.BidScreen));
            AddMain(parent);
            parent = new ParentSingleUIContainer(nameof(RageCardGameShellViewModel.ColorScreen));
            AddMain(parent);
            return Task.CompletedTask;
        }
    }
}
