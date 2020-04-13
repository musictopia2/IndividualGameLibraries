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
using BasicGamingUIWPFLibrary.BasicControls.GameBoards;
using SolitaireBoardGameCP.Data;
using BasicGameFrameworkLibrary.GameBoardCollections;
using System.Windows.Controls;
using System.Windows;
using BasicGamingUIWPFLibrary.GameGraphics.GamePieces;
using System.Windows.Data;
using SolitaireBoardGameCP.ViewModels;
//i think this is the most common things i like to do
namespace SolitaireBoardGameWPF.Views
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
            thisC.Name = nameof(SolitaireBoardGameMainViewModel.MakeMoveAsync); //not sure if my idea works.  hopefully so.
            thisC.SetBinding(CheckerPiecesWPF.MainColorProperty, new Binding(nameof(GameSpace.Color)));
            thisC.SetBinding(CheckerPiecesWPF.HasImageProperty, new Binding(nameof(GameSpace.HasImage)));
            thisC.CommandParameter = thisItem; //try this.
            thisC.Init();
            return thisC;
        }
    }
}
