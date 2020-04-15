using BasicGamingUIXFLibrary.GameGraphics.GamePieces;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using MastermindCP.Data;
using MastermindCP.Logic;
using System.Threading.Tasks;
using Xamarin.Forms;
using static MastermindXF.GlobalXF;
namespace MastermindXF.Views
{
    public class SolutionView : ContentView, IUIView
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
            StackLayout stack = new StackLayout()
            {
                Orientation = StackOrientation.Horizontal
            };
            //this can be larger because nothing else is showing now at this point.
            global.Solution.ForEach(bead =>
            {
                CirclePieceXF<EnumColorPossibilities> thisControl = new CirclePieceXF<EnumColorPossibilities>();
                thisControl.SetBinding(CirclePieceXF<EnumColorPossibilities>.MainColorProperty, new Binding(nameof(Bead.UIColor)));
                thisControl.BindingContext = bead;
                thisControl.HeightRequest = GuessWidthHeight;
                thisControl.WidthRequest = GuessWidthHeight; //can experiment this time.
                thisControl.NeedsWhiteBorders = true;
                thisControl.Init();
                stack.Children.Add(thisControl);
            });
            Content = stack;
        }
        object IUIView.DataContext
        {
            get => BindingContext;
            set => BindingContext = value;
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
