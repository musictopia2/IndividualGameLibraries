using BasicGameFramework.BasicEventModels;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.Messenging;
using SkiaSharp.Views.Desktop;
using SkiaSharp.Views.WPF;
using SnakesAndLaddersCP;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;
namespace SnakesAndLaddersWPF
{
    public class GameboardWPF : UserControl, IHandle<NewTurnEventModel>, IHandle<GamePieceWPF>
    {
        private SKElement? _thisElement;
        private GameBoardGraphicsCP? _privateBoard;
        private Canvas? _turnCanvas;
        private Canvas? _otherCanvas;
        private CustomBasicList<GamePieceWPF>? _pieceList;
        private EventAggregator? _thisE;
        private SnakesAndLaddersMainGameClass? _mainGame;
        private SnakesAndLaddersViewModel? _thisMod;
        public void Handle(NewTurnEventModel message)
        {
            NewTurnProcesses();
        }
        public void Handle(GamePieceWPF message)
        {
            ChangePiece(message); // hopefully this works as well since we know what type we are expecting.
        }
        public void UpdateBoard()
        {
            int index = default;
            foreach (var thisItem in _mainGame!.PlayerList!)
            {
                index += 1;
                GamePieceWPF thisGraphics = _pieceList![index - 1];
                thisGraphics.DataContext = null; //clear it out first.
                thisGraphics.DataContext = thisItem; // link to player
            }
            NewTurnProcesses();
        }
        public void LoadBoard()
        {
            _thisE = Resolve<EventAggregator>();
            _thisE.Subscribe(this);
            _thisElement = new SKElement();
            _thisElement.PaintSurface += ThisElement_PaintSurface;
            _thisElement.MouseUp += ThisElement_MouseUp;
            _privateBoard = new GameBoardGraphicsCP();
            _privateBoard.HeightWidth = 50; // i think
            _privateBoard.LoadBoard();
            _turnCanvas = new Canvas();
            _otherCanvas = new Canvas();
            _turnCanvas.IsHitTestVisible = false; //this means won't consider this which is what i want.
            _otherCanvas.IsHitTestVisible = false;
            _mainGame = Resolve<SnakesAndLaddersMainGameClass>();
            _thisMod = Resolve<SnakesAndLaddersViewModel>();
            _pieceList = new CustomBasicList<GamePieceWPF>();
            int index = default;
            foreach (var thisItem in _mainGame.PlayerList!)
            {
                index += 1;
                GamePieceWPF thisGraphics = new GamePieceWPF();
                thisGraphics.Index = index; // this will populate the color
                thisGraphics.DataContext = thisItem; // link to player
                thisGraphics.SetBinding(GamePieceWPF.NumberProperty, new Binding(nameof(SnakesAndLaddersPlayerItem.SpaceNumber)));
                thisGraphics.Width = _privateBoard.HeightWidth;
                thisGraphics.Height = _privateBoard.HeightWidth;
                thisGraphics.Init();
                _pieceList.Add(thisGraphics);
            }
            Grid thisGrid = new Grid();
            thisGrid.Children.Add(_thisElement);
            thisGrid.Children.Add(_otherCanvas);
            thisGrid.Children.Add(_turnCanvas);
            Content = thisGrid;
            Height = _privateBoard.HeightWidth * 10;
            Width = _privateBoard.HeightWidth * 10;
            NewTurnProcesses();
        }
        private void ChangePiece(GamePieceWPF thisPiece)
        {
            if (thisPiece.Number == 0)
                return;// no need to change here because nothing here.
            var bounds = _privateBoard!.GetBounds(thisPiece.Number);
            Canvas.SetTop(thisPiece, bounds.Top);
            Canvas.SetLeft(thisPiece, bounds.Left);
        }
        private GamePieceWPF FindControl(SnakesAndLaddersPlayerItem thisPlayer)
        {
            foreach (var thisPiece in _pieceList!)
            {
                if (thisPiece.DataContext.Equals(thisPlayer) == true)
                    return thisPiece;
            }
            throw new BasicBlankException("Control not found");
        }
        private void NewTurnProcesses()
        {
            if (_mainGame!.SingleInfo == null)
                return; //hopefully i won't regret this.
            _otherCanvas!.Children.Clear();
            _turnCanvas!.Children.Clear();
            var thisList = _mainGame.PlayerList.ToCustomBasicList();
            var otherControl = FindControl(_mainGame.SingleInfo);
            thisList.RemoveSpecificItem(_mainGame.SingleInfo);
            foreach (var thisPlayer in thisList)
            {
                var thisControl = FindControl(thisPlayer);
                _otherCanvas.Children.Add(thisControl);
                ChangePiece(thisControl);
            }
            _turnCanvas.Children.Add(otherControl);
            ChangePiece(otherControl);
        }
        private void ThisElement_MouseUp(object sender, MouseButtonEventArgs e)
        {
            int index;
            var thisPoint = e.GetPosition(_thisElement);
            index = _privateBoard!.SpaceClicked((float)thisPoint.X, (float)thisPoint.Y);
            if (_thisMod!.SpaceClickCommand!.CanExecute(index) == true)
                _thisMod.SpaceClickCommand.Execute(index);
        }
        private void ThisElement_PaintSurface(object? sender, SKPaintSurfaceEventArgs e)
        {
            _privateBoard!.PaintBoard(e.Surface.Canvas);
        }
    }
}