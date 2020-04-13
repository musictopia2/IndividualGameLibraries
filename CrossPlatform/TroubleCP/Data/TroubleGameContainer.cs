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
namespace TroubleCP.Data
{
    [SingletonGame]
    [AutoReset]
    public class TroubleGameContainer : BasicGameContainer<TroublePlayerItem, TroubleSaveInfo>
    {
        public TroubleGameContainer(
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
        internal Func<int, bool>? IsValidMove { get; set; }
        internal Func<int, Task>? MakeMoveAsync { get; set; }
        public Action? PositionDice { get; set; }
        internal Func<bool>? CanRollDice { get; set; }
        internal Func<Task>? RollDiceAsync { get; set; }
    }
}