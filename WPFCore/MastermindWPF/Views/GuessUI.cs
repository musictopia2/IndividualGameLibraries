using BasicGamingUIWPFLibrary.GameGraphics.GamePieces;
using BasicGamingUIWPFLibrary.Helpers;
using CommonBasicStandardLibraries.Exceptions;
using MastermindCP.Data;
using MastermindCP.ViewModels;
using System.Reflection;
using System.Windows.Controls;
using System.Windows.Data;
using static MastermindWPF.GlobalWPF;
namespace MastermindWPF.Views
{
    public class GuessUI : UserControl
    {

        private readonly StackPanel _thisStack = new StackPanel();
        public void PopulateControls(Guess guess)
        {
            _thisStack.Children.Clear();
            if (guess.YourBeads.Count == 0)
            {
                return; //maybe you don't need it at that moment (?)
                //throw new BasicBlankException("You need the 4.  Otherwise don't know anymore");
            }
            if (guess.YourBeads.Count != 4)
            {
                throw new BasicBlankException("Only 4 are supported");
            }
            guess.YourBeads.ForEach(bead =>
            {
                //we may have to manually add to the special list (?)
                CirclePieceWPF<EnumColorPossibilities> thisControl = new CirclePieceWPF<EnumColorPossibilities>();
                thisControl.SetBinding(CirclePieceWPF<EnumColorPossibilities>.MainColorProperty, new Binding(nameof(Bead.UIColor)));
                thisControl.DataContext = bead;
                thisControl.Height = GuessWidthHeight;
                thisControl.Width = GuessWidthHeight;
                thisControl.NeedsWhiteBorders = true;
                thisControl.CommandParameter = bead;
                thisControl.Init();
                
                
                //try to manually hook up this time
                //thisControl.Command = 


                thisControl.Name = nameof(GameBoardViewModel.ChangeMind);
                GamePackageViewModelBinder.ManuelElements.Add(thisControl); //try this way.
                //Binding newBind = new Binding(nameof(HintChooserViewModel.ChangeMindCommand));
                //newBind.Source = _thisMod!.Guess1;
                //thisControl.SetBinding(CirclePieceWPF<EnumColorPossibilities>.CommandProperty, newBind);
                _thisStack.Children.Add(thisControl);
            });
        }

        //you have to manually populate controls.
        public void Init(Guess guess)
        {
            //_thisList = thisGuess.YourBeads;
            //_thisList.CollectionChanged += CollectionChanged;
            _thisStack.Orientation = Orientation.Horizontal;
            //_thisMod = thisMod;
            Content = _thisStack;
            PopulateControls(guess);
        }
    }
}
