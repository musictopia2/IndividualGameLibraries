﻿using BasicGamingUIWPFLibrary.BasicControls.ChoicePickers;
using BasicGamingUIWPFLibrary.BasicControls.SimpleControls;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using FluxxCP.Containers;
using FluxxCP.ViewModels;
using SkiaSharp;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using static BasicGamingUIWPFLibrary.Helpers.SharedUIFunctions; //this usually will be used too.
namespace FluxxWPF.Views
{
    public class ActionDirectionView : BaseFrameWPF, IUIView
    {
        public ActionDirectionView(FluxxVMData model, ActionContainer actionContainer, KeeperContainer keeperContainer)
        {
            Text = "Direction";
            StackPanel stack = new StackPanel();
            SKRect rect = ThisFrame.GetControlArea();
            SetUpMarginsOnParentControl(stack, rect); //i think.
            ListChooserWPF list = new ListChooserWPF();
            list.ItemHeight = 60;
            list.LoadLists(actionContainer.Direction1!);
            stack.Children.Add(list);
            var button = GetGamingButton("Choose Direction", nameof(ActionDirectionViewModel.DirectionAsync));
            button.HorizontalAlignment = HorizontalAlignment.Left;
            button.VerticalAlignment = VerticalAlignment.Top;
            StackPanel otherStack = new StackPanel();
            otherStack.Orientation = Orientation.Horizontal;
            stack.Children.Add(otherStack);
            otherStack.Children.Add(button);
            button = ActionHelpers.GetKeeperButton();
            otherStack.Children.Add(button);
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
