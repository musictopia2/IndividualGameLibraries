using BasicControlsAndWindowsCore.Controls.NavigationControls;
using BasicGameFrameworkLibrary.BasicEventModels;
using BasicGameFrameworkLibrary.RegularDeckOfCards;
using BasicGameFrameworkLibrary.TestUtilities;
using BasicGamingUIWPFLibrary.BasicControls.GameFrames;
using BasicGamingUIWPFLibrary.BasicControls.SimpleControls;
using BasicGamingUIWPFLibrary.BasicControls.SingleCardFrames;
using BasicGamingUIWPFLibrary.GameGraphics.Cards;
using BasicGamingUIWPFLibrary.Helpers;
using CommonBasicStandardLibraries.Messenging;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using OldMaidCP.Data;
using OldMaidCP.ViewModels;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using System.Windows;
using System.Windows.Controls;
using static BasicGamingUIWPFLibrary.Helpers.SharedUIFunctions; //this usually will be used too.
using ts = BasicGameFrameworkLibrary.GameGraphicsCP.Cards.DeckOfCardsCP;

namespace OldMaidWPF.Views
{
    public class OldMaidMainView : UserControl, IUIView, IHandleAsync<LoadEventModel>
    {
        private readonly IEventAggregator _aggregator;
        private readonly OldMaidVMData _model;
        private readonly BasePileWPF<RegularSimpleCard, ts, DeckOfCardsWPF<RegularSimpleCard>> _discardGPile;
        private readonly BaseHandWPF<RegularSimpleCard, ts, DeckOfCardsWPF<RegularSimpleCard>> _playerHandWPF;

        public OldMaidMainView(IEventAggregator aggregator,
            TestOptions test,
            OldMaidVMData model
            )
        {
            _aggregator = aggregator;
            _model = model;
            _aggregator.Subscribe(this);

            _discardGPile = new BasePileWPF<RegularSimpleCard, ts, DeckOfCardsWPF<RegularSimpleCard>>();
            _playerHandWPF = new BaseHandWPF<RegularSimpleCard, ts, DeckOfCardsWPF<RegularSimpleCard>>();
            _playerHandWPF.Divider = 2;

            StackPanel mainStack = new StackPanel();
            ParentSingleUIContainer? restoreP = null;
            if (test.SaveOption == EnumTestSaveCategory.RestoreOnly)
            {
                restoreP = new ParentSingleUIContainer()
                {
                    Name = nameof(OldMaidMainViewModel.RestoreScreen)
                };
            }


            StackPanel otherStack = new StackPanel();
            otherStack.Orientation = Orientation.Horizontal;
            otherStack.Children.Add(_discardGPile); // can reposition or not even have as well.
            SimpleLabelGrid firstInfo = new SimpleLabelGrid();
            firstInfo.AddRow("Turn", nameof(OldMaidMainViewModel.NormalTurn));
            firstInfo.AddRow("Status", nameof(OldMaidMainViewModel.Status));
            var endButton = GetGamingButton("End Turn", nameof(OldMaidMainViewModel.EndTurnAsync));
            endButton.HorizontalAlignment = HorizontalAlignment.Left;
            otherStack.Children.Add(endButton);


            ParentSingleUIContainer opponent = new ParentSingleUIContainer()
            {
                Name = nameof(OldMaidMainViewModel.OpponentScreen)
            };

            mainStack.Children.Add(opponent);
            mainStack.Children.Add(_playerHandWPF);
            mainStack.Children.Add(otherStack);
            mainStack.Children.Add(firstInfo.GetContent);


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


            _playerHandWPF!.LoadList(_model.PlayerHand1!, ts.TagUsed); // i think
            _discardGPile!.Init(_model.Pile1!, ts.TagUsed); // may have to be here (well see)
            _discardGPile.StartListeningDiscardPile(); // its the main one.


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
