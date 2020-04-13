using FluxxCP.Containers;
using FluxxCP.ViewModels;
using System.Windows;
using System.Windows.Controls;
using static BasicControlsAndWindowsCore.Helpers.GridHelper;
using static BasicGamingUIWPFLibrary.Helpers.SharedUIFunctions;

namespace FluxxWPF
{
    public static class ActionHelpers
    {
        internal static Button GetKeeperButton()
        {
            var thisBut = GetGamingButton("Show Keepers", nameof(BasicActionScreen.ShowKeepersAsync));
            thisBut.HorizontalAlignment = HorizontalAlignment.Left;
            thisBut.VerticalAlignment = VerticalAlignment.Top;
            return thisBut;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="element">the unique parts</param>
        /// <returns>this is supposed to be the content you set.</returns>
        internal static StackPanel GetFinalStack(UIElement element, FluxxVMData model, ActionContainer actionContainer, KeeperContainer keeperContainer)
        {
            StackPanel mainStack = new StackPanel();

            Grid tempGrid = new Grid();
            tempGrid.Margin = new Thickness(3, 3, 3, 5);
            AddLeftOverColumn(tempGrid, 50);
            AddLeftOverColumn(tempGrid, 50);
            AddAutoColumns(tempGrid, 1);
            var currentCard = new ShowCardUI(model, actionContainer, keeperContainer, EnumShowCategory.MainAction);
            currentCard.Width = 700;
            AddControlToGrid(tempGrid, currentCard, 0, 0);
            var actionCard = new ShowCardUI(model, actionContainer, keeperContainer, EnumShowCategory.CurrentAction);
            AddControlToGrid(tempGrid, actionCard, 0, 1);
            actionCard.Width = 700; // since it does not seem to autosize.
            GoalHandWPF goal1 = new GoalHandWPF();
            goal1.LoadList(actionContainer.PrivateGoals!, "");
            AddControlToGrid(tempGrid, goal1, 0, 2);
            mainStack.Children.Add(tempGrid);
            tempGrid = new Grid();
            AddLeftOverColumn(tempGrid, 50);
            AddLeftOverColumn(tempGrid, 50);
            var yourCards = new FluxxHandWPF();
            yourCards.LoadList(actionContainer.YourCards!, "");
            AddControlToGrid(tempGrid, yourCards, 0, 0);
            var yourKeepers = new KeeperHandWPF();
            yourKeepers.MinWidth = 400;
            yourKeepers.LoadList(actionContainer.YourKeepers!, "");
            AddControlToGrid(tempGrid, yourKeepers, 0, 1);
            tempGrid.Margin = new Thickness(3, 3, 3, 5);
            mainStack.Children.Add(tempGrid);
            mainStack.Children.Add(element);
            return mainStack;
        }


    }
}
