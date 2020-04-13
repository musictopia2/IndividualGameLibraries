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
using CribbageCP.Data;
using BasicGameFrameworkLibrary.TestUtilities;
using BasicControlsAndWindowsCore.Controls.NavigationControls;
using CribbageCP.ViewModels;
using static BasicControlsAndWindowsCore.Helpers.GridHelper; //usually needs this as well for grid helpers.
using BasicGamingUIWPFLibrary.Helpers;
using System.Windows;
using ts = BasicGameFrameworkLibrary.GameGraphicsCP.Cards.DeckOfCardsCP;
using BasicGamingUIWPFLibrary.BasicControls.SingleCardFrames;
using BasicGameFrameworkLibrary.RegularDeckOfCards;
using BasicGamingUIWPFLibrary.GameGraphics.Cards;
using BasicGamingUIWPFLibrary.BasicControls.GameFrames;
using BasicGamingUIWPFLibrary.BasicControls.SimpleControls;

namespace CribbageWPF.Views
{
    public class CribbageMainView : UserControl, IUIView, IHandleAsync<LoadEventModel>
    {
        //TODO:  has to build the main page.  no need for new game since that is completely separated out into another class.
        private readonly IEventAggregator _aggregator;
        private readonly CribbageVMData _model;
        private readonly BaseDeckWPF<CribbageCard, ts, DeckOfCardsWPF<CribbageCard>> _deckGPile;
        private readonly BasePileWPF<CribbageCard, ts, DeckOfCardsWPF<CribbageCard>> _discardGPile;

        private readonly ScoreBoardWPF _score;
        private readonly BaseHandWPF<CribbageCard, ts, DeckOfCardsWPF<CribbageCard>> _playerHandWPF;

        private readonly BaseHandWPF<CribbageCard, ts, DeckOfCardsWPF<CribbageCard>> _crib1;
        private readonly BaseHandWPF<CribbageCard, ts, DeckOfCardsWPF<CribbageCard>> _main1;
        private readonly ScoreUI _otherScore = new ScoreUI();
        public CribbageMainView(IEventAggregator aggregator,
            TestOptions test,
            CribbageVMData model
            )
        {
            _aggregator = aggregator;
            _model = model;
            _aggregator.Subscribe(this);

            _deckGPile = new BaseDeckWPF<CribbageCard, ts, DeckOfCardsWPF<CribbageCard>>();
            _discardGPile = new BasePileWPF<CribbageCard, ts, DeckOfCardsWPF<CribbageCard>>();
            _score = new ScoreBoardWPF();
            _playerHandWPF = new BaseHandWPF<CribbageCard, ts, DeckOfCardsWPF<CribbageCard>>();
            _crib1 = new BaseHandWPF<CribbageCard, ts, DeckOfCardsWPF<CribbageCard>>();
            _main1 = new BaseHandWPF<CribbageCard, ts, DeckOfCardsWPF<CribbageCard>>();
            _otherScore = new ScoreUI();
            _main1.Divider = 1.5;


            StackPanel mainStack = new StackPanel();
            ParentSingleUIContainer? restoreP = null;
            if (test.SaveOption == EnumTestSaveCategory.RestoreOnly)
            {
                restoreP = new ParentSingleUIContainer()
                {
                    Name = nameof(CribbageMainViewModel.RestoreScreen)
                };
            }


            StackPanel otherStack = new StackPanel();
            otherStack.Orientation = Orientation.Horizontal;
            otherStack.Children.Add(_deckGPile);
            otherStack.Children.Add(_discardGPile); // can reposition or not even have as well.
            otherStack.Children.Add(_main1);
            mainStack.Children.Add(otherStack);



            _score.AddColumn("Cards Left", false, nameof(CribbagePlayerItem.ObjectCount)); //very common.
            _score.AddColumn("Is Skunk Hole", false, nameof(CribbagePlayerItem.IsSkunk), useTrueFalse: true);
            _score.AddColumn("First Position", false, nameof(CribbagePlayerItem.FirstPosition));
            _score.AddColumn("Second Position", false, nameof(CribbagePlayerItem.SecondPosition));
            _score.AddColumn("Score Round", false, nameof(CribbagePlayerItem.ScoreRound));
            _score.AddColumn("Total Score", false, nameof(CribbagePlayerItem.TotalScore));
            SimpleLabelGrid firstInfo = new SimpleLabelGrid();
            firstInfo.AddRow("Turn", nameof(CribbageMainViewModel.NormalTurn));
            firstInfo.AddRow("Status", nameof(CribbageMainViewModel.Status));
            firstInfo.AddRow("Dealer", nameof(CribbageMainViewModel.Dealer));
            SimpleLabelGrid others = new SimpleLabelGrid();
            others.AddRow("Count", nameof(CribbageMainViewModel.TotalCount));
            mainStack.Children.Add(_playerHandWPF);
            otherStack = new StackPanel();
            otherStack.Orientation = Orientation.Horizontal;
            mainStack.Children.Add(otherStack);

            var button = GetGamingButton("Continue", nameof(CribbageMainViewModel.ContinueAsync));
            otherStack.Children.Add(button);
            button = GetGamingButton("To Crib", nameof(CribbageMainViewModel.CribAsync));
            otherStack.Children.Add(button);
            button = GetGamingButton("Play", nameof(CribbageMainViewModel.PlayAsync));
            otherStack.Children.Add(button);
            otherStack = new StackPanel();
            otherStack.Orientation = Orientation.Horizontal;
            mainStack.Children.Add(otherStack);
            otherStack.Children.Add(firstInfo.GetContent);
            otherStack.Children.Add(_crib1);
            Grid finalGrid = new Grid();
            AddPixelRow(finalGrid, 300); // hopefully this is enough
            AddLeftOverRow(finalGrid, 1);
            AddLeftOverColumn(finalGrid, 70);
            AddLeftOverColumn(finalGrid, 30);
            AddControlToGrid(finalGrid, mainStack, 0, 0);
            Grid.SetRowSpan(mainStack, 2);
            StackPanel finalStack = new StackPanel();
            finalStack.Children.Add(others.GetContent);
            finalStack.Children.Add(_otherScore);
            AddControlToGrid(finalGrid, finalStack, 0, 1);
            AddControlToGrid(finalGrid, _score, 1, 1);


            _deckGPile.Margin = new Thickness(5, 5, 5, 5);

            _discardGPile.Margin = new Thickness(5, 5, 5, 5);


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

            CribbageSaveInfo save = cons!.Resolve<CribbageSaveInfo>(); //usually needs this part for multiplayer games.

            _score!.LoadLists(save.PlayerList);
            _playerHandWPF!.LoadList(_model.PlayerHand1!, ts.TagUsed); // i think
            _discardGPile!.Init(_model.Pile1!, ts.TagUsed); // may have to be here (well see)
            _discardGPile.StartListeningDiscardPile(); // its the main one.

            _deckGPile!.Init(_model.Deck1!, ts.TagUsed); // try here.  may have to do something else as well (?)
            _deckGPile.StartListeningMainDeck();

            _crib1!.LoadList(_model!.CribFrame!, ts.TagUsed);
            _main1!.LoadList(_model.MainFrame!, ts.TagUsed);
            _otherScore.LoadLists(_model);

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
