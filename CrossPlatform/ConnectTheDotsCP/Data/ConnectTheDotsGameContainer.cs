using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.BasicGameDataClasses;
using BasicGameFrameworkLibrary.CommandClasses;
using BasicGameFrameworkLibrary.CommonInterfaces;
using BasicGameFrameworkLibrary.DIContainers;
using BasicGameFrameworkLibrary.MultiplayerClasses.GameContainers;
using BasicGameFrameworkLibrary.TestUtilities;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.RandomGenerator;
using CommonBasicStandardLibraries.Messenging;
using ConnectTheDotsCP.Graphics;
using System;
using System.Collections.Generic;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
//i think this is the most common things i like to do
namespace ConnectTheDotsCP.Data
{
    [SingletonGame]
    [AutoReset]
    public class ConnectTheDotsGameContainer : BasicGameContainer<ConnectTheDotsPlayerItem, ConnectTheDotsSaveInfo>
    {
        public ConnectTheDotsGameContainer(
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
        internal Dictionary<int, SquareInfo>? SquareList { get; set; }
        internal Dictionary<int, LineInfo>? LineList { get; set; }
        internal Dictionary<int, DotInfo>? DotList { get; set; }
        internal LineInfo PreviousLine { get; set; } = new LineInfo();
        internal DotInfo PreviousDot { get; set; } = new DotInfo();

        internal Func<int, Task>? MakeMoveAsync { get; set; }

    }
}
