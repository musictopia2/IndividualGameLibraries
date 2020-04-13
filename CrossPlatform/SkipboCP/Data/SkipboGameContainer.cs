using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.BasicDrawables.Interfaces;
using BasicGameFrameworkLibrary.BasicGameDataClasses;
using BasicGameFrameworkLibrary.CommandClasses;
using BasicGameFrameworkLibrary.CommonInterfaces;
using BasicGameFrameworkLibrary.DIContainers;
using BasicGameFrameworkLibrary.MultiplayerClasses.GameContainers;
using BasicGameFrameworkLibrary.TestUtilities;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.RandomGenerator;
using CommonBasicStandardLibraries.Messenging;
using SkipboCP.Cards;
using System;
using System.Threading.Tasks;

namespace SkipboCP.Data
{
    [SingletonGame]
    [AutoReset] //usually needs reset
    public class SkipboGameContainer : CardGameContainer<SkipboCardInformation, SkipboPlayerItem, SkipboSaveInfo>
    {
        public SkipboGameContainer(BasicData basicData,
            TestOptions test,
            IGameInfo gameInfo,
            IAsyncDelayer delay,
            IEventAggregator aggregator,
            CommandContainer command,
            IGamePackageResolver resolver,
            IListShuffler<SkipboCardInformation> deckList,
            RandomGenerator random)
            : base(basicData, test, gameInfo, delay, aggregator, command, resolver, deckList, random)
        {
        }
        //this needs to also hold the delegate to load the proper screen for a player.
        internal Func<Task>? LoadPlayerPilesAsync { get; set; }

        internal Func<int, int, bool>? IsValidMove { get; set; }

    }
}
