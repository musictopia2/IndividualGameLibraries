using CommonBasicStandardLibraries.CollectionClasses;
using MastermindCP;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using static BasicControlsAndWindowsCore.Helpers.GridHelper;
namespace MastermindWPF
{
    public class BoardUI : UserControl, IScroll
    {
        private CustomBasicCollection<Guess>? _guessList;

        private ScrollViewer? _thisScroll;

        public static readonly DependencyProperty TopProperty = DependencyProperty.Register("Top", typeof(double), typeof(BoardUI), new PropertyMetadata());
        private Grid? _thisGrid;
        private MastermindViewModel? _thisMod;
        public static void SetTopProperty(DependencyObject item, double Value)
        {
            item.SetValue(TopProperty, Value);
        }

        public static double GetTopProperty(DependencyObject item)
        {
            return (double)item.GetValue(TopProperty);
        }

        public void Init(MastermindViewModel thisMod)
        {
            _thisMod = thisMod;
            _thisGrid = new Grid();
            _guessList = thisMod.Guess1!.GuessList;
            _guessList.CollectionChanged += CollectionChanged;
            AddAutoColumns(_thisGrid, 2);
            _thisScroll = new ScrollViewer();
            _thisScroll.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            _thisScroll.Content = _thisGrid;
            Content = _thisScroll;
            PopulateControls();
        }

        private void CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            PopulateControls();
        }
        private void PopulateControls() //try private instead of public.  hopefully that works.
        {
            _thisGrid!.Children.Clear();
            _thisGrid.RowDefinitions.Clear();
            if (_guessList!.Count == 0)
                return;
            int x = 0;
            AddAutoRows(_thisGrid, _guessList.Count);
            _guessList.ForEach(firstGuess =>
            {
                GuessUI thisUI = new GuessUI();
                AddControlToGrid(_thisGrid, thisUI, x, 0);
                thisUI.Init(firstGuess, _thisMod!);
                HintUI thisHint = new HintUI();
                thisHint.Margin = new Thickness(5, 0, 0, 0);
                AddControlToGrid(_thisGrid, thisHint, x, 1);
                thisHint.Init(firstGuess);
                x++;
            });
        }

        void IScroll.ScrollToGuess(Guess thisGuess)
        {
            if (thisGuess.Equals(_guessList.First()))
                _thisScroll!.ScrollToTop();
            else if (thisGuess.Equals(_guessList.Last()))
                _thisScroll!.ScrollToBottom();
            else
            {
                int index = _guessList!.IndexOf(thisGuess);
                if (index <= 6)
                    _thisScroll!.ScrollToTop();
                else
                    _thisScroll!.ScrollToBottom();
            }
        }
    }
}