using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.BasicGameDataClasses;
using BasicGameFrameworkLibrary.CommonInterfaces;
using BasicGameFrameworkLibrary.DIContainers;
using BasicGameFrameworkLibrary.Extensions;
using BasicGameFrameworkLibrary.NetworkingClasses.Interfaces;
using BasicGameFrameworkLibrary.TestUtilities;
using CommonBasicStandardLibraries.Messenging;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using ThreeLetterFunCP.Data;
using ThreeLetterFunCP.EventModels;
//i think this is the most common things i like to do
namespace ThreeLetterFunCP.BeginningClasses
{
    [SingletonGame]
    public class FirstOptionProcessClass : IFirstOptionProcesses
    {
        private readonly BasicData _basicData;
        private readonly TestOptions _test;
        private readonly IAsyncDelayer _delayer;
        private readonly IGamePackageResolver _resolver;
        private readonly IEventAggregator _aggregator;
        private readonly INetworkMessages? _network;
        private readonly IMessageChecker? _checker;
        public FirstOptionProcessClass(BasicData basicData,
            TestOptions test,
            IAsyncDelayer delayer,
            IGamePackageResolver resolver,
            IEventAggregator aggregator
            )
        {
            _basicData = basicData;
            _test = test;
            _delayer = delayer;
            _resolver = resolver;
            _aggregator = aggregator;
            _network = _basicData.GetNetwork();
            _checker = _basicData.GetChecker();
        }
        async Task IFirstOptionProcesses.BeginningOptionSelectedAsync(EnumFirstOption firstOption)
        {
            if (_basicData.Client == false)
            {
                //send message to client so they can process.
                await _network!.SendAllAsync("firstoption", firstOption);
            }
            else if (_test.NoAnimations == false)
            {
                await _delayer.DelayMilli(300);
            }
            //this needs to send message to shell view model to do the next part.
            if (firstOption == EnumFirstOption.Beginner)
            {
                ThreeLetterFunSaveInfo saveRoot = _resolver.Resolve<ThreeLetterFunSaveInfo>();
                saveRoot.Level = EnumLevel.Easy;
                await _aggregator.PublishAsync(new NextScreenEventModel(EnumNextScreen.Cards));
                if (_basicData.Client == true)
                {
                    _checker!.IsEnabled = true;
                }
                return;
            }
            await _aggregator.PublishAsync(new NextScreenEventModel(EnumNextScreen.Advanced));
            if (_basicData.Client == true)
            {
                _checker!.IsEnabled = true;
            }
        }
    }
}
