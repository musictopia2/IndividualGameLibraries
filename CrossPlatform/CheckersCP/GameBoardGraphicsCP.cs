using BasicGameFramework.Attributes;
using BasicGameFramework.DIContainers;
using BasicGameFramework.GameGraphicsCP.CheckersChessHelpers;
using BasicGameFramework.GameGraphicsCP.GamePieces;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using CommonBasicStandardLibraries.Exceptions;
using SkiaSharp;
using System.Windows.Input;
namespace CheckersCP
{
    [SingletonGame]
    public class GameBoardGraphicsCP : CheckersChessBaseBoard<CheckerPiecesCP, SpaceCP>
    {
        private readonly CheckersMainGameClass _mainGame;
        public GameBoardGraphicsCP(IGamePackageResolver MainContainer) : base(MainContainer)
        {
            _mainGame = MainContainer.Resolve<CheckersMainGameClass>();
        }

        public override string TagUsed => "main"; //i think.

        protected override void AfterClearBoard()
        {
            _mainGame!.ThisGlobal!.SpaceList = PrivateSpaceList;
        }

        protected override bool CanHighlight()
        {
            return _mainGame!.SaveRoot!.SpaceHighlighted > 0;
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
            return EnumGame.Checkers;
        }

        protected override void PossibleAnimations(SKCanvas thisCanvas)
        {
            if (_mainGame!.ThisGlobal!.Animates!.AnimationGoing == false)
                return;
            var thisPlayer = _mainGame.PlayerList!.GetWhoPlayer();
            var thisPiece = GetGamePiece(thisPlayer.Color.ToColor(), _mainGame.ThisGlobal.Animates.CurrentLocation);
            if (thisPiece.ActualHeight == 0 || thisPiece.ActualWidth == 0)
                throw new BasicBlankException("The actual height and actualwidth cannot be 0");
            thisPiece.IsCrowned = _mainGame.ThisGlobal.CurrentCrowned;
            thisPiece.NeedsToClear = false;
            thisPiece.DrawImage(thisCanvas);
        }
        protected override ICommand SpaceCommand()
        {
            return _mainGame!.ThisMod!.SpaceCommand!;
        }
    }
}