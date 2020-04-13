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
using HitTheDeckCP.Data;
using BasicGameFrameworkLibrary.TestUtilities;
using BasicControlsAndWindowsCore.Controls.NavigationControls;
using HitTheDeckCP.ViewModels;
using static BasicControlsAndWindowsCore.Helpers.GridHelper; //usually needs this as well for grid helpers.
using BasicGamingUIWPFLibrary.Helpers;
using System.Windows;
using BasicGamingUIWPFLibrary.BasicControls.SingleCardFrames;
using BasicGameFrameworkLibrary.RegularDeckOfCards;
using BasicGamingUIWPFLibrary.GameGraphics.Cards;
using BasicGamingUIWPFLibrary.BasicControls.GameFrames;
using BasicGamingUIWPFLibrary.BasicControls.SimpleControls;
using HitTheDeckCP.Cards;

namespace HitTheDeckWPF.Views
{
    public class HitTheDeckMainView : UserControl, IUIView, IHandleAsync<LoadEventModel>
    {
        //TODO:  has to build the main page.  no need for new game since that is completely separated out into another class.
        private readonly IEventAggregator _aggregator;
        private readonly HitTheDeckVMData _model;
        private readonly BaseDeckWPF<HitTheDeckCardInformation, HitTheDeckGraphicsCP, CardGraphicsWPF> _deckGPile;
        private readonly BasePileWPF<HitTheDeckCardInformation, HitTheDeckGraphicsCP, CardGraphicsWPF> _discardGPile;

        private readonly ScoreBoardWPF _score;
        private readonly BaseHandWPF<HitTheDeckCardInformation, HitTheDeckGraphicsCP, CardGraphicsWPF> _playerHandWPF;

        public HitTheDeckMainView(IEventAggregator aggregator,
            TestOptions test,
            HitTheDeckVMData model
            )
        {
            _aggregator = aggregator;
            _model = model;
            _aggregator.Subscribe(this);

            _deckGPile = new BaseDeckWPF<HitTheDeckCardInformation, HitTheDeckGraphicsCP, CardGraphicsWPF>();
            _discardGPile = new BasePileWPF<HitTheDeckCardInformation, HitTheDeckGraphicsCP, CardGraphicsWPF>();
            _score = new ScoreBoardWPF();
            _playerHandWPF = new BaseHandWPF<HitTheDeckCardInformation, HitTheDeckGraphicsCP, CardGraphicsWPF>();

            StackPanel mainStack = new StackPanel();
            ParentSingleUIContainer? restoreP = null;
            if (test.SaveOption == EnumTestSaveCategory.RestoreOnly)
            {
                restoreP = new ParentSingleUIContainer()
                {
                    Name = nameof(HitTheDeckMainViewModel.RestoreScreen)
                };
            }


            StackPanel otherStack = new StackPanel();
            otherStack.Orientation = Orientation.Horizontal;
            otherStack.Children.Add(_deckGPile);
            otherStack.Children.Add(_discardGPile); // can reposition or not even have as well.
            mainStack.Children.Add(otherStack);

            StackPanel tempStack = new StackPanel();
            otherStack.Children.Add(tempStack);
            Button otherButs;
            otherButs = GetGamingButton("Flip Deck", nameof(HitTheDeckMainViewModel.FlipAsync));
            tempStack.Children.Add(otherButs);
            otherButs = GetGamingButton("Cut Deck", nameof(HitTheDeckMainViewModel.CutAsync));
            tempStack.Children.Add(otherButs);
            var endButton = GetGamingButton("End Turn", nameof(HitTheDeckMainViewModel.EndTurnAsync));
            endButton.HorizontalAlignment = HorizontalAlignment.Left;
            tempStack.Children.Add(endButton);
            _score.AddColumn("Cards Left", true, nameof(HitTheDeckPlayerItem.ObjectCount), rightMargin: 5);
            _score.AddColumn("Total Points", true, nameof(HitTheDeckPlayerItem.TotalPoints), rightMargin: 5);
            _score.AddColumn("Previous Points", true, nameof(HitTheDeckPlayerItem.PreviousPoints), rightMargin: 5);
            SimpleLabelGrid firstInfo = new SimpleLabelGrid();
            firstInfo.AddRow("Turn", nameof(HitTheDeckMainViewModel.NormalTurn));
            firstInfo.AddRow("Next", nameof(HitTheDeckMainViewModel.NextPlayer));
            firstInfo.AddRow("Status", nameof(HitTheDeckMainViewModel.Status));

            mainStack.Children.Add(_playerHandWPF);
            mainStack.Children.Add(firstInfo.GetContent);

            mainStack.Children.Add(_score);


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

            HitTheDeckSaveInfo save = cons!.Resolve<HitTheDeckSaveInfo>(); //usually needs this part for multiplayer games.

            _score!.LoadLists(save.PlayerList);
            _playerHandWPF!.LoadList(_model.PlayerHand1!, ""); // i think
            _discardGPile!.Init(_model.Pile1!, ""); // may have to be here (well see)
            _discardGPile.StartListeningDiscardPile(); // its the main one.

            _deckGPile!.Init(_model.Deck1!, ""); // try here.  may have to do something else as well (?)
            _deckGPile.StartListeningMainDeck();

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
