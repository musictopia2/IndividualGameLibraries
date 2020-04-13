using BasicControlsAndWindowsCore.Controls.NavigationControls;
using BasicGameFrameworkLibrary.BasicEventModels;
using BasicGameFrameworkLibrary.RegularDeckOfCards;
using BasicGameFrameworkLibrary.TestUtilities;
using BasicGamingUIWPFLibrary.BasicControls.GameFrames;
using BasicGamingUIWPFLibrary.BasicControls.SimpleControls;
using BasicGamingUIWPFLibrary.BasicControls.TrickUIs;
using BasicGamingUIWPFLibrary.GameGraphics.Cards;
using BasicGamingUIWPFLibrary.Helpers;
using CommonBasicStandardLibraries.Messenging;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using Spades2PlayerCP.Cards;
using Spades2PlayerCP.Data;
using Spades2PlayerCP.ViewModels;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using System.Windows.Controls;
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;
using ts = BasicGameFrameworkLibrary.GameGraphicsCP.Cards.DeckOfCardsCP;

namespace Spades2PlayerWPF.Views
{
    public class Spades2PlayerMainView : UserControl, IUIView, IHandleAsync<LoadEventModel>
    {
        private readonly IEventAggregator _aggregator;
        private readonly Spades2PlayerVMData _model;

        private readonly ScoreBoardWPF _score;
        private readonly BaseHandWPF<Spades2PlayerCardInformation, ts, DeckOfCardsWPF<Spades2PlayerCardInformation>> _playerHandWPF;

        private readonly TwoPlayerTrickWPF<EnumSuitList, Spades2PlayerCardInformation, ts, DeckOfCardsWPF<Spades2PlayerCardInformation>> _trick1;


        public Spades2PlayerMainView(IEventAggregator aggregator,
            TestOptions test,
            Spades2PlayerVMData model
            )
        {
            _aggregator = aggregator;
            _model = model;
            _aggregator.Subscribe(this);

            _score = new ScoreBoardWPF();
            _playerHandWPF = new BaseHandWPF<Spades2PlayerCardInformation, ts, DeckOfCardsWPF<Spades2PlayerCardInformation>>();

            _trick1 = new TwoPlayerTrickWPF<EnumSuitList, Spades2PlayerCardInformation, ts, DeckOfCardsWPF<Spades2PlayerCardInformation>>();


            StackPanel mainStack = new StackPanel();
            ParentSingleUIContainer? restoreP = null;
            if (test.SaveOption == EnumTestSaveCategory.RestoreOnly)
            {
                restoreP = new ParentSingleUIContainer()
                {
                    Name = nameof(Spades2PlayerMainViewModel.RestoreScreen)
                };
            }


            ParentSingleUIContainer parent = new ParentSingleUIContainer()
            {
                Name = nameof(Spades2PlayerMainViewModel.BeginningScreen)
            };
            mainStack.Children.Add(parent);


            _score.AddColumn("Cards Left", false, nameof(Spades2PlayerPlayerItem.ObjectCount)); //very common.
            _score.AddColumn("# Bidded", false, nameof(Spades2PlayerPlayerItem.HowManyBids));
            _score.AddColumn("Tricks Won", false, nameof(Spades2PlayerPlayerItem.TricksWon));
            _score.AddColumn("Bags", false, nameof(Spades2PlayerPlayerItem.Bags));
            _score.AddColumn("Current Score", false, nameof(Spades2PlayerPlayerItem.CurrentScore));
            _score.AddColumn("Total Score", false, nameof(Spades2PlayerPlayerItem.TotalScore));
            SimpleLabelGrid firstInfo = new SimpleLabelGrid();
            firstInfo.AddRow("Turn", nameof(Spades2PlayerMainViewModel.NormalTurn));
            firstInfo.AddRow("Status", nameof(Spades2PlayerMainViewModel.Status));
            firstInfo.AddRow("Round", nameof(Spades2PlayerMainViewModel.RoundNumber));

            mainStack.Children.Add(_trick1);
            parent = new ParentSingleUIContainer()
            {
                Name = nameof(Spades2PlayerMainViewModel.BiddingScreen)
            };
            mainStack.Children.Add(parent);
            mainStack.Children.Add(_playerHandWPF);
            mainStack.Children.Add(firstInfo.GetContent);

            mainStack.Children.Add(_score);





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

            Spades2PlayerSaveInfo save = cons!.Resolve<Spades2PlayerSaveInfo>(); //usually needs this part for multiplayer games.

            _score!.LoadLists(save.PlayerList);
            _playerHandWPF!.LoadList(_model.PlayerHand1!, ts.TagUsed); // i think
            _trick1!.Init(_model.TrickArea1!, ts.TagUsed);
            return this.RefreshBindingsAsync(_aggregator);
        }



        Task IUIView.TryActivateAsync()
        {
            return Task.CompletedTask;
        }

        Task IUIView.TryCloseAsync()
        {
            _trick1.Dispose();
            return Task.CompletedTask;
        }
    }
}
