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
using YahtzeeHandsDownCP.Data;
using BasicGameFrameworkLibrary.TestUtilities;
using BasicControlsAndWindowsCore.Controls.NavigationControls;
using YahtzeeHandsDownCP.ViewModels;
using static BasicControlsAndWindowsCore.Helpers.GridHelper; //usually needs this as well for grid helpers.
using BasicGamingUIWPFLibrary.Helpers;
using System.Windows;
using BasicGamingUIWPFLibrary.BasicControls.SingleCardFrames;
using BasicGameFrameworkLibrary.RegularDeckOfCards;
using BasicGamingUIWPFLibrary.GameGraphics.Cards;
using BasicGamingUIWPFLibrary.BasicControls.GameFrames;
using BasicGamingUIWPFLibrary.BasicControls.SimpleControls;
using YahtzeeHandsDownCP.Cards;
using BasicGameFrameworkLibrary.DrawableListsObservable;

namespace YahtzeeHandsDownWPF.Views
{
    public class YahtzeeHandsDownMainView : UserControl, IUIView, IHandleAsync<LoadEventModel>
    {
        //TODO:  has to build the main page.  no need for new game since that is completely separated out into another class.
        private readonly IEventAggregator _aggregator;
        private readonly YahtzeeHandsDownVMData _model;
        private readonly BaseDeckWPF<YahtzeeHandsDownCardInformation, YahtzeeHandsDownGraphicsCP, CardGraphicsWPF> _deckGPile;
        private readonly BasePileWPF<YahtzeeHandsDownCardInformation, YahtzeeHandsDownGraphicsCP, CardGraphicsWPF> _discardGPile;

        private readonly ScoreBoardWPF _score;
        private readonly BaseHandWPF<YahtzeeHandsDownCardInformation, YahtzeeHandsDownGraphicsCP, CardGraphicsWPF> _playerHandWPF;

        private readonly ComboHandWPF _combo1;
        private readonly ChanceSinglePileWPF _chance1;

        public YahtzeeHandsDownMainView(IEventAggregator aggregator,
            TestOptions test,
            YahtzeeHandsDownVMData model
            )
        {
            _aggregator = aggregator;
            _model = model;
            _aggregator.Subscribe(this);

            _deckGPile = new BaseDeckWPF<YahtzeeHandsDownCardInformation, YahtzeeHandsDownGraphicsCP, CardGraphicsWPF>();
            _discardGPile = new BasePileWPF<YahtzeeHandsDownCardInformation, YahtzeeHandsDownGraphicsCP, CardGraphicsWPF>();
            _score = new ScoreBoardWPF();
            _playerHandWPF = new BaseHandWPF<YahtzeeHandsDownCardInformation, YahtzeeHandsDownGraphicsCP, CardGraphicsWPF>();
            _combo1 = new ComboHandWPF();
            _chance1 = new ChanceSinglePileWPF();

            StackPanel mainStack = new StackPanel();
            ParentSingleUIContainer? restoreP = null;
            if (test.SaveOption == EnumTestSaveCategory.RestoreOnly)
            {
                restoreP = new ParentSingleUIContainer()
                {
                    Name = nameof(YahtzeeHandsDownMainViewModel.RestoreScreen)
                };
            }


            StackPanel otherStack = new StackPanel();
            otherStack.Orientation = Orientation.Horizontal;
            otherStack.Children.Add(_deckGPile);
            otherStack.Children.Add(_discardGPile); // can reposition or not even have as well.

            _chance1.Margin = new Thickness(5, 5, 5, 5);
            _chance1.HorizontalAlignment = HorizontalAlignment.Left;
            _chance1.VerticalAlignment = VerticalAlignment.Top;
            otherStack.Children.Add(_chance1);
            mainStack.Children.Add(otherStack);
            _score.AddColumn("Cards Left", true, nameof(YahtzeeHandsDownPlayerItem.ObjectCount)); //very common.
            _score.AddColumn("Total Score", true, nameof(YahtzeeHandsDownPlayerItem.TotalScore));
            _score.AddColumn("Won Last Round", true, nameof(YahtzeeHandsDownPlayerItem.WonLastRound));
            _score.AddColumn("Score Round", true, nameof(YahtzeeHandsDownPlayerItem.ScoreRound));
            SimpleLabelGrid firstInfo = new SimpleLabelGrid();
            firstInfo.AddRow("Turn", nameof(YahtzeeHandsDownMainViewModel.NormalTurn));
            firstInfo.AddRow("Status", nameof(YahtzeeHandsDownMainViewModel.Status));
            mainStack.Children.Add(_playerHandWPF);
            var otherButton = GetGamingButton("Go Out", nameof(YahtzeeHandsDownMainViewModel.GoOutAsync));
            mainStack.Children.Add(otherButton);
            var endButton = GetGamingButton("End Turn", nameof(YahtzeeHandsDownMainViewModel.EndTurnAsync));
            endButton.HorizontalAlignment = HorizontalAlignment.Left;
            mainStack.Children.Add(endButton);
            mainStack.Children.Add(firstInfo.GetContent);
            mainStack.Children.Add(_score);
            otherStack = new StackPanel();
            otherStack.Orientation = Orientation.Horizontal;
            otherStack.Children.Add(mainStack);
            _combo1.HandType = HandObservable<ComboCardInfo>.EnumHandList.Vertical;
            otherStack.Children.Add(_combo1);

            _deckGPile.Margin = new Thickness(5, 5, 5, 5);

            _discardGPile.Margin = new Thickness(5, 5, 5, 5);


            if (restoreP != null)
            {
                //todo:  figure out where to place the restore ui if there is a restore option.
                mainStack.Children.Add(restoreP); //default add to grid but does not have to.
            }
            Content = otherStack;

        }
        Task IHandleAsync<LoadEventModel>.HandleAsync(LoadEventModel message)
        {

            GamePackageViewModelBinder.ManuelElements.Clear(); //often times i have to add manually.

            YahtzeeHandsDownSaveInfo save = cons!.Resolve<YahtzeeHandsDownSaveInfo>(); //usually needs this part for multiplayer games.

            _score!.LoadLists(save.PlayerList);
            _playerHandWPF!.LoadList(_model.PlayerHand1!, ""); // i think
            _discardGPile!.Init(_model.Pile1!, ""); // may have to be here (well see)
            _discardGPile.StartListeningDiscardPile(); // its the main one.

            _deckGPile!.Init(_model.Deck1!, ""); // try here.  may have to do something else as well (?)
            _deckGPile.StartListeningMainDeck();
            _combo1!.LoadList(_model.ComboHandList!, "combo");
            _chance1!.Init(_model.ChancePile!, "");
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
