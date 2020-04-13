using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.BasicGameDataClasses;
using BasicGameFrameworkLibrary.CommandClasses;
using BasicGameFrameworkLibrary.CommonInterfaces;
using BasicGameFrameworkLibrary.DIContainers;
using BasicGameFrameworkLibrary.MultiplayerClasses.GameContainers;
using BasicGameFrameworkLibrary.TestUtilities;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.RandomGenerator;
using CommonBasicStandardLibraries.Messenging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RollEmCP.Data
{
    [SingletonGame]
    //if singleton is needed, do it as well.
    public class RollEmGameContainer : BasicGameContainer<RollEmPlayerItem, RollEmSaveInfo>
    {
        public RollEmGameContainer(BasicData basicData,
            TestOptions test,
            IGameInfo gameInfo,
            IAsyncDelayer delay,
            IEventAggregator aggregator,
            CommandContainer command,
            IGamePackageResolver resolver,
            RandomGenerator random) : base(basicData, test, gameInfo, delay, aggregator, command, resolver, random)
        {
        }
        internal Dictionary<int, NumberInfo> NumberList { get; set; } = new Dictionary<int, NumberInfo>();

        internal Func<int, Task>? MakeMoveAsync { get; set; }

    }
}