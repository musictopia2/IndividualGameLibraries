using BasicGameFramework.BasicEventModels;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.Messenging;
using SkiaSharp.Views.Forms;
using SnakesAndLaddersCP;
using Xamarin.Forms;
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;
using cs = CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.SColorString;
using static BasicGameFramework.StandardImplementations.CrossPlatform.DataClasses.GlobalScreenClass;
using BasicGameFramework.StandardImplementations.CrossPlatform.DataClasses;

namespace SnakesAndLaddersXF
{
    public class GameboardXF : ContentView, IHandle<NewTurnEventModel>, IHandle<GamePieceXF>, IHandle<RepaintEventModel>
    {
        private SKCanvasView? _thisElement;
        private GameBoardGraphicsCP? _privateBoard;
        //private CustomBasicList<GamePieceXF>? _pieceList;
        private EventAggregator? _thisE;
        private SnakesAndLaddersMainGameClass? _mainGame;
        private SnakesAndLaddersViewModel? _thisMod;
        public void Handle(NewTurnEventModel message)
        {
            _thisElement!.InvalidateSurface();
        }
        public void Handle(GamePieceXF message)
        {
            _thisElement!.InvalidateSurface();
        }
        //public void UpdateBoard()
        //{
        //    int index = default;
        //    foreach (var thisItem in _mainGame!.PlayerList!)
        //    {
        //        index += 1;
        //        GamePieceXF thisGraphics = _pieceList![index - 1];
        //        thisGraphics.BindingContext = null; //clear it out first.
        //        thisGraphics.BindingContext = thisItem; // link to player
        //    }
        //    _thisElement!.InvalidateSurface();
        //}
        private SKCanvasView? _otherElement;
        public void LoadBoard()
        {
            _thisE = Resolve<EventAggregator>();
            _thisE.Subscribe(this);
            _thisE.Subscribe(this, EnumRepaintCategories.fromskiasharpboard.ToString());
            _thisElement = new SKCanvasView();
            _thisElement.EnableTouchEvents = true;
            _thisElement.Touch += ElementTouch;
            _thisElement.PaintSurface += ThisElement_PaintSurface;
            _otherElement = new SKCanvasView();
            _otherElement.PaintSurface += OtherPaint;
            _privateBoard = new GameBoardGraphicsCP();
            if (ScreenUsed == EnumScreen.SmallPhone)
                _privateBoard.HeightWidth = 45;
            else
                _privateBoard.HeightWidth = 115;
            _privateBoard.LoadBoard();
            _mainGame = Resolve<SnakesAndLaddersMainGameClass>();
            _thisMod = Resolve<SnakesAndLaddersViewModel>();
            //_pieceList = new CustomBasicList<GamePieceXF>();
            //int index = default;
            //foreach (var thisItem in _mainGame.PlayerList!)
            //{
            //    index += 1;
            //    GamePieceXF thisGraphics = new GamePieceXF();
            //    thisGraphics.Index = index; // this will populate the color
            //    thisGraphics.BindingContext = thisItem; // link to player
            //    thisGraphics.SetBinding(GamePieceXF.NumberProperty, new Binding(nameof(SnakesAndLaddersPlayerItem.SpaceNumber)));
            //    thisGraphics.WidthRequest = _privateBoard.HeightWidth;
            //    thisGraphics.HeightRequest = _privateBoard.HeightWidth;
            //    thisGraphics.Init();
            //    _pieceList.Add(thisGraphics);
            //}
            HeightRequest = _privateBoard.HeightWidth * 7.5f;
            WidthRequest = _privateBoard.HeightWidth * 7.5;
            Grid grid = new Grid();
            grid.Children.Add(_otherElement);
            grid.Children.Add(_thisElement);
            Content = grid;
        }

        private void OtherPaint(object sender, SKPaintSurfaceEventArgs e)
        {
            _privateBoard!.PaintBoard(e.Surface.Canvas);
        }

        private void ElementTouch(object sender, SKTouchEventArgs e)
        {
            int index;
            var thisPoint = e.Location;
            index = _privateBoard!.SpaceClicked((float)thisPoint.X, (float)thisPoint.Y);
            if (_thisMod!.SpaceClickCommand!.CanExecute(index) == true)
                _thisMod.SpaceClickCommand.Execute(index);
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
        public void Handle(RepaintEventModel Message)
        {
            _thisElement!.InvalidateSurface();
        }
    }
}
