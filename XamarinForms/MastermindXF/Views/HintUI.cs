using BasicGamingUIXFLibrary.GameGraphics.GamePieces;
using MastermindCP.Data;
using Xamarin.Forms;
using static MastermindXF.GlobalXF;
using cs = CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.SColorString;
namespace MastermindXF
{
    public class HintUI : ContentView
    {


        public static readonly BindableProperty HowManyCompletelyCorrectProperty = BindableProperty.Create(propertyName: "HowManyCompletelyCorrect", returnType: typeof(int), declaringType: typeof(HintUI), defaultValue: 0, defaultBindingMode: BindingMode.TwoWay, propertyChanged: HintsChanged);
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
        private static void HintsChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var thisItem = (HintUI)bindable;
            thisItem.UpdateUI();
        }

        public static readonly BindableProperty HowManySemiCorrectProperty = BindableProperty.Create(propertyName: "HowManySemiCorrect", returnType: typeof(int), declaringType: typeof(HintUI), defaultValue: 0, defaultBindingMode: BindingMode.TwoWay, propertyChanged: HintsChanged);
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
        private void UpdateUI()
        {
            int correctAccount = 0;
            int semiAccount = 0;
            foreach (var firstCon in _thisStack!.Children)
            {
                var thisCon = (CirclePieceXF<EnumColorPossibilities>)firstCon!;
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
        private StackLayout? _thisStack;
        public void Init(Guess thisGuess)
        {
            _thisStack = new StackLayout();
            _thisStack.Orientation = StackOrientation.Horizontal;
            _thisStack.Spacing = 0;
            // at first, act like its nothing.
            int x;
            for (x = 1; x <= 4; x++)
            {
                CirclePieceXF<EnumColorPossibilities> thisCon = new CirclePieceXF<EnumColorPossibilities>();
                thisCon.NeedsWhiteBorders = true;
                thisCon.HeightRequest = GuessWidthHeight;
                thisCon.WidthRequest = GuessWidthHeight;
                thisCon.Init();
                _thisStack.Children.Add(thisCon);
            }
            Content = _thisStack;
            SetBinding(HowManyCompletelyCorrectProperty, new Binding(nameof(Guess.HowManyBlacks)));
            SetBinding(HowManySemiCorrectProperty, new Binding(nameof(Guess.HowManyAquas)));
            BindingContext = thisGuess;
        }
    }
}