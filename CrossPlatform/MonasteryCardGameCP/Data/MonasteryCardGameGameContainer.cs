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
using MonasteryCardGameCP.Logic;

namespace MonasteryCardGameCP.Data
{
    [SingletonGame]
    [AutoReset] //usually needs reset
    public class MonasteryCardGameGameContainer : CardGameContainer<MonasteryCardInfo, MonasteryCardGamePlayerItem, MonasteryCardGameSaveInfo>
    {
        public MonasteryCardGameGameContainer(BasicData basicData,
            TestOptions test,
            IGameInfo gameInfo,
            IAsyncDelayer delay,
            IEventAggregator aggregator,
            CommandContainer command,
            IGamePackageResolver resolver,
            IListShuffler<MonasteryCardInfo> deckList,
            RandomGenerator random)
            : base(basicData, test, gameInfo, delay, aggregator, command, resolver, deckList, random)
        {
        }
        internal RummyClass? Rummys { get; set; } //this has to be loaded later because of the deck class;
    }
}
