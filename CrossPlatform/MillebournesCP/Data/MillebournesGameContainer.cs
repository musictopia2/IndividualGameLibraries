using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.BasicDrawables.Interfaces;
using BasicGameFrameworkLibrary.BasicGameDataClasses;
using BasicGameFrameworkLibrary.CommandClasses;
using BasicGameFrameworkLibrary.CommonInterfaces;
using BasicGameFrameworkLibrary.DIContainers;
using BasicGameFrameworkLibrary.MultiplayerClasses.GameContainers;
using BasicGameFrameworkLibrary.TestUtilities;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.RandomGenerator;
using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.Messenging;
using MillebournesCP.Cards;
using MillebournesCP.Logic;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MillebournesCP.Data
{
    [SingletonGame]
    [AutoReset] //usually needs reset
    public class MillebournesGameContainer : CardGameContainer<MillebournesCardInformation, MillebournesPlayerItem, MillebournesSaveInfo>
    {
        public MillebournesGameContainer(BasicData basicData,
            TestOptions test,
            IGameInfo gameInfo,
            IAsyncDelayer delay,
            IEventAggregator aggregator,
            CommandContainer command,
            IGamePackageResolver resolver,
            IListShuffler<MillebournesCardInformation> deckList,
            RandomGenerator random)
            : base(basicData, test, gameInfo, delay, aggregator, command, resolver, deckList, random)
        {
        }

        public CoupeInfo CurrentCoupe = new CoupeInfo(); //hopefully this is okay.
        public TeamCP? CurrentCP;
        public CustomBasicList<TeamCP> TeamList = new CustomBasicList<TeamCP>();
        public TeamCP FindTeam(int teamNumber)
        {
            return TeamList.Single(items => items.TeamNumber == teamNumber);
        }
        internal Func<EnumPileType, int, Task>? TeamClickAsync { get; set; } //not sure who will handle this.

        internal Func<Task>? CloseCoupeAsync { get; set; }
        internal Func<Task>? LoadCoupeAsync { get; set; }
    }
}
