using BasicGameFrameworkLibrary.BasicEventModels;
using BasicGameFrameworkLibrary.DrawableListsObservable;
using BasicGameFrameworkLibrary.TestUtilities;
using BasicGamingUIXFLibrary.BasicControls.GameFrames;
using BasicGamingUIXFLibrary.BasicControls.SimpleControls;
using BasicGamingUIXFLibrary.BasicControls.SingleCardFrames;
using BasicGamingUIXFLibrary.Helpers;
using BasicXFControlsAndPages.MVVMFramework.Controls;
using CommonBasicStandardLibraries.Messenging;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using Xamarin.Forms;
using YahtzeeHandsDownCP.Cards;
using YahtzeeHandsDownCP.Data;
using YahtzeeHandsDownCP.ViewModels;
using static BasicGamingUIXFLibrary.Helpers.SharedUIFunctions; //this usually will be used too.
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;

namespace YahtzeeHandsDownXF.Views
{
    public class YahtzeeHandsDownMainView : ContentView, IUIView, IHandleAsync<LoadEventModel>
    {
        private readonly IEventAggregator _aggregator;
        private readonly YahtzeeHandsDownVMData _model;
        private readonly BaseDeckXF<YahtzeeHandsDownCardInformation, YahtzeeHandsDownGraphicsCP, CardGraphicsXF> _deckGPile;
        private readonly BasePileXF<YahtzeeHandsDownCardInformation, YahtzeeHandsDownGraphicsCP, CardGraphicsXF> _discardGPile;

        private readonly ScoreBoardXF _score;
        private readonly BaseHandXF<YahtzeeHandsDownCardInformation, YahtzeeHandsDownGraphicsCP, CardGraphicsXF> _playerHandWPF;

        private readonly ComboHandXF _combo1;
        private readonly ChanceSinglePileXF _chance1;
        public YahtzeeHandsDownMainView(IEventAggregator aggregator,
            TestOptions test,
            YahtzeeHandsDownVMData model
            )
        {
            _aggregator = aggregator;
            _model = model;
            _aggregator.Subscribe(this);

            _deckGPile = new BaseDeckXF<YahtzeeHandsDownCardInformation, YahtzeeHandsDownGraphicsCP, CardGraphicsXF>();
            _discardGPile = new BasePileXF<YahtzeeHandsDownCardInformation, YahtzeeHandsDownGraphicsCP, CardGraphicsXF>();
            _score = new ScoreBoardXF();
            _playerHandWPF = new BaseHandXF<YahtzeeHandsDownCardInformation, YahtzeeHandsDownGraphicsCP, CardGraphicsXF>();
            _combo1 = new ComboHandXF();
            _chance1 = new ChanceSinglePileXF();

            StackLayout mainStack = new StackLayout();
            ParentSingleUIContainer? restoreP = null;
            if (test.SaveOption == EnumTestSaveCategory.RestoreOnly)
            {
                restoreP = new ParentSingleUIContainer(nameof(YahtzeeHandsDownMainViewModel.RestoreScreen));
            }

            StackLayout otherStack = new StackLayout();
            otherStack.Orientation = StackOrientation.Horizontal;
            otherStack.Children.Add(_deckGPile);
            otherStack.Children.Add(_discardGPile); // can reposition or not even have as well.
            _chance1.Margin = new Thickness(5, 5, 5, 5);
            _chance1.HorizontalOptions = LayoutOptions.Start;
            _chance1.VerticalOptions = LayoutOptions.Start;
            otherStack.Children.Add(_chance1);
            mainStack.Children.Add(otherStack);
            _score.AddColumn("Cards Left", true, nameof(YahtzeeHandsDownPlayerItem.ObjectCount)); //very common.
            _score.AddColumn("Total Score", true, nameof(YahtzeeHandsDownPlayerItem.TotalScore));
            _score.AddColumn("Won Last Round", true, nameof(YahtzeeHandsDownPlayerItem.WonLastRound));
            _score.AddColumn("Score Round", true, nameof(YahtzeeHandsDownPlayerItem.ScoreRound));
            SimpleLabelGridXF firstInfo = new SimpleLabelGridXF();
            firstInfo.AddRow("Turn", nameof(YahtzeeHandsDownMainViewModel.NormalTurn));
            firstInfo.AddRow("Status", nameof(YahtzeeHandsDownMainViewModel.Status));
            mainStack.Children.Add(_playerHandWPF);

            var otherButton = GetGamingButton("Go Out", nameof(YahtzeeHandsDownMainViewModel.GoOutAsync));
            mainStack.Children.Add(otherButton);
            var endButton = GetGamingButton("End Turn", nameof(YahtzeeHandsDownMainViewModel.EndTurnAsync));

            mainStack.Children.Add(endButton);
            mainStack.Children.Add(firstInfo.GetContent);
            mainStack.Children.Add(_score);

            otherStack = new StackLayout();
            otherStack.Orientation = StackOrientation.Horizontal;
            otherStack.Children.Add(mainStack);
            _combo1.HandType = HandObservable<ComboCardInfo>.EnumHandList.Vertical;
            otherStack.Children.Add(_combo1);

            _deckGPile.Margin = new Thickness(5, 5, 5, 5);

            _discardGPile.Margin = new Thickness(5, 5, 5, 5);

            if (restoreP != null)
            {
                mainStack.Children.Add(restoreP); //default add to grid but does not have to.
            }
            Content = otherStack;
        }
        Task IHandleAsync<LoadEventModel>.HandleAsync(LoadEventModel message)
        {
            GamePackageViewModelBinder.ManuelElements.Clear(); //often times i have to add manually.

            YahtzeeHandsDownSaveInfo save = cons!.Resolve<YahtzeeHandsDownSaveInfo>(); //usually needs this part for multiplayer games.

            _score!.LoadLists(save.PlayerList);
            _playerHandWPF!.LoadList(_model.PlayerHand1!, ""); // i think
            _discardGPile!.Init(_model.Pile1!, ""); // may have to be here (well see)
            _discardGPile.StartListeningDiscardPile(); // its the main one.

            _deckGPile!.Init(_model.Deck1!, ""); // try here.  may have to do something else as well (?)
            _deckGPile.StartListeningMainDeck();
            _combo1!.LoadList(_model.ComboHandList!, "combo");
            _chance1!.Init(_model.ChancePile!, "");
            return this.RefreshBindingsAsync(_aggregator);
        }


        object IUIView.DataContext
        {
            get => BindingContext;
            set => BindingContext = value;
        }
        Task IUIView.TryActivateAsync()
        {
            return Task.CompletedTask;
        }

        Task IUIView.TryCloseAsync()
        {
            _playerHandWPF.Dispose(); //at least will help improve performance.
            return Task.CompletedTask;
        }
    }
}
