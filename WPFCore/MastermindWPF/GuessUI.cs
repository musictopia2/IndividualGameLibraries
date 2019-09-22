using BaseGPXWindowsAndControlsCore.GameGraphics.GamePieces;
using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.Exceptions;
using MastermindCP;
using System.Windows.Controls;
using System.Windows.Data;
using static MastermindWPF.GlobalWPF;
namespace MastermindWPF
{
    public class GuessUI : UserControl
    {
        private CustomBasicCollection<Bead>? _thisList;
        private readonly StackPanel _thisStack = new StackPanel();
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
                CirclePieceWPF<EnumColorPossibilities> thisControl = new CirclePieceWPF<EnumColorPossibilities>();
                thisControl.SetBinding(CirclePieceWPF<EnumColorPossibilities>.MainColorProperty, new Binding(nameof(Bead.UIColor)));
                thisControl.DataContext = thisBead;
                thisControl.Height = GuessWidthHeight;
                thisControl.Width = GuessWidthHeight;
                thisControl.NeedsWhiteBorders = true;
                thisControl.CommandParameter = thisBead;
                thisControl.Init();
                Binding newBind = new Binding(nameof(HintChooserViewModel.ChangeMindCommand));
                newBind.Source = _thisMod!.Guess1;
                thisControl.SetBinding(CirclePieceWPF<EnumColorPossibilities>.CommandProperty, newBind);
                _thisStack.Children.Add(thisControl);
            });
        }
        public void Init(Guess thisGuess, MastermindViewModel thisMod)
        {
            _thisList = thisGuess.YourBeads;
            _thisList.CollectionChanged += CollectionChanged;
            _thisStack.Orientation = Orientation.Horizontal;
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