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
using System;
using System.Threading.Tasks;
using UnoCP.Cards;

namespace UnoCP.Data
{
    [SingletonGame]
    [AutoReset] //usually needs reset
    public class UnoGameContainer : CardGameContainer<UnoCardInformation, UnoPlayerItem, UnoSaveInfo>
    {
        public UnoGameContainer(BasicData basicData,
            TestOptions test,
            IGameInfo gameInfo,
            IAsyncDelayer delay,
            IEventAggregator aggregator,
            CommandContainer command,
            IGamePackageResolver resolver,
            IListShuffler<UnoCardInformation> deckList,
            RandomGenerator random)
            : base(basicData, test, gameInfo, delay, aggregator, command, resolver, deckList, random)
        {
        }
        internal bool AlreadyUno { get; set; }
        internal Func<Task>? DoFinishAsync { get; set; }
        internal Func<int, bool>? CanPlay { get; set; }
        internal Func<Task>? CloseSaidUnoAsync { get; set; }
        internal Func<Task>? OpenSaidUnoAsync { get; set; }
    }
}
