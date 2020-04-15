using BattleshipCP.Data;
using BattleshipCP.ViewModels;
using CommonBasicStandardLibraries.CollectionClasses;
using Xamarin.Forms;
namespace BattleshipXF
{
    public class ShipControlXF : ContentView
    {
        private StackLayout? _thisStack;
        public void LoadShips(CustomBasicList<ShipInfoCP> thisList, BattleshipMainViewModel model)
        {
            _thisStack = new StackLayout();
            foreach (var thisShip in thisList)
            {
                var thisControl = new ShipInfoXF();
                thisControl.CreateShip(thisShip, model);
                _thisStack.Children.Add(thisControl);
            }
            Content = _thisStack;
        }

    }
}