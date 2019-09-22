using BaseGPXPagesAndControlsXF.GameGraphics.GamePieces;
using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.Exceptions;
using MastermindCP;
using Xamarin.Forms;
using static MastermindXF.GlobalXF;
namespace MastermindXF
{
    public class GuessUI : ContentView
    {
        private CustomBasicCollection<Bead>? _thisList;
        private readonly StackLayout _thisStack = new StackLayout();
        private MastermindViewModel? _thisMod;
        private void PopulateControls()
        {
            _thisStack.Children.Clear();
            if (_thisList!.Count == 0)
                return;
            if (_thisList.Count != 4)
                throw new BasicBlankException("Only 4 are supported");
            _thisList.ForEach(thisBead =>
            {
                CirclePieceXF<EnumColorPossibilities> thisControl = new CirclePieceXF<EnumColorPossibilities>();
                thisControl.SetBinding(CirclePieceXF<EnumColorPossibilities>.MainColorProperty, new Binding(nameof(Bead.UIColor)));
                thisControl.BindingContext = thisBead;
                thisControl.HeightRequest = GuessWidthHeight;
                thisControl.WidthRequest = GuessWidthHeight;
                thisControl.NeedsWhiteBorders = true;
                thisControl.CommandParameter = thisBead;
                thisControl.Init();
                Binding newBind = new Binding(nameof(HintChooserViewModel.ChangeMindCommand));
                newBind.Source = _thisMod!.Guess1;
                thisControl.SetBinding(CirclePieceXF<EnumColorPossibilities>.CommandProperty, newBind);
                _thisStack.Children.Add(thisControl);
            });
        }
        public void Init(Guess thisGuess, MastermindViewModel thisMod)
        {
            _thisList = thisGuess.YourBeads;
            _thisList.CollectionChanged += CollectionChanged;
            _thisStack.Orientation = StackOrientation.Horizontal;
            _thisStack.Spacing = 0;
            _thisMod = thisMod;
            Content = _thisStack;
            PopulateControls();
        }

        private void CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            PopulateControls();
        }
    }
}