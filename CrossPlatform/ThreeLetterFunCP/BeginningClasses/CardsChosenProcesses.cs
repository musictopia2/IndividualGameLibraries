using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.BasicGameDataClasses;
using BasicGameFrameworkLibrary.CommonInterfaces;
using BasicGameFrameworkLibrary.Extensions;
using BasicGameFrameworkLibrary.NetworkingClasses.Interfaces;
using BasicGameFrameworkLibrary.TestUtilities;
using CommonBasicStandardLibraries.Messenging;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using ThreeLetterFunCP.EventModels;
using ThreeLetterFunCP.Logic;

namespace ThreeLetterFunCP.BeginningClasses
{
    [SingletonGame]
    [AutoReset]
    public class CardsChosenProcesses : ICardsChosenProcesses
    {
        private readonly BasicData _basicData;
        private readonly TestOptions _test;
        private readonly IAsyncDelayer _delayer;
        private readonly IEventAggregator _aggregator;
        private readonly ThreeLetterFunMainGameClass _mainGame;
        private readonly IShuffleTiles _shuffle;
        private readonly INetworkMessages? _network;
        private readonly IMessageChecker? _checker;

        public CardsChosenProcesses(BasicData basicData,
            TestOptions test,
            IAsyncDelayer delayer,
            IEventAggregator aggregator,
            ThreeLetterFunMainGameClass mainGame,
            IShuffleTiles shuffle
            )
        {
            _basicData = basicData;
            _test = test;
            _delayer = delayer;
            _aggregator = aggregator;
            _mainGame = mainGame;
            _shuffle = shuffle;
            _network = _basicData.GetNetwork();
            _checker = _basicData.GetChecker();
        }

        async Task ICardsChosenProcesses.CardsChosenAsync(int howManyCards)
        {
            if (_basicData.Client == false)
            {
                //send message to client so they can process.
                await _network!.SendAllAsync("howmanycards", howManyCards);
            }
            else if (_test.NoAnimations == false)
            {
                await _delayer.DelayMilli(300);
            }
            await _aggregator.PublishAsync(new NextScreenEventModel(EnumNextScreen.Finished));
            if (_basicData.Client == true)
            {
                _checker!.IsEnabled = true;
                return;
            }
            await _shuffle.StartShufflingAsync(_mainGame, howManyCards);
        }
    }
}
