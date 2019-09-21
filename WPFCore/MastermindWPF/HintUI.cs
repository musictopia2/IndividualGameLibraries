using BaseGPXWindowsAndControlsCore.GameGraphics.GamePieces;
using MastermindCP;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using static MastermindWPF.GlobalWPF;
using cs = CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.SColorString;
namespace MastermindWPF
{
    public class HintUI : UserControl
    {
        public static readonly DependencyProperty HowManyCompletelyCorrectProperty = DependencyProperty.Register("HowManyCompletelyCorrect", typeof(int), typeof(HintUI), new FrameworkPropertyMetadata(new PropertyChangedCallback(HowManyCompletelyCorrectPropertyChanged)));

        public int HowManyCompletelyCorrect
        {
            get
            {
                return (int)GetValue(HowManyCompletelyCorrectProperty);
            }
            set
            {
                SetValue(HowManyCompletelyCorrectProperty, value);
            }
        }

        private static void HowManyCompletelyCorrectPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var thisItem = (HintUI)sender;
            thisItem.UpdateUI();
        }

        public static readonly DependencyProperty HowManySemiCorrectProperty = DependencyProperty.Register("HowManySemiCorrect", typeof(int), typeof(HintUI), new FrameworkPropertyMetadata(new PropertyChangedCallback(HowManySemiCorrectPropertyChanged)));

        public int HowManySemiCorrect
        {
            get
            {
                return (int)GetValue(HowManySemiCorrectProperty);
            }
            set
            {
                SetValue(HowManySemiCorrectProperty, value);
            }
        }

        private static void HowManySemiCorrectPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var thisItem = (HintUI)sender;
            thisItem.UpdateUI();
        }
        private void UpdateUI()
        {
            int correctAccount = 0;
            int semiAccount = 0;
            foreach (var firstCon in _thisStack!.Children)
            {
                var thisCon = (CirclePieceWPF<EnumColorPossibilities>)firstCon!;
                if (correctAccount < HowManyCompletelyCorrect)
                {
                    thisCon.MainColor = cs.Black;
                    correctAccount += 1;
                }
                else if (semiAccount < HowManySemiCorrect)
                {
                    thisCon.MainColor = cs.Aqua;
                    semiAccount += 1;
                }
                else
                    thisCon.MainColor = cs.Transparent;// i think
            }
        }

        // has to have 4 parts no matter what.
        private StackPanel? _thisStack;
        public void Init(Guess thisGuess)
        {
            _thisStack = new StackPanel();
            _thisStack.Orientation = Orientation.Horizontal;
            // at first, act like its nothing.
            int x;
            for (x = 1; x <= 4; x++)
            {
                CirclePieceWPF<EnumColorPossibilities> thisCon = new CirclePieceWPF<EnumColorPossibilities>();
                thisCon.NeedsWhiteBorders = true;
                thisCon.Height = GuessWidthHeight;
                thisCon.Width = GuessWidthHeight;
                thisCon.Init();
                _thisStack.Children.Add(thisCon);
            }
            Content = _thisStack;
            SetBinding(HowManyCompletelyCorrectProperty, new Binding(nameof(Guess.HowManyBlacks)));
            SetBinding(HowManySemiCorrectProperty, new Binding(nameof(Guess.HowManyAquas)));
            DataContext = thisGuess;
        }
    }
}