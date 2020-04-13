using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.GameGraphicsCP.CheckersChessHelpers;
using BasicGameFrameworkLibrary.GameGraphicsCP.GamePieces;
using CheckersCP.Data;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using CommonBasicStandardLibraries.Exceptions;
using SkiaSharp;

namespace CheckersCP.Logic
{
    [SingletonGame]
    [AutoReset]
    public class GameBoardGraphicsCP : CheckersChessBaseBoard<CheckerPiecesCP, SpaceCP>
    {
        private readonly CheckersGameContainer _container;

        public GameBoardGraphicsCP(CheckersGameContainer container) : base(container.Resolver, container.Command)
        {
            _container = container;
        }

        public override string TagUsed => "main"; //i think.

        protected override void AfterClearBoard()
        {
            _container.SpaceList = PrivateSpaceList;
        }

        protected override bool CanHighlight()
        {
            return _container.SaveRoot!.SpaceHighlighted > 0;
        }
        //hopefully will be this simple (?)
        protected override bool CanStartPaint()
        {
            return true; //maybe okay because by the time this runs, it would have already been okay.  if i am wrong, rethink.
        }

        protected override EnumGame GetGame()
        {
            return EnumGame.Checkers;
        }

        protected override void PossibleAnimations(SKCanvas thisCanvas)
        {
            if (_container.Animates!.AnimationGoing == false)
                return;
            var thisPlayer = _container.PlayerList!.GetWhoPlayer();
            var thisPiece = GetGamePiece(thisPlayer.Color.ToColor(), _container.Animates.CurrentLocation);
            if (thisPiece.ActualHeight == 0 || thisPiece.ActualWidth == 0)
                throw new BasicBlankException("The actual height and actualwidth cannot be 0");
            thisPiece.IsCrowned = _container.CurrentCrowned;
            thisPiece.NeedsToClear = false;
            thisPiece.DrawImage(thisCanvas);
        }

    }
}
