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
using MilkRunCP.Cards;
using System;

namespace MilkRunCP.Data
{
    [SingletonGame]
    [AutoReset] //usually needs reset
    public class MilkRunGameContainer : CardGameContainer<MilkRunCardInformation, MilkRunPlayerItem, MilkRunSaveInfo>
    {
        public MilkRunGameContainer(BasicData basicData,
            TestOptions test,
            IGameInfo gameInfo,
            IAsyncDelayer delay,
            IEventAggregator aggregator,
            CommandContainer command,
            IGamePackageResolver resolver,
            IListShuffler<MilkRunCardInformation> deckList,
            RandomGenerator random)
            : base(basicData, test, gameInfo, delay, aggregator, command, resolver, deckList, random)
        {
        }
        internal Func<int, int, EnumPileType, EnumMilkType, bool>? CanMakeMove { get; set; }
    }
}
