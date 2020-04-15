using BasicGamingUIXFLibrary.BasicControls.ChoicePickers;
using BasicGamingUIXFLibrary.BasicControls.SimpleControls;
using FluxxCP;
using FluxxCP.Containers;
using FluxxCP.ViewModels;
using Xamarin.Forms;
using static BasicGamingUIXFLibrary.Helpers.SharedUIFunctions;
using static BasicXFControlsAndPages.Helpers.GridHelper;
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;

namespace FluxxXF
{
    public static class ActionHelpers
    {
        internal static Button GetKeeperButton()
        {
            var thisBut = GetGamingButton("Show Keepers", nameof(BasicActionScreen.ShowKeepersAsync));
            thisBut.HorizontalOptions = LayoutOptions.Start;
            thisBut.VerticalOptions = LayoutOptions.Start;
            return thisBut;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="element">the unique parts</param>
        /// <returns>this is supposed to be the content you set.</returns>
        internal static StackLayout GetFinalStack(View element, FluxxVMData model, ActionContainer actionContainer, KeeperContainer keeperContainer)
        {
            StackLayout mainStack = new StackLayout();

            Grid tempGrid = new Grid();
            tempGrid.Margin = new Thickness(3, 3, 3, 5);
            AddLeftOverColumn(tempGrid, 50);
            AddLeftOverColumn(tempGrid, 50);
            AddAutoColumns(tempGrid, 1);
            var currentCard = new ShowCardUI(model, actionContainer, keeperContainer, EnumShowCategory.MainAction);
            currentCard.WidthRequest = 700;
            AddControlToGrid(tempGrid, currentCard, 0, 0);
            var actionCard = new ShowCardUI(model, actionContainer, keeperContainer, EnumShowCategory.CurrentAction);
            AddControlToGrid(tempGrid, actionCard, 0, 1);
            actionCard.WidthRequest = 700; // since it does not seem to autosize.
            GoalHandXF goal1 = new GoalHandXF();
            goal1.LoadList(actionContainer.PrivateGoals!, "");
            AddControlToGrid(tempGrid, goal1, 0, 2);
            mainStack.Children.Add(tempGrid);
            tempGrid = new Grid();
            AddLeftOverColumn(tempGrid, 50);
            AddLeftOverColumn(tempGrid, 50);
            var yourCards = new FluxxHandXF();
            yourCards.LoadList(actionContainer.YourCards!, "");
            AddControlToGrid(tempGrid, yourCards, 0, 0);
            var yourKeepers = new KeeperHandXF();
            yourKeepers.MinimumWidthRequest = 400;
            yourKeepers.LoadList(actionContainer.YourKeepers!, "");
            AddControlToGrid(tempGrid, yourKeepers, 0, 1);
            tempGrid.Margin = new Thickness(3, 3, 3, 5);
            mainStack.Children.Add(tempGrid);
            mainStack.Children.Add(element);
            return mainStack;
        }


    }
}