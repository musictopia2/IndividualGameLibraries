using BasicGameFrameworkLibrary.BasicEventModels;
using BasicGameFrameworkLibrary.DIContainers;
using BasicGameFrameworkLibrary.Dominos;
using BasicGameFrameworkLibrary.StandardImplementations.CrossPlatform.DataClasses;
using BasicGameFrameworkLibrary.TestUtilities;
using BasicGamingUIXFLibrary.BasicControls.GameFrames;
using BasicGamingUIXFLibrary.BasicControls.SimpleControls;
using BasicGamingUIXFLibrary.GameGraphics.Dominos;
using BasicGamingUIXFLibrary.Helpers;
using BasicXFControlsAndPages.MVVMFramework.Controls;
using CommonBasicStandardLibraries.Messenging;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using DominosRegularCP.Data;
using DominosRegularCP.ViewModels;
using SkiaSharp;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using Xamarin.Forms;
using static BasicGameFrameworkLibrary.StandardImplementations.CrossPlatform.DataClasses.GlobalScreenClass;
using static BasicGamingUIXFLibrary.Helpers.SharedUIFunctions; //this usually will be used too.
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;
using ts = BasicGameFrameworkLibrary.GameGraphicsCP.Dominos.DominosCP;

namespace DominosRegularXF.Views
{
    public class DominosRegularMainView : ContentView, IUIView, IHandleAsync<LoadEventModel>
    {
        private readonly IEventAggregator _aggregator;
        private readonly DominosRegularVMData _model;
        private readonly IGamePackageResolver _resolver;
        private readonly BoneYardXF<SimpleDominoInfo, ts, DominosXF<SimpleDominoInfo>,
            DominosBasicShuffler<SimpleDominoInfo>> _bone;
        private readonly BaseHandXF<SimpleDominoInfo, ts, DominosXF<SimpleDominoInfo>> _playerHandXF;
        private readonly ScoreBoardXF _score;
        private readonly GameBoardUI _gameBoard1;
        public DominosRegularMainView(IEventAggregator aggregator,
            TestOptions test,
            DominosRegularVMData model,
            IGamePackageResolver resolver
            )
        {
            _aggregator = aggregator;
            _model = model;
            _resolver = resolver;
            _aggregator.Subscribe(this);
            StackLayout mainStack = new StackLayout();
            _gameBoard1 = new GameBoardUI();
            ParentSingleUIContainer? restoreP = null;
            if (test.SaveOption == EnumTestSaveCategory.RestoreOnly)
            {
                restoreP = new ParentSingleUIContainer(nameof(DominosRegularMainViewModel.RestoreScreen));
            }


            _bone = new BoneYardXF<SimpleDominoInfo, ts, DominosXF<SimpleDominoInfo>, DominosBasicShuffler<SimpleDominoInfo>>();
            _playerHandXF = new BaseHandXF<SimpleDominoInfo, ts, DominosXF<SimpleDominoInfo>>();
            _score = new ScoreBoardXF();
            if (ScreenUsed != EnumScreen.SmallPhone)
                _bone.HeightRequest = 300;
            else
                _bone.HeightRequest = 90;
            _bone.WidthRequest = 300;
            mainStack.Children.Add(_bone);
            SimpleLabelGridXF firstInfo = new SimpleLabelGridXF();
            firstInfo.AddRow("Turn", nameof(DominosRegularMainViewModel.NormalTurn));
            firstInfo.AddRow("Status", nameof(DominosRegularMainViewModel.Status));
            mainStack.Children.Add(firstInfo.GetContent);
            _score.AddColumn("Total Score", true, nameof(DominosRegularPlayerItem.TotalScore));
            _score.AddColumn("Dominos Left", true, nameof(DominosRegularPlayerItem.ObjectCount)); // if not important, can just comment
            mainStack.Children.Add(_score);
            Button endTurn = GetGamingButton("End Turn", nameof(DominosRegularMainViewModel.EndTurnAsync));
            endTurn.HorizontalOptions = LayoutOptions.Start;

            StackLayout otherStack = new StackLayout();
            otherStack.Orientation = StackOrientation.Horizontal;
            mainStack.Children.Add(otherStack);
            otherStack.Children.Add(_gameBoard1);
            mainStack.Children.Add(_playerHandXF);
            otherStack.Children.Add(firstInfo.GetContent);
            otherStack = new StackLayout();
            otherStack.Orientation = StackOrientation.Horizontal;
            otherStack.Children.Add(_score);
            otherStack.Children.Add(endTurn);
            mainStack.Children.Add(otherStack);




            if (restoreP != null)
            {
                mainStack.Children.Add(restoreP); //default add to grid but does not have to.
            }
            Content = mainStack;
        }
        Task IHandleAsync<LoadEventModel>.HandleAsync(LoadEventModel message)
        {

            GamePackageViewModelBinder.ManuelElements.Clear(); //often times i have to add manually.

            DominosRegularSaveInfo save = cons!.Resolve<DominosRegularSaveInfo>(); //usually needs this part for multiplayer games.

            _score!.LoadLists(save.PlayerList);
            _playerHandXF!.LoadList(_model.PlayerHand1, ts.TagUsed);
            _model.BoneYard.MaxSize = new SKSize(600, 600);
            _model.BoneYard.ScatterPieces();
            _bone!.LoadList(_model.BoneYard, ts.TagUsed);
            _gameBoard1!.LoadList(_model, _resolver);
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
            return Task.CompletedTask;
        }
    }
}