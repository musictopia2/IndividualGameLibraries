using BasicGameFrameworkLibrary.BasicEventModels;
using BasicGameFrameworkLibrary.DIContainers;
using BasicGameFrameworkLibrary.Extensions;
using BasicGameFrameworkLibrary.GameGraphicsCP.Interfaces;
using BasicGameFrameworkLibrary.RegularDeckOfCards;
using BasicGameFrameworkLibrary.TestUtilities;
using BasicGamingUIXFLibrary.BasicControls.GameFrames;
using BasicGamingUIXFLibrary.BasicControls.SimpleControls;
using BasicGamingUIXFLibrary.BasicControls.SingleCardFrames;
using BasicGamingUIXFLibrary.BasicControls.TrickUIs;
using BasicGamingUIXFLibrary.GameGraphics.Cards;
using BasicGamingUIXFLibrary.Helpers;
using BasicXFControlsAndPages.MVVMFramework.Controls;
using CommonBasicStandardLibraries.Messenging;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using SixtySix2PlayerCP.Cards;
using SixtySix2PlayerCP.Data;
using SixtySix2PlayerCP.ViewModels;
using SkiaSharp;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using Xamarin.Forms;
using static BasicGamingUIXFLibrary.Helpers.SharedUIFunctions; //this usually will be used too.
using static BasicXFControlsAndPages.Helpers.GridHelper;
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;
using ts = BasicGameFrameworkLibrary.GameGraphicsCP.Cards.DeckOfCardsCP;

namespace SixtySix2PlayerXF.Views
{
    public class SixtySix2PlayerMainView : ContentView, IUIView, IHandleAsync<LoadEventModel>
    {
        private readonly IEventAggregator _aggregator;
        private readonly SixtySix2PlayerVMData _model;
        private readonly IGamePackageResolver _resolver;
        private readonly BaseDeckXF<SixtySix2PlayerCardInformation, ts, DeckOfCardsXF<SixtySix2PlayerCardInformation>> _deckGPile;
        private readonly BasePileXF<SixtySix2PlayerCardInformation, ts, DeckOfCardsXF<SixtySix2PlayerCardInformation>> _discardGPile;

        private readonly ScoreBoardXF _score;
        private readonly BaseHandXF<SixtySix2PlayerCardInformation, ts, DeckOfCardsXF<SixtySix2PlayerCardInformation>> _playerHandWPF;
        private readonly TwoPlayerTrickXF<EnumSuitList, SixtySix2PlayerCardInformation, ts, DeckOfCardsXF<SixtySix2PlayerCardInformation>> _trick1;
        private readonly GuideUI _guide1 = new GuideUI();
        private readonly BaseHandXF<SixtySix2PlayerCardInformation, ts, DeckOfCardsXF<SixtySix2PlayerCardInformation>> _marriage1;
        private readonly StackLayout _deckStack;

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

            _deckGPile = new BaseDeckXF<SixtySix2PlayerCardInformation, ts, DeckOfCardsXF<SixtySix2PlayerCardInformation>>();
            _discardGPile = new BasePileXF<SixtySix2PlayerCardInformation, ts, DeckOfCardsXF<SixtySix2PlayerCardInformation>>();
            _score = new ScoreBoardXF();
            _playerHandWPF = new BaseHandXF<SixtySix2PlayerCardInformation, ts, DeckOfCardsXF<SixtySix2PlayerCardInformation>>();
            _trick1 = new TwoPlayerTrickXF<EnumSuitList, SixtySix2PlayerCardInformation, ts, DeckOfCardsXF<SixtySix2PlayerCardInformation>>();
            _deckStack = new StackLayout();
            _marriage1 = new BaseHandXF<SixtySix2PlayerCardInformation, ts, DeckOfCardsXF<SixtySix2PlayerCardInformation>>();

            StackLayout mainStack = new StackLayout();
            ParentSingleUIContainer? restoreP = null;
            if (test.SaveOption == EnumTestSaveCategory.RestoreOnly)
            {
                restoreP = new ParentSingleUIContainer(nameof(SixtySix2PlayerMainViewModel.RestoreScreen));
            }

            StackLayout otherStack = new StackLayout();
            otherStack.Orientation = StackOrientation.Horizontal;
            otherStack.Children.Add(_deckStack);
            _deckStack.Children.Add(_discardGPile); // can reposition or not even have as well.
            otherStack.Children.Add(_trick1);
            mainStack.Children.Add(otherStack);
            _score.AddColumn("Cards Left", true, nameof(SixtySix2PlayerPlayerItem.ObjectCount));
            _score.AddColumn("Tricks Won", true, nameof(SixtySix2PlayerPlayerItem.TricksWon));
            _score.AddColumn("Score Round", true, nameof(SixtySix2PlayerPlayerItem.ScoreRound));
            _score.AddColumn("Game Points Round", true, nameof(SixtySix2PlayerPlayerItem.GamePointsRound));
            _score.AddColumn("Total Points Game", true, nameof(SixtySix2PlayerPlayerItem.GamePointsGame));
            SimpleLabelGridXF firstInfo = new SimpleLabelGridXF();
            firstInfo.AddRow("Turn", nameof(SixtySix2PlayerMainViewModel.NormalTurn));
            firstInfo.AddRow("Trump", nameof(SixtySix2PlayerMainViewModel.TrumpSuit));
            firstInfo.AddRow("Status", nameof(SixtySix2PlayerMainViewModel.Status));
            firstInfo.AddRow("Bonus", nameof(SixtySix2PlayerMainViewModel.BonusPoints));
            mainStack.Children.Add(_playerHandWPF);
            mainStack.Children.Add(firstInfo.GetContent);

            mainStack.Children.Add(_score);
            otherStack = new StackLayout();
            otherStack.Orientation = StackOrientation.Horizontal;

            var thisBut = GetGamingButton("Go Out", nameof(SixtySix2PlayerMainViewModel.GoOutAsync));
            otherStack.Children.Add(thisBut);
            thisBut = GetGamingButton("Announce Marriage", nameof(SixtySix2PlayerMainViewModel.AnnouceMarriageAsync));
            otherStack.Children.Add(thisBut);
            mainStack.Children.Add(otherStack);
            mainStack.Children.Add(_guide1);
            mainStack.Children.Add(_marriage1);
            


            _deckGPile.Margin = new Thickness(5, 5, 5, 5);

            _discardGPile.Margin = new Thickness(5, 5, 5, 5);

            if (restoreP != null)
            {
                //todo:  figure out where to place the restore ui if there is a restore option.
                mainStack.Children.Add(restoreP); //default add to grid but does not have to.
            }
            Content = mainStack;
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
            _trick1.Dispose();
            return Task.CompletedTask;
        }
    }
}
