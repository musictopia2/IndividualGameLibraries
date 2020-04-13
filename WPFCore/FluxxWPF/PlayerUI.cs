using BasicGamingUIWPFLibrary.BasicControls.ChoicePickers;
using BasicGamingUIWPFLibrary.BasicControls.SimpleControls;
using BasicGamingUIWPFLibrary.Helpers;
using FluxxCP.Containers;
using FluxxCP.ViewModels;
using SkiaSharp;
using System.Windows.Controls;
using static BasicGamingUIWPFLibrary.Helpers.SharedUIFunctions; //this usually will be used too.

namespace FluxxWPF
{
    public class PlayerUI : BaseFrameWPF
    {
        private readonly ListChooserWPF _list = new ListChooserWPF();
        //public int ItemHeight
        //{
        //    get
        //    {
        //        return _list.ItemHeight;
        //    }
        //    set
        //    {
        //        _list.ItemHeight = value;
        //    }
        //}
        //public int ItemWidth
        //{
        //    get
        //    {
        //        return _list.ItemWidth;
        //    }
        //    set
        //    {
        //        _list.ItemWidth = value;
        //    }
        //}

        public PlayerUI(ActionContainer actionContainer, bool usePlayerButton, int itemHeight = 40, int itemWidth = 300 )
        {
            GamePackageViewModelBinder.ManuelElements.Clear();
            Text = "Player List";
            StackPanel thisStack = new StackPanel();
            SKRect thisRect = ThisFrame.GetControlArea();
            SetUpMarginsOnParentControl(thisStack, thisRect); //i think.
            _list.ItemHeight = itemHeight;
            _list.ItemWidth = itemWidth;
            _list.LoadLists(actionContainer.Player1!);
            thisStack.Children.Add(_list);
            if (usePlayerButton)
            {
                var button = GetGamingButton("Choose Player", nameof(ActionTakeUseViewModel.ChoosePlayerAsync)); // i think
                GamePackageViewModelBinder.ManuelElements.Add(button); //try this too.
                var bind = GetVisibleBinding(nameof(ActionContainer.ButtonChoosePlayerVisible));
                button.SetBinding(VisibilityProperty, bind);
                thisStack.Children.Add(button);

            }
            Grid grid = new Grid();
            grid.Children.Add(ThisDraw);
            grid.Children.Add(thisStack);
            Content = grid;
        }
    }
}
