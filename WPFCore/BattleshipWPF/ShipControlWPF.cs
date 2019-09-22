using BattleshipCP;
using CommonBasicStandardLibraries.CollectionClasses;
using System.Windows.Controls;
namespace BattleshipWPF
{
    public class ShipControlWPF : UserControl
    {
        private StackPanel? _thisStack;
        public void LoadShips(CustomBasicList<ShipInfoCP> thisList)
        {
            _thisStack = new StackPanel();
            foreach (var thisShip in thisList)
            {
                var thisControl = new ShipInfoWPF();
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
                var thisControl = new ShipInfoWPF();
                thisControl.CreateShip(thisShip);
                _thisStack.Children.Add(thisControl);
            }
        }
    }
}