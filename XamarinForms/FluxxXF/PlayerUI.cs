using BaseGPXPagesAndControlsXF.BasicControls.ChoicePickers;
using BaseGPXPagesAndControlsXF.BasicControls.SimpleControls;
using FluxxCP;
using Xamarin.Forms;
using static BaseGPXPagesAndControlsXF.BasePageProcesses.Pages.SharedPageFunctions;
namespace FluxxXF
{
    public class PlayerUI : BaseFrameXF
    {
        private readonly ListChooserXF _thisList = new ListChooserXF(); //iffy.
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
            StackLayout thisStack = new StackLayout();
            SetUpMarginsOnParentControl(thisStack); //i think.
            _thisList.LoadLists(actionMod.Player1!);
            thisStack.Children.Add(_thisList);
            var ThisBut = GetGamingButton("Choose Player", nameof(ActionViewModel.ChoosePlayerCommand)); // i think
            var ThisBind = new Binding(nameof(ActionViewModel.ButtonChoosePlayerVisible));
            ThisBut.SetBinding(IsVisibleProperty, ThisBind);
            thisStack.Children.Add(ThisBut);
            Grid ThisGrid = new Grid();
            ThisGrid.Children.Add(ThisDraw);
            ThisGrid.Children.Add(thisStack);
            Content = ThisGrid;
        }
    }
}