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
using MarthaSolitaireCP.Data;
using static BasicXFControlsAndPages.Helpers.GridHelper; //since i use the grid a lot too.
using BasicGamingUIXFLibrary.BasicControls.SingleCardFrames;
using ts = BasicGameFrameworkLibrary.GameGraphicsCP.Cards.DeckOfCardsCP;
using BasicGamingUIXFLibrary.GameGraphics.Cards;
using MarthaSolitaireCP.ViewModels;
using BasicGameFrameworkLibrary.SolitaireClasses.Cards;
using BasicGamingUIXFLibrary.BasicControls.MultipleFrameContainers;
using BasicGamingUIXFLibrary.BasicControls.SolitaireClasses;
using BasicGamingUIXFLibrary.BasicControls.SimpleControls;
using MarthaSolitaireCP.Logic;
using BasicGameFrameworkLibrary.SolitaireClasses.PileObservable;

namespace MarthaSolitaireXF.Views
{
    public class MarthaSolitaireMainView : ContentView, IUIView, IHandleAsync<LoadEventModel>
    {
        private readonly IEventAggregator _aggregator;

        private readonly BasicMultiplePilesXF<SolitaireCard, ts, DeckOfCardsXF<SolitaireCard>> _main; //if something change here.
        private readonly SolitairePilesXF _waste;
        public MarthaSolitaireMainView(IEventAggregator aggregator)
        {
            _aggregator = aggregator;
            _aggregator.Subscribe(this);

            StackLayout stack = new StackLayout();
            StackLayout otherStack = new StackLayout();
            otherStack.Orientation = StackOrientation.Horizontal;
            _main = new BasicMultiplePilesXF<SolitaireCard, ts, DeckOfCardsXF<SolitaireCard>>();
            _main.Margin = new Thickness(10, 5, 5, 5);
            var autoBut = GetGamingButton("Auto Make Move", nameof(MarthaSolitaireMainViewModel.AutoMoveAsync));
            var scoresAlone = new SimpleLabelGridXF();
            scoresAlone.AddRow("Score", nameof(MarthaSolitaireMainViewModel.Score));
            var tempGrid = scoresAlone.GetContent;
            _waste = new SolitairePilesXF();
            otherStack.Children.Add(_main);
            StackLayout tempStack = new StackLayout();
            otherStack.Children.Add(tempStack);
            stack.Children.Add(otherStack);

            tempStack.Children.Add(tempGrid);
            tempStack.Children.Add(autoBut);
            stack.Children.Add(_waste);
            Content = stack; //if not doing this, rethink.
        }
        Task IHandleAsync<LoadEventModel>.HandleAsync(LoadEventModel message)
        {
            MarthaSolitaireMainViewModel model = (MarthaSolitaireMainViewModel)BindingContext;
            var tempWaste = (WastePiles)model.WastePiles1!;
            _waste.Init(tempWaste.Piles);
            var tempMain = (MainPilesCP)model.MainPiles1!;
            _main.Init(tempMain.Piles, ts.TagUsed);
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
