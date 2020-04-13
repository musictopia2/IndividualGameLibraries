using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.BasicGameDataClasses;
using BasicGameFrameworkLibrary.CommandClasses;
using BasicGameFrameworkLibrary.CommonInterfaces;
using BasicGameFrameworkLibrary.DIContainers;
using BasicGameFrameworkLibrary.Dominos;
using BasicGameFrameworkLibrary.MultiplayerClasses.GameContainers;
using BasicGameFrameworkLibrary.TestUtilities;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.RandomGenerator;
using CommonBasicStandardLibraries.Messenging;
using System;
using System.Threading.Tasks;

namespace DiceDominosCP.Data
{
    [SingletonGame]
    //if singleton is needed, do it as well.
    public class DiceDominosGameContainer : BasicGameContainer<DiceDominosPlayerItem, DiceDominosSaveInfo>
    {
        public DiceDominosGameContainer(BasicData basicData,
            TestOptions test,
            IGameInfo gameInfo,
            IAsyncDelayer delay,
            IEventAggregator aggregator,
            CommandContainer command,
            IGamePackageResolver resolver,
            RandomGenerator random,
            DominosBasicShuffler<SimpleDominoInfo> dominosShuffler
            ) : base(basicData, test, gameInfo, delay, aggregator, command, resolver, random)
        {
            DominosShuffler = dominosShuffler;
        }

        public DominosBasicShuffler<SimpleDominoInfo> DominosShuffler { get; } //hopefully this simple.
        //Task DominoClickedAsync(SimpleDominoInfo thisDomino)
        internal Func<SimpleDominoInfo, Task>? DominoClickedAsync { get; set; }
    }
}
