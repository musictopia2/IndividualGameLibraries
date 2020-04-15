using BasicGamingUIXFLibrary.BasicControls.ChoicePickers;
using BasicGamingUIXFLibrary.BasicControls.SimpleControls;
using BasicGamingUIXFLibrary.Helpers;
using FluxxCP.Containers;
using FluxxCP.ViewModels;
using Xamarin.Forms;
using static BasicGamingUIXFLibrary.Helpers.SharedUIFunctions;
namespace FluxxXF
{
    public class PlayerUI : BaseFrameXF
    {
        private readonly ListChooserXF _list = new ListChooserXF(); //iffy.

        public PlayerUI(ActionContainer actionContainer, bool usePlayerButton, int itemHeight = 40, int itemWidth = 300)
        {
            GamePackageViewModelBinder.ManuelElements.Clear();
            Text = "Player List";
            StackLayout thisStack = new StackLayout();
            SetUpMarginsOnParentControl(thisStack); //i think.
            _list.ItemHeight = itemHeight;
            _list.ItemWidth = itemWidth;
            _list.LoadLists(actionContainer.Player1!);
            thisStack.Children.Add(_list);
            if (usePlayerButton)
            {
                var button = GetGamingButton("Choose Player", nameof(ActionTakeUseViewModel.ChoosePlayerAsync)); // i think
                GamePackageViewModelBinder.ManuelElements.Add(button); //try this too.
                var bind = new Binding(nameof(ActionContainer.ButtonChoosePlayerVisible));
                button.SetBinding(IsVisibleProperty, bind);
                thisStack.Children.Add(button);

            }
            Grid grid = new Grid();
            grid.Children.Add(ThisDraw);
            grid.Children.Add(thisStack);
            Content = grid;
        }
    }
}