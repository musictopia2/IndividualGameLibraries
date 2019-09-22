using BaseGPXPagesAndControlsXF.BasicControls.SimpleControls;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.Exceptions;
using SorryCardGameCP;
using System;
using System.ComponentModel;
using System.Windows.Input;
using Xamarin.Forms;
namespace SorryCardGameXF
{
    public class BoardXF : BaseFrameXF
    {
        private SorryCardGamePlayerItem? _thisPlayer;
        private CustomBasicList<CardGraphicsXF>? _thisList;
        private ICommand? _clickCommand;
        public void LoadList(SorryCardGamePlayerItem tempPlayer)
        {
            _thisPlayer = tempPlayer;
            StackLayout thisStack = new StackLayout();
            thisStack.Orientation = StackOrientation.Horizontal;
            _clickCommand = _thisPlayer.ClickCommand;
            thisStack.InputTransparent = true;
            _thisList = new CustomBasicList<CardGraphicsXF>();
            for (int x = 1; x <= 4; x++)
            {
                var thisCard = new CardGraphicsXF();
                _thisList.Add(thisCard);
                thisCard.SendSize("", new SorryCardGameCardInformation());
                thisCard.BindingContext = null;
                thisCard.InputTransparent = true;
                thisStack.Children.Add(thisCard);
            }
            Text = _thisPlayer.NickName;
            BindingContext = _thisPlayer;
            var thisRect = ThisFrame.GetControlArea();
            thisStack.Margin = new Thickness(thisRect.Left + 3, thisRect.Top + 10, 3, 3);
            var thisGrid = new Grid();
            ThisDraw.EnableTouchEvents = true;
            ThisDraw.Touch += ThisDraw_Touch;
            thisGrid.Children.Add(ThisDraw);
            thisGrid.Children.Add(thisStack);
            Content = thisGrid;
            _clickCommand!.CanExecuteChanged += ClickCommandChange;
            _thisPlayer.PropertyChanged += ThisPlayerPropertyChange;
            RefreshList();
        }
        private void ThisDraw_Touch(object sender, SkiaSharp.Views.Forms.SKTouchEventArgs e)
        {
            if (_clickCommand!.CanExecute(null) == false)
                return;
            _clickCommand.Execute(null);
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
                if (thisG.BindingContext == null)
                {
                    thisCard = new SorryCardGameCardInformation();
                    thisCard.Sorry = EnumSorry.OnBoard;
                    thisCard.Color = _thisPlayer!.Color.ToColor();
                    thisG.BindingContext = thisCard;
                }
                else
                {
                    thisCard = thisG.BindingContext as SorryCardGameCardInformation;
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