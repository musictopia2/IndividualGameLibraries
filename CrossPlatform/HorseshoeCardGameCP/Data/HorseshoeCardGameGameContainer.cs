using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.BasicDrawables.Dictionary;
using BasicGameFrameworkLibrary.BasicDrawables.Interfaces;
using BasicGameFrameworkLibrary.BasicGameDataClasses;
using BasicGameFrameworkLibrary.CommandClasses;
using BasicGameFrameworkLibrary.CommonInterfaces;
using BasicGameFrameworkLibrary.DIContainers;
using BasicGameFrameworkLibrary.MultiplayerClasses.GameContainers;
using BasicGameFrameworkLibrary.RegularDeckOfCards;
using BasicGameFrameworkLibrary.TestUtilities;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.RandomGenerator;
using CommonBasicStandardLibraries.Messenging;
using HorseshoeCardGameCP.Cards;
using System;

namespace HorseshoeCardGameCP.Data
{
    [SingletonGame]
    [AutoReset] //usually needs reset
    public class HorseshoeCardGameGameContainer : TrickGameContainer<HorseshoeCardGameCardInformation, HorseshoeCardGamePlayerItem, HorseshoeCardGameSaveInfo, EnumSuitList>
    {
        public HorseshoeCardGameGameContainer(BasicData basicData,
            TestOptions test,
            IGameInfo gameInfo,
            IAsyncDelayer delay,
            IEventAggregator aggregator,
            CommandContainer command,
            IGamePackageResolver resolver,
            IListShuffler<HorseshoeCardGameCardInformation> deckList,
            RandomGenerator random)
            : base(basicData, test, gameInfo, delay, aggregator, command, resolver, deckList, random)
        {
        }
        internal Func<DeckObservableDict<HorseshoeCardGameCardInformation>>? GetCurrentHandList { get; set; }
    }
}
