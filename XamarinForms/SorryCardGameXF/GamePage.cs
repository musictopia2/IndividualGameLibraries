using BasicGameFrameworkLibrary.BasicGameDataClasses;
using BasicGameFrameworkLibrary.CommonInterfaces;
using BasicGameFrameworkLibrary.StandardImplementations.XamarinForms.Interfaces;
using BasicGamingUIXFLibrary.Shells;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace SorryCardGameXF
{
    public class GamePage : BasicBoardGamesShellView
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
            //if any exceptions to the shell, do here or override other things.
            return Task.CompletedTask;
        }
    }
}
