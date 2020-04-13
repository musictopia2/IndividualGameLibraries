using BasicControlsAndWindowsCore.Controls.NavigationControls;
using BasicGameFrameworkLibrary.BasicEventModels;
using BasicGameFrameworkLibrary.TestUtilities;
using BasicGamingUIWPFLibrary.BasicControls.GameFrames;
using BasicGamingUIWPFLibrary.BasicControls.SimpleControls;
using BasicGamingUIWPFLibrary.BasicControls.SingleCardFrames;
using BasicGamingUIWPFLibrary.Helpers;
using CommonBasicStandardLibraries.Messenging;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using System.Windows;
using System.Windows.Controls;
using UnoCP.Cards;
using UnoCP.Data;
using UnoCP.ViewModels;
using static BasicGamingUIWPFLibrary.Helpers.SharedUIFunctions; //this usually will be used too.
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;
using ts = BasicGameFrameworkLibrary.GameGraphicsCP.Cards.BaseColorCardsCP;

namespace UnoWPF.Views
{
    public class UnoMainView : UserControl, IUIView, IHandleAsync<LoadEventModel>
    {
        private readonly IEventAggregator _aggregator;
        private readonly UnoVMData _model;
        private readonly BaseDeckWPF<UnoCardInformation, UnoGraphicsCP, CardGraphicsWPF> _deckGPile;
        private readonly BasePileWPF<UnoCardInformation, UnoGraphicsCP, CardGraphicsWPF> _discardGPile;

        private readonly ScoreBoardWPF _score;
        private readonly BaseHandWPF<UnoCardInformation, UnoGraphicsCP, CardGraphicsWPF> _playerHandWPF;

        public UnoMainView(IEventAggregator aggregator,
            TestOptions test,
            UnoVMData model
            )
        {
            _aggregator = aggregator;
            _model = model;
            _aggregator.Subscribe(this);

            _deckGPile = new BaseDeckWPF<UnoCardInformation, UnoGraphicsCP, CardGraphicsWPF>();
            _discardGPile = new BasePileWPF<UnoCardInformation, UnoGraphicsCP, CardGraphicsWPF>();
            _score = new ScoreBoardWPF();
            _playerHandWPF = new BaseHandWPF<UnoCardInformation, UnoGraphicsCP, CardGraphicsWPF>();

            StackPanel mainStack = new StackPanel();
            ParentSingleUIContainer? restoreP = null;
            if (test.SaveOption == EnumTestSaveCategory.RestoreOnly)
            {
                restoreP = new ParentSingleUIContainer()
                {
                    Name = nameof(UnoMainViewModel.RestoreScreen)
                };
            }


            StackPanel otherStack = new StackPanel();
            otherStack.Orientation = Orientation.Horizontal;
            otherStack.Children.Add(_deckGPile);
            otherStack.Children.Add(_discardGPile); // can reposition or not even have as well.
            var endButton = GetGamingButton("End Turn", nameof(UnoMainViewModel.EndTurnAsync));
            endButton.HorizontalAlignment = HorizontalAlignment.Left;
            otherStack.Children.Add(endButton);
            ParentSingleUIContainer parent = new ParentSingleUIContainer()
            {
                Name = nameof(UnoMainViewModel.SayUnoScreen)
            };
            otherStack.Children.Add(parent);
            mainStack.Children.Add(otherStack);
            _score.AddColumn("Cards Left", true, nameof(UnoPlayerItem.ObjectCount)); //very common.
            _score.AddColumn("Total Points", true, nameof(UnoPlayerItem.TotalPoints), rightMargin: 10);
            _score.AddColumn("Previous Points", true, nameof(UnoPlayerItem.PreviousPoints), rightMargin: 10);
            SimpleLabelGrid firstInfo = new SimpleLabelGrid();
            firstInfo.AddRow("Turn", nameof(UnoMainViewModel.NormalTurn));
            firstInfo.AddRow("Next", nameof(UnoMainViewModel.NextPlayer));
            firstInfo.AddRow("Status", nameof(UnoMainViewModel.Status));

            mainStack.Children.Add(_playerHandWPF);
            mainStack.Children.Add(firstInfo.GetContent);

            otherStack = new StackPanel();
            otherStack.Orientation = Orientation.Horizontal;
            mainStack.Children.Add(otherStack);
            otherStack.Children.Add(_score);


            _deckGPile.Margin = new Thickness(5, 5, 5, 5);

            _discardGPile.Margin = new Thickness(5, 5, 5, 5);


            if (restoreP != null)
            {
                mainStack.Children.Add(restoreP); //default add to grid but does not have to.
            }
            UnoSaveInfo save = cons!.Resolve<UnoSaveInfo>(); //usually needs this part for multiplayer games.
            _score!.LoadLists(save.PlayerList);
            _playerHandWPF!.LoadList(_model.PlayerHand1!, ts.TagUsed); // i think
            _discardGPile!.Init(_model.Pile1!, ts.TagUsed); // may have to be here (well see)
            _discardGPile.StartListeningDiscardPile(); // its the main one.

            _deckGPile!.Init(_model.Deck1!, ts.TagUsed); // try here.  may have to do something else as well (?)
            _deckGPile.StartListeningMainDeck();

            Content = mainStack;

        }
        Task IHandleAsync<LoadEventModel>.HandleAsync(LoadEventModel message)
        {

            //GamePackageViewModelBinder.ManuelElements.Clear(); //often times i have to add manually.




            return Task.CompletedTask;
            //return this.RefreshBindingsAsync(_aggregator);
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
