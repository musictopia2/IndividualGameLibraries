using BasicControlsAndWindowsCore.Controls.NavigationControls;
using BasicGameFrameworkLibrary.BasicEventModels;
using BasicGameFrameworkLibrary.DIContainers;
using BasicGameFrameworkLibrary.Extensions;
using BasicGameFrameworkLibrary.GameGraphicsCP.Interfaces;
using BasicGameFrameworkLibrary.RegularDeckOfCards;
using BasicGameFrameworkLibrary.TestUtilities;
using BasicGamingUIWPFLibrary.BasicControls.GameFrames;
using BasicGamingUIWPFLibrary.BasicControls.SimpleControls;
using BasicGamingUIWPFLibrary.BasicControls.SingleCardFrames;
using BasicGamingUIWPFLibrary.BasicControls.TrickUIs;
using BasicGamingUIWPFLibrary.GameGraphics.Cards;
using BasicGamingUIWPFLibrary.Helpers;
using CommonBasicStandardLibraries.Messenging;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using SixtySix2PlayerCP.Cards;
using SixtySix2PlayerCP.Data;
using SixtySix2PlayerCP.ViewModels;
using SkiaSharp;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using System.Windows;
using System.Windows.Controls;
using static BasicControlsAndWindowsCore.Helpers.GridHelper; //usually needs this as well for grid helpers.
using static BasicGamingUIWPFLibrary.Helpers.SharedUIFunctions; //this usually will be used too.
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;
using ts = BasicGameFrameworkLibrary.GameGraphicsCP.Cards.DeckOfCardsCP;
namespace SixtySix2PlayerWPF.Views
{
    public class SixtySix2PlayerMainView : UserControl, IUIView, IHandleAsync<LoadEventModel>
    {
        private readonly IEventAggregator _aggregator;
        private readonly SixtySix2PlayerVMData _model;
        private readonly IGamePackageResolver _resolver;
        private readonly BaseDeckWPF<SixtySix2PlayerCardInformation, ts, DeckOfCardsWPF<SixtySix2PlayerCardInformation>> _deckGPile;
        private readonly BasePileWPF<SixtySix2PlayerCardInformation, ts, DeckOfCardsWPF<SixtySix2PlayerCardInformation>> _discardGPile;

        private readonly ScoreBoardWPF _score;
        private readonly BaseHandWPF<SixtySix2PlayerCardInformation, ts, DeckOfCardsWPF<SixtySix2PlayerCardInformation>> _playerHandWPF;

        private readonly TwoPlayerTrickWPF<EnumSuitList, SixtySix2PlayerCardInformation, ts, DeckOfCardsWPF<SixtySix2PlayerCardInformation>> _trick1;

        private readonly GuideUI _guide1 = new GuideUI();
        private readonly BaseHandWPF<SixtySix2PlayerCardInformation, ts, DeckOfCardsWPF<SixtySix2PlayerCardInformation>> _marriage1;
        private readonly StackPanel _deckStack;

        public SixtySix2PlayerMainView(IEventAggregator aggregator,
            TestOptions test,
            SixtySix2PlayerVMData model,
            IGamePackageResolver resolver
            )
        {
            _aggregator = aggregator;
            _model = model;
            _resolver = resolver;
            _aggregator.Subscribe(this);

            _deckGPile = new BaseDeckWPF<SixtySix2PlayerCardInformation, ts, DeckOfCardsWPF<SixtySix2PlayerCardInformation>>();
            _discardGPile = new BasePileWPF<SixtySix2PlayerCardInformation, ts, DeckOfCardsWPF<SixtySix2PlayerCardInformation>>();
            _score = new ScoreBoardWPF();
            _playerHandWPF = new BaseHandWPF<SixtySix2PlayerCardInformation, ts, DeckOfCardsWPF<SixtySix2PlayerCardInformation>>();

            _trick1 = new TwoPlayerTrickWPF<EnumSuitList, SixtySix2PlayerCardInformation, ts, DeckOfCardsWPF<SixtySix2PlayerCardInformation>>();
            _deckStack = new StackPanel();
            _marriage1 = new BaseHandWPF<SixtySix2PlayerCardInformation, ts, DeckOfCardsWPF<SixtySix2PlayerCardInformation>>();
            _guide1 = new GuideUI();


            StackPanel mainStack = new StackPanel();
            ParentSingleUIContainer? restoreP = null;
            if (test.SaveOption == EnumTestSaveCategory.RestoreOnly)
            {
                restoreP = new ParentSingleUIContainer()
                {
                    Name = nameof(SixtySix2PlayerMainViewModel.RestoreScreen)
                };
            }


            StackPanel otherStack = new StackPanel();
            otherStack.Orientation = Orientation.Horizontal;
            otherStack.Children.Add(_deckStack);
            _deckStack.Children.Add(_discardGPile); // can reposition or not even have as well.
            mainStack.Children.Add(otherStack);
            _score.AddColumn("Cards Left", true, nameof(SixtySix2PlayerPlayerItem.ObjectCount));
            _score.AddColumn("Tricks Won", true, nameof(SixtySix2PlayerPlayerItem.TricksWon));
            _score.AddColumn("Score Round", true, nameof(SixtySix2PlayerPlayerItem.ScoreRound));
            _score.AddColumn("Game Points Round", true, nameof(SixtySix2PlayerPlayerItem.GamePointsRound));
            _score.AddColumn("Total Points Game", true, nameof(SixtySix2PlayerPlayerItem.GamePointsGame));
            SimpleLabelGrid firstInfo = new SimpleLabelGrid();
            firstInfo.AddRow("Turn", nameof(SixtySix2PlayerMainViewModel.NormalTurn));
            firstInfo.AddRow("Trump", nameof(SixtySix2PlayerMainViewModel.TrumpSuit));
            firstInfo.AddRow("Status", nameof(SixtySix2PlayerMainViewModel.Status));
            firstInfo.AddRow("Bonus", nameof(SixtySix2PlayerMainViewModel.BonusPoints));
            otherStack.Children.Add(_trick1);
            mainStack.Children.Add(_playerHandWPF);
            mainStack.Children.Add(firstInfo.GetContent);

            mainStack.Children.Add(_score);
            otherStack = new StackPanel();
            otherStack.Orientation = Orientation.Horizontal;
            var thisBut = GetGamingButton("Go Out", nameof(SixtySix2PlayerMainViewModel.GoOutAsync));
            otherStack.Children.Add(thisBut);
            thisBut = GetGamingButton("Announce Marriage", nameof(SixtySix2PlayerMainViewModel.AnnouceMarriageAsync));
            otherStack.Children.Add(thisBut);
            mainStack.Children.Add(otherStack);
            mainStack.Children.Add(_marriage1);
            Grid finalGrid = new Grid();
            AddLeftOverColumn(finalGrid, 60);
            AddLeftOverColumn(finalGrid, 40); // hopefully that works.
            AddControlToGrid(finalGrid, mainStack, 0, 0);
            AddControlToGrid(finalGrid, _guide1, 0, 1);

            _deckGPile.Margin = new Thickness(5, 5, 5, 5);

            _discardGPile.Margin = new Thickness(5, 5, 5, 5);


            if (restoreP != null)
            {
                mainStack.Children.Add(restoreP); //default add to grid but does not have to.
            }
            Content = finalGrid;

        }
        Task IHandleAsync<LoadEventModel>.HandleAsync(LoadEventModel message)
        {

            GamePackageViewModelBinder.ManuelElements.Clear(); //often times i have to add manually.

            SixtySix2PlayerSaveInfo save = cons!.Resolve<SixtySix2PlayerSaveInfo>(); //usually needs this part for multiplayer games.

            _score!.LoadLists(save.PlayerList);
            _playerHandWPF!.LoadList(_model.PlayerHand1!, ts.TagUsed); // i think
            _discardGPile!.Init(_model.Pile1!, ts.TagUsed); // may have to be here (well see)
            _discardGPile.StartListeningDiscardPile(); // its the main one.

            _deckGPile!.Init(_model.Deck1!, ts.TagUsed); // try here.  may have to do something else as well (?)
            _deckGPile.StartListeningMainDeck();
            _trick1!.Init(_model.TrickArea1!, ts.TagUsed);

            _guide1.LoadList(_model);
            _marriage1!.LoadList(_model.Marriage1!, ts.TagUsed);
            var thisCard = new SixtySix2PlayerCardInformation();
            IProportionImage thisP = _resolver.Resolve<IProportionImage>(ts.TagUsed);
            SKSize thisSize = thisCard.DefaultSize.GetSizeUsed(thisP.Proportion);
            var heights = thisSize.Height / 1.5f;
            _deckGPile.Margin = new Thickness(6, heights * -1, 0, 0);
            _deckStack!.Children.Add(_deckGPile);

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
