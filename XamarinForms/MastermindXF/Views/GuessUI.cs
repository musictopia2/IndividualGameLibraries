using BasicGamingUIXFLibrary.GameGraphics.GamePieces;
using BasicGamingUIXFLibrary.Helpers;
using BasicXFControlsAndPages.MVVMFramework.ViewLinkersPlusBinders;
using CommonBasicStandardLibraries.Exceptions;
using MastermindCP.Data;
using MastermindCP.ViewModels;
using Xamarin.Forms;
using static MastermindXF.GlobalXF;
namespace MastermindXF.Views
{
    public class GuessUI : ContentView
    {
        private readonly StackLayout _thisStack = new StackLayout();
        public void PopulateControls(Guess guess)
        {
            _thisStack.Children.Clear();
            if (guess.YourBeads.Count == 0)
            {
                return; //maybe you don't need it at that moment (?)
            }
            if (guess.YourBeads.Count != 4)
            {
                throw new BasicBlankException("Only 4 are supported");
            }
            guess.YourBeads.ForEach(bead =>
            {
                //we may have to manually add to the special list (?)
                CirclePieceXF<EnumColorPossibilities> thisControl = new CirclePieceXF<EnumColorPossibilities>();
                thisControl.SetBinding(CirclePieceXF<EnumColorPossibilities>.MainColorProperty, new Binding(nameof(Bead.UIColor)));
                thisControl.BindingContext = bead;
                thisControl.HeightRequest = GuessWidthHeight;
                thisControl.WidthRequest = GuessWidthHeight;
                thisControl.NeedsWhiteBorders = true;
                thisControl.CommandParameter = bead;
                thisControl.Init();
                thisControl.SetName(nameof(GameBoardViewModel.ChangeMind));
                GamePackageViewModelBinder.ManuelElements.Add(thisControl); //try this way.
                _thisStack.Children.Add(thisControl);
            });
        }

        public void Init(Guess guess)
        {
            _thisStack.Orientation = StackOrientation.Horizontal;
            Content = _thisStack;
            PopulateControls(guess);
        }
    }
}