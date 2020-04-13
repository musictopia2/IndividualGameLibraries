using System;
using System.Text;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using System.Linq;
using CommonBasicStandardLibraries.BasicDataSettingsAndProcesses;
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;
using CommonBasicStandardLibraries.CollectionClasses;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using fs = CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.JsonSerializers.FileHelpers;
using js = CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.JsonSerializers.NewtonJsonStrings; //just in case i need those 2.
using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.GameGraphicsCP.CheckersChessHelpers;
using ChessCP.Data;
using BasicGameFrameworkLibrary.DIContainers;
using SkiaSharp;
//i think this is the most common things i like to do
namespace ChessCP.Logic
{
    [SingletonGame]
    [AutoReset]
    public class GameBoardGraphicsCP : CheckersChessBaseBoard<PieceCP, SpaceCP>
    {
        private readonly ChessGameContainer _gameContainer;
        public GameBoardGraphicsCP(ChessGameContainer gameContainer) : base(gameContainer.Resolver, gameContainer.Command)
        {
            _gameContainer = gameContainer;
        }

        public override string TagUsed => "main"; //i think.

        protected override void AfterClearBoard()
        {
            _gameContainer.SpaceList = PrivateSpaceList;
        }

        protected override bool CanHighlight()
        {
            if (_gameContainer.SaveRoot!.SpaceHighlighted > 0)
                return true;
            return _gameContainer.SaveRoot.PreviousMove.SpaceFrom > 0 && _gameContainer.SaveRoot.PreviousMove.SpaceTo > 0;
        }

        protected override bool CanStartPaint()
        {
            return true; //hopefully this simple
        }

        

        protected override EnumGame GetGame()
        {
            return EnumGame.Chess;
        }

        protected override void PossibleAnimations(SKCanvas thisCanvas) //hopefully that simple.
        {
            if (_gameContainer.Animates!.AnimationGoing == false)
                return;
            var thisPlayer = _gameContainer.PlayerList!.GetWhoPlayer();
            var thisPiece = GetGamePiece(thisPlayer.Color.ToColor(), _gameContainer.Animates.CurrentLocation);
            thisPiece.ActualHeight = 32;
            thisPiece.ActualWidth = 32;
            thisPiece.WhichPiece = _gameContainer.CurrentPiece;
            thisPiece.NeedsToClear = false;
            thisPiece.DrawImage(thisCanvas);
        }

    }
}