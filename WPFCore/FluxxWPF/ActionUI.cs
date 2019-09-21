using BaseGPXWindowsAndControlsCore.BasicControls.ChoicePickers;
using BaseGPXWindowsAndControlsCore.BasicControls.SimpleControls;
using FluxxCP;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using static BaseGPXWindowsAndControlsCore.BaseWindows.SharedWindowFunctions;
using static BasicControlsAndWindowsCore.Helpers.GridHelper;
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;
namespace FluxxWPF
{
    public class ActionUI : UserControl
    {
        private ActionViewModel? _actionMod;
        private StackPanel? _mainStack;
        public void LoadControls()
        {
            FluxxViewModel thisMod = Resolve<FluxxViewModel>();
            var tempObj = thisMod.Action1;
            _actionMod = (ActionViewModel)tempObj!;
            DataContext = _actionMod; // try to cast to an entire viewmodel
            var thisBind = GetVisibleBinding(nameof(ActionViewModel.EntireVisible));
            SetBinding(VisibilityProperty, thisBind);
            _mainStack = new StackPanel();
            Grid tempGrid = new Grid();
            tempGrid.Margin = new Thickness(3, 3, 3, 5);
            AddLeftOverColumn(tempGrid, 50);
            AddLeftOverColumn(tempGrid, 50);
            AddAutoColumns(tempGrid, 1);
            var currentCard = new ShowCardUI();
            currentCard.LoadControls(EnumShowCategory.CurrentAction);
            currentCard.Width = 700;
            AddControlToGrid(tempGrid, currentCard, 0, 0);
            var actionCard = new ShowCardUI();
            actionCard.LoadControls(EnumShowCategory.MainAction);
            AddControlToGrid(tempGrid, actionCard, 0, 1);
            actionCard.Width = 700; // since it does not seem to autosize.
            GoalHandWPF goal1 = new GoalHandWPF();
            goal1.LoadList(_actionMod.PrivateGoals!, "");
            AddControlToGrid(tempGrid, goal1, 0, 2);
            _mainStack.Children.Add(tempGrid);
            tempGrid = new Grid();
            AddLeftOverColumn(tempGrid, 50);
            AddLeftOverColumn(tempGrid, 50);
            var yourCards = new FluxxHandWPF();
            yourCards.LoadList(_actionMod.YourCards!, "");
            AddControlToGrid(tempGrid, yourCards, 0, 0);
            var yourKeepers = new KeeperHandWPF();
            yourKeepers.MinWidth = 400;
            yourKeepers.LoadList(_actionMod.YourKeepers!, "");
            AddControlToGrid(tempGrid, yourKeepers, 0, 1);
            tempGrid.Margin = new Thickness(3, 3, 3, 5);
            _mainStack.Children.Add(tempGrid);
            LoadRulesScreen(); // done
            LoadFirstRandomScreen(); // done
            LoadTradeHandsScreen(); // done
            LoadUseTakeScreen(); // done
            LoadEveryBody1Screen(); // done
            LoadDrawUseScreen(); // done
            LoadDirectionsScreen(); // done
            LoadDoAgainScreen(); // done
            Content = _mainStack;
        }
        private void LoadRulesScreen() // done
        {
            StackPanel thisStack = new StackPanel();
            SetVisibleBinding(thisStack, EnumActionCategory.Rules);
            var firstInfo = new SimpleLabelGrid();
            firstInfo.AddRow("Rules To Discard", nameof(ActionViewModel.RulesToDiscard)); // i think
            thisStack.Children.Add(firstInfo.GetContent);
            var nextLabel = GetDefaultLabel();
            nextLabel.Margin = new Thickness(3, 15, 0, 2);
            nextLabel.Text = "Choose Rules To Discard";
            ScrollViewer thisScroll = new ScrollViewer(); // could have to scroll
            thisScroll.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            thisScroll.Height = 400;
            ListChooserWPF rule1 = new ListChooserWPF();
            rule1.LoadLists(_actionMod!.Rule1!);
            rule1.ItemHeight = 40;
            rule1.ItemWidth = 600;
            thisScroll.Content = rule1;
            StackPanel otherStack = new StackPanel();
            otherStack.Orientation = Orientation.Horizontal;
            thisStack.Children.Add(otherStack);
            otherStack.Children.Add(thisScroll);
            var thisBut = GetGamingButton("View Card", nameof(ActionViewModel.ViewRuleCardCommand));
            StackPanel finalStack = new StackPanel();
            finalStack.Children.Add(thisBut);
            thisBut = GetGamingButton("Discard Selected Rule(s)", nameof(ActionViewModel.DiscardRulesCommand));
            finalStack.Children.Add(thisBut);
            thisBut = GetKeeperButton();
            finalStack.Children.Add(thisBut);
            otherStack.Children.Add(finalStack);
        }
        internal static Button GetKeeperButton()
        {
            var thisBut = GetGamingButton("Show Keepers", nameof(ActionViewModel.ShowKeepersCommand));
            thisBut.HorizontalAlignment = HorizontalAlignment.Left;
            thisBut.VerticalAlignment = VerticalAlignment.Top;
            return thisBut;
        }
        private void SetVisibleBinding(StackPanel thisStack, EnumActionCategory actionCategory)
        {
            var thisBind = new Binding(nameof(ActionViewModel.ActionCategory));
            ActionVisibleConverter ThisConvert = new ActionVisibleConverter();
            thisBind.Converter = ThisConvert;
            thisBind.ConverterParameter = actionCategory;
            thisStack.SetBinding(VisibilityProperty, thisBind);
            _mainStack!.Children.Add(thisStack); // each will be a stack panel.  if i change, will simply have a control and stack will contain one element.
        }
        private void LoadDirectionsScreen()
        {
            StackPanel thisStack = new StackPanel();
            SetVisibleBinding(thisStack, EnumActionCategory.Directions);
            DirectionUI thisDirection = new DirectionUI();
            thisDirection.LoadList(_actionMod!);
            thisStack.Children.Add(thisDirection);
        }
        private void LoadDoAgainScreen()
        {
            StackPanel thisStack = new StackPanel();
            SetVisibleBinding(thisStack, EnumActionCategory.DoAgain);
            CardListUI thisCard = new CardListUI();
            thisCard.LoadControls(_actionMod!);
            thisStack.Children.Add(thisCard);
        }
        private void LoadTradeHandsScreen()
        {
            StackPanel thisStack = new StackPanel();
            SetVisibleBinding(thisStack, EnumActionCategory.TradeHands);
            PlayerUI thisPlayer = new PlayerUI();
            thisPlayer.ItemHeight = 50;
            thisPlayer.ItemWidth = 800;
            thisPlayer.LoadLists(_actionMod!);
            thisStack.Children.Add(thisPlayer); // hopefully this simple.
            var ThisBut = GetKeeperButton();
            thisStack.Children.Add(ThisBut);
        }
        private void LoadUseTakeScreen()
        {
            var thisStack = new StackPanel();
            SetVisibleBinding(thisStack, EnumActionCategory.UseTake);
            var thisPlayer = new PlayerUI();
            thisPlayer.ItemHeight = 50;
            thisPlayer.ItemWidth = 800;
            thisPlayer.LoadLists(_actionMod!);
            thisStack.Children.Add(thisPlayer);
            var otherHand = new FluxxHandWPF();
            otherHand.LoadList(_actionMod!.OtherHand!, "");
            otherHand.MinWidth = 400;
            thisStack.Children.Add(otherHand);
            var thisBut = GetGamingButton("Choose Card", nameof(ActionViewModel.ChooseCardCommand));
            var thisBind = GetVisibleBinding(nameof(ActionViewModel.ButtonChooseCardVisible));
            thisBut.SetBinding(Button.VisibilityProperty, thisBind);
            thisBut.HorizontalAlignment = HorizontalAlignment.Left;
            thisBut.VerticalAlignment = VerticalAlignment.Top;
            thisStack.Children.Add(thisBut);
            thisBut = GetKeeperButton();
            thisStack.Children.Add(thisBut);
        }
        private void LoadEveryBody1Screen()
        {
            var thisStack = new StackPanel();
            thisStack.Orientation = Orientation.Horizontal;
            SetVisibleBinding(thisStack, EnumActionCategory.Everybody1);
            var thisPlayer = new PlayerUI();
            thisPlayer.ItemHeight = 50;
            thisPlayer.ItemWidth = 800;
            thisPlayer.LoadLists(_actionMod!);
            thisStack.Children.Add(thisPlayer);
            StackPanel finalStack = new StackPanel();
            thisStack.Children.Add(finalStack);
            FluxxHandWPF otherHand = new FluxxHandWPF();
            otherHand.LoadList(_actionMod!.TempHand!, "");
            otherHand.Margin = new Thickness(3, 15, 0, 0);
            finalStack.Children.Add(otherHand);
            var thisBut = GetGamingButton("Give Cards To Selected Player", nameof(ActionViewModel.GiveCardsCommand)); // i think
            thisBut.HorizontalAlignment = HorizontalAlignment.Left;
            thisBut.VerticalAlignment = VerticalAlignment.Top;
            finalStack.Children.Add(thisBut);
            thisBut = GetKeeperButton();
            finalStack.Children.Add(thisBut);
        }
        private void LoadDrawUseScreen()
        {
            StackPanel thisStack = new StackPanel();
            SetVisibleBinding(thisStack, EnumActionCategory.DrawUse);
            FluxxHandWPF thisHand = new FluxxHandWPF();
            thisHand.LoadList(_actionMod!.TempHand!, ""); // i think this is the only difference for this one.
            thisStack.Children.Add(thisHand);
            var ThisBut = GetGamingButton("Choose Card", nameof(ActionViewModel.ChooseCardCommand)); // i think
            ThisBut.HorizontalAlignment = HorizontalAlignment.Left;
            ThisBut.VerticalAlignment = VerticalAlignment.Top;
            thisStack.Children.Add(ThisBut);
            ThisBut = GetKeeperButton();
            thisStack.Children.Add(ThisBut);
        }
        private void LoadFirstRandomScreen() // done i think
        {
            StackPanel thisStack = new StackPanel();
            SetVisibleBinding(thisStack, EnumActionCategory.FirstRandom);
            FluxxHandWPF thisHand = new FluxxHandWPF();
            thisHand.LoadList(_actionMod!.OtherHand!, "");
            thisHand.MinWidth = 400;
            thisStack.Children.Add(thisHand);
            var thisBut = GetGamingButton("Choose Card", nameof(ActionViewModel.ChooseCardCommand)); // i think
            thisBut.HorizontalAlignment = HorizontalAlignment.Left;
            thisBut.VerticalAlignment = VerticalAlignment.Top;
            thisStack.Children.Add(thisBut);
            thisBut = GetKeeperButton();
            thisStack.Children.Add(thisBut);
        }
    }
}