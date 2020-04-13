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
namespace SorryCP.Data
{
    [SingletonGame] //this one will not be reset.
    public class SorryGameContainer : BasicGameContainer<SorryPlayerItem, SorrySaveInfo>
    {
        public SorryGameContainer(
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
        internal int MovePlayer { get; set; }
        internal int PlayerGoingBack { get; set; }
        internal AnimateSkiaSharpGameBoard Animates { get; set; } = new AnimateSkiaSharpGameBoard(); //hopefully this simple (?)

        //this time, i want to set delegates to stop the overflows.

        internal Func<Task>? DrawClickAsync { get; set; }
        internal Func<EnumColorChoice, Task>? HomeClickedAsync { get; set; }
        internal Func<int, Task>? SpaceClickedAsync { get; set; }
    }
}
