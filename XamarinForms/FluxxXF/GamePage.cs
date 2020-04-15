using BasicGameFrameworkLibrary.BasicGameDataClasses;
using BasicGameFrameworkLibrary.CommonInterfaces;
using BasicGameFrameworkLibrary.StandardImplementations.XamarinForms.Interfaces;
using BasicGamingUIXFLibrary.Shells;
using BasicXFControlsAndPages.MVVMFramework.Controls;
using FluxxCP.ViewModels;
using System.Threading.Tasks; //most of the time, i will be using asyncs.

namespace FluxxXF
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
            ParentSingleUIContainer parent = new ParentSingleUIContainer(nameof(FluxxShellViewModel.ActionScreen));
            AddMain(parent);
            parent = new ParentSingleUIContainer(nameof(FluxxShellViewModel.KeeperScreen));
            AddMain(parent);
            return Task.CompletedTask;
        }
    }
}