using BasicControlsAndWindowsCore.Helpers;
using BowlingDiceGameCP.Data;
using CommonBasicStandardLibraries.Exceptions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
namespace BowlingDiceGameWPF
{
    public class CompleteFrameWPF : UserControl
    {
        private Grid? _thisGrid;
        private TextBlock FindLabel(int row, int column)
        {
            int x = 0;
            int y = 0;
            foreach (var thisCon in _thisGrid!.Children)
            {
                x += 1;
                if (x > 2)
                {
                    if (Grid.GetColumn((UIElement)thisCon!) == column && Grid.GetRow((UIElement)thisCon!) == row)
                    {
                        y += 1;
                        if (y == 2)
                            return (TextBlock)thisCon!;
                    }
                }
            }
            throw new BasicBlankException("No label found for row " + row + " and column " + column);
        }
        public void SavedFrame(FrameInfoCP thisFrame)
        {
            var thisFirst = _thisGrid!.Children[1];
            var thisLabel = (TextBlock)thisFirst; // this should be fine.
            thisLabel.DataContext = thisFrame;
            for (var x = 1; x <= 3; x++)
            {
                thisLabel = FindLabel(0, x - 1);
                thisLabel.DataContext = null; //this may have to be done (?)
                thisLabel.DataContext = thisFrame.SectionList[x];
            }
        }
        public void LoadFrame(FrameInfoCP thisFrame)
        {
            if (thisFrame.SectionList.Count != 3)
                throw new BasicBlankException("Must have 3 sections, not " + thisFrame.SectionList.Count);
            _thisGrid = new Grid();
            int x;
            for (x = 1; x <= 3; x++)
            {
                GridHelper.AddLeftOverColumn(_thisGrid, 1);
                if (x < 3)
                    GridHelper.AddLeftOverRow(_thisGrid, 1);// for equal
            }
            BlankBowlingBorderWPF mainOne = new BlankBowlingBorderWPF();
            _thisGrid.Children.Add(mainOne);
            Grid.SetRowSpan(mainOne, 2);
            Grid.SetColumnSpan(mainOne, 3);
            TextBlock thisLabel = new TextBlock();
            thisLabel.DataContext = thisFrame;
            Binding thisBind = new Binding(nameof(FrameInfoCP.Score));
            var thisCon = new BowlingConverterWPF();
            thisBind.Converter = thisCon;
            thisLabel.Foreground = Brushes.White;
            thisLabel.SetBinding(TextBlock.TextProperty, thisBind);
            GridHelper.AddControlToGrid(_thisGrid, thisLabel, 1, 0);
            Grid.SetColumnSpan(thisLabel, 3); // use all 3 columns
            thisLabel.HorizontalAlignment = HorizontalAlignment.Center;
            int SectionSize = 20; // trial and error
            for (x = 1; x <= 3; x++)
            {
                BlankBowlingBorderWPF thisBlank = new BlankBowlingBorderWPF();
                thisBlank.Width = SectionSize;
                GridHelper.AddControlToGrid(_thisGrid, thisBlank, 0, x - 1);
                thisLabel = new TextBlock();
                thisLabel.Foreground = Brushes.White;
                thisLabel.HorizontalAlignment = HorizontalAlignment.Center;
                thisLabel.SetBinding(TextBlock.TextProperty, new Binding(nameof(SectionInfoCP.Score)));
                thisLabel.DataContext = thisFrame.SectionList[x]; // not 0 based because of dictionary.  the key starts with 1
                GridHelper.AddControlToGrid(_thisGrid, thisLabel, 0, x - 1);
            }
            Content = _thisGrid;
        }
    }
}