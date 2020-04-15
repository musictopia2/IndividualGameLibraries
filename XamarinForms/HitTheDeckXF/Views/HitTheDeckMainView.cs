using BasicGameFrameworkLibrary.BasicEventModels;
using BasicGameFrameworkLibrary.TestUtilities;
using BasicGamingUIXFLibrary.BasicControls.GameFrames;
using BasicGamingUIXFLibrary.BasicControls.SimpleControls;
using BasicGamingUIXFLibrary.BasicControls.SingleCardFrames;
using BasicGamingUIXFLibrary.Helpers;
using BasicXFControlsAndPages.MVVMFramework.Controls;
using CommonBasicStandardLibraries.Messenging;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using HitTheDeckCP.Cards;
using HitTheDeckCP.Data;
using HitTheDeckCP.ViewModels;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using Xamarin.Forms;
using static BasicGamingUIXFLibrary.Helpers.SharedUIFunctions; //this usually will be used too.
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;

namespace HitTheDeckXF.Views
{
    public class HitTheDeckMainView : ContentView, IUIView, IHandleAsync<LoadEventModel>
    {
        private readonly IEventAggregator _aggregator;
        private readonly HitTheDeckVMData _model;
        private readonly BaseDeckXF<HitTheDeckCardInformation, HitTheDeckGraphicsCP, CardGraphicsXF> _deckGPile;
        private readonly BasePileXF<HitTheDeckCardInformation, HitTheDeckGraphicsCP, CardGraphicsXF> _discardGPile;

        private readonly ScoreBoardXF _score;
        private readonly BaseHandXF<HitTheDeckCardInformation, HitTheDeckGraphicsCP, CardGraphicsXF> _playerHandWPF;
        public HitTheDeckMainView(IEventAggregator aggregator,
            TestOptions test,
            HitTheDeckVMData model
            )
        {
            _aggregator = aggregator;
            _model = model;
            _aggregator.Subscribe(this);

            _deckGPile = new BaseDeckXF<HitTheDeckCardInformation, HitTheDeckGraphicsCP, CardGraphicsXF>();
            _discardGPile = new BasePileXF<HitTheDeckCardInformation, HitTheDeckGraphicsCP, CardGraphicsXF>();
            _score = new ScoreBoardXF();
            _playerHandWPF = new BaseHandXF<HitTheDeckCardInformation, HitTheDeckGraphicsCP, CardGraphicsXF>();
            StackLayout mainStack = new StackLayout();
            ParentSingleUIContainer? restoreP = null;
            if (test.SaveOption == EnumTestSaveCategory.RestoreOnly)
            {
                restoreP = new ParentSingleUIContainer(nameof(HitTheDeckMainViewModel.RestoreScreen));
            }

            StackLayout otherStack = new StackLayout();
            otherStack.Orientation = StackOrientation.Horizontal;
            otherStack.Children.Add(_deckGPile);
            otherStack.Children.Add(_discardGPile); // can reposition or not even have as well.

            StackLayout tempStack = new StackLayout();
            otherStack.Children.Add(tempStack);
            Button otherButs;
            otherButs = GetGamingButton("Flip Deck", nameof(HitTheDeckMainViewModel.FlipAsync));
            tempStack.Children.Add(otherButs);
            otherButs = GetGamingButton("Cut Deck", nameof(HitTheDeckMainViewModel.CutAsync));
            tempStack.Children.Add(otherButs);
            var endButton = GetGamingButton("End Turn", nameof(HitTheDeckMainViewModel.EndTurnAsync));
            endButton.HorizontalOptions = LayoutOptions.Start;
            tempStack.Children.Add(endButton);
            mainStack.Children.Add(otherStack);
            _score.AddColumn("Cards Left", true, nameof(HitTheDeckPlayerItem.ObjectCount), rightMargin: 5);
            _score.AddColumn("Total Points", true, nameof(HitTheDeckPlayerItem.TotalPoints), rightMargin: 5);
            _score.AddColumn("Previous Points", true, nameof(HitTheDeckPlayerItem.PreviousPoints), rightMargin: 5);
            SimpleLabelGridXF firstInfo = new SimpleLabelGridXF();
            firstInfo.AddRow("Turn", nameof(HitTheDeckMainViewModel.NormalTurn));
            firstInfo.AddRow("Next", nameof(HitTheDeckMainViewModel.NextPlayer));
            firstInfo.AddRow("Status", nameof(HitTheDeckMainViewModel.Status));


            mainStack.Children.Add(_playerHandWPF);
            mainStack.Children.Add(firstInfo.GetContent);

            mainStack.Children.Add(_score);


            _deckGPile.Margin = new Thickness(5, 5, 5, 5);

            _discardGPile.Margin = new Thickness(5, 5, 5, 5);

            if (restoreP != null)
            {
                mainStack.Children.Add(restoreP); //default add to grid but does not have to.
            }
            Content = mainStack;
        }
        Task IHandleAsync<LoadEventModel>.HandleAsync(LoadEventModel message)
        {
            GamePackageViewModelBinder.ManuelElements.Clear(); //often times i have to add manually.

            HitTheDeckSaveInfo save = cons!.Resolve<HitTheDeckSaveInfo>(); //usually needs this part for multiplayer games.

            _score!.LoadLists(save.PlayerList);
            _playerHandWPF!.LoadList(_model.PlayerHand1!, ""); // i think
            _discardGPile!.Init(_model.Pile1!, ""); // may have to be here (well see)
            _discardGPile.StartListeningDiscardPile(); // its the main one.

            _deckGPile!.Init(_model.Deck1!, ""); // try here.  may have to do something else as well (?)
            _deckGPile.StartListeningMainDeck();

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
