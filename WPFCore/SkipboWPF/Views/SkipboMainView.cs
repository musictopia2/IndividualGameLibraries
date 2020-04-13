using BasicControlsAndWindowsCore.Controls.NavigationControls;
using BasicGameFrameworkLibrary.BasicEventModels;
using BasicGameFrameworkLibrary.TestUtilities;
using BasicGamingUIWPFLibrary.BasicControls.GameFrames;
using BasicGamingUIWPFLibrary.BasicControls.MultipleFrameContainers;
using BasicGamingUIWPFLibrary.BasicControls.SimpleControls;
using BasicGamingUIWPFLibrary.BasicControls.SingleCardFrames;
using BasicGamingUIWPFLibrary.Helpers;
using CommonBasicStandardLibraries.Messenging;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using SkipboCP.Cards;
using SkipboCP.Data;
using SkipboCP.ViewModels;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using System.Windows;
using System.Windows.Controls;
using static BasicGamingUIWPFLibrary.Helpers.SharedUIFunctions; //this usually will be used too.
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;

namespace SkipboWPF.Views
{
    public class SkipboMainView : UserControl, IUIView, IHandleAsync<LoadEventModel>
    {
        private readonly IEventAggregator _aggregator;
        private readonly SkipboVMData _model;
        private readonly BaseDeckWPF<SkipboCardInformation, SkipboGraphicsCP, CardGraphicsWPF> _deckGPile;

        private readonly ScoreBoardWPF _score;
        private readonly BaseHandWPF<SkipboCardInformation, SkipboGraphicsCP, CardGraphicsWPF> _playerHandWPF;

        private readonly BasicMultiplePilesWPF<SkipboCardInformation, SkipboGraphicsCP, CardGraphicsWPF> _publicGraphics;


        public SkipboMainView(IEventAggregator aggregator,
            TestOptions test,
            SkipboVMData model
            )
        {
            _aggregator = aggregator;
            _model = model;
            _aggregator.Subscribe(this);

            _deckGPile = new BaseDeckWPF<SkipboCardInformation, SkipboGraphicsCP, CardGraphicsWPF>();
            _score = new ScoreBoardWPF();
            _playerHandWPF = new BaseHandWPF<SkipboCardInformation, SkipboGraphicsCP, CardGraphicsWPF>();
            _publicGraphics = new BasicMultiplePilesWPF<SkipboCardInformation, SkipboGraphicsCP, CardGraphicsWPF>();

            StackPanel mainStack = new StackPanel();
            ParentSingleUIContainer? restoreP = null;
            if (test.SaveOption == EnumTestSaveCategory.RestoreOnly)
            {
                restoreP = new ParentSingleUIContainer()
                {
                    Name = nameof(SkipboMainViewModel.RestoreScreen)
                };
            }


            StackPanel otherStack = new StackPanel();
            otherStack.Orientation = Orientation.Horizontal;
            otherStack.Children.Add(_deckGPile);
            otherStack.Children.Add(_publicGraphics);
            mainStack.Children.Add(otherStack);
            StackPanel tempStack = new StackPanel();
            var thisLabel = GetDefaultLabel();
            thisLabel.Text = "Cards To Reshuffle";
            tempStack.Children.Add(thisLabel);
            otherStack.Children.Add(tempStack);
            thisLabel = GetDefaultLabel();
            thisLabel.SetBinding(TextBlock.TextProperty, nameof(SkipboMainViewModel.CardsToShuffle));
            tempStack.Children.Add(thisLabel);
            _score.AddColumn("In Stock", false, nameof(SkipboPlayerItem.InStock));
            int x;
            for (x = 1; x <= 4; x++) //has to change for flinch.
            {
                var thisStr = "Discard" + x;
                _score.AddColumn(thisStr, false, thisStr);
            }
            _score.AddColumn("Stock Left", false, nameof(SkipboPlayerItem.StockLeft));
            _score.AddColumn("Cards Left", false, nameof(SkipboPlayerItem.ObjectCount)); //very common.
            SimpleLabelGrid firstInfo = new SimpleLabelGrid();
            firstInfo.AddRow("Turn", nameof(SkipboMainViewModel.NormalTurn));
            firstInfo.AddRow("Status", nameof(SkipboMainViewModel.Status));


            otherStack.Children.Add(_score);

            mainStack.Children.Add(_playerHandWPF);
            ParentSingleUIContainer parent = new ParentSingleUIContainer()
            {
                Name = nameof(SkipboMainViewModel.PlayerPilesScreen)
            };
            mainStack.Children.Add(parent);
            mainStack.Children.Add(firstInfo.GetContent);

            _deckGPile.Margin = new Thickness(5, 5, 5, 5);



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

            SkipboSaveInfo save = cons!.Resolve<SkipboSaveInfo>(); //usually needs this part for multiplayer games.

            _score!.LoadLists(save.PlayerList);
            _playerHandWPF!.LoadList(_model.PlayerHand1!, ""); // i think

            _deckGPile!.Init(_model.Deck1!, ""); // try here.  may have to do something else as well (?)
            _deckGPile.StartListeningMainDeck();
            _publicGraphics!.Init(_model.PublicPiles!, ""); // i think
            _publicGraphics.StartAnimationListener("public");
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
