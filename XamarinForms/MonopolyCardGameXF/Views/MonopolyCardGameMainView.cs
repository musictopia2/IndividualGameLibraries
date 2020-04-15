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
using MonopolyCardGameCP.Data;
using Xamarin.Forms;
using BasicXFControlsAndPages.MVVMFramework.Controls;
using BasicGameFrameworkLibrary.TestUtilities;
using MonopolyCardGameCP.ViewModels;
using BasicGamingUIXFLibrary.Helpers;
using BasicGamingUIXFLibrary.BasicControls.SingleCardFrames;
using BasicGamingUIXFLibrary.GameGraphics.Cards;
using BasicGamingUIXFLibrary.BasicControls.GameFrames;
using BasicGamingUIXFLibrary.BasicControls.SimpleControls;
using MonopolyCardGameCP.Cards;
using BasicGameFrameworkLibrary.DrawableListsObservable;

namespace MonopolyCardGameXF.Views
{
    public class MonopolyCardGameMainView : ContentView, IUIView, IHandleAsync<LoadEventModel>
    {
        private readonly IEventAggregator _aggregator;
        private readonly MonopolyCardGameVMData _model;
        private readonly MonopolyCardGameGameContainer _gameContainer;
        private readonly BaseDeckXF<MonopolyCardGameCardInformation, MonopolyCardGameGraphicsCP, CardGraphicsXF> _deckGPile;

        private readonly ScoreBoardXF _score;
        private readonly BaseHandXF<MonopolyCardGameCardInformation, MonopolyCardGameGraphicsCP, CardGraphicsXF> _playerHandWPF;

        private readonly StackLayout _tradeStack = new StackLayout();
        private readonly CustomBasicList<MonopolyHandXF> _customList = new CustomBasicList<MonopolyHandXF>();
        private readonly ShowCardUI _cardDetail = new ShowCardUI();

        public MonopolyCardGameMainView(IEventAggregator aggregator,
            TestOptions test,
            MonopolyCardGameVMData model,
            MonopolyCardGameGameContainer gameContainer
            )
        {
            _aggregator = aggregator;
            _model = model;
            _aggregator.Subscribe(this);
            _gameContainer = gameContainer;

            _deckGPile = new BaseDeckXF<MonopolyCardGameCardInformation, MonopolyCardGameGraphicsCP, CardGraphicsXF>();
            _score = new ScoreBoardXF();
            _playerHandWPF = new BaseHandXF<MonopolyCardGameCardInformation, MonopolyCardGameGraphicsCP, CardGraphicsXF>();

            StackLayout mainStack = new StackLayout();
            ParentSingleUIContainer? restoreP = null;
            if (test.SaveOption == EnumTestSaveCategory.RestoreOnly)
            {
                restoreP = new ParentSingleUIContainer(nameof(MonopolyCardGameMainViewModel.RestoreScreen));
            }

            StackLayout otherStack = new StackLayout();
            otherStack.Orientation = StackOrientation.Horizontal;
            otherStack.Children.Add(_deckGPile);
            _score.AddColumn("Cards Left", true, nameof(MonopolyCardGamePlayerItem.ObjectCount)); //very common.
            _score.AddColumn("Previous Money", true, nameof(MonopolyCardGamePlayerItem.PreviousMoney), useCurrency: true);
            _score.AddColumn("Total Money", true, nameof(MonopolyCardGamePlayerItem.TotalMoney), useCurrency: true);
            SimpleLabelGridXF firstInfo = new SimpleLabelGridXF();
            firstInfo.AddRow("Turn", nameof(MonopolyCardGameMainViewModel.NormalTurn));
            firstInfo.AddRow("Status", nameof(MonopolyCardGameMainViewModel.Status));

            

            mainStack.Children.Add(_cardDetail);

            Grid finalGrid = new Grid();
            AddLeftOverColumn(finalGrid, 1);
            AddAutoColumns(finalGrid, 1);
            _tradeStack.Orientation = StackOrientation.Horizontal;
            AddControlToGrid(finalGrid, _tradeStack, 0, 1);
            AddControlToGrid(finalGrid, mainStack, 0, 0);

            StackLayout tempStack = new StackLayout();
            var thisBut = GetGamingButton("Resume", nameof(MonopolyCardGameMainViewModel.ResumeAsync));
            tempStack.Children.Add(thisBut);
            thisBut = GetGamingButton("Go Out", nameof(MonopolyCardGameMainViewModel.GoOutAsync));
            tempStack.Children.Add(thisBut);
            otherStack.Children.Add(tempStack);
            mainStack.Children.Add(otherStack);
            mainStack.Children.Add(_playerHandWPF);
            mainStack.Children.Add(firstInfo.GetContent);
            mainStack.Children.Add(_score);

            _deckGPile.Margin = new Thickness(5, 5, 5, 5);


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

            MonopolyCardGameSaveInfo save = cons!.Resolve<MonopolyCardGameSaveInfo>(); //usually needs this part for multiplayer games.

            _score!.LoadLists(save.PlayerList);
            _playerHandWPF!.LoadList(_model.PlayerHand1!, ""); // i think

            _deckGPile!.Init(_model.Deck1!, ""); // try here.  may have to do something else as well (?)
            _deckGPile.StartListeningMainDeck();

            _cardDetail!.LoadControls(_model);
            var thisList = _gameContainer!.PlayerList!.GetAllPlayersStartingWithSelf();
            thisList.ForEach(thisPlayer =>
            {
                MonopolyHandXF thisHand = new MonopolyHandXF();
                thisHand.HandType = HandObservable<MonopolyCardGameCardInformation>.EnumHandList.Vertical;
                thisHand.Divider = 1.6;
                thisHand.HeightRequest = 900;
                thisHand.LoadList(thisPlayer.TradePile!, "");
                thisPlayer.TradePile!.Scroll = thisHand;
                _tradeStack.Children.Add(thisHand);
                _customList!.Add(thisHand);
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

        Task IUIView.TryCloseAsync()
        {
            _playerHandWPF.Dispose(); //at least will help improve performance.
            return Task.CompletedTask;
        }
    }
}
