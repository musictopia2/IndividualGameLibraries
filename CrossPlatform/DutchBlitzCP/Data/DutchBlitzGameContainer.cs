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
using DutchBlitzCP.Cards;

namespace DutchBlitzCP.Data
{
    [SingletonGame]
    [AutoReset] //usually needs reset
    public class DutchBlitzGameContainer : CardGameContainer<DutchBlitzCardInformation, DutchBlitzPlayerItem, DutchBlitzSaveInfo>
    {
        public DutchBlitzGameContainer(BasicData basicData,
            TestOptions test,
            IGameInfo gameInfo,
            IAsyncDelayer delay,
            IEventAggregator aggregator,
            CommandContainer command,
            IGamePackageResolver resolver,
            IListShuffler<DutchBlitzCardInformation> deckList,
            RandomGenerator random)
            : base(basicData, test, gameInfo, delay, aggregator, command, resolver, deckList, random)
        {
        }
        public int MaxDiscard; //try this way.
        public ComputerCards? CurrentComputer;
        public CustomBasicList<ComputerCards> ComputerPlayers = new CustomBasicList<ComputerCards>();
    }
}
