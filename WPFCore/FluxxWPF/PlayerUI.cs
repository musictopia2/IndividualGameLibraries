using BaseGPXWindowsAndControlsCore.BasicControls.ChoicePickers;
using BaseGPXWindowsAndControlsCore.BasicControls.SimpleControls;
using FluxxCP;
using SkiaSharp;
using System.Windows.Controls;
using static BaseGPXWindowsAndControlsCore.BaseWindows.SharedWindowFunctions;
namespace FluxxWPF
{
    public class PlayerUI : BaseFrameWPF
    {
        private readonly ListChooserWPF _thisList = new ListChooserWPF();
        public int ItemHeight
        {
            get
            {
                return _thisList.ItemHeight;
            }
            set
            {
                _thisList.ItemHeight = value;
            }
        }
        public int ItemWidth
        {
            get
            {
                return _thisList.ItemWidth;
            }
            set
            {
                _thisList.ItemWidth = value;
            }
        }
        public void LoadLists(ActionViewModel actionMod)
        {
            Text = "Player List";
            StackPanel thisStack = new StackPanel();
            SKRect thisRect = ThisFrame.GetControlArea();
            SetUpMarginsOnParentControl(thisStack, thisRect); //i think.
            _thisList.LoadLists(actionMod.Player1!);
            thisStack.Children.Add(_thisList);
            var ThisBut = GetGamingButton("Choose Player", nameof(ActionViewModel.ChoosePlayerCommand)); // i think
            var ThisBind = GetVisibleBinding(nameof(ActionViewModel.ButtonChoosePlayerVisible));
            ThisBut.SetBinding(Button.VisibilityProperty, ThisBind);
            thisStack.Children.Add(ThisBut);
            Grid ThisGrid = new Grid();
            ThisGrid.Children.Add(ThisDraw);
            ThisGrid.Children.Add(thisStack);
            Content = ThisGrid;
        }
    }
}