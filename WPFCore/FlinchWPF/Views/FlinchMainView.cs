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
using FlinchCP.Data;
using BasicGameFrameworkLibrary.TestUtilities;
using BasicControlsAndWindowsCore.Controls.NavigationControls;
using FlinchCP.ViewModels;
using static BasicControlsAndWindowsCore.Helpers.GridHelper; //usually needs this as well for grid helpers.
using BasicGamingUIWPFLibrary.Helpers;
using System.Windows;
using BasicGamingUIWPFLibrary.BasicControls.SingleCardFrames;
using BasicGameFrameworkLibrary.RegularDeckOfCards;
using BasicGamingUIWPFLibrary.GameGraphics.Cards;
using BasicGamingUIWPFLibrary.BasicControls.GameFrames;
using BasicGamingUIWPFLibrary.BasicControls.SimpleControls;
using FlinchCP.Cards;

namespace FlinchWPF.Views
{
    public class FlinchMainView : UserControl, IUIView, IHandleAsync<LoadEventModel>
    {
        private readonly IEventAggregator _aggregator;
        private readonly FlinchVMData _model;
        private readonly BaseDeckWPF<FlinchCardInformation, FlinchGraphicsCP, CardGraphicsWPF> _deckGPile;

        private readonly ScoreBoardWPF _score;
        private readonly BaseHandWPF<FlinchCardInformation, FlinchGraphicsCP, CardGraphicsWPF> _playerHandWPF;
        private readonly PublicPilesWPF _publicGraphics;

        public FlinchMainView(IEventAggregator aggregator,
            TestOptions test,
            FlinchVMData model
            )
        {
            _aggregator = aggregator;
            _model = model;
            _aggregator.Subscribe(this);

            _deckGPile = new BaseDeckWPF<FlinchCardInformation, FlinchGraphicsCP, CardGraphicsWPF>();
            _score = new ScoreBoardWPF();
            _playerHandWPF = new BaseHandWPF<FlinchCardInformation, FlinchGraphicsCP, CardGraphicsWPF>();
            _publicGraphics = new PublicPilesWPF();
            _publicGraphics.Width = 700;
            _publicGraphics.Height = 500;
            StackPanel mainStack = new StackPanel();
            ParentSingleUIContainer? restoreP = null;
            if (test.SaveOption == EnumTestSaveCategory.RestoreOnly)
            {
                restoreP = new ParentSingleUIContainer()
                {
                    Name = nameof(FlinchMainViewModel.RestoreScreen)
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
            thisLabel.SetBinding(TextBlock.TextProperty, nameof(FlinchMainViewModel.CardsToShuffle));
            tempStack.Children.Add(thisLabel);
            _score.AddColumn("In Stock", false, nameof(FlinchPlayerItem.InStock));
            int x;
            for (x = 1; x <= 5; x++) //has to change for flinch.
            {
                var thisStr = "Discard" + x;
                _score.AddColumn(thisStr, false, thisStr);
            }
            _score.AddColumn("Stock Left", false, nameof(FlinchPlayerItem.StockLeft));
            _score.AddColumn("Cards Left", false, nameof(FlinchPlayerItem.ObjectCount)); //very common.
            SimpleLabelGrid firstInfo = new SimpleLabelGrid();
            firstInfo.AddRow("Turn", nameof(FlinchMainViewModel.NormalTurn));
            firstInfo.AddRow("Status", nameof(FlinchMainViewModel.Status));
            otherStack.Children.Add(_score);
            Button endButton = GetGamingButton("End Turn", nameof(FlinchMainViewModel.EndTurnAsync));
            endButton.HorizontalAlignment = HorizontalAlignment.Left;
            endButton.VerticalAlignment = VerticalAlignment.Top;
            otherStack.Children.Add(endButton);
            otherStack.Children.Add(firstInfo.GetContent);
            mainStack.Children.Add(_playerHandWPF);
            ParentSingleUIContainer parent = new ParentSingleUIContainer()
            {
                Name = nameof(FlinchMainViewModel.PlayerPilesScreen)
            };
            mainStack.Children.Add(parent);


            _deckGPile.Margin = new Thickness(5, 5, 5, 5);



            if (restoreP != null)
            {
                mainStack.Children.Add(restoreP); //default add to grid but does not have to.
            }
            Content = mainStack;

        }
        Task IHandleAsync<LoadEventModel>.HandleAsync(LoadEventModel message)
        {

            GamePackageViewModelBinder.ManuelElements.Clear(); //often times i have to add manually.

            FlinchSaveInfo save = cons!.Resolve<FlinchSaveInfo>(); //usually needs this part for multiplayer games.

            _score!.LoadLists(save.PlayerList);
            _playerHandWPF!.LoadList(_model.PlayerHand1!, ""); // i think

            _deckGPile!.Init(_model.Deck1!, ""); // try here.  may have to do something else as well (?)
            _deckGPile.StartListeningMainDeck();
            _publicGraphics!.Init(_model.PublicPiles!); // i think
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
