using BasicGameFramework.MultiplayerClasses.InterfaceMessages;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace SorryCP
{
    public class MultiPlayerDraw : IDrawCardNM
    {
        private readonly SorryMainGameClass _mainGame;
        public MultiPlayerDraw(SorryMainGameClass mainGame)
        {
            _mainGame = mainGame;
        }
        async Task IDrawCardNM.DrawCardReceivedAsync(string data)
        {
            await _mainGame.DrawCardAsync();
        }
    }
}