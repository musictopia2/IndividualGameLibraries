using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.BasicDrawables.Interfaces;
using BasicGameFrameworkLibrary.BasicGameDataClasses;
using BasicGameFrameworkLibrary.CommandClasses;
using BasicGameFrameworkLibrary.CommonInterfaces;
using BasicGameFrameworkLibrary.DIContainers;
using BasicGameFrameworkLibrary.MultiplayerClasses.GameContainers;
using BasicGameFrameworkLibrary.RegularDeckOfCards;
using BasicGameFrameworkLibrary.TestUtilities;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.RandomGenerator;
using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.Messenging;
using System;

namespace BladesOfSteelCP.Data
{
    [SingletonGame]
    [AutoReset] //usually needs reset
    public class BladesOfSteelGameContainer : CardGameContainer<RegularSimpleCard, BladesOfSteelPlayerItem, BladesOfSteelSaveInfo>
    {
        public BladesOfSteelGameContainer(BasicData basicData,
            TestOptions test,
            IGameInfo gameInfo,
            IAsyncDelayer delay,
            IEventAggregator aggregator,
            CommandContainer command,
            IGamePackageResolver resolver,
            IListShuffler<RegularSimpleCard> deckList,
            RandomGenerator random)
            : base(basicData, test, gameInfo, delay, aggregator, command, resolver, deckList, random)
        {
        }
        internal int OtherTurn
        {
            get
            {
                return SaveRoot!.PlayOrder.OtherTurn;
            }
            set
            {
                SaveRoot!.PlayOrder.OtherTurn = value;
            }
        }

        internal Func<ICustomBasicList<RegularSimpleCard>, EnumAttackGroup>? GetAttackStage { get; set; }
        internal Func<ICustomBasicList<RegularSimpleCard>, EnumDefenseGroup>? GetDefenseStage { get; set; }
    }
}
