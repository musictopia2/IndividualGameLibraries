using BasicGamingUIXFLibrary.Helpers;
using BasicXFControlsAndPages.Helpers;
using CommonBasicStandardLibraries.CollectionClasses;
using MinesweeperCP.Logic;
using MinesweeperXF.Views;
using System.Threading.Tasks;
using Xamarin.Forms;
namespace MinesweeperXF
{
    public class GameboardXF : ContentView
    {
        private readonly Grid _thisGrid;
        private readonly MinesweeperMainGameClass _gameBoard;
        public GameboardXF(MinesweeperMainGameClass gameBoard)
        {

            _thisGrid = new Grid();
            int x;
            for (x = 1; x <= 9; x++)
            {
                GridHelper.AddLeftOverColumn(_thisGrid, 1);
                GridHelper.AddLeftOverRow(_thisGrid, 1); // to make them even.
            }
            Content = _thisGrid;
            _gameBoard = gameBoard;
        }

        private readonly CustomBasicList<MineSquareXF> _mines = new CustomBasicList<MineSquareXF>();

        public void Init()
        {
            var thisList = _gameBoard.GetSquares();
            _thisGrid.Children.Clear();
            GamePackageViewModelBinder.ManuelElements.Clear(); //most likely has to do manually here too
            foreach (var thisSquare in thisList)
            {
                MineSquareXF thisGraphics = new MineSquareXF(thisSquare);
                thisGraphics.HorizontalOptions = LayoutOptions.Fill;
                thisGraphics.VerticalOptions = LayoutOptions.Fill;
                _mines.Add(thisGraphics);

                GamePackageViewModelBinder.ManuelElements.Add(thisGraphics);

                GridHelper.AddControlToGrid(_thisGrid, thisGraphics, thisSquare.Row - 1, thisSquare.Column - 1);
            }
            //probably after all is done recheck the bindings.
        }
        public async Task StartUpAsync()
        {
            await _mines.ForEachAsync(async x =>
            {
                await Task.CompletedTask;
                x.StartUp();
            });
        }


    }
}