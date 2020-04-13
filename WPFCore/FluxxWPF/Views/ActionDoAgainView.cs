using BasicGamingUIWPFLibrary.BasicControls.ChoicePickers;
using BasicGamingUIWPFLibrary.BasicControls.SimpleControls;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using FluxxCP.Containers;
using FluxxCP.ViewModels;
using SkiaSharp;
using System.Threading.Tasks;
using System.Windows.Controls;
using static BasicGamingUIWPFLibrary.Helpers.SharedUIFunctions; //this usually will be used too.

namespace FluxxWPF.Views
{
    public class ActionDoAgainView : BaseFrameWPF, IUIView
    {
        public ActionDoAgainView(FluxxVMData model, ActionContainer actionContainer, KeeperContainer keeperContainer)
        {
            Text = "Card List";
            StackPanel stack = new StackPanel();
            SKRect thisRect = ThisFrame.GetControlArea();
            SetUpMarginsOnParentControl(stack, thisRect); //i think.
            ListChooserWPF list = new ListChooserWPF();
            list.LoadLists(actionContainer.CardList1!); // i think
            ScrollViewer thisScroll = new ScrollViewer();
            thisScroll.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            thisScroll.Content = list;
            thisScroll.Height = 500; // well see.
            stack.Orientation = Orientation.Horizontal;
            stack.Children.Add(thisScroll);
            StackPanel finalStack = new StackPanel();
            stack.Children.Add(finalStack);
            var button = GetGamingButton("Select Card", nameof(ActionDoAgainViewModel.SelectCardAsync)); // i think
            finalStack.Children.Add(button);
            button = GetGamingButton("View Card", nameof(ActionDoAgainViewModel.ViewCard));
            finalStack.Children.Add(button);
            button = ActionHelpers.GetKeeperButton();
            finalStack.Children.Add(button);
            Grid grid = new Grid();
            grid.Children.Add(ThisDraw);
            grid.Children.Add(stack);
            Content = ActionHelpers.GetFinalStack(grid, model, actionContainer, keeperContainer);
        }
        Task IUIView.TryActivateAsync()
        {
            return Task.CompletedTask;
        }

        Task IUIView.TryCloseAsync()
        {
            return Task.CompletedTask;
        }
    }
}
