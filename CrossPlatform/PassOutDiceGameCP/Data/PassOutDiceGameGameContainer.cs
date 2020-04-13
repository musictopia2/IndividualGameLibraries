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
using System.Threading.Tasks; //most of the time, i will be using asyncs.
//i think this is the most common things i like to do
namespace PassOutDiceGameCP.Data
{
    [SingletonGame]
    [AutoReset]
    public class PassOutDiceGameGameContainer : BasicGameContainer<PassOutDiceGamePlayerItem, PassOutDiceGameSaveInfo>
    {
        public PassOutDiceGameGameContainer(
            BasicData basicData,
            TestOptions test,
            IGameInfo gameInfo,
            IAsyncDelayer delay,
            IEventAggregator aggregator,
            CommandContainer command,
            IGamePackageResolver resolver,
            RandomGenerator random) : base(basicData,
                test,
                gameInfo,
                delay,
                aggregator,
                command,
                resolver,
                random)
        {
        }
        internal Dictionary<int, SpaceInfo>? SpaceList { get; set; }
        internal Func<int, Task>? MakeMoveAsync { get; set; }
    }
}
