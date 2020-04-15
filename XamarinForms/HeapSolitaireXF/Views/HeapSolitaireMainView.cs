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
using Xamarin.Forms;
using static BasicGamingUIXFLibrary.Helpers.SharedUIFunctions; //this usually will be used too.
using CommonBasicStandardLibraries.Messenging;
using BasicGameFrameworkLibrary.BasicEventModels;
using HeapSolitaireCP.Data;
using static BasicXFControlsAndPages.Helpers.GridHelper; //since i use the grid a lot too.
using BasicGamingUIXFLibrary.BasicControls.SingleCardFrames;
using ts = BasicGameFrameworkLibrary.GameGraphicsCP.Cards.DeckOfCardsCP;
using BasicGamingUIXFLibrary.GameGraphics.Cards;
using HeapSolitaireCP.ViewModels;
using BasicGamingUIXFLibrary.BasicControls.MultipleFrameContainers;
using BasicGamingUIXFLibrary.BasicControls.SimpleControls;

namespace HeapSolitaireXF.Views
{
    public class HeapSolitaireMainView : ContentView, IUIView, IHandleAsync<LoadEventModel>
    {
        //TODO:  has to build the main page.  no need for new game since that is completely separated out into another class.
        private readonly IEventAggregator _aggregator;

        private readonly BasicMultiplePilesXF<HeapSolitaireCardInfo, ts, DeckOfCardsXF<HeapSolitaireCardInfo>> _mainPile;
        private readonly CustomWasteUI _wastePile;
        public HeapSolitaireMainView(IEventAggregator aggregator)
        {
            _aggregator = aggregator;
            _aggregator.Subscribe(this);

            StackLayout stack = new StackLayout();

            _mainPile = new BasicMultiplePilesXF<HeapSolitaireCardInfo, ts, DeckOfCardsXF<HeapSolitaireCardInfo>>();
            _wastePile = new CustomWasteUI();
            _mainPile.Margin = new Thickness(5);
            stack.Children.Add(_mainPile);
            StackLayout others = new StackLayout()
            {
                Orientation = StackOrientation.Horizontal
            };
            stack.Children.Add(others);
            others.Children.Add(_wastePile);
            StackLayout finalStack = new StackLayout();
            SimpleLabelGridXF scoresAlone = new SimpleLabelGridXF();
            scoresAlone.AddRow("Score", nameof(HeapSolitaireMainViewModel.Score));
            finalStack.Children.Add(scoresAlone.GetContent);
            others.Children.Add(finalStack);
            Content = stack; //if not doing this, rethink.

        }
        Task IHandleAsync<LoadEventModel>.HandleAsync(LoadEventModel message)
        {
            HeapSolitaireMainViewModel model = (HeapSolitaireMainViewModel)BindingContext;
            _mainPile.Init(model.Main1, ts.TagUsed);
            _wastePile.Init(model);

            return Task.CompletedTask;
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
            _aggregator.Unsubscribe(this); //looks like you have to unsubscribe each time. now.
            return Task.CompletedTask;
        }
    }
}
