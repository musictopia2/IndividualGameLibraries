using A8RoundRummyCP;
using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.Exceptions;
using System.Collections.Specialized;
using Xamarin.Forms;
using static BaseGPXPagesAndControlsXF.BasePageProcesses.Pages.SharedPageFunctions;
namespace A8RoundRummyXF
{
    public class RoundUI : ContentView
    {
        private CustomBasicCollection<RoundClass>? _roundList;
        private StackLayout? _thisStack;
        public void Init(A8RoundRummyMainGameClass mainGame)
        {
            _roundList = mainGame.SaveRoot!.RoundList;
            _roundList.CollectionChanged += RoundList_CollectionChanged;
            _thisStack = new StackLayout();
            PopulateList();
            Content = _thisStack;
        }
        private void RoundList_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                PopulateList();
                return;
            }
            if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                if (e.OldStartingIndex != 0)
                    throw new BasicBlankException("Must be 0 for remove.  Otherwise, rethinking is required");
                _thisStack!.Children.RemoveAt(0);
            }
        }
        private void PopulateList()
        {
            _thisStack!.Children.Clear();
            _roundList!.ForEach(thisRound =>
            {
                var thisLabel = GetDefaultLabel();
                thisLabel.FontSize = 20;
                thisLabel.FontAttributes = FontAttributes.Bold;
                thisLabel.Text = thisRound.Description;
                thisLabel.Margin = new Thickness(0, 0, 0, 5);
                _thisStack.Children.Add(thisLabel);
            });
        }
        public void Update(A8RoundRummyMainGameClass mainGame)
        {
            _roundList = mainGame.SaveRoot!.RoundList;
            _roundList.CollectionChanged -= RoundList_CollectionChanged;
            _roundList.CollectionChanged += RoundList_CollectionChanged;
            PopulateList();
        }
    }
}