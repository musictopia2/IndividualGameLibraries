using BasicControlsAndWindowsCore.Controls.NavigationControls;
using BasicGameFrameworkLibrary.BasicEventModels;
using BasicGameFrameworkLibrary.RegularDeckOfCards;
using BasicGameFrameworkLibrary.TestUtilities;
using BasicGamingUIWPFLibrary.BasicControls.GameFrames;
using BasicGamingUIWPFLibrary.BasicControls.SimpleControls;
using BasicGamingUIWPFLibrary.BasicControls.SingleCardFrames;
using BasicGamingUIWPFLibrary.BasicControls.TrickUIs;
using BasicGamingUIWPFLibrary.GameGraphics.Cards;
using BasicGamingUIWPFLibrary.Helpers;
using CaliforniaJackCP.Cards;
using CaliforniaJackCP.Data;
using CaliforniaJackCP.ViewModels;
using CommonBasicStandardLibraries.Messenging;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using System.Windows;
using System.Windows.Controls;
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;
using ts = BasicGameFrameworkLibrary.GameGraphicsCP.Cards.DeckOfCardsCP;

namespace CaliforniaJackWPF.Views
{
    public class CaliforniaJackMainView : UserControl, IUIView, IHandleAsync<LoadEventModel>
    {
        private readonly IEventAggregator _aggregator;
        private readonly CaliforniaJackVMData _model;
        private readonly BaseDeckWPF<CaliforniaJackCardInformation, ts, DeckOfCardsWPF<CaliforniaJackCardInformation>> _deckGPile;

        private readonly ScoreBoardWPF _score;
        private readonly BaseHandWPF<CaliforniaJackCardInformation, ts, DeckOfCardsWPF<CaliforniaJackCardInformation>> _playerHandWPF;

        private readonly TwoPlayerTrickWPF<EnumSuitList, CaliforniaJackCardInformation, ts, DeckOfCardsWPF<CaliforniaJackCardInformation>> _trick1;


        public CaliforniaJackMainView(IEventAggregator aggregator,
            TestOptions test,
            CaliforniaJackVMData model
            )
        {
            _aggregator = aggregator;
            _model = model;
            _aggregator.Subscribe(this);

            _deckGPile = new BaseDeckWPF<CaliforniaJackCardInformation, ts, DeckOfCardsWPF<CaliforniaJackCardInformation>>();
            _score = new ScoreBoardWPF();
            _playerHandWPF = new BaseHandWPF<CaliforniaJackCardInformation, ts, DeckOfCardsWPF<CaliforniaJackCardInformation>>();

            _trick1 = new TwoPlayerTrickWPF<EnumSuitList, CaliforniaJackCardInformation, ts, DeckOfCardsWPF<CaliforniaJackCardInformation>>();


            StackPanel mainStack = new StackPanel();
            ParentSingleUIContainer? restoreP = null;
            if (test.SaveOption == EnumTestSaveCategory.RestoreOnly)
            {
                restoreP = new ParentSingleUIContainer()
                {
                    Name = nameof(CaliforniaJackMainViewModel.RestoreScreen)
                };
            }


            StackPanel otherStack = new StackPanel();
            otherStack.Orientation = Orientation.Horizontal;
            otherStack.Children.Add(_deckGPile);
            mainStack.Children.Add(otherStack);
            _score.AddColumn("Cards Left", false, nameof(CaliforniaJackPlayerItem.ObjectCount)); //very common.
            _score.AddColumn("Tricks Won", true, nameof(CaliforniaJackPlayerItem.TricksWon), rightMargin: 10);
            _score.AddColumn("Points", true, nameof(CaliforniaJackPlayerItem.Points), rightMargin: 10);
            _score.AddColumn("Total Score", true, nameof(CaliforniaJackPlayerItem.TotalScore), rightMargin: 10);
            SimpleLabelGrid firstInfo = new SimpleLabelGrid();
            firstInfo.AddRow("Turn", nameof(CaliforniaJackMainViewModel.NormalTurn));
            firstInfo.AddRow("Trump", nameof(CaliforniaJackMainViewModel.TrumpSuit));
            firstInfo.AddRow("Status", nameof(CaliforniaJackMainViewModel.Status));
            mainStack.Children.Add(_trick1);
            mainStack.Children.Add(_playerHandWPF);
            mainStack.Children.Add(firstInfo.GetContent);
            mainStack.Children.Add(_score);

            _deckGPile.Margin = new Thickness(5, 5, 5, 5);



            if (restoreP != null)
            {
                mainStack.Children.Add(restoreP); //default add to grid but does not have to.
            }
            Content = mainStack;

        }
        Task IHandleAsync<LoadEventModel>.HandleAsync(LoadEventModel message)
        {

            GamePackageViewModelBinder.ManuelElements.Clear(); //often times i have to add manually.

            CaliforniaJackSaveInfo save = cons!.Resolve<CaliforniaJackSaveInfo>(); //usually needs this part for multiplayer games.

            _score!.LoadLists(save.PlayerList);
            _playerHandWPF!.LoadList(_model.PlayerHand1!, ts.TagUsed); // i think

            _deckGPile!.Init(_model.Deck1!, ts.TagUsed); // try here.  may have to do something else as well (?)
            _deckGPile.StartListeningMainDeck();
            _trick1!.Init(_model.TrickArea1!, ts.TagUsed);
            return this.RefreshBindingsAsync(_aggregator);
        }



        Task IUIView.TryActivateAsync()
        {
            return Task.CompletedTask;
        }

        Task IUIView.TryCloseAsync()
        {
            _trick1.Dispose();
            return Task.CompletedTask;
        }
    }
}
