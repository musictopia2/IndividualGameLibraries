using BaseGPXPagesAndControlsXF.BasicControls.ChoicePickers;
using BaseGPXPagesAndControlsXF.BasicControls.SimpleControls;
using FluxxCP;
using Xamarin.Forms;
using static BaseGPXPagesAndControlsXF.BasePageProcesses.Pages.SharedPageFunctions;
using static BasicXFControlsAndPages.Helpers.GridHelper;
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;

namespace FluxxXF
{
    public class ActionUI : ContentView
    {
        public ActionUI()
        {
            IsVisible = false; //to start.
        }
        private ActionViewModel? _actionMod;
        private StackLayout? _mainStack;
        public void LoadControls()
        {
            FluxxViewModel thisMod = Resolve<FluxxViewModel>();
            var tempObj = thisMod.Action1;
            _actionMod = (ActionViewModel)tempObj!;
            BindingContext = _actionMod; // try to cast to an entire viewmodel
            var thisBind = new Binding(nameof(ActionViewModel.EntireVisible));
            SetBinding(IsVisibleProperty, thisBind);
            _mainStack = new StackLayout();
            Grid tempGrid = new Grid();
            tempGrid.Margin = new Thickness(3, 3, 3, 5);
            AddLeftOverColumn(tempGrid, 50);
            AddLeftOverColumn(tempGrid, 50);
            AddAutoColumns(tempGrid, 1);
            var currentCard = new ShowCardUI();
            currentCard.LoadControls(EnumShowCategory.CurrentAction);
            currentCard.WidthRequest = 700; //iffy
            AddControlToGrid(tempGrid, currentCard, 0, 0);
            var actionCard = new ShowCardUI();
            actionCard.LoadControls(EnumShowCategory.MainAction);
            AddControlToGrid(tempGrid, actionCard, 0, 1);
            actionCard.WidthRequest = 700; // since it does not seem to autosize.
            GoalHandXF goal1 = new GoalHandXF();
            goal1.LoadList(_actionMod.PrivateGoals!, "");
            AddControlToGrid(tempGrid, goal1, 0, 2);
            _mainStack.Children.Add(tempGrid);
            tempGrid = new Grid();
            AddLeftOverColumn(tempGrid, 50);
            AddLeftOverColumn(tempGrid, 50);
            var yourCards = new FluxxHandXF();
            yourCards.LoadList(_actionMod.YourCards!, "");
            AddControlToGrid(tempGrid, yourCards, 0, 0);
            var yourKeepers = new KeeperHandXF();
            yourKeepers.MinimumWidthRequest = 400;
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
            StackLayout thisStack = new StackLayout();
            SetVisibleBinding(thisStack, EnumActionCategory.Rules);
            var firstInfo = new SimpleLabelGridXF();
            firstInfo.AddRow("Rules To Discard", nameof(ActionViewModel.RulesToDiscard)); // i think
            thisStack.Children.Add(firstInfo.GetContent);
            var nextLabel = GetDefaultLabel();
            nextLabel.Margin = new Thickness(3, 15, 0, 2);
            nextLabel.Text = "Choose Rules To Discard";
            ScrollView thisScroll = new ScrollView(); // could have to scroll
            thisScroll.Orientation = ScrollOrientation.Vertical;
            thisScroll.HeightRequest = 400;
            ListChooserXF rule1 = new ListChooserXF();
            rule1.LoadLists(_actionMod!.Rule1!);
            rule1.ItemHeight = 40;
            rule1.ItemWidth = 600;
            thisScroll.Content = rule1;
            StackLayout otherStack = new StackLayout();
            otherStack.Orientation = StackOrientation.Horizontal;
            thisStack.Children.Add(otherStack);
            otherStack.Children.Add(thisScroll);
            var thisBut = GetGamingButton("View Card", nameof(ActionViewModel.ViewRuleCardCommand));
            StackLayout finalStack = new StackLayout();
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
            thisBut.HorizontalOptions = LayoutOptions.Start;
            thisBut.VerticalOptions = LayoutOptions.Start;
            return thisBut;
        }
        private void SetVisibleBinding(StackLayout thisStack, EnumActionCategory actionCategory)
        {
            var thisBind = new Binding(nameof(ActionViewModel.ActionCategory));
            ActionVisibleConverter ThisConvert = new ActionVisibleConverter();
            thisBind.Converter = ThisConvert;
            thisBind.ConverterParameter = actionCategory;
            thisStack.SetBinding(IsVisibleProperty, thisBind);
            _mainStack!.Children.Add(thisStack); // each will be a stack panel.  if i change, will simply have a control and stack will contain one element.
        }
        private void LoadDirectionsScreen()
        {
            StackLayout thisStack = new StackLayout();
            SetVisibleBinding(thisStack, EnumActionCategory.Directions);
            DirectionUI thisDirection = new DirectionUI();
            thisDirection.LoadList(_actionMod!);
            thisStack.Children.Add(thisDirection);
        }
        private void LoadDoAgainScreen()
        {
            StackLayout thisStack = new StackLayout();
            SetVisibleBinding(thisStack, EnumActionCategory.DoAgain);
            CardListUI thisCard = new CardListUI();
            thisCard.LoadControls(_actionMod!);
            thisStack.Children.Add(thisCard);
        }
        private void LoadTradeHandsScreen()
        {
            StackLayout thisStack = new StackLayout();
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
            var thisStack = new StackLayout();
            SetVisibleBinding(thisStack, EnumActionCategory.UseTake);
            var thisPlayer = new PlayerUI();
            thisPlayer.ItemHeight = 50;
            thisPlayer.ItemWidth = 800;
            thisPlayer.LoadLists(_actionMod!);
            thisStack.Children.Add(thisPlayer);
            var otherHand = new FluxxHandXF();
            otherHand.LoadList(_actionMod!.OtherHand!, "");
            otherHand.MinimumWidthRequest = 400;
            thisStack.Children.Add(otherHand);
            var thisBut = GetGamingButton("Choose Card", nameof(ActionViewModel.ChooseCardCommand));
            var thisBind = new Binding(nameof(ActionViewModel.ButtonChooseCardVisible));
            thisBut.SetBinding(IsVisibleProperty, thisBind);
            thisBut.HorizontalOptions = LayoutOptions.Start;
            thisBut.VerticalOptions = LayoutOptions.Start;
            thisStack.Children.Add(thisBut);
            thisBut = GetKeeperButton();
            thisStack.Children.Add(thisBut);
        }
        private void LoadEveryBody1Screen()
        {
            var thisStack = new StackLayout();
            thisStack.Orientation = StackOrientation.Horizontal;
            SetVisibleBinding(thisStack, EnumActionCategory.Everybody1);
            var thisPlayer = new PlayerUI();
            thisPlayer.ItemHeight = 50;
            thisPlayer.ItemWidth = 800;
            thisPlayer.LoadLists(_actionMod!);
            thisStack.Children.Add(thisPlayer);
            StackLayout finalStack = new StackLayout();
            thisStack.Children.Add(finalStack);
            FluxxHandXF otherHand = new FluxxHandXF();
            otherHand.LoadList(_actionMod!.TempHand!, "");
            otherHand.Margin = new Thickness(3, 15, 0, 0);
            finalStack.Children.Add(otherHand);
            var thisBut = GetGamingButton("Give Cards To Selected Player", nameof(ActionViewModel.GiveCardsCommand)); // i think
            thisBut.HorizontalOptions = LayoutOptions.Start;
            thisBut.VerticalOptions = LayoutOptions.Start;
            finalStack.Children.Add(thisBut);
            thisBut = GetKeeperButton();
            finalStack.Children.Add(thisBut);
        }
        private void LoadDrawUseScreen()
        {
            StackLayout thisStack = new StackLayout();
            SetVisibleBinding(thisStack, EnumActionCategory.DrawUse);
            FluxxHandXF thisHand = new FluxxHandXF();
            thisHand.LoadList(_actionMod!.TempHand!, ""); // i think this is the only difference for this one.
            thisStack.Children.Add(thisHand);
            var ThisBut = GetGamingButton("Choose Card", nameof(ActionViewModel.ChooseCardCommand)); // i think
            ThisBut.HorizontalOptions = LayoutOptions.Start;
            ThisBut.VerticalOptions = LayoutOptions.Start;
            thisStack.Children.Add(ThisBut);
            ThisBut = GetKeeperButton();
            thisStack.Children.Add(ThisBut);
        }
        private void LoadFirstRandomScreen() // done i think
        {
            StackLayout thisStack = new StackLayout();
            SetVisibleBinding(thisStack, EnumActionCategory.FirstRandom);
            FluxxHandXF thisHand = new FluxxHandXF();
            thisHand.LoadList(_actionMod!.OtherHand!, "");
            thisHand.MinimumWidthRequest = 400;
            thisStack.Children.Add(thisHand);
            var thisBut = GetGamingButton("Choose Card", nameof(ActionViewModel.ChooseCardCommand)); // i think
            thisBut.HorizontalOptions = LayoutOptions.Start;
            thisBut.VerticalOptions = LayoutOptions.Start;
            thisStack.Children.Add(thisBut);
            thisBut = GetKeeperButton();
            thisStack.Children.Add(thisBut);
        }
    }
}