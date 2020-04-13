using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.BasicDrawables.Interfaces;
using BasicGameFrameworkLibrary.BasicGameDataClasses;
using BasicGameFrameworkLibrary.CommandClasses;
using BasicGameFrameworkLibrary.CommonInterfaces;
using BasicGameFrameworkLibrary.DIContainers;
using BasicGameFrameworkLibrary.MultiplayerClasses.GameContainers;
using BasicGameFrameworkLibrary.RegularDeckOfCards;
using BasicGameFrameworkLibrary.SpecializedGameTypes.RummyClasses;
using BasicGameFrameworkLibrary.TestUtilities;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.RandomGenerator;
using CommonBasicStandardLibraries.Messenging;
using System;
using System.Threading.Tasks;

namespace OpetongCP.Data
{
    [SingletonGame]
    [AutoReset] //usually needs reset
    public class OpetongGameContainer : CardGameContainer<RegularRummyCard, OpetongPlayerItem, OpetongSaveInfo>
    {
        public OpetongGameContainer(BasicData basicData,
            TestOptions test,
            IGameInfo gameInfo,
            IAsyncDelayer delay,
            IEventAggregator aggregator,
            CommandContainer command,
            IGamePackageResolver resolver,
            IListShuffler<RegularRummyCard> deckList,
            RandomGenerator random)
            : base(basicData, test, gameInfo, delay, aggregator, command, resolver, deckList, random)
        {
            Rummys = new RummyProcesses<EnumSuitList, EnumColorList, RegularRummyCard>();
            Rummys.HasSecond = true;
            Rummys.HasWild = false;
            Rummys.LowNumber = 1;
            Rummys.HighNumber = 13;
            Rummys.NeedMatch = false;
        }
        internal RummyProcesses<EnumSuitList, EnumColorList, RegularRummyCard> Rummys { get; set; }
        internal Func<int, Task>? DrawFromPoolAsync { get; set; }
    }
}