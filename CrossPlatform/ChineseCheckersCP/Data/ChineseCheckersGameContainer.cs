using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.BasicGameDataClasses;
using BasicGameFrameworkLibrary.CommandClasses;
using BasicGameFrameworkLibrary.CommonInterfaces;
using BasicGameFrameworkLibrary.DIContainers;
using BasicGameFrameworkLibrary.GameGraphicsCP.Animations;
using BasicGameFrameworkLibrary.MultiplayerClasses.GameContainers;
using BasicGameFrameworkLibrary.TestUtilities;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.RandomGenerator;
using CommonBasicStandardLibraries.Messenging;
using System;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
//i think this is the most common things i like to do
namespace ChineseCheckersCP.Data
{
    [SingletonGame] //this one will not be reset.
    //this time, can risk keeping as singleton.
    public class ChineseCheckersGameContainer : BasicGameContainer<ChineseCheckersPlayerItem, ChineseCheckersSaveInfo>
    {
        public ChineseCheckersGameContainer(
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
            Animates = new AnimateSkiaSharpGameBoard();
        }

        public Func<bool>? CanMove { get; set; }
        public Func<int, Task>? MakeMoveAsync { get; set; }
        public AnimateSkiaSharpGameBoard Animates { get; set; }
        public ChineseCheckersVMData? Model { get; set; }
    }
}
