using BasicControlsAndWindowsCore.Controls.NavigationControls;
using BasicGameFrameworkLibrary.BasicEventModels;
using BasicGameFrameworkLibrary.TestUtilities;
using BasicGamingUIWPFLibrary.BasicControls.GameFrames;
using BasicGamingUIWPFLibrary.BasicControls.SimpleControls;
using BasicGamingUIWPFLibrary.BasicControls.SingleCardFrames;
using BasicGamingUIWPFLibrary.Helpers;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.Messenging;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using System.Windows;
using System.Windows.Controls;
using TeeItUpCP.Cards;
using TeeItUpCP.Data;
using TeeItUpCP.ViewModels;
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;

namespace TeeItUpWPF.Views
{
    public class TeeItUpMainView : UserControl, IUIView, IHandleAsync<LoadEventModel>
    {
        private readonly IEventAggregator _aggregator;
        private readonly TeeItUpVMData _model;
        private readonly TeeItUpGameContainer _gameContainer;
        private readonly BaseDeckWPF<TeeItUpCardInformation, TeeItUpGraphicsCP, CardGraphicsWPF> _deckGPile;
        private readonly BasePileWPF<TeeItUpCardInformation, TeeItUpGraphicsCP, CardGraphicsWPF> _discardGPile;
        private readonly ScoreBoardWPF _score;
        private readonly BasePileWPF<TeeItUpCardInformation, TeeItUpGraphicsCP, CardGraphicsWPF> _otherPile;
        private readonly StackPanel _boardStack;
        private readonly CustomBasicList<CardBoardWPF<TeeItUpCardInformation, TeeItUpGraphicsCP, CardGraphicsWPF>> _boardList = new CustomBasicList<CardBoardWPF<TeeItUpCardInformation, TeeItUpGraphicsCP, CardGraphicsWPF>>();
        public TeeItUpMainView(IEventAggregator aggregator,
            TestOptions test,
            TeeItUpVMData model,
            TeeItUpGameContainer gameContainer
            )
        {
            _aggregator = aggregator;
            _model = model;
            _gameContainer = gameContainer;
            _aggregator.Subscribe(this);

            _deckGPile = new BaseDeckWPF<TeeItUpCardInformation, TeeItUpGraphicsCP, CardGraphicsWPF>();
            _discardGPile = new BasePileWPF<TeeItUpCardInformation, TeeItUpGraphicsCP, CardGraphicsWPF>();
            _score = new ScoreBoardWPF();
            _otherPile = new BasePileWPF<TeeItUpCardInformation, TeeItUpGraphicsCP, CardGraphicsWPF>();
            _boardStack = new StackPanel();
            _boardStack.Orientation = Orientation.Horizontal;

            StackPanel mainStack = new StackPanel();
            ParentSingleUIContainer? restoreP = null;
            if (test.SaveOption == EnumTestSaveCategory.RestoreOnly)
            {
                restoreP = new ParentSingleUIContainer()
                {
                    Name = nameof(TeeItUpMainViewModel.RestoreScreen)
                };
            }


            StackPanel otherStack = new StackPanel();
            otherStack.Orientation = Orientation.Horizontal;
            otherStack.Children.Add(_deckGPile);
            otherStack.Children.Add(_discardGPile); // can reposition or not even have as well.
            otherStack.Children.Add(_otherPile);
            mainStack.Children.Add(otherStack);

            _score.AddColumn("Went Out", true, nameof(TeeItUpPlayerItem.WentOut), useTrueFalse: true);
            _score.AddColumn("Previous Score", true, nameof(TeeItUpPlayerItem.PreviousScore));
            _score.AddColumn("Total Score", true, nameof(TeeItUpPlayerItem.TotalScore));
            SimpleLabelGrid firstInfo = new SimpleLabelGrid();
            firstInfo.AddRow("Turn", nameof(TeeItUpMainViewModel.NormalTurn));
            firstInfo.AddRow("Status", nameof(TeeItUpMainViewModel.Status));
            firstInfo.AddRow("Round", nameof(TeeItUpMainViewModel.Round));
            firstInfo.AddRow("Instructions", nameof(TeeItUpMainViewModel.Instructions));
            otherStack.Children.Add(_score);
            otherStack.Children.Add(firstInfo.GetContent);
            mainStack.Children.Add(_boardStack);

            _deckGPile.Margin = new Thickness(5, 5, 5, 5);

            _discardGPile.Margin = new Thickness(5, 5, 5, 5);

            _otherPile.Margin = new Thickness(5);
            if (restoreP != null)
            {
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
                CardBoardWPF<TeeItUpCardInformation, TeeItUpGraphicsCP, CardGraphicsWPF> thisControl = new CardBoardWPF<TeeItUpCardInformation, TeeItUpGraphicsCP, CardGraphicsWPF>();
                _boardList!.Add(thisControl);
                thisControl.LoadList(thisPlayer.PlayerBoard!, "");
                _boardStack!.Children.Add(thisControl);
            });

            return this.RefreshBindingsAsync(_aggregator);
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
