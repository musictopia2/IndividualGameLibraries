using BaseGPXWindowsAndControlsCore.BasicControls.SimpleControls;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.Exceptions;
using SorryCardGameCP;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
namespace SorryCardGameWPF
{
    public class BoardWPF : BaseFrameWPF
    {
        private SorryCardGamePlayerItem? _thisPlayer;
        private CustomBasicList<CardGraphicsWPF>? _thisList;
        private ICommand? _clickCommand;
        public void LoadList(SorryCardGamePlayerItem tempPlayer)
        {
            _thisPlayer = tempPlayer;
            StackPanel thisStack = new StackPanel();
            thisStack.Orientation = Orientation.Horizontal;
            _clickCommand = _thisPlayer.ClickCommand;
            _thisList = new CustomBasicList<CardGraphicsWPF>();
            for (int x = 1; x <= 4; x++)
            {
                var thisCard = new CardGraphicsWPF();
                _thisList.Add(thisCard);
                thisCard.SendSize("", new SorryCardGameCardInformation());
                thisCard.DataContext = null;
                thisStack.Children.Add(thisCard);
            }
            Text = _thisPlayer.NickName;
            DataContext = _thisPlayer;
            var thisRect = ThisFrame.GetControlArea();
            thisStack.Margin = new Thickness(thisRect.Left + 3, thisRect.Top + 10, 3, 3);
            var thisGrid = new Grid();
            thisGrid.Children.Add(ThisDraw);
            thisGrid.Children.Add(thisStack);
            Content = thisGrid;
            _clickCommand!.CanExecuteChanged += ClickCommandChange;
            MouseUp += BoardWPF_MouseUp;
            _thisPlayer.PropertyChanged += ThisPlayerPropertyChange;
            RefreshList();
        }
        private void ThisPlayerPropertyChange(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(SorryCardGamePlayerItem.HowManyAtHome))
            {
                RefreshList();
            }
            else if (e.PropertyName == nameof(SorryCardGamePlayerItem.Color))
            {
                _thisList!.ForEach(thisCard => thisCard.Color = _thisPlayer!.Color.ToColor());
            }
        }
        private void BoardWPF_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (_clickCommand!.CanExecute(null) == false)
                return;
            _clickCommand.Execute(null);
        }
        private void ClickCommandChange(object? sender, EventArgs e)
        {
            IsEnabled = _clickCommand!.CanExecute(null); //hopefully this still works.
        }
        private void RefreshList()
        {
            if (_thisList!.Count != 4)
                throw new BasicBlankException("Must have 4 cards at all times.");
            int accounted = 0;
            SorryCardGameCardInformation? thisCard;
            _thisList.ForEach(thisG =>
            {
                if (thisG.DataContext == null)
                {
                    thisCard = new SorryCardGameCardInformation();
                    thisCard.Sorry = EnumSorry.OnBoard;
                    thisCard.Color = _thisPlayer!.Color.ToColor();
                    thisG.DataContext = thisCard;
                }
                else
                {
                    thisCard = thisG.DataContext as SorryCardGameCardInformation;
                }
                accounted++;
                if (accounted <= _thisPlayer!.HowManyAtHome)
                    thisCard!.Category = EnumCategory.Home;
                else
                    thisCard!.Category = EnumCategory.Start;
            });
        }
    }
}