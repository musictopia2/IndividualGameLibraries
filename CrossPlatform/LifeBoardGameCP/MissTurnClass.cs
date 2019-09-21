using BasicGameFramework.Attributes;
using BasicGameFramework.MultiplayerClasses.BasicPlayerClasses;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace LifeBoardGameCP
{
    [SingletonGame]
    public class MissTurnClass : IMissTurnClass<LifeBoardGamePlayerItem>
    {
        private readonly LifeBoardGameViewModel _thisMod;
        public MissTurnClass(LifeBoardGameViewModel thisMod)
        {
            _thisMod = thisMod;
        }
        async Task IMissTurnClass<LifeBoardGamePlayerItem>.PlayerMissTurnAsync(LifeBoardGamePlayerItem player)
        {
            await _thisMod.ShowGameMessageAsync($"{player.NickName} has lost this turn");
        }
    }
}