using BasicGameFrameworkLibrary.BasicEventModels;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using MinesweeperCP.Data;
using MinesweeperCP.ViewModels;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
//i think this is the most common things i like to do
namespace MinesweeperCP.Logic
{
    public static class Helpers
    {
        public static async Task MessageGameOverAsync(this MinesweeperMainGameClass game, string message)
        {
            await UIPlatform.ShowMessageAsync(message);
            //rethink about anything else.
            await game.SendGameOverAsync();
        }
        public static void PopulateMinesNeeded(this ILevelVM level)
        {
            if ((int)level.LevelChosen == (int)EnumLevel.Easy)
                level.HowManyMinesNeeded = 10;
            else if ((int)level.LevelChosen == (int)EnumLevel.Medium)
                level.HowManyMinesNeeded = 20;
            else
                level.HowManyMinesNeeded = 30;
        }
    }
}
