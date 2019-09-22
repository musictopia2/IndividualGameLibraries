using BaseGPXWindowsAndControlsCore.BasicControls.GameBoards;
using BaseGPXWindowsAndControlsCore.GameGraphics.GamePieces;
using ConnectFourCP;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using cs = CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.SColorString;
namespace ConnectFourWPF
{
    public class GameBoardWPF : ImageGameBoard<SpaceInfoCP>
    {
        protected override float CalculateLeft(float oldWidth, float column)
        {
            if (column == 0 || column == 1)
                return base.CalculateLeft(oldWidth, column);
            var starts = oldWidth * column;
            if (column == 2)
                return starts + 10;
            if (column == 3)
                return starts + 20;
            if (column == 4)
                return starts + 20;
            if (column == 5)
                return starts + 25;
            if (column == 6)
                return starts + 30;
            throw new Exception("Not found");
        }
        protected override void StartInit()
        {
            base.StartInit();
            CanClearAtEnd = false;
        }
        protected override Control GetControl(SpaceInfoCP thisItem, int index)
        {
            CheckerPiecesWPF output = new CheckerPiecesWPF();
            output.DataContext = thisItem;
            output.Margin = new Thickness(0, 0, 5, 0);
            output.Height = 95;
            output.Width = 95;
            output.SetBinding(CheckerPiecesWPF.CommandProperty, GetCommandBinding(nameof(ConnectFourViewModel.ColumnCommand)));
            output.SetBinding(CheckerPiecesWPF.MainColorProperty, new Binding(nameof(SpaceInfoCP.Color)));
            output.SetBinding(CheckerPiecesWPF.HasImageProperty, new Binding(nameof(SpaceInfoCP.HasImage)));
            output.BlankColor = cs.Aqua;
            output.CommandParameter = thisItem;
            output.Init(); // try this
            return output;
        }
    }
}