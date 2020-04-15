using BasicXFControlsAndPages.Helpers;
using BowlingDiceGameCP.Data;
using CommonBasicStandardLibraries.Exceptions;
using Xamarin.Forms;
namespace BowlingDiceGameXF
{
    public class CompleteFrameXF : ContentView
    {
        private Grid? _thisGrid;
        private Label FindLabel(int row, int column)
        {
            int x = 0;
            int y = 0;
            foreach (var thisCon in _thisGrid!.Children)
            {
                x += 1;
                if (x > 2)
                {
                    if (Grid.GetColumn(thisCon!) == column && Grid.GetRow(thisCon!) == row)
                    {
                        y += 1;
                        if (y == 2)
                            return (Label)thisCon!;
                    }
                }
            }
            throw new BasicBlankException("No label found for row " + row + " and column " + column);
        }
        public void SavedFrame(FrameInfoCP thisFrame)
        {
            var thisFirst = _thisGrid!.Children[1];
            var thisLabel = (Label)thisFirst; // this should be fine.
            thisLabel.BindingContext = thisFrame;
            for (var x = 1; x <= 3; x++)
            {
                thisLabel = FindLabel(0, x - 1);
                thisLabel.BindingContext = null; //this may have to be done (?)
                thisLabel.BindingContext = thisFrame.SectionList[x];
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
            BlankBowlingBorderXF mainOne = new BlankBowlingBorderXF();
            _thisGrid.Children.Add(mainOne);
            Grid.SetRowSpan(mainOne, 2);
            Grid.SetColumnSpan(mainOne, 3);
            Label thisLabel = new Label();
            thisLabel.BindingContext = thisFrame;
            Binding thisBind = new Binding(nameof(FrameInfoCP.Score));
            var thisCon = new BowlingConverterXF();
            thisBind.Converter = thisCon;
            thisLabel.TextColor = Color.White;
            thisLabel.SetBinding(Label.TextProperty, thisBind);
            thisLabel.FontSize = 10;
            GridHelper.AddControlToGrid(_thisGrid, thisLabel, 1, 0);
            Grid.SetColumnSpan(thisLabel, 3); // use all 3 columns
            thisLabel.HorizontalOptions = LayoutOptions.Center;
            int SectionSize = 20; // trial and error
            for (x = 1; x <= 3; x++)
            {
                BlankBowlingBorderXF thisBlank = new BlankBowlingBorderXF();
                thisBlank.WidthRequest = SectionSize;
                thisBlank.HeightRequest = 13;
                GridHelper.AddControlToGrid(_thisGrid, thisBlank, 0, x - 1);
                thisLabel = new Label();
                thisLabel.TextColor = Color.White;
                thisLabel.HorizontalOptions = LayoutOptions.Center;
                thisLabel.FontSize = 10;
                thisLabel.HeightRequest = 13;
                thisLabel.SetBinding(Label.TextProperty, new Binding(nameof(SectionInfoCP.Score)));
                thisLabel.BindingContext = thisFrame.SectionList[x]; // not 0 based because of dictionary.  the key starts with 1
                GridHelper.AddControlToGrid(_thisGrid, thisLabel, 0, x - 1);
            }
            Content = _thisGrid;
        }
    }
}