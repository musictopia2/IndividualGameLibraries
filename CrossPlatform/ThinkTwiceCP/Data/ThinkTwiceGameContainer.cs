using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.BasicGameDataClasses;
using BasicGameFrameworkLibrary.CommandClasses;
using BasicGameFrameworkLibrary.CommonInterfaces;
using BasicGameFrameworkLibrary.DIContainers;
using BasicGameFrameworkLibrary.MultiplayerClasses.GameContainers;
using BasicGameFrameworkLibrary.MultiplayerClasses.MiscHelpers;
using BasicGameFrameworkLibrary.TestUtilities;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.RandomGenerator;
using CommonBasicStandardLibraries.Messenging;
using System;
using System.Threading.Tasks;

namespace ThinkTwiceCP.Data
{
    [SingletonGame]
    //if singleton is needed, do it as well.
    public class ThinkTwiceGameContainer : BasicGameContainer<ThinkTwicePlayerItem, ThinkTwiceSaveInfo>
    {
        public ThinkTwiceGameContainer(BasicData basicData,
            TestOptions test,
            IGameInfo gameInfo,
            IAsyncDelayer delay,
            IEventAggregator aggregator,
            CommandContainer command,
            IGamePackageResolver resolver,
            RandomGenerator random) : base(basicData, test, gameInfo, delay, aggregator, command, resolver, random)
        {
        }

        internal Func<Task>? ScoreClickAsync { get; set; }
        public Func<Task>? CategoryClicked { get; set; }
    }
}
