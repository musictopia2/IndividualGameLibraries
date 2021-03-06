using BasicGamingUIXFLibrary.Helpers;
using BasicXFControlsAndPages.Helpers;
using BasicXFControlsAndPages.MVVMFramework.ViewLinkersPlusBinders;
using BattleshipCP.Data;
using BattleshipCP.ViewModels;
using CommonBasicStandardLibraries.Exceptions;
using Xamarin.Forms;
namespace BattleshipXF
{
    public class ShipInfoXF : ContentView
    {
        public void CreateShip(ShipInfoCP thisShip, BattleshipMainViewModel model)
        {
            BindingContext = thisShip;
            Grid thisGrid = new Grid();
            float labelSize;
            labelSize = 40; // can try 40.  can experiment though.
            thisGrid.RowSpacing = 0;
            thisGrid.ColumnSpacing = 0;
            Button thisBut = new Button();
            GridHelper.AddPixelRow(thisGrid, 50);
            int x;
            for (x = 1; x <= 5; x++)
                GridHelper.AddPixelColumn(thisGrid, (int)labelSize);
            GridHelper.AddPixelColumn(thisGrid, 120); // not sure
            thisBut.BorderColor = Color.White;
            thisBut.BorderWidth = 2;
            thisBut.FontAttributes = FontAttributes.Bold;
            thisBut.FontSize = 14;
            thisBut.Text = thisShip.ShipName;
            var thisBind = new Binding(nameof(ShipInfoCP.Visible));
            thisBut.SetBinding(IsVisibleProperty, thisBind);
            if (thisShip.ShipCategory == EnumShipList.None)
                throw new BasicBlankException("Can't be none");

            thisBut.SetName(nameof(BattleshipMainViewModel.ChooseShip));
            GamePackageViewModelBinder.ManuelElements.Add(thisBut); //try this one as well.
            thisBut.CommandParameter = thisShip.ShipCategory;
            thisBut.Margin = new Thickness(5, 0, 0, 0);
            thisBind = new Binding(nameof(BattleshipMainViewModel.ShipSelected));
            thisBind.Source = model;
            IValueConverter thisConv = new ChooseShipConverter();
            thisBind.Converter = thisConv;
            thisBind.ConverterParameter = thisShip.ShipCategory;
            thisBut.SetBinding(BackgroundColorProperty, thisBind);
            GridHelper.AddControlToGrid(thisGrid, thisBut, 0, 5);
            x = 0;
            foreach (var thisPiece in thisShip.PieceList!.Values)
            {
                Label thisText = new Label();
                thisText.BindingContext = thisPiece;
                thisConv = new ShipLabelConverter();
                thisBind = new Binding(nameof(PieceInfoCP.DidHit));
                thisBind.Converter = thisConv;
                thisText.SetBinding(BackgroundColorProperty, thisBind);
                thisText.FontAttributes = FontAttributes.Bold;
                thisText.FontSize = 14;
                thisText.TextColor = Color.Black;
                thisText.SetBinding(Label.TextProperty, new Binding(nameof(PieceInfoCP.Location)));
                thisText.HorizontalTextAlignment = TextAlignment.Center;
                thisText.VerticalTextAlignment = TextAlignment.Center;
                GridHelper.AddControlToGrid(thisGrid, thisText, 0, x);
                x += 1;
            }
            Content = thisGrid;
        }
    }
}