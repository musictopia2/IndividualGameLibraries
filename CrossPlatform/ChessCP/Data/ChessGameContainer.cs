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
namespace ChessCP.Data
{
    [SingletonGame]
    [AutoReset]
    public class ChessGameContainer : BasicGameContainer<ChessPlayerItem, ChessSaveInfo>
    {
        public ChessGameContainer(
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
            Animates = new AnimateSkiaSharpGameBoard();
            if (BasicData.IsXamarinForms == false)
            {
                Animates.LongestTravelTime = 200;
            }
            else
            {
                Animates.LongestTravelTime = 75;
            }
            CurrentCrowned = false;
            CurrentPiece = EnumPieceType.None;
        }

        public AnimateSkiaSharpGameBoard Animates;
        internal bool CurrentCrowned { get; set; }
        internal CustomBasicList<MoveInfo> CompleteMoveList { get; set; } = new CustomBasicList<MoveInfo>();
        internal CustomBasicList<MoveInfo> CurrentMoveList { get; set; } = new CustomBasicList<MoveInfo>();
        public CustomBasicList<SpaceCP>? SpaceList;
        internal EnumPieceType CurrentPiece { get; set; }

    }
}
