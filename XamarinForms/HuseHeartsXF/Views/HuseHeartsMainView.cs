using BasicGameFrameworkLibrary.BasicEventModels;
using BasicGameFrameworkLibrary.RegularDeckOfCards;
using BasicGameFrameworkLibrary.TestUtilities;
using BasicGamingUIXFLibrary.BasicControls.GameFrames;
using BasicGamingUIXFLibrary.BasicControls.SimpleControls;
using BasicGamingUIXFLibrary.BasicControls.TrickUIs;
using BasicGamingUIXFLibrary.GameGraphics.Cards;
using BasicGamingUIXFLibrary.Helpers;
using BasicXFControlsAndPages.MVVMFramework.Controls;
using CommonBasicStandardLibraries.Messenging;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using HuseHeartsCP.Cards;
using HuseHeartsCP.Data;
using HuseHeartsCP.ViewModels;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using Xamarin.Forms;
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;
using ts = BasicGameFrameworkLibrary.GameGraphicsCP.Cards.DeckOfCardsCP;

namespace HuseHeartsXF.Views
{
    public class HuseHeartsMainView : ContentView, IUIView, IHandleAsync<LoadEventModel>
    {
        private readonly IEventAggregator _aggregator;
        private readonly HuseHeartsVMData _model;

        private readonly ScoreBoardXF _score;
        private readonly BaseHandXF<HuseHeartsCardInformation, ts, DeckOfCardsXF<HuseHeartsCardInformation>> _playerHandWPF;

        private readonly SeveralPlayersTrickXF<EnumSuitList, HuseHeartsCardInformation, ts, DeckOfCardsXF<HuseHeartsCardInformation>, HuseHeartsPlayerItem> _trick1;

        private readonly BaseHandXF<HuseHeartsCardInformation, ts, DeckOfCardsXF<HuseHeartsCardInformation>>? _dummy1;
        private readonly BaseHandXF<HuseHeartsCardInformation, ts, DeckOfCardsXF<HuseHeartsCardInformation>>? _blind1;

        public HuseHeartsMainView(IEventAggregator aggregator,
            TestOptions test,
            HuseHeartsVMData model
            )
        {
            _aggregator = aggregator;
            _model = model;
            _aggregator.Subscribe(this);

            _score = new ScoreBoardXF();
            _playerHandWPF = new BaseHandXF<HuseHeartsCardInformation, ts, DeckOfCardsXF<HuseHeartsCardInformation>>();
            _trick1 = new SeveralPlayersTrickXF<EnumSuitList, HuseHeartsCardInformation, ts, DeckOfCardsXF<HuseHeartsCardInformation>, HuseHeartsPlayerItem>();
            _dummy1 = new BaseHandXF<HuseHeartsCardInformation, ts, DeckOfCardsXF<HuseHeartsCardInformation>>();
            _blind1 = new BaseHandXF<HuseHeartsCardInformation, ts, DeckOfCardsXF<HuseHeartsCardInformation>>();
            _dummy1.Divider = 1.4;
            _playerHandWPF.Divider = 1.4;
            StackLayout mainStack = new StackLayout();
            ParentSingleUIContainer? restoreP = null;
            if (test.SaveOption == EnumTestSaveCategory.RestoreOnly)
            {
                restoreP = new ParentSingleUIContainer(nameof(HuseHeartsMainViewModel.RestoreScreen));
            }

            _score.AddColumn("Cards Left", false, nameof(HuseHeartsPlayerItem.ObjectCount)); //very common.
            _score.AddColumn("Tricks Won", false, nameof(HuseHeartsPlayerItem.TricksWon));
            _score.AddColumn("Current Score", false, nameof(HuseHeartsPlayerItem.CurrentScore));
            _score.AddColumn("Previous Score", false, nameof(HuseHeartsPlayerItem.PreviousScore));
            _score.AddColumn("Total Score", false, nameof(HuseHeartsPlayerItem.TotalScore));
            SimpleLabelGridXF firstInfo = new SimpleLabelGridXF();
            firstInfo.AddRow("Turn", nameof(HuseHeartsMainViewModel.NormalTurn));
            firstInfo.AddRow("Status", nameof(HuseHeartsMainViewModel.Status));
            firstInfo.AddRow("Round", nameof(HuseHeartsMainViewModel.RoundNumber));


            StackLayout otherStack = new StackLayout();
            otherStack.Orientation = StackOrientation.Horizontal;
            mainStack.Children.Add(otherStack);

            

            otherStack.Children.Add(_score);
            otherStack.Children.Add(firstInfo.GetContent);
            otherStack.Children.Add(_trick1);
            ParentSingleUIContainer parent = new ParentSingleUIContainer(nameof(HuseHeartsMainViewModel.PassingScreen));

            otherStack.Children.Add(parent);
            parent = new ParentSingleUIContainer(nameof(HuseHeartsMainViewModel.MoonScreen));
            otherStack.Children.Add(parent);
            otherStack.Children.Add(_blind1);
            mainStack.Children.Add(_playerHandWPF);
            mainStack.Children.Add(_dummy1);




            if (restoreP != null)
            {
                mainStack.Children.Add(restoreP); //default add to grid but does not have to.
            }
            Content = mainStack;
        }
        Task IHandleAsync<LoadEventModel>.HandleAsync(LoadEventModel message)
        {
            GamePackageViewModelBinder.ManuelElements.Clear(); //often times i have to add manually.

            HuseHeartsSaveInfo save = cons!.Resolve<HuseHeartsSaveInfo>(); //usually needs this part for multiplayer games.

            _score!.LoadLists(save.PlayerList);
            _playerHandWPF!.LoadList(_model.PlayerHand1!, ts.TagUsed); // i think
            _trick1!.Init(_model.TrickArea1!, _model.TrickArea1, ts.TagUsed);

            _dummy1!.LoadList(_model.Dummy1!, ts.TagUsed);
            _blind1!.LoadList(_model.Blind1!, ts.TagUsed);

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
            _trick1.Dispose();
            return Task.CompletedTask;
        }
    }
}
