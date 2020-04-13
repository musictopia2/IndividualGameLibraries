using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.BasicDrawables.Interfaces;
using BasicGameFrameworkLibrary.BasicGameDataClasses;
using BasicGameFrameworkLibrary.CommandClasses;
using BasicGameFrameworkLibrary.CommonInterfaces;
using BasicGameFrameworkLibrary.DIContainers;
using BasicGameFrameworkLibrary.MultiplayerClasses.MiscHelpers;
using BasicGameFrameworkLibrary.RegularDeckOfCards;
using BasicGameFrameworkLibrary.TestUtilities;
using HuseHeartsCP.Cards;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.RandomGenerator;
using CommonBasicStandardLibraries.Messenging;
using HuseHeartsCP.Logic;
using BasicGameFrameworkLibrary.MultiplayerClasses.GameContainers;

namespace HuseHeartsCP.Data
{
    [SingletonGame]
    [AutoReset] //usually needs reset
    public class HuseHeartsGameContainer : TrickGameContainer<HuseHeartsCardInformation, HuseHeartsPlayerItem, HuseHeartsSaveInfo, EnumSuitList>
    {
        public HuseHeartsGameContainer(BasicData basicData,
            TestOptions test,
            IGameInfo gameInfo,
            IAsyncDelayer delay,
            IEventAggregator aggregator,
            CommandContainer command,
            IGamePackageResolver resolver,
            IListShuffler<HuseHeartsCardInformation> deckList,
            RandomGenerator random,
            HuseHeartsDelegates delegates
            
            )
            : base(basicData, test, gameInfo, delay, aggregator, command, resolver, deckList, random)
        {
            delegates.SetDummyList = ((value) => SaveRoot.DummyList = value);
            delegates.GetDummyList = (() => SaveRoot.DummyList);
        }
    }
}