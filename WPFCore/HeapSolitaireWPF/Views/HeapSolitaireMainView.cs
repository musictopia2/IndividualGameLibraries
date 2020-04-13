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
using System.Windows.Controls;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using static BasicGamingUIWPFLibrary.Helpers.SharedUIFunctions; //this usually will be used too.
using CommonBasicStandardLibraries.Messenging;
using BasicGameFrameworkLibrary.BasicEventModels;
using HeapSolitaireCP.Data;
using static BasicControlsAndWindowsCore.Helpers.GridHelper; //since i use the grid a lot too.
using BasicGamingUIWPFLibrary.BasicControls.SingleCardFrames;
using ts = BasicGameFrameworkLibrary.GameGraphicsCP.Cards.DeckOfCardsCP;
using BasicGamingUIWPFLibrary.GameGraphics.Cards;
using System.Windows;
using HeapSolitaireCP.ViewModels;
using BasicGamingUIWPFLibrary.BasicControls.MultipleFrameContainers;
using BasicGamingUIWPFLibrary.BasicControls.SimpleControls;

namespace HeapSolitaireWPF.Views
{
    public class HeapSolitaireMainView : UserControl, IUIView, IHandleAsync<LoadEventModel>
    {
        //TODO:  has to build the main page.  no need for new game since that is completely separated out into another class.
        private readonly IEventAggregator _aggregator;

        private readonly BasicMultiplePilesWPF<HeapSolitaireCardInfo, ts, DeckOfCardsWPF<HeapSolitaireCardInfo>> _mainPile;
        private readonly CustomWasteUI _wastePile;
        public HeapSolitaireMainView(IEventAggregator aggregator)
        {
            _aggregator = aggregator;
			_aggregator.Subscribe(this);

            StackPanel stack = new StackPanel();

            _mainPile = new BasicMultiplePilesWPF<HeapSolitaireCardInfo, ts, DeckOfCardsWPF<HeapSolitaireCardInfo>>();
            _wastePile = new CustomWasteUI();
            _mainPile.Margin = new Thickness(5);
            stack.Children.Add(_mainPile);
            StackPanel others = new StackPanel()
            {
                Orientation = Orientation.Horizontal
            };
            stack.Children.Add(others);
            others.Children.Add(_wastePile);
            StackPanel finalStack = new StackPanel();
            SimpleLabelGrid scoresAlone = new SimpleLabelGrid();
            scoresAlone.AddRow("Score", nameof(HeapSolitaireMainViewModel.Score));
            finalStack.Children.Add(scoresAlone.GetContent);
            others.Children.Add(finalStack);
            Content = stack; //if not doing this, rethink.
        }
        Task IHandleAsync<LoadEventModel>.HandleAsync(LoadEventModel message)
        {

            HeapSolitaireMainViewModel model = (HeapSolitaireMainViewModel)DataContext;
            _mainPile.Init(model.Main1, ts.TagUsed);
            _wastePile.Init(model);

            return Task.CompletedTask;
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
