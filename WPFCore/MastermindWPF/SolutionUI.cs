using BaseGPXWindowsAndControlsCore.GameGraphics.GamePieces;
using CommonBasicStandardLibraries.Exceptions;
using MastermindCP;
using System.Windows.Controls;
using System.Windows.Data;
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;
using static MastermindWPF.GlobalWPF;
namespace MastermindWPF
{
    public class SolutionUI : UserControl, IEndSolution
    {

        private readonly StackPanel _thisStack = new StackPanel();
        private GlobalClass? _thisGlobal;
        public SolutionUI()
        {
            _thisStack.Orientation = Orientation.Horizontal;
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
                CirclePieceWPF<EnumColorPossibilities> thisControl = new CirclePieceWPF<EnumColorPossibilities>();
                thisControl.SetBinding(CirclePieceWPF<EnumColorPossibilities>.MainColorProperty, new Binding(nameof(Bead.UIColor)));
                thisControl.DataContext = thisBead;
                thisControl.Height = GuessWidthHeight;
                thisControl.Width = GuessWidthHeight;
                thisControl.NeedsWhiteBorders = true;
                thisControl.CommandParameter = thisBead;
                thisControl.Init();
                _thisStack.Children.Add(thisControl);
            });
        }
    }
}