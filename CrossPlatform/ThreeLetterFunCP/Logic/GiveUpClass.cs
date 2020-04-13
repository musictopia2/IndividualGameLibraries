using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.BasicGameDataClasses;
using BasicGameFrameworkLibrary.Extensions;
using BasicGameFrameworkLibrary.NetworkingClasses.Interfaces;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace ThreeLetterFunCP.Logic
{
    [SingletonGame]
    [AutoReset]
    public class GiveUpClass
    {
        private readonly GlobalHelpers _global;
        private readonly BasicData _basicData;
        private readonly ThreeLetterFunMainGameClass _mainGame;
        private readonly INetworkMessages? _network;
        public GiveUpClass(GlobalHelpers global,
            BasicData basicData,
            ThreeLetterFunMainGameClass mainGame)
        {
            _global = global;
            _basicData = basicData;
            _mainGame = mainGame;
            _global.SelfGiveUpAsync = SelfGiveUpAsync;
            _network = _basicData.GetNetwork();
        }
        public async Task SelfGiveUpAsync(bool doStop)
        {
            if (_basicData.MultiPlayer)
            {
                if (doStop)
                {
                    _global.Stops!.ManualStop(false);
                }
                var player = _mainGame.PlayerList.GetSelf();
                _mainGame.WhoTurn = player.Id;
                await _network!.SendAllAsync("giveup", player.Id);
            }

            await _mainGame.GiveUpAsync(); //still needs this function.
        }
    }
}
