using A8RoundRummyCP.Data;
using A8RoundRummyCP.Logic;
using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.Exceptions;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using static BasicGamingUIWPFLibrary.Helpers.SharedUIFunctions; //this usually will be used too.

namespace A8RoundRummyWPF
{
    public class RoundUI : UserControl
    {
        private CustomBasicCollection<RoundClass>? _roundList;
        private StackPanel? _thisStack;
        public void Init(A8RoundRummyGameContainer gameContainer)
        {
            _roundList = gameContainer.SaveRoot!.RoundList;
            _roundList.CollectionChanged += RoundList_CollectionChanged;
            _thisStack = new StackPanel();
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
                thisLabel.FontSize = 30;
                thisLabel.FontWeight = FontWeights.Bold;
                thisLabel.Text = thisRound.Description;
                thisLabel.Margin = new Thickness(0, 0, 0, 20);
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
