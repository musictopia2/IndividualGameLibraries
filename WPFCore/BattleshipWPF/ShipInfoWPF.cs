using BasicControlsAndWindowsCore.Helpers;
using BasicGamingUIWPFLibrary.Helpers;
using BattleshipCP.Data;
using BattleshipCP.ViewModels;
using CommonBasicStandardLibraries.Exceptions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
namespace BattleshipWPF
{
    public class ShipInfoWPF : UserControl
    {
        public void CreateShip(ShipInfoCP thisShip, BattleshipMainViewModel model)
        {
            DataContext = thisShip;
            Grid thisGrid = new Grid();
            float labelSize;
            labelSize = 40; // can try 40.  can experiment though.
            Button thisBut = new Button();
            GridHelper.AddAutoRows(thisGrid, 1);
            int x;
            for (x = 1; x <= 5; x++)
                GridHelper.AddPixelColumn(thisGrid, (int)labelSize);
            GridHelper.AddPixelColumn(thisGrid, 100); // not sure
            thisBut.BorderBrush = Brushes.White;
            thisBut.BorderThickness = new Thickness(2, 2, 2, 2);
            thisBut.FontWeight = FontWeights.Bold;
            thisBut.FontSize = 14;
            thisBut.Content = thisShip.ShipName;
            IValueConverter thisConv;
            thisConv = new BooleanToVisibilityConverter();
            var thisBind = new Binding(nameof(ShipInfoCP.Visible));
            thisBind.Converter = thisConv;
            thisBut.SetBinding(VisibilityProperty, thisBind);
            if (thisShip.ShipCategory == EnumShipList.None)
                throw new BasicBlankException("Can't be none");

            thisBut.Name = nameof(BattleshipMainViewModel.ChooseShip);
            GamePackageViewModelBinder.ManuelElements.Add(thisBut); //try this one as well.
            thisBut.CommandParameter = thisShip.ShipCategory;
            thisBut.Margin = new Thickness(5, 0, 0, 0);
            thisBind = new Binding(nameof(BattleshipMainViewModel.ShipSelected));
            thisBind.Source = model; //try this for now.  otherwise, i have to create custom button control.
            thisConv = new ChooseShipConverter();
            thisBind.Converter = thisConv;
            thisBind.ConverterParameter = thisShip.ShipCategory;
            thisBut.SetBinding(Button.BackgroundProperty, thisBind);
            GridHelper.AddControlToGrid(thisGrid, thisBut, 0, 5);
            x = 0;
            foreach (var thisPiece in thisShip.PieceList!.Values)
            {
                TextBlock thisText = new TextBlock();
                Border thisBorder = new Border();
                thisBorder.BorderBrush = Brushes.Black;
                thisBorder.BorderThickness = new Thickness(2, 2, 2, 2);
                GridHelper.AddControlToGrid(thisGrid, thisBorder, 0, x);
                thisText.DataContext = thisPiece;
                thisBorder.DataContext = thisPiece; // has to put here as well
                thisConv = new ShipLabelConverter();
                thisBind = new Binding(nameof(PieceInfoCP.DidHit));
                thisBind.Converter = thisConv;
                thisBorder.SetBinding(Border.BackgroundProperty, thisBind);
                thisText.FontWeight = FontWeights.Bold;
                thisText.FontSize = 14;
                thisText.SetBinding(TextBlock.TextProperty, new Binding(nameof(PieceInfoCP.Location)));
                thisText.HorizontalAlignment = HorizontalAlignment.Center;
                thisText.VerticalAlignment = VerticalAlignment.Center;
                GridHelper.AddControlToGrid(thisGrid, thisText, 0, x);
                x += 1;
            }
            Content = thisGrid;
        }
    }
}