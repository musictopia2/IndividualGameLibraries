using CommonBasicStandardLibraries.CollectionClasses;
using RummyDiceCP;
using System;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
namespace RummyDiceWPF
{
    public class RummyDiceListWPF : UserControl
    {
        private RummyBoardCP? _thisMod;
        private CustomBasicCollection<RummyDiceInfo>? _diceList;
        private StackPanel? _thisStack;
        private RummyDiceGraphicsWPF? FindControl(RummyDiceInfo thisDice)
        {
            foreach (var thisCon in _thisStack!.Children)
            {
                var deck = (RummyDiceGraphicsWPF)thisCon!;
                if (deck.DataContext.Equals(thisDice) == true)
                    return deck;
            }
            return null;
        }
        private Binding GetCommandBinding(string path)
        {
            Binding thisBind = new Binding(path);
            thisBind.Source = _thisMod;
            return thisBind;
        }
        private void RefreshItems()
        {
            CustomBasicList<RummyDiceGraphicsWPF> tempList = new CustomBasicList<RummyDiceGraphicsWPF>();
            foreach (var thisDice in _diceList!)
            {
                var thisD = FindControl(thisDice);
                tempList.Add(thisD!);
            }
            _thisStack!.Children.Clear();
            foreach (var thisD in tempList)
                _thisStack.Children.Add(thisD);
        }
        private void DiceBindings(RummyDiceGraphicsWPF thisGraphics, RummyDiceInfo thisDice) // needs the dice for the data context
        {
            thisGraphics.DataContext = thisDice;
            thisGraphics.CommandParameter = thisDice;
            var ThisBind = GetCommandBinding(nameof(RummyBoardCP.DiceCommand));
            thisGraphics.SetBinding(RummyDiceGraphicsWPF.CommandProperty, ThisBind);
        }
        private void PopulateList()
        {
            foreach (var firstDice in _diceList!)
            {
                RummyDiceGraphicsWPF thisGraphics = new RummyDiceGraphicsWPF(); // this does the bindings already as well
                thisGraphics.SendDiceInfo(firstDice); //i think this too now.
                DiceBindings(thisGraphics, firstDice);
                _thisStack!.Children.Add(thisGraphics);
            }
        }
        public void LoadDiceViewModel(RummyDiceMainGameClass mainGame)
        {
            _thisMod = mainGame.MainBoard1;
            Margin = new Thickness(3, 3, 3, 3);
            DataContext = _thisMod;
            _diceList = mainGame.SaveRoot!.DiceList;
            _diceList.CollectionChanged += DiceList_CollectionChanged;
            _thisStack = new StackPanel();
            _thisStack.Orientation = Orientation.Horizontal; // will always be horizontal for this one.
            PopulateList();
            Content = _thisStack;
        }
        public void UpdateDiceViewModel(RummyDiceMainGameClass mainGame)
        {
            _thisMod = mainGame.MainBoard1;
            DataContext = _thisMod;
            _diceList!.CollectionChanged -= DiceList_CollectionChanged;
            _diceList = mainGame.SaveRoot!.DiceList;
            _diceList.CollectionChanged += DiceList_CollectionChanged;
            _thisStack!.Children.Clear();
            PopulateList();
        }
        private void DiceList_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (var ThisItem in e.NewItems)
                {
                    var newDice = (RummyDiceInfo)ThisItem!;
                    RummyDiceGraphicsWPF thisD = new RummyDiceGraphicsWPF();
                    DiceBindings(thisD, newDice); // well see what we need
                    thisD.SendDiceInfo(newDice);
                    _thisStack!.Children.Add(thisD);
                }
            }
            if (e.Action == NotifyCollectionChangedAction.Replace)
            {
                if (e.OldItems.Count == e.NewItems.Count)
                {
                    int x;
                    var loopTo = e.OldItems.Count;
                    for (x = 1; x <= loopTo; x++)
                    {
                        var oldDice = (RummyDiceInfo)e.OldItems[x - 1]!;
                        var newDice = (RummyDiceInfo)e.NewItems[x - 1]!;
                        var thisCon = FindControl(oldDice!);
                        thisCon!.DataContext = newDice;
                    }
                }
                else
                    throw new Exception("Not sure when the numbers don't match");
            }
            if (e.Action == NotifyCollectionChangedAction.Remove) //old cards
            {
                foreach (var thisItem in e.OldItems)
                {
                    var oldDice = (RummyDiceInfo)thisItem!;
                    var thisCon = FindControl(oldDice);
                    _thisStack!.Children.Remove(thisCon); // because not there anymore.
                }
            }
            if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                _thisStack!.Children.Clear(); // needs to clear and do nothing else because no dice left
                PopulateList();
            }
            if ((int)e.Action == (int)NotifyCollectionChangedAction.Move)
            {
                if (e.OldStartingIndex == e.NewStartingIndex)
                    RefreshItems(); //several changes
                else
                {
                    var firstCon = _thisStack!.Children[e.OldStartingIndex];
                    _ = _thisStack.Children[e.NewStartingIndex];
                    _thisStack.Children.Remove(firstCon);
                    _thisStack.Children.Insert(e.NewStartingIndex, firstCon);
                }
            }
        }
    }
}