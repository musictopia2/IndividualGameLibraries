using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.BasicGameDataClasses;
using BasicGameFrameworkLibrary.CommandClasses;
using BasicGameFrameworkLibrary.CommonInterfaces;
using BasicGameFrameworkLibrary.DIContainers;
using BasicGameFrameworkLibrary.GameGraphicsCP.Animations;
using BasicGameFrameworkLibrary.MultiplayerClasses.GameContainers;
using BasicGameFrameworkLibrary.TestUtilities;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.RandomGenerator;
using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.Messenging;
namespace CheckersCP.Data
{
    [SingletonGame]
    [AutoReset] //taking a risk on this one.
    public class CheckersGameContainer : BasicGameContainer<CheckersPlayerItem, CheckersSaveInfo>
    {
        //this means no more global is needed because this can be the global now.
        public CheckersGameContainer(
            BasicData basicData,
            TestOptions test,
            IGameInfo gameInfo,
            IAsyncDelayer delay,
            IEventAggregator aggregator,
            CommandContainer command,
            IGamePackageResolver resolver,
            RandomGenerator random) : base(basicData,
                test,
                gameInfo,
                delay,
                aggregator,
                command,
                resolver,
                random)
        {
            //this means no more need for function to loading board.
            Animates = new AnimateSkiaSharpGameBoard();
            Animates.LongestTravelTime = 100;
            CurrentCrowned = false; //i think.
        }
        //hopefully no need to have autoclear (?)
        //otherwise, we need the other global class that gets cleared out.
        public AnimateSkiaSharpGameBoard Animates;
        public bool CurrentCrowned;
        public CustomBasicList<MoveInfo> CompleteMoveList { get; set; } = new CustomBasicList<MoveInfo>();
        public CustomBasicList<MoveInfo> CurrentMoveList { get; set; } = new CustomBasicList<MoveInfo>();
        public CustomBasicList<SpaceCP>? SpaceList;

    }
}
