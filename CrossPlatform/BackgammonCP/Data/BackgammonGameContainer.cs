using BackgammonCP.Graphics;
using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.BasicGameDataClasses;
using BasicGameFrameworkLibrary.CommandClasses;
using BasicGameFrameworkLibrary.CommonInterfaces;
using BasicGameFrameworkLibrary.DIContainers;
using BasicGameFrameworkLibrary.GameGraphicsCP.Animations;
using BasicGameFrameworkLibrary.MultiplayerClasses.GameContainers;
using BasicGameFrameworkLibrary.TestUtilities;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.RandomGenerator;
using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.Messenging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
//i think this is the most common things i like to do
namespace BackgammonCP.Data
{
    [SingletonGame]
    [AutoReset]
    public class BackgammonGameContainer : BasicGameContainer<BackgammonPlayerItem, BackgammonSaveInfo>
    {
        public BackgammonGameContainer(
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
            Animates.LongestTravelTime = 100;
        }

        internal Dictionary<int, TriangleClass> TriangleList { get; set; } = new Dictionary<int, TriangleClass>();
        internal AnimateSkiaSharpGameBoard Animates { get; set; } = new AnimateSkiaSharpGameBoard();
        internal bool MoveInProgress { get; set; }
        internal CustomBasicList<MoveInfo> MoveList { get; set; } = new CustomBasicList<MoveInfo>();
        internal int FirstDiceValue { get; set; }
        internal int SecondDiceValue { get; set; }
        internal bool HadDoubles()
        {
            if (FirstDiceValue == 0)
                throw new BasicBlankException("The dice can never roll a 0.  Must populate the dice value first");
            return FirstDiceValue == SecondDiceValue;
        }
        internal Func<int, Task>? MakeMoveAsync { get; set; }
        internal Action? DiceVisibleProcesses { get; set; }
    }
}
