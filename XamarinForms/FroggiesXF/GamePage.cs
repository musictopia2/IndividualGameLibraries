using BasicGameFrameworkLibrary.BasicGameDataClasses;
using BasicGameFrameworkLibrary.CommonInterfaces;
using BasicGameFrameworkLibrary.StandardImplementations.XamarinForms.Interfaces;
using BasicGamingUIXFLibrary.Shells;
using BasicXFControlsAndPages.MVVMFramework.Controls;
using FroggiesCP.ViewModels;
using System.Threading.Tasks; //most of the time, i will be using asyncs.

namespace FroggiesXF
{
    public class GamePage : SinglePlayerShellView
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
            ParentSingleUIContainer parent = new ParentSingleUIContainer(nameof(FroggiesShellViewModel.OpeningScreen));
            AddMain(parent); //hopefully this simple (?)
            parent = new ParentSingleUIContainer(nameof(FroggiesShellViewModel.TestScreen));
            AddMain(parent);
            return Task.CompletedTask;
        }
    }
}
