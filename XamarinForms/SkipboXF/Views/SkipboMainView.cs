using System;
using System.Text;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using System.Linq;
using CommonBasicStandardLibraries.BasicDataSettingsAndProcesses;
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;
using CommonBasicStandardLibraries.CollectionClasses;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using fs = CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.JsonSerializers.FileHelpers;
using js = CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.JsonSerializers.NewtonJsonStrings; //just in case i need those 2.
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using static BasicGamingUIXFLibrary.Helpers.SharedUIFunctions; //this usually will be used too.
using static BasicXFControlsAndPages.Helpers.GridHelper;
using CommonBasicStandardLibraries.Messenging;
using BasicGameFrameworkLibrary.BasicEventModels;
using SkipboCP.Data;
using Xamarin.Forms;
using BasicXFControlsAndPages.MVVMFramework.Controls;
using BasicGameFrameworkLibrary.TestUtilities;
using SkipboCP.ViewModels;
using BasicGamingUIXFLibrary.Helpers;
using BasicGamingUIXFLibrary.BasicControls.SingleCardFrames;
using BasicGamingUIXFLibrary.GameGraphics.Cards;
using BasicGamingUIXFLibrary.BasicControls.GameFrames;
using BasicGamingUIXFLibrary.BasicControls.SimpleControls;
using SkipboCP.Cards;
using BasicGamingUIXFLibrary.BasicControls.MultipleFrameContainers;

namespace SkipboXF.Views
{
    public class SkipboMainView : ContentView, IUIView, IHandleAsync<LoadEventModel>
    {
        private readonly IEventAggregator _aggregator;
        private readonly SkipboVMData _model;
        private readonly BaseDeckXF<SkipboCardInformation, SkipboGraphicsCP, CardGraphicsXF> _deckGPile;

        private readonly ScoreBoardXF _score;
        private readonly BaseHandXF<SkipboCardInformation, SkipboGraphicsCP, CardGraphicsXF> _playerHandWPF;

        private readonly BasicMultiplePilesXF<SkipboCardInformation, SkipboGraphicsCP, CardGraphicsXF> _publicGraphics;

        public SkipboMainView(IEventAggregator aggregator,
            TestOptions test,
            SkipboVMData model
            )
        {
            _aggregator = aggregator;
            _model = model;
            _aggregator.Subscribe(this);

            _deckGPile = new BaseDeckXF<SkipboCardInformation, SkipboGraphicsCP, CardGraphicsXF>();
            _score = new ScoreBoardXF();
            _playerHandWPF = new BaseHandXF<SkipboCardInformation, SkipboGraphicsCP, CardGraphicsXF>();
            _publicGraphics = new BasicMultiplePilesXF<SkipboCardInformation, SkipboGraphicsCP, CardGraphicsXF>();
            StackLayout mainStack = new StackLayout();
            ParentSingleUIContainer? restoreP = null;
            if (test.SaveOption == EnumTestSaveCategory.RestoreOnly)
            {
                restoreP = new ParentSingleUIContainer(nameof(SkipboMainViewModel.RestoreScreen));
            }

            StackLayout otherStack = new StackLayout();
            otherStack.Orientation = StackOrientation.Horizontal;
            otherStack.Children.Add(_deckGPile);
            otherStack.Children.Add(_publicGraphics);
            mainStack.Children.Add(otherStack);
            _score.AddColumn("In Stock", false, nameof(SkipboPlayerItem.InStock));
            int x;
            for (x = 1; x <= 4; x++) //has to change for flinch.
            {
                var thisStr = "Discard" + x;
                _score.AddColumn(thisStr, false, thisStr);
            }
            _score.AddColumn("Stock Left", false, nameof(SkipboPlayerItem.StockLeft));
            _score.AddColumn("Cards Left", false, nameof(SkipboPlayerItem.ObjectCount)); //very common.
            SimpleLabelGridXF firstInfo = new SimpleLabelGridXF();
            firstInfo.AddRow("RS Cards", nameof(SkipboMainViewModel.CardsToShuffle));
            firstInfo.AddRow("Turn", nameof(SkipboMainViewModel.NormalTurn));
            firstInfo.AddRow("Status", nameof(SkipboMainViewModel.Status));


            mainStack.Children.Add(_playerHandWPF);
            ParentSingleUIContainer parent = new ParentSingleUIContainer(nameof(SkipboMainViewModel.PlayerPilesScreen));
            mainStack.Children.Add(parent);
            mainStack.Children.Add(firstInfo.GetContent);
            mainStack.Children.Add(_score);


            _deckGPile.Margin = new Thickness(5, 5, 5, 5);


            if (restoreP != null)
            {
                mainStack.Children.Add(restoreP); //default add to grid but does not have to.
            }
            GamePackageViewModelBinder.ManuelElements.Clear(); //often times i have to add manually.

            SkipboSaveInfo save = cons!.Resolve<SkipboSaveInfo>(); //usually needs this part for multiplayer games.

            _score!.LoadLists(save.PlayerList);
            _playerHandWPF!.LoadList(_model.PlayerHand1!, ""); // i think

            _deckGPile!.Init(_model.Deck1!, ""); // try here.  may have to do something else as well (?)
            _deckGPile.StartListeningMainDeck();
            _publicGraphics!.Init(_model.PublicPiles!, ""); // i think
            _publicGraphics.StartAnimationListener("public");
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
            _publicGraphics.Unregister(); //this too.
            return Task.CompletedTask;
        }
    }
}
