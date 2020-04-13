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
using HuseHeartsCP.Data;
using BasicGameFrameworkLibrary.TestUtilities;
using BasicControlsAndWindowsCore.Controls.NavigationControls;
using HuseHeartsCP.ViewModels;
using static BasicControlsAndWindowsCore.Helpers.GridHelper; //usually needs this as well for grid helpers.
using BasicGamingUIWPFLibrary.Helpers;
using System.Windows;
using BasicGamingUIWPFLibrary.BasicControls.SingleCardFrames;
using BasicGameFrameworkLibrary.RegularDeckOfCards;
using BasicGamingUIWPFLibrary.GameGraphics.Cards;
using BasicGamingUIWPFLibrary.BasicControls.GameFrames;
using BasicGamingUIWPFLibrary.BasicControls.SimpleControls;
using HuseHeartsCP.Cards;
using BasicGamingUIWPFLibrary.BasicControls.TrickUIs;
using ts = BasicGameFrameworkLibrary.GameGraphicsCP.Cards.DeckOfCardsCP;

namespace HuseHeartsWPF.Views
{
    public class HuseHeartsMainView : UserControl, IUIView, IHandleAsync<LoadEventModel>
    {
        private readonly IEventAggregator _aggregator;
        private readonly HuseHeartsVMData _model;

        private readonly ScoreBoardWPF _score;
        private readonly BaseHandWPF<HuseHeartsCardInformation, ts, DeckOfCardsWPF<HuseHeartsCardInformation>> _playerHandWPF;

        private readonly SeveralPlayersTrickWPF<EnumSuitList, HuseHeartsCardInformation, ts, DeckOfCardsWPF<HuseHeartsCardInformation>, HuseHeartsPlayerItem> _trick1;

        private readonly BaseHandWPF<HuseHeartsCardInformation, ts, DeckOfCardsWPF<HuseHeartsCardInformation>>? _dummy1;
        private readonly BaseHandWPF<HuseHeartsCardInformation, ts, DeckOfCardsWPF<HuseHeartsCardInformation>>? _blind1;


        public HuseHeartsMainView(IEventAggregator aggregator,
            TestOptions test,
            HuseHeartsVMData model
            )
        {
            _aggregator = aggregator;
            _model = model;
            _aggregator.Subscribe(this);

            _score = new ScoreBoardWPF();
            _playerHandWPF = new BaseHandWPF<HuseHeartsCardInformation, ts, DeckOfCardsWPF<HuseHeartsCardInformation>>();
            _playerHandWPF.Divider = 1.4;
            _trick1 = new SeveralPlayersTrickWPF<EnumSuitList, HuseHeartsCardInformation, ts, DeckOfCardsWPF<HuseHeartsCardInformation>, HuseHeartsPlayerItem>();
            _dummy1 = new BaseHandWPF<HuseHeartsCardInformation, ts, DeckOfCardsWPF<HuseHeartsCardInformation>>();
            _blind1 = new BaseHandWPF<HuseHeartsCardInformation, ts, DeckOfCardsWPF<HuseHeartsCardInformation>>();

            _trick1.Width = 350;
            _dummy1.Divider = 1.4;



            StackPanel mainStack = new StackPanel();
            ParentSingleUIContainer? restoreP = null;
            if (test.SaveOption == EnumTestSaveCategory.RestoreOnly)
            {
                restoreP = new ParentSingleUIContainer()
                {
                    Name = nameof(HuseHeartsMainViewModel.RestoreScreen)
                };
            }
            StackPanel otherStack = new StackPanel();
            otherStack.Orientation = Orientation.Horizontal;
            mainStack.Children.Add(otherStack);




            _score.AddColumn("Cards Left", false, nameof(HuseHeartsPlayerItem.ObjectCount)); //very common.
            _score.AddColumn("Tricks Won", false, nameof(HuseHeartsPlayerItem.TricksWon));
            _score.AddColumn("Current Score", false, nameof(HuseHeartsPlayerItem.CurrentScore));
            _score.AddColumn("Previous Score", false, nameof(HuseHeartsPlayerItem.PreviousScore));
            _score.AddColumn("Total Score", false, nameof(HuseHeartsPlayerItem.TotalScore));
            SimpleLabelGrid firstInfo = new SimpleLabelGrid();
            firstInfo.AddRow("Turn", nameof(HuseHeartsMainViewModel.NormalTurn));
            firstInfo.AddRow("Status", nameof(HuseHeartsMainViewModel.Status));
            firstInfo.AddRow("Round", nameof(HuseHeartsMainViewModel.RoundNumber));

            otherStack.Children.Add(_score);
            otherStack.Children.Add(firstInfo.GetContent);
            otherStack.Children.Add(_trick1);
            ParentSingleUIContainer parent = new ParentSingleUIContainer()
            {
                Name = nameof(HuseHeartsMainViewModel.PassingScreen)
            };

            otherStack.Children.Add(parent);
            parent = new ParentSingleUIContainer()
            {
                Name = nameof(HuseHeartsMainViewModel.MoonScreen)
            };
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
