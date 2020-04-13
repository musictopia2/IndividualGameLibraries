using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.BasicDrawables.Interfaces;
using BasicGameFrameworkLibrary.BasicGameDataClasses;
using BasicGameFrameworkLibrary.CommandClasses;
using BasicGameFrameworkLibrary.CommonInterfaces;
using BasicGameFrameworkLibrary.DIContainers;
using BasicGameFrameworkLibrary.MultiplayerClasses.MiscHelpers;
using BasicGameFrameworkLibrary.RegularDeckOfCards;
using BasicGameFrameworkLibrary.TestUtilities;
using SkuckCardGameCP.Cards;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.RandomGenerator;
using CommonBasicStandardLibraries.Messenging;
using System;
using System.Threading.Tasks;
using BasicGameFrameworkLibrary.MultiplayerClasses.GameContainers;

namespace SkuckCardGameCP.Data
{
    [SingletonGame]
    [AutoReset] //usually needs reset
    public class SkuckCardGameGameContainer : TrickGameContainer<SkuckCardGameCardInformation, SkuckCardGamePlayerItem, SkuckCardGameSaveInfo, EnumSuitList>
    {
        public SkuckCardGameGameContainer(BasicData basicData,
            TestOptions test,
            IGameInfo gameInfo,
            IAsyncDelayer delay,
            IEventAggregator aggregator,
            CommandContainer command,
            IGamePackageResolver resolver,
            IListShuffler<SkuckCardGameCardInformation> deckList,
            RandomGenerator random)
            : base(basicData, test, gameInfo, delay, aggregator, command, resolver, deckList, random)
        {
        }
        public Func<Task>? ComputerTurnAsync { get; set; }
        public Func<Task>? StartNewTrickAsync { get; set; }
        public Func<Task>? ShowHumanCanPlayAsync { get; set; }
    }
}
