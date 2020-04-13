using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.BasicGameDataClasses;
using BasicGameFrameworkLibrary.CommandClasses;
using BasicGameFrameworkLibrary.CommonInterfaces;
using BasicGameFrameworkLibrary.DIContainers;
using BasicGameFrameworkLibrary.MultiplayerClasses.GameContainers;
using BasicGameFrameworkLibrary.TestUtilities;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.RandomGenerator;
using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.Messenging;
using System;
using System.Threading.Tasks;

namespace CountdownCP.Data
{
    [SingletonGame]
    //if singleton is needed, do it as well.
    public class CountdownGameContainer : BasicGameContainer<CountdownPlayerItem, CountdownSaveInfo>
    {
        public CountdownGameContainer(BasicData basicData,
            TestOptions test,
            IGameInfo gameInfo,
            IAsyncDelayer delay,
            IEventAggregator aggregator,
            CommandContainer command,
            IGamePackageResolver resolver,
            RandomGenerator random) : base(basicData, test, gameInfo, delay, aggregator, command, resolver, random)
        {
        }
        internal Func<SimpleNumber, Task>? MakeMoveAsync { get; set; }
        internal Func<CustomBasicList<SimpleNumber>>? GetNumberList { get; set; }
    }
}
