using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.BasicDrawables.Dictionary;
using BasicGameFrameworkLibrary.BasicDrawables.Interfaces;
using BasicGameFrameworkLibrary.BasicGameDataClasses;
using BasicGameFrameworkLibrary.CommandClasses;
using BasicGameFrameworkLibrary.CommonInterfaces;
using BasicGameFrameworkLibrary.DIContainers;
using BasicGameFrameworkLibrary.MultiplayerClasses.GameContainers;
using BasicGameFrameworkLibrary.MultiplayerClasses.MiscHelpers;
using BasicGameFrameworkLibrary.TestUtilities;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.RandomGenerator;
using CommonBasicStandardLibraries.Messenging;
using MonopolyCardGameCP.Cards;
using MonopolyCardGameCP.Logic;
using System;

namespace MonopolyCardGameCP.Data
{
    [SingletonGame]
    [AutoReset] //usually needs reset
    public class MonopolyCardGameGameContainer : CardGameContainer<MonopolyCardGameCardInformation, MonopolyCardGamePlayerItem, MonopolyCardGameSaveInfo>
    {
        public MonopolyCardGameGameContainer(BasicData basicData,
            TestOptions test,
            IGameInfo gameInfo,
            IAsyncDelayer delay,
            IEventAggregator aggregator,
            CommandContainer command,
            IGamePackageResolver resolver,
            IListShuffler<MonopolyCardGameCardInformation> deckList,
            RandomGenerator random)
            : base(basicData, test, gameInfo, delay, aggregator, command, resolver, deckList, random)
        {
        }
        internal Action<TradePile, DeckRegularDict<MonopolyCardGameCardInformation>, TradePile>? ProcessTrade { get; set; }
    }
}