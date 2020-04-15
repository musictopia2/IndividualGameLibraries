using BasicGameFrameworkLibrary.GameBoardCollections;
using BasicGameFrameworkLibrary.StandardImplementations.CrossPlatform.DataClasses;
using BasicGamingUIXFLibrary.BasicControls.GameBoards;
using BasicGamingUIXFLibrary.GameGraphics.GamePieces;
using BasicXFControlsAndPages.MVVMFramework.ViewLinkersPlusBinders;
using SolitaireBoardGameCP.Data;
using SolitaireBoardGameCP.ViewModels;
using Xamarin.Forms;
using static BasicGameFrameworkLibrary.StandardImplementations.CrossPlatform.DataClasses.GlobalScreenClass;

namespace SolitaireBoardGameXF.Views
{
    public class SolitaireGameBoard : ImageGameBoardXF<GameSpace>
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
        protected override View GetControl(GameSpace thisItem, int index)
        {
            CheckerPiecesXF thisC = new CheckerPiecesXF();
            thisC.BindingContext = thisItem;
            thisC.Margin = new Thickness(0, 0, 5, 0);
            if (ScreenUsed == EnumScreen.SmallPhone)
            {
                thisC.HeightRequest = 35;
                thisC.WidthRequest = 35;
            }
            else if (ScreenUsed == EnumScreen.SmallTablet)
            {
                thisC.HeightRequest = 70;
                thisC.WidthRequest = 70;
            }
            else
            {
                thisC.HeightRequest = 100;
                thisC.WidthRequest = 100;
            }
            thisC.SetName(nameof(SolitaireBoardGameMainViewModel.MakeMoveAsync)); //not sure if my idea works.  hopefully so.
            thisC.SetBinding(CheckerPiecesXF.MainColorProperty, new Binding(nameof(GameSpace.Color)));
            thisC.SetBinding(CheckerPiecesXF.HasImageProperty, new Binding(nameof(GameSpace.HasImage)));
            thisC.CommandParameter = thisItem; //try this.
            thisC.Init();
            return thisC;
        }
    }
}