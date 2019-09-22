using BaseGPXPagesAndControlsXF.GameGraphics.GamePieces;
using CommonBasicStandardLibraries.Exceptions;
using MastermindCP;
using Xamarin.Forms;
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;
using static MastermindXF.GlobalXF;
namespace MastermindXF
{
    public class SolutionUI : ContentView, IEndSolution
    {
        private readonly StackLayout _thisStack = new StackLayout();
        private GlobalClass? _thisGlobal;
        public SolutionUI()
        {
            _thisStack.Orientation = StackOrientation.Horizontal;
            Content = _thisStack;
        }

        void IEndSolution.HideSolution()
        {
            _thisStack.Children.Clear();
        }

        void IEndSolution.ShowSolution()
        {
            if (_thisGlobal == null)
                _thisGlobal = Resolve<GlobalClass>(); //can't unit test ui anyways.
            if (_thisGlobal.Solution!.Count != 4)
                throw new BasicBlankException("Must have 4 for the solution");
            if (_thisStack.Children.Count != 0)
                throw new BasicBlankException("Did not previously hide the solution");
            _thisGlobal.Solution.ForEach(thisBead =>
            {
                CirclePieceXF<EnumColorPossibilities> thisControl = new CirclePieceXF<EnumColorPossibilities>();
                thisControl.SetBinding(CirclePieceXF<EnumColorPossibilities>.MainColorProperty, new Binding(nameof(Bead.UIColor)));
                thisControl.BindingContext = thisBead;
                thisControl.HeightRequest = GuessWidthHeight;
                thisControl.WidthRequest = GuessWidthHeight;
                thisControl.NeedsWhiteBorders = true;
                thisControl.CommandParameter = thisBead;
                thisControl.Init();
                _thisStack.Children.Add(thisControl);
            });
        }
    }
}