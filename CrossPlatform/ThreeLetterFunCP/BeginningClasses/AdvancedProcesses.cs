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
    public class AdvancedProcesses : IAdvancedProcesses
    {
        private readonly BasicData _basicData;
        private readonly TestOptions _test;
        private readonly IAsyncDelayer _delayer;
        private readonly IEventAggregator _aggregator;
        private readonly ThreeLetterFunMainGameClass _mainGame;
        private readonly IShuffleTiles _shuffle;
        private readonly INetworkMessages? _network;
        private readonly IMessageChecker? _checker;

        public AdvancedProcesses(BasicData basicData,
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

        async Task IAdvancedProcesses.ChoseAdvancedOptions(bool easy, bool shortGame)
        {
            if (_basicData.Client == false)
            {
                AdvancedSettingModel model = new AdvancedSettingModel()
                {
                    IsEasy = easy,
                    ShortGame = shortGame
                };


                await _network!.SendAllAsync("advancedsettings", model);
            }
            else if (_test.NoAnimations == false)
            {
                await _delayer.DelayMilli(500);
            }
            await _aggregator.PublishAsync(new NextScreenEventModel(EnumNextScreen.Finished));
            if (_basicData.Client == true)
            {
                _checker!.IsEnabled = true;
                return;
            }
            _mainGame.SaveRoot.ShortGame = shortGame;
            if (easy)
            {
                _mainGame.SaveRoot.Level = Data.EnumLevel.Moderate;
            }
            else
            {
                _mainGame.SaveRoot.Level = Data.EnumLevel.Hard;
            }
            await _shuffle.StartShufflingAsync(_mainGame, 0);
        }
    }
}