using BattleshipCP.Data;
using BattleshipCP.ViewModels;
using CommonBasicStandardLibraries.CollectionClasses;
using System.Windows.Controls;
namespace BattleshipWPF
{
    public class ShipControlWPF : UserControl
    {
        private StackPanel? _thisStack;
        public void LoadShips(CustomBasicList<ShipInfoCP> thisList, BattleshipMainViewModel model)
        {
            _thisStack = new StackPanel();
            foreach (var thisShip in thisList)
            {
                var thisControl = new ShipInfoWPF();
                thisControl.CreateShip(thisShip, model);
                _thisStack.Children.Add(thisControl);
            }
            Content = _thisStack;
        }
        
    }
}
