using BasicGamingUIWPFLibrary.GameGraphics.GamePieces;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using MastermindCP.Data;
using MastermindCP.Logic;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using System.Windows.Controls;
using System.Windows.Data;
//i think this is the most common things i like to do
namespace MastermindWPF.Views
{
    public class SolutionView : UserControl, IUIView
    {

        public SolutionView(GlobalClass global)
        {
            if (global.Solution == null)
            {
                throw new BasicBlankException("No solution to even show");
            }
            if (global.Solution.Count != 4)
            {
                throw new BasicBlankException("Must have 4 for the solution");
            }
            StackPanel stack = new StackPanel()
            {
                Orientation = Orientation.Horizontal
            };
            //this can be larger because nothing else is showing now at this point.
            global.Solution.ForEach(bead =>
            {
                CirclePieceWPF<EnumColorPossibilities> thisControl = new CirclePieceWPF<EnumColorPossibilities>();
                thisControl.SetBinding(CirclePieceWPF<EnumColorPossibilities>.MainColorProperty, new Binding(nameof(Bead.UIColor)));
                thisControl.DataContext = bead;
                thisControl.Height = 250;
                thisControl.Width = 250; //can experiment this time.
                thisControl.NeedsWhiteBorders = true;
                //thisControl.CommandParameter = bead;
                thisControl.Init();
                stack.Children.Add(thisControl);
            });
            Content = stack;
        }

        Task IUIView.TryActivateAsync()
        {
            return Task.CompletedTask;
        }

        Task IUIView.TryCloseAsync()
        {
            return Task.CompletedTask;
        }
    }
}
