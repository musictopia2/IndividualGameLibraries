using BasicGameFrameworkLibrary.BasicEventModels;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.Messenging;
using SkiaSharp.Views.Forms;
using Xamarin.Forms;
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;
using cs = CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.SColorString;
using static BasicGameFrameworkLibrary.StandardImplementations.CrossPlatform.DataClasses.GlobalScreenClass;
using BasicGameFrameworkLibrary.StandardImplementations.CrossPlatform.DataClasses;
using SnakesAndLaddersCP.GraphicsCP;
using SnakesAndLaddersCP.Logic;
using BasicGamingUIXFLibrary.GameGraphics.Base;
using BasicXFControlsAndPages.MVVMFramework.ViewLinkersPlusBinders;
using SnakesAndLaddersCP.ViewModels;
using SkiaSharp;
using SnakesAndLaddersCP.Data;
using System.Windows.Input;

namespace SnakesAndLaddersXF
{
    public class GameboardXF : GraphicsCommand, IHandle<NewTurnEventModel>, IHandle<GamePieceXF>, IHandle<RepaintEventModel>
    {
        private GameBoardGraphicsCP? _privateBoard;
        //private CustomBasicList<GamePieceXF>? _pieceList;
        private EventAggregator? _thisE;
        private SnakesAndLaddersMainGameClass? _mainGame;
        public void Handle(NewTurnEventModel message)
        {
            ThisDraw!.InvalidateSurface();
        }
        public void Handle(GamePieceXF message)
        {
            ThisDraw!.InvalidateSurface();
        }
        private SKCanvasView? _otherElement;
        public void LoadBoard()
        {
            _thisE = Resolve<EventAggregator>();
            _thisE.Subscribe(this);
            _thisE.Subscribe(this, EnumRepaintCategories.FromSkiasharpboard.ToString());
            this.SetName(nameof(SnakesAndLaddersMainViewModel.MakeMoveAsync));

            ThisDraw.PaintSurface += ThisElement_PaintSurface;
            _otherElement = new SKCanvasView();
            _otherElement.PaintSurface += OtherPaint;
            _privateBoard = new GameBoardGraphicsCP();
            if (ScreenUsed == EnumScreen.SmallPhone)
                _privateBoard.HeightWidth = 45;
            else
                _privateBoard.HeightWidth = 115;
            _privateBoard.LoadBoard();
            _mainGame = Resolve<SnakesAndLaddersMainGameClass>();
            HeightRequest = _privateBoard.HeightWidth * 7.5f;
            WidthRequest = _privateBoard.HeightWidth * 7.5;
            Grid grid = new Grid();
            grid.Children.Add(_otherElement);
            grid.Children.Add(ThisDraw);
            Content = grid;
        }
        protected override void SetUpContent()
        {
            
        }
        private void OtherPaint(object sender, SKPaintSurfaceEventArgs e)
        {
            _privateBoard!.PaintBoard(e.Surface.Canvas);
        }

        private void ThisElement_PaintSurface(object? sender, SKPaintSurfaceEventArgs e)
        {
            e.Surface.Canvas.Clear(); //i think.
            var tempList = _mainGame!.PlayerList.ToCustomBasicList();
            tempList.KeepConditionalItems(items => items.SpaceNumber > 0);
            tempList.RemoveSpecificItem(_mainGame.SingleInfo!);
            PieceGraphicsCP piece;
            tempList.ForEach(thisPlayer =>
            {
                piece = GetPiece(thisPlayer);
                piece.DrawImage(e.Surface.Canvas);
            });
            if (_mainGame.SingleInfo!.SpaceNumber > 0)
            {
                piece = GetPiece(_mainGame.SingleInfo);
                piece.DrawImage(e.Surface.Canvas);
            }
        }
        private string GetColor(SnakesAndLaddersPlayerItem player)
        {
            int index = player.Id;
            return index switch
            {
                1 => cs.Blue,
                2 => cs.DeepPink,
                3 => cs.Orange,
                4 => cs.ForestGreen,
                _ => throw new BasicBlankException("No Color Found"),
            };
        }
        private PieceGraphicsCP GetPiece(SnakesAndLaddersPlayerItem player)
        {
            PieceGraphicsCP output = new PieceGraphicsCP();
            output.Location = _privateBoard!.GetBounds(player.SpaceNumber).Location;
            output.Number = player.SpaceNumber;
            output.MainColor = GetColor(player);
            output.ActualHeight = _privateBoard.HeightWidth;
            output.ActualWidth = _privateBoard.HeightWidth;
            output.NeedsToClear = false;
            return output;
        }

        protected override void BeforeProcessClick(ICommand thisCommand, object thisParameter, SKPoint clickPoint)
        {
            int index = _privateBoard!.SpaceClicked(clickPoint.X, clickPoint.Y);
            CommandParameter = index; //so it changes over time for this case.
            base.BeforeProcessClick(thisCommand, thisParameter, clickPoint);
        }
        public void Handle(RepaintEventModel Message)
        {
            ThisDraw!.InvalidateSurface(); //iffy.
        }
    }
}
