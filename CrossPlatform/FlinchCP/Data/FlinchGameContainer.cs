using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.BasicDrawables.Interfaces;
using BasicGameFrameworkLibrary.BasicGameDataClasses;
using BasicGameFrameworkLibrary.CommandClasses;
using BasicGameFrameworkLibrary.CommonInterfaces;
using BasicGameFrameworkLibrary.DIContainers;
using BasicGameFrameworkLibrary.MultiplayerClasses.MiscHelpers;
using BasicGameFrameworkLibrary.RegularDeckOfCards;
using BasicGameFrameworkLibrary.TestUtilities;
using FlinchCP.Cards;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.RandomGenerator;
using CommonBasicStandardLibraries.Messenging;
using System;
using System.Threading.Tasks;
using BasicGameFrameworkLibrary.MultiplayerClasses.GameContainers;

namespace FlinchCP.Data
{
    [SingletonGame]
    [AutoReset] //usually needs reset
    public class FlinchGameContainer : CardGameContainer<FlinchCardInformation, FlinchPlayerItem, FlinchSaveInfo>
    {
        public FlinchGameContainer(BasicData basicData,
            TestOptions test,
            IGameInfo gameInfo,
            IAsyncDelayer delay,
            IEventAggregator aggregator,
            CommandContainer command,
            IGamePackageResolver resolver,
            IListShuffler<FlinchCardInformation> deckList,
            RandomGenerator random)
            : base(basicData, test, gameInfo, delay, aggregator, command, resolver, deckList, random)
        {
        }
        internal Func<Task>? LoadPlayerPilesAsync { get; set; }

        internal Func<int, int, bool>? IsValidMove { get; set; }
    }
}
