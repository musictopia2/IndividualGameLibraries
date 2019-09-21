using BaseGPXWindowsAndControlsCore.BasicControls.GameBoards;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using XPuzzleCP;
namespace XPuzzleWPF
{
    public class XPuzzleGameBoard : BasicGameBoard<XPuzzleSpaceInfo>
    {
        protected override void StartInit()
        {
            Padding = new Thickness(5, 5, 5, 5);
        }
        protected override Control GetControl(XPuzzleSpaceInfo thisItem, int index)
        {
            Button thisBut = new Button();
            thisBut.Height = 190;
            thisBut.Width = 190;
            thisBut.DataContext = thisItem;
            thisBut.SetBinding(ContentProperty, new Binding(nameof(XPuzzleSpaceInfo.Text)));
            thisBut.SetBinding(BackgroundProperty, GetColorBinding(nameof(XPuzzleSpaceInfo.Color)));
            thisBut.SetBinding(System.Windows.Controls.Primitives.ButtonBase.CommandProperty, GetCommandBinding(nameof(XPuzzleViewModel.SpaceCommand)));
            thisBut.BorderThickness = new Thickness(2, 2, 2, 2);
            thisBut.BorderBrush = Brushes.White;
            thisBut.FontSize = 150;
            thisBut.Foreground = Brushes.White;
            thisBut.SetBinding(Button.CommandParameterProperty, new Binding(".")); // try this
            return thisBut;
        }
    }
}