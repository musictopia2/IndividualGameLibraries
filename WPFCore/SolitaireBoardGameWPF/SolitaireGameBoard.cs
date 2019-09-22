using BaseGPXWindowsAndControlsCore.BasicControls.GameBoards;
using BaseGPXWindowsAndControlsCore.GameGraphics.GamePieces;
using BasicGameFramework.GameBoardCollections;
using SolitaireBoardGameCP;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
namespace SolitaireBoardGameWPF
{
    public class SolitaireGameBoard : ImageGameBoard<GameSpace>
    {
        public SolitaireGameBoard()
        {
            CanClearAtEnd = false; //i guess it depends on game.  connect four, had to be true.  this one had to be false.
        }
        protected override bool CanAddControl(IBoardCollection<GameSpace> itemsSource, int row, int column)
        {
            if (column >= 3 && column <= 5)
                return true;
            if (row >= 3 && row <= 5)
                return true;
            return false;
        }
        protected override Control GetControl(GameSpace thisItem, int index)
        {
            CheckerPiecesWPF thisC = new CheckerPiecesWPF();
            thisC.DataContext = thisItem;
            thisC.Margin = new Thickness(0, 0, 5, 0);
            thisC.Height = 80;
            thisC.Width = 80;
            thisC.SetBinding(CheckerPiecesWPF.CommandProperty, GetCommandBinding(nameof(SolitaireBoardGameViewModel.SpaceCommand)));
            thisC.SetBinding(CheckerPiecesWPF.MainColorProperty, new Binding(nameof(GameSpace.Color)));
            thisC.SetBinding(CheckerPiecesWPF.HasImageProperty, new Binding(nameof(GameSpace.HasImage)));
            thisC.CommandParameter = thisItem; //try this.
            thisC.Init();
            return thisC;
        }
    }
}