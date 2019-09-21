using BowlingDiceGameCP;
using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.Exceptions;
using System.Windows;
using System.Windows.Controls;
namespace BowlingDiceGameWPF
{
    public class DiceListWPF : UserControl
    {

        private StackPanel? _thisStack;
        public void SaveBoard(CustomBasicList<SingleDiceInfo> thisList)
        {
            if (thisList.Count != 10)
                throw new BasicBlankException("Must have 10 bowling dice to represent the maximum score is 10");
            int x = 0;
            foreach (var thisInfo in thisList)
            {
                var firsts = _thisStack!.Children[x];
                var thisGraphics = (SingleDiceWPF)firsts;
                thisGraphics.DataContext = thisInfo;
                x += 1;
            }
        }
        public void LoadBoard(CustomBasicList<SingleDiceInfo> thisList)
        {
            if (thisList.Count != 10)
                throw new BasicBlankException("Must have 10 bowling dice to represent the maximum score is 10");
            _thisStack = new StackPanel();
            _thisStack.Orientation = Orientation.Horizontal;
            _thisStack.Margin = new Thickness(5, 5, 5, 5);
            foreach (var thisInfo in thisList)
            {
                SingleDiceWPF thisGraphics = new SingleDiceWPF();
                thisGraphics.DataContext = thisInfo;
                thisGraphics.Width = 80;
                thisGraphics.Height = 80;
                thisGraphics.SetBinding(SingleDiceWPF.DidHitProperty, nameof(SingleDiceInfo.DidHit));
                thisGraphics.Margin = new Thickness(0, 0, 10, 0);
                _thisStack.Children.Add(thisGraphics);
                thisGraphics.Init();
            }
            Content = _thisStack;
        }
    }
}
