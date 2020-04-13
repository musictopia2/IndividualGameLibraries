using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.MultiplayerClasses.BasicPlayerClasses;
using CandylandCP.Data;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace CandylandCP.Logic
{
    [SingletonGame]
    public class CandylandMissTurn : IMissTurnClass<CandylandPlayerItem>
    {
        public CandylandMissTurn()
        {

        }
        public async Task PlayerMissTurnAsync(CandylandPlayerItem player)
        {
            await UIPlatform.ShowMessageAsync($"{player.NickName}  missed the turn for falling in a pit");
        }
    }
}
