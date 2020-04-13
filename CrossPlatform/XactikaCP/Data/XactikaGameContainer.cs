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
using XactikaCP.Cards;

namespace XactikaCP.Data
{
    [SingletonGame]
    [AutoReset] //usually needs reset
    public class XactikaGameContainer : TrickGameContainer<XactikaCardInformation, XactikaPlayerItem, XactikaSaveInfo, EnumShapes>
    {
        public XactikaGameContainer(BasicData basicData,
            TestOptions test,
            IGameInfo gameInfo,
            IAsyncDelayer delay,
            IEventAggregator aggregator,
            CommandContainer command,
            IGamePackageResolver resolver,
            IListShuffler<XactikaCardInformation> deckList,
            RandomGenerator random)
            : base(basicData, test, gameInfo, delay, aggregator, command, resolver, deckList, random)
        {
        }
        internal Func<Task>? LoadBiddingAsync { get; set; }
        internal Func<Task>? CloseBiddingAsync { get; set; }
        internal Func<Task>? LoadShapeButtonAsync { get; set; }
        internal Func<Task>? CloseShapeButtonAsync { get; set; }
        internal bool ShowedOnce { get; set; } //i think needs to be here.  since its possible other classes need it.
        internal Func<Task>? StartNewTrickAsync { get; set; }
        internal Func<Task>? ShowHumanCanPlayAsync { get; set; }
        internal Action? ShowTurn { get; set; }
    }
}
