using BasicGameFrameworkLibrary.BasicEventModels;
using BasicGameFrameworkLibrary.TestUtilities;
using BasicGamingUIXFLibrary.BasicControls.GameFrames;
using BasicGamingUIXFLibrary.BasicControls.SimpleControls;
using BasicGamingUIXFLibrary.BasicControls.SingleCardFrames;
using BasicGamingUIXFLibrary.GameGraphics.Cards;
using BasicGamingUIXFLibrary.Helpers;
using BasicXFControlsAndPages.MVVMFramework.Controls;
using CommonBasicStandardLibraries.Messenging;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using CribbageCP.Data;
using CribbageCP.ViewModels;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using Xamarin.Forms;
using static BasicGamingUIXFLibrary.Helpers.SharedUIFunctions; //this usually will be used too.
using static BasicXFControlsAndPages.Helpers.GridHelper;
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;
using ts = BasicGameFrameworkLibrary.GameGraphicsCP.Cards.DeckOfCardsCP;

namespace CribbageXF.Views
{
    public class CribbageMainView : ContentView, IUIView, IHandleAsync<LoadEventModel>
    {
        private readonly IEventAggregator _aggregator;
        private readonly CribbageVMData _model;
        private readonly BaseDeckXF<CribbageCard, ts, DeckOfCardsXF<CribbageCard>> _deckGPile;
        private readonly BasePileXF<CribbageCard, ts, DeckOfCardsXF<CribbageCard>> _discardGPile;

        private readonly ScoreBoardXF _score;
        private readonly BaseHandXF<CribbageCard, ts, DeckOfCardsXF<CribbageCard>> _playerHandWPF;

        private readonly BaseHandXF<CribbageCard, ts, DeckOfCardsXF<CribbageCard>> _crib1;
        private readonly BaseHandXF<CribbageCard, ts, DeckOfCardsXF<CribbageCard>> _main1;
        private readonly ScoreUI _otherScore = new ScoreUI();

        public CribbageMainView(IEventAggregator aggregator,
            TestOptions test,
            CribbageVMData model
            )
        {
            _aggregator = aggregator;
            _model = model;
            _aggregator.Subscribe(this);

            _deckGPile = new BaseDeckXF<CribbageCard, ts, DeckOfCardsXF<CribbageCard>>();
            _discardGPile = new BasePileXF<CribbageCard, ts, DeckOfCardsXF<CribbageCard>>();
            _score = new ScoreBoardXF();
            _playerHandWPF = new BaseHandXF<CribbageCard, ts, DeckOfCardsXF<CribbageCard>>();

            _crib1 = new BaseHandXF<CribbageCard, ts, DeckOfCardsXF<CribbageCard>>();
            _main1 = new BaseHandXF<CribbageCard, ts, DeckOfCardsXF<CribbageCard>>();
            _otherScore = new ScoreUI();
            _main1.Divider = 1.5;


            StackLayout mainStack = new StackLayout();
            ParentSingleUIContainer? restoreP = null;
            if (test.SaveOption == EnumTestSaveCategory.RestoreOnly)
            {
                restoreP = new ParentSingleUIContainer(nameof(CribbageMainViewModel.RestoreScreen));
            }

            StackLayout otherStack = new StackLayout();
            otherStack.Orientation = StackOrientation.Horizontal;
            otherStack.Children.Add(_deckGPile);
            otherStack.Children.Add(_discardGPile); // can reposition or not even have as well.
            otherStack.Children.Add(_main1);
            mainStack.Children.Add(otherStack);
            _score.AddColumn("Cards Left", false, nameof(CribbagePlayerItem.ObjectCount)); //very common.
            _score.AddColumn("Skunk Hole", false, nameof(CribbagePlayerItem.IsSkunk), useTrueFalse: true);
            _score.AddColumn("First P", false, nameof(CribbagePlayerItem.FirstPosition));
            _score.AddColumn("Second P", false, nameof(CribbagePlayerItem.SecondPosition));
            _score.AddColumn("Score Round", false, nameof(CribbagePlayerItem.ScoreRound));
            _score.AddColumn("Total Score", false, nameof(CribbagePlayerItem.TotalScore));
            SimpleLabelGridXF firstInfo = new SimpleLabelGridXF();

            firstInfo.AddRow("Turn", nameof(CribbageMainViewModel.NormalTurn));
            firstInfo.AddRow("Status", nameof(CribbageMainViewModel.Status));
            firstInfo.AddRow("Dealer", nameof(CribbageMainViewModel.Dealer));
            SimpleLabelGridXF others = new SimpleLabelGridXF();
            others.AddRow("Count", nameof(CribbageMainViewModel.TotalCount));
            mainStack.Children.Add(_playerHandWPF);
            otherStack = new StackLayout();
            otherStack.Orientation = StackOrientation.Horizontal;
            mainStack.Children.Add(otherStack);
            var button = GetGamingButton("Continue", nameof(CribbageMainViewModel.ContinueAsync));
            otherStack.Children.Add(button);
            button = GetGamingButton("To Crib", nameof(CribbageMainViewModel.CribAsync));
            otherStack.Children.Add(button);
            button = GetGamingButton("Play", nameof(CribbageMainViewModel.PlayAsync));
            otherStack.Children.Add(button);
            otherStack = new StackLayout();
            otherStack.Orientation = StackOrientation.Horizontal;
            mainStack.Children.Add(otherStack);
            otherStack.Children.Add(firstInfo.GetContent);
            otherStack.Children.Add(_crib1);
            Grid finalGrid = new Grid();
            AddPixelRow(finalGrid, 300); // hopefully this is enough
            AddLeftOverRow(finalGrid, 1);
            AddLeftOverColumn(finalGrid, 70);
            AddLeftOverColumn(finalGrid, 30);
            AddControlToGrid(finalGrid, mainStack, 0, 0);
            Grid.SetRowSpan(mainStack, 2);
            StackLayout finalStack = new StackLayout();
            finalStack.Children.Add(others.GetContent);
            finalStack.Children.Add(_otherScore);
            AddControlToGrid(finalGrid, finalStack, 0, 1);
            AddControlToGrid(finalGrid, _score, 1, 1);


            _deckGPile.Margin = new Thickness(5, 5, 5, 5);

            _discardGPile.Margin = new Thickness(5, 5, 5, 5);

            if (restoreP != null)
            {
                //todo:  figure out where to place the restore ui if there is a restore option.
                mainStack.Children.Add(restoreP); //default add to grid but does not have to.
            }
            Content = finalGrid;
        }
        Task IHandleAsync<LoadEventModel>.HandleAsync(LoadEventModel message)
        {
            GamePackageViewModelBinder.ManuelElements.Clear(); //often times i have to add manually.

            CribbageSaveInfo save = cons!.Resolve<CribbageSaveInfo>(); //usually needs this part for multiplayer games.

            _score!.LoadLists(save.PlayerList);
            _playerHandWPF!.LoadList(_model.PlayerHand1!, ts.TagUsed); // i think
            _discardGPile!.Init(_model.Pile1!, ts.TagUsed); // may have to be here (well see)
            _discardGPile.StartListeningDiscardPile(); // its the main one.

            _deckGPile!.Init(_model.Deck1!, ts.TagUsed); // try here.  may have to do something else as well (?)
            _deckGPile.StartListeningMainDeck();

            _crib1!.LoadList(_model!.CribFrame!, ts.TagUsed);
            _main1!.LoadList(_model.MainFrame!, ts.TagUsed);
            _otherScore.LoadLists(_model);

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
            return Task.CompletedTask;
        }
    }
}
