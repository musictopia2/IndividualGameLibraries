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
using TeeItUpCP.Data;
using Xamarin.Forms;
using BasicXFControlsAndPages.MVVMFramework.Controls;
using BasicGameFrameworkLibrary.TestUtilities;
using TeeItUpCP.ViewModels;
using BasicGamingUIXFLibrary.Helpers;
using BasicGamingUIXFLibrary.BasicControls.SingleCardFrames;
using BasicGamingUIXFLibrary.GameGraphics.Cards;
using BasicGamingUIXFLibrary.BasicControls.GameFrames;
using BasicGamingUIXFLibrary.BasicControls.SimpleControls;
using TeeItUpCP.Cards;

namespace TeeItUpXF.Views
{
    public class TeeItUpMainView : ContentView, IUIView, IHandleAsync<LoadEventModel>
    {
        private readonly IEventAggregator _aggregator;
        private readonly TeeItUpVMData _model;
        private readonly TeeItUpGameContainer _gameContainer;
        private readonly BaseDeckXF<TeeItUpCardInformation, TeeItUpGraphicsCP, CardGraphicsXF> _deckGPile;
        private readonly BasePileXF<TeeItUpCardInformation, TeeItUpGraphicsCP, CardGraphicsXF> _discardGPile;

        private readonly ScoreBoardXF _score;
        private readonly BasePileXF<TeeItUpCardInformation, TeeItUpGraphicsCP, CardGraphicsXF> _otherPile;
        private readonly StackLayout _boardStack;
        private readonly CustomBasicList<CardBoardXF<TeeItUpCardInformation, TeeItUpGraphicsCP, CardGraphicsXF>> _boardList = new CustomBasicList<CardBoardXF<TeeItUpCardInformation, TeeItUpGraphicsCP, CardGraphicsXF>>();
        public TeeItUpMainView(IEventAggregator aggregator,
            TestOptions test,
            TeeItUpVMData model,
            TeeItUpGameContainer gameContainer
            )
        {
            _aggregator = aggregator;
            _model = model;
            _aggregator.Subscribe(this);
            _gameContainer = gameContainer;
            _deckGPile = new BaseDeckXF<TeeItUpCardInformation, TeeItUpGraphicsCP, CardGraphicsXF>();
            _discardGPile = new BasePileXF<TeeItUpCardInformation, TeeItUpGraphicsCP, CardGraphicsXF>();
            _score = new ScoreBoardXF();
            _boardStack = new StackLayout();
            _boardStack.Orientation = StackOrientation.Horizontal;
            _otherPile = new BasePileXF<TeeItUpCardInformation, TeeItUpGraphicsCP, CardGraphicsXF>();
            StackLayout mainStack = new StackLayout();
            ParentSingleUIContainer? restoreP = null;
            if (test.SaveOption == EnumTestSaveCategory.RestoreOnly)
            {
                restoreP = new ParentSingleUIContainer(nameof(TeeItUpMainViewModel.RestoreScreen));
            }

            StackLayout otherStack = new StackLayout();
            otherStack.Orientation = StackOrientation.Horizontal;
            otherStack.Children.Add(_deckGPile);
            otherStack.Children.Add(_discardGPile); // can reposition or not even have as well.
            otherStack.Children.Add(_otherPile);
            mainStack.Children.Add(otherStack);
            _score.AddColumn("Went Out", true, nameof(TeeItUpPlayerItem.WentOut), useTrueFalse: true);
            _score.AddColumn("Previous Score", true, nameof(TeeItUpPlayerItem.PreviousScore));
            _score.AddColumn("Total Score", true, nameof(TeeItUpPlayerItem.TotalScore));
            SimpleLabelGridXF firstInfo = new SimpleLabelGridXF();
            firstInfo.AddRow("Turn", nameof(TeeItUpMainViewModel.NormalTurn));
            firstInfo.AddRow("Status", nameof(TeeItUpMainViewModel.Status));
            firstInfo.AddRow("Round", nameof(TeeItUpMainViewModel.Round));
            firstInfo.AddRow("Instructions", nameof(TeeItUpMainViewModel.Instructions));

            otherStack.Children.Add(_score);
            otherStack.Children.Add(firstInfo.GetContent);
            mainStack.Children.Add(_boardStack);


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

            TeeItUpSaveInfo save = cons!.Resolve<TeeItUpSaveInfo>(); //usually needs this part for multiplayer games.

            _score!.LoadLists(save.PlayerList);
            _discardGPile!.Init(_model.Pile1!, ""); // may have to be here (well see)
            _discardGPile.StartListeningDiscardPile(); // its the main one.

            _deckGPile!.Init(_model.Deck1!, ""); // try here.  may have to do something else as well (?)
            _deckGPile.StartListeningMainDeck();

            _otherPile!.Init(_model.OtherPile!, "");
            _otherPile.StartAnimationListener("otherpile");
            CustomBasicList<TeeItUpPlayerItem> thisList;
            if (_gameContainer.BasicData!.MultiPlayer == true)
                thisList = _gameContainer!.PlayerList!.GetAllPlayersStartingWithSelf();
            else
                thisList = _gameContainer!.PlayerList!.ToCustomBasicList();
            thisList.ForEach(thisPlayer =>
            {
                CardBoardXF<TeeItUpCardInformation, TeeItUpGraphicsCP, CardGraphicsXF> thisControl = new CardBoardXF<TeeItUpCardInformation, TeeItUpGraphicsCP, CardGraphicsXF>();
                _boardList!.Add(thisControl);
                thisControl.LoadList(thisPlayer.PlayerBoard!, "");
                _boardStack!.Children.Add(thisControl);
            });

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
        //hopefully not slower when going to new rounds.  otherwise, has to think about efficiency here too.
        Task IUIView.TryCloseAsync()
        {

            return Task.CompletedTask;
        }
    }
}
