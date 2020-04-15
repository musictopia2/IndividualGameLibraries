using BasicGameFrameworkLibrary.BasicEventModels;
using BasicGameFrameworkLibrary.TestUtilities;
using BasicGamingUIXFLibrary.BasicControls.GameFrames;
using BasicGamingUIXFLibrary.BasicControls.SimpleControls;
using BasicGamingUIXFLibrary.BasicControls.SingleCardFrames;
using BasicXFControlsAndPages.MVVMFramework.Controls;
using CommonBasicStandardLibraries.Messenging;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using UnoCP.Cards;
using UnoCP.Data;
using UnoCP.ViewModels;
using Xamarin.Forms;
using static BasicGamingUIXFLibrary.Helpers.SharedUIFunctions; //this usually will be used too.
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;
using ts = BasicGameFrameworkLibrary.GameGraphicsCP.Cards.BaseColorCardsCP;

namespace UnoXF.Views
{
    public class UnoMainView : ContentView, IUIView, IHandleAsync<LoadEventModel>
    {
        private readonly IEventAggregator _aggregator;
        private readonly UnoVMData _model;
        private readonly BaseDeckXF<UnoCardInformation, UnoGraphicsCP, CardGraphicsXF> _deckGPile;
        private readonly BasePileXF<UnoCardInformation, UnoGraphicsCP, CardGraphicsXF> _discardGPile;

        private readonly ScoreBoardXF _score;
        private readonly BaseHandXF<UnoCardInformation, UnoGraphicsCP, CardGraphicsXF> _playerHandWPF;
        public UnoMainView(IEventAggregator aggregator,
            TestOptions test,
            UnoVMData model
            )
        {
            _aggregator = aggregator;
            _model = model;
            _aggregator.Subscribe(this);

            _deckGPile = new BaseDeckXF<UnoCardInformation, UnoGraphicsCP, CardGraphicsXF>();
            _discardGPile = new BasePileXF<UnoCardInformation, UnoGraphicsCP, CardGraphicsXF>();
            _score = new ScoreBoardXF();
            _playerHandWPF = new BaseHandXF<UnoCardInformation, UnoGraphicsCP, CardGraphicsXF>();
            StackLayout mainStack = new StackLayout();
            ParentSingleUIContainer? restoreP = null;
            if (test.SaveOption == EnumTestSaveCategory.RestoreOnly)
            {
                restoreP = new ParentSingleUIContainer(nameof(UnoMainViewModel.RestoreScreen));
            }

            StackLayout otherStack = new StackLayout();
            otherStack.Orientation = StackOrientation.Horizontal;
            otherStack.Children.Add(_deckGPile);
            otherStack.Children.Add(_discardGPile); // can reposition or not even have as well.

            var endButton = GetGamingButton("End Turn", nameof(UnoMainViewModel.EndTurnAsync));
            endButton.HorizontalOptions = LayoutOptions.Start;
            otherStack.Children.Add(endButton);
            ParentSingleUIContainer parent = new ParentSingleUIContainer(nameof(UnoMainViewModel.SayUnoScreen));
            otherStack.Children.Add(parent);

            mainStack.Children.Add(otherStack);
            _score.AddColumn("Cards Left", true, nameof(UnoPlayerItem.ObjectCount)); //very common.
            _score.AddColumn("Total Points", true, nameof(UnoPlayerItem.TotalPoints), rightMargin: 10);
            _score.AddColumn("Previous Points", true, nameof(UnoPlayerItem.PreviousPoints), rightMargin: 10);
            SimpleLabelGridXF firstInfo = new SimpleLabelGridXF();
            firstInfo.AddRow("Turn", nameof(UnoMainViewModel.NormalTurn));
            firstInfo.AddRow("Next", nameof(UnoMainViewModel.NextPlayer));
            firstInfo.AddRow("Status", nameof(UnoMainViewModel.Status));


            mainStack.Children.Add(_playerHandWPF);
            mainStack.Children.Add(firstInfo.GetContent);

            otherStack = new StackLayout();
            otherStack.Orientation = StackOrientation.Horizontal;
            mainStack.Children.Add(otherStack);
            otherStack.Children.Add(_score);
            //this is only a starting point.


            _deckGPile.Margin = new Thickness(5, 5, 5, 5);

            _discardGPile.Margin = new Thickness(5, 5, 5, 5);

            if (restoreP != null)
            {
                mainStack.Children.Add(restoreP); //default add to grid but does not have to.
            }
            UnoSaveInfo save = cons!.Resolve<UnoSaveInfo>(); //usually needs this part for multiplayer games.
            _score!.LoadLists(save.PlayerList);
            _playerHandWPF!.LoadList(_model.PlayerHand1!, ts.TagUsed); // i think
            _discardGPile!.Init(_model.Pile1!, ts.TagUsed); // may have to be here (well see)
            _discardGPile.StartListeningDiscardPile(); // its the main one.

            _deckGPile!.Init(_model.Deck1!, ts.TagUsed); // try here.  may have to do something else as well (?)
            _deckGPile.StartListeningMainDeck();


            Content = mainStack;
        }
        Task IHandleAsync<LoadEventModel>.HandleAsync(LoadEventModel message)
        {

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
