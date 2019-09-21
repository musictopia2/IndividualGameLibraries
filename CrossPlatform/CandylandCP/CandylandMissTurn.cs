using BasicGameFramework.Attributes;
using BasicGameFramework.MultiplayerClasses.BasicPlayerClasses;
using BasicGameFramework.ViewModelInterfaces;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace CandylandCP
{
    [SingletonGame]
    public class CandylandMissTurn : IMissTurnClass<CandylandPlayerItem>
    {
        private readonly IBlankGameVM _thisMod;
        public CandylandMissTurn(IBlankGameVM thisMod)
        {
            _thisMod = thisMod;
        }
        public async Task PlayerMissTurnAsync(CandylandPlayerItem player)
        {
            await _thisMod.ShowGameMessageAsync($"{player.NickName}  missed the turn for falling in a pit");
        }
    }
}