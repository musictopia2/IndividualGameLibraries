using BattleshipCP;
using CommonBasicStandardLibraries.CollectionClasses;
using Xamarin.Forms;
namespace BattleshipXF
{
    public class ShipControlXF : ContentView
    {
        private StackLayout? _thisStack;
        public void LoadShips(CustomBasicList<ShipInfoCP> thisList)
        {
            _thisStack = new StackLayout();
            foreach (var thisShip in thisList)
            {
                var thisControl = new ShipInfoXF();
                thisControl.CreateShip(thisShip);
                _thisStack.Children.Add(thisControl);
            }
            Content = _thisStack;
        }
        public void UpdateShips(CustomBasicList<ShipInfoCP> thisList)
        {
            _thisStack!.Children.Clear();
            foreach (var thisShip in thisList)
            {
                var thisControl = new ShipInfoXF();
                thisControl.CreateShip(thisShip);
                _thisStack.Children.Add(thisControl);
            }
        }
    }
}