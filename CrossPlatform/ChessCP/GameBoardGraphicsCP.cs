using BasicGameFramework.Attributes;
using BasicGameFramework.DIContainers;
using BasicGameFramework.GameGraphicsCP.CheckersChessHelpers;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using SkiaSharp;
using System.Windows.Input;
namespace ChessCP
{
    [SingletonGame]
    public class GameBoardGraphicsCP : CheckersChessBaseBoard<PieceCP, SpaceCP>
    {
        private readonly ChessMainGameClass _mainGame;
        public GameBoardGraphicsCP(IGamePackageResolver MainContainer) : base(MainContainer)
        {
            _mainGame = MainContainer.Resolve<ChessMainGameClass>();
        }

        public override string TagUsed => "main"; //i think.

        protected override void AfterClearBoard()
        {
            _mainGame!.ThisGlobal!.SpaceList = PrivateSpaceList;
        }

        protected override bool CanHighlight()
        {
            if (_mainGame.SaveRoot!.SpaceHighlighted > 0)
                return true;
            return _mainGame.SaveRoot.PreviousMove.SpaceFrom > 0 && _mainGame.SaveRoot.PreviousMove.SpaceTo > 0;
        }

        protected override bool CanStartPaint()
        {
            return _mainGame.DidChooseColors;
        }

        protected override void ContinueClick(int index, ICommand thisCommand)
        {
            thisCommand.Execute(index);
        }

        protected override EnumGame GetGame()
        {
            return EnumGame.Chess;
        }

        protected override void PossibleAnimations(SKCanvas thisCanvas) //hopefully that simple.
        {
            if (_mainGame.ThisGlobal!.Animates!.AnimationGoing == false)
                return;
            var thisPlayer = _mainGame.PlayerList!.GetWhoPlayer();
            var thisPiece = GetGamePiece(thisPlayer.Color.ToColor(), _mainGame.ThisGlobal.Animates.CurrentLocation);
            thisPiece.ActualHeight = 32;
            thisPiece.ActualWidth = 32;
            thisPiece.WhichPiece = _mainGame.ThisGlobal.CurrentPiece;
            thisPiece.NeedsToClear = false;
            thisPiece.DrawImage(thisCanvas);
        }

        protected override ICommand SpaceCommand()
        {
            return _mainGame.ThisMod!.SpaceCommand!;
        }
    }
}
