using BasicGameFrameworkLibrary.Attributes;
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
using RageCardGameCP.Cards;
using System;
using System.Threading.Tasks;

namespace RageCardGameCP.Data
{
    [SingletonGame]
    [AutoReset] //usually needs reset
    public class RageCardGameGameContainer : TrickGameContainer<RageCardGameCardInformation, RageCardGamePlayerItem, RageCardGameSaveInfo, EnumColor>
    {
        public RageCardGameGameContainer(BasicData basicData,
            TestOptions test,
            IGameInfo gameInfo,
            IAsyncDelayer delay,
            IEventAggregator aggregator,
            CommandContainer command,
            IGamePackageResolver resolver,
            IListShuffler<RageCardGameCardInformation> deckList,
            RandomGenerator random)
            : base(basicData, test, gameInfo, delay, aggregator, command, resolver, deckList, random)
        {
        }
        internal Func<Task>? ColorChosenAsync { get; set; }
        internal Action? ShowLeadColor { get; set; }
        internal Func<Task>? ChooseColorAsync { get; set; }
    }
}
