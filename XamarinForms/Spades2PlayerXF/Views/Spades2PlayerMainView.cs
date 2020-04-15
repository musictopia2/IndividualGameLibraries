using BasicGameFrameworkLibrary.BasicEventModels;
using BasicGameFrameworkLibrary.RegularDeckOfCards;
using BasicGameFrameworkLibrary.TestUtilities;
using BasicGamingUIXFLibrary.BasicControls.GameFrames;
using BasicGamingUIXFLibrary.BasicControls.SimpleControls;
using BasicGamingUIXFLibrary.BasicControls.TrickUIs;
using BasicGamingUIXFLibrary.GameGraphics.Cards;
using BasicGamingUIXFLibrary.Helpers;
using BasicXFControlsAndPages.MVVMFramework.Controls;
using CommonBasicStandardLibraries.Messenging;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using Spades2PlayerCP.Cards;
using Spades2PlayerCP.Data;
using Spades2PlayerCP.ViewModels;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using Xamarin.Forms;
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;
using ts = BasicGameFrameworkLibrary.GameGraphicsCP.Cards.DeckOfCardsCP;

namespace Spades2PlayerXF.Views
{
    public class Spades2PlayerMainView : ContentView, IUIView, IHandleAsync<LoadEventModel>
    {
        private readonly IEventAggregator _aggregator;
        private readonly Spades2PlayerVMData _model;

        private readonly ScoreBoardXF _score;
        private readonly BaseHandXF<Spades2PlayerCardInformation, ts, DeckOfCardsXF<Spades2PlayerCardInformation>> _playerHandWPF;

        private readonly TwoPlayerTrickXF<EnumSuitList, Spades2PlayerCardInformation, ts, DeckOfCardsXF<Spades2PlayerCardInformation>> _trick1;

        public Spades2PlayerMainView(IEventAggregator aggregator,
            TestOptions test,
            Spades2PlayerVMData model
            )
        {
            _aggregator = aggregator;
            _model = model;
            _aggregator.Subscribe(this);

            _score = new ScoreBoardXF();
            _trick1 = new TwoPlayerTrickXF<EnumSuitList, Spades2PlayerCardInformation, ts, DeckOfCardsXF<Spades2PlayerCardInformation>>();
            _playerHandWPF = new BaseHandXF<Spades2PlayerCardInformation, ts, DeckOfCardsXF<Spades2PlayerCardInformation>>();

            StackLayout mainStack = new StackLayout();
            ParentSingleUIContainer? restoreP = null;
            if (test.SaveOption == EnumTestSaveCategory.RestoreOnly)
            {
                restoreP = new ParentSingleUIContainer(nameof(Spades2PlayerMainViewModel.RestoreScreen));
            }

            ParentSingleUIContainer parent = new ParentSingleUIContainer(nameof(Spades2PlayerMainViewModel.BeginningScreen));
            mainStack.Children.Add(parent);

            _score.AddColumn("Cards", false, nameof(Spades2PlayerPlayerItem.ObjectCount)); //very common.
            _score.AddColumn("Bidded", false, nameof(Spades2PlayerPlayerItem.HowManyBids));
            _score.AddColumn("Won", false, nameof(Spades2PlayerPlayerItem.TricksWon));
            _score.AddColumn("Bags", false, nameof(Spades2PlayerPlayerItem.Bags));
            _score.AddColumn("C Score", false, nameof(Spades2PlayerPlayerItem.CurrentScore));
            _score.AddColumn("T Score", false, nameof(Spades2PlayerPlayerItem.TotalScore)); SimpleLabelGridXF firstInfo = new SimpleLabelGridXF();
            firstInfo.AddRow("Turn", nameof(Spades2PlayerMainViewModel.NormalTurn));
            firstInfo.AddRow("Status", nameof(Spades2PlayerMainViewModel.Status));
            firstInfo.AddRow("Round", nameof(Spades2PlayerMainViewModel.RoundNumber));

            mainStack.Children.Add(_trick1);
            parent = new ParentSingleUIContainer(nameof(Spades2PlayerMainViewModel.BiddingScreen));
            mainStack.Children.Add(parent);
            mainStack.Children.Add(_playerHandWPF);
            StackLayout other = new StackLayout();
            other.Orientation = StackOrientation.Horizontal;
            mainStack.Children.Add(other);
            other.Children.Add(firstInfo.GetContent);
            other.Children.Add(_score);
            

            if (restoreP != null)
            {
                mainStack.Children.Add(restoreP); //default add to grid but does not have to.
            }
            Content = mainStack;
        }
        Task IHandleAsync<LoadEventModel>.HandleAsync(LoadEventModel message)
        {
            GamePackageViewModelBinder.ManuelElements.Clear(); //often times i have to add manually.

            Spades2PlayerSaveInfo save = cons!.Resolve<Spades2PlayerSaveInfo>(); //usually needs this part for multiplayer games.

            _score!.LoadLists(save.PlayerList);
            _playerHandWPF!.LoadList(_model.PlayerHand1!, ts.TagUsed); // i think
            _trick1!.Init(_model.TrickArea1!, ts.TagUsed);
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
            _trick1.Dispose();
            _playerHandWPF.Dispose();
            return Task.CompletedTask;
        }
    }
}
