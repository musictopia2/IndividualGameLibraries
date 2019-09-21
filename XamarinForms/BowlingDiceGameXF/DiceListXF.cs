using BowlingDiceGameCP;
using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.Exceptions;
using Xamarin.Forms;
namespace BowlingDiceGameXF
{
    public class DiceListXF : ContentView
    {
        private StackLayout? _thisStack;
        public void SaveBoard(CustomBasicList<SingleDiceInfo> thisList)
        {
            if (thisList.Count != 10)
                throw new BasicBlankException("Must have 10 bowling dice to represent the maximum score is 10");
            int x = 0;
            foreach (var thisInfo in thisList)
            {
                var firsts = _thisStack!.Children[x];
                var thisGraphics = (SingleDiceXF)firsts;
                thisGraphics.BindingContext = thisInfo;
                x += 1;
            }
        }
        public void LoadBoard(CustomBasicList<SingleDiceInfo> thisList)
        {
            if (thisList.Count != 10)
                throw new BasicBlankException("Must have 10 bowling dice to represent the maximum score is 10");
            _thisStack = new StackLayout();
            _thisStack.Orientation = StackOrientation.Horizontal;
            _thisStack.Margin = new Thickness(5, 5, 5, 5);
            foreach (var thisInfo in thisList)
            {
                SingleDiceXF thisGraphics = new SingleDiceXF();
                thisGraphics.BindingContext = thisInfo;
                thisGraphics.WidthRequest = 40;
                thisGraphics.HeightRequest = 40;
                thisGraphics.SetBinding(SingleDiceXF.DidHitProperty, nameof(SingleDiceInfo.DidHit));
                thisGraphics.Margin = new Thickness(0, 0, 10, 0);
                _thisStack.Children.Add(thisGraphics);
                thisGraphics.Init();
            }
            Content = _thisStack;
        }
    }
}