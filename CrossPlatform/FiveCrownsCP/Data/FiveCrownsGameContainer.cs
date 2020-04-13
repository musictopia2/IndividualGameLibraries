using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.BasicDrawables.Interfaces;
using BasicGameFrameworkLibrary.BasicGameDataClasses;
using BasicGameFrameworkLibrary.CommandClasses;
using BasicGameFrameworkLibrary.CommonInterfaces;
using BasicGameFrameworkLibrary.DIContainers;
using BasicGameFrameworkLibrary.MultiplayerClasses.MiscHelpers;
using BasicGameFrameworkLibrary.RegularDeckOfCards;
using BasicGameFrameworkLibrary.TestUtilities;
using FiveCrownsCP.Cards;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.RandomGenerator;
using CommonBasicStandardLibraries.Messenging;
using BasicGameFrameworkLibrary.MultiplayerClasses.GameContainers;

namespace FiveCrownsCP.Data
{
    [SingletonGame]
    [AutoReset] //usually needs reset
    public class FiveCrownsGameContainer : CardGameContainer<FiveCrownsCardInformation, FiveCrownsPlayerItem, FiveCrownsSaveInfo>
    {
        public FiveCrownsGameContainer(BasicData basicData,
            TestOptions test,
            IGameInfo gameInfo,
            IAsyncDelayer delay,
            IEventAggregator aggregator,
            CommandContainer command,
            IGamePackageResolver resolver,
            IListShuffler<FiveCrownsCardInformation> deckList,
            RandomGenerator random)
            : base(basicData, test, gameInfo, delay, aggregator, command, resolver, deckList, random)
        {
        }
    }
}
