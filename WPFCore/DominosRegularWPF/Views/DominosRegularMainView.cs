using BasicControlsAndWindowsCore.Controls.NavigationControls;
using BasicGameFrameworkLibrary.BasicEventModels;
using BasicGameFrameworkLibrary.DIContainers;
using BasicGameFrameworkLibrary.Dominos;
using BasicGameFrameworkLibrary.TestUtilities;
using BasicGamingUIWPFLibrary.BasicControls.GameFrames;
using BasicGamingUIWPFLibrary.BasicControls.SimpleControls;
using BasicGamingUIWPFLibrary.GameGraphics.Dominos;
using BasicGamingUIWPFLibrary.Helpers;
using CommonBasicStandardLibraries.Messenging;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using DominosRegularCP.Data;
using DominosRegularCP.ViewModels;
using SkiaSharp;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using System.Windows;
using System.Windows.Controls;
using static BasicGamingUIWPFLibrary.Helpers.SharedUIFunctions; //this usually will be used too.
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;
using ts = BasicGameFrameworkLibrary.GameGraphicsCP.Dominos.DominosCP;

namespace DominosRegularWPF.Views
{
    public class DominosRegularMainView : UserControl, IUIView, IHandleAsync<LoadEventModel>
    {
        private readonly IEventAggregator _aggregator;
        private readonly DominosRegularVMData _model;
        private readonly IGamePackageResolver _resolver;
        private readonly BoneYardWPF<SimpleDominoInfo, ts, DominosWPF<SimpleDominoInfo>,
            DominosBasicShuffler<SimpleDominoInfo>> _bone;
        private readonly BaseHandWPF<SimpleDominoInfo, ts, DominosWPF<SimpleDominoInfo>> _playerHandWPF;
        private readonly ScoreBoardWPF _score;
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
            StackPanel mainStack = new StackPanel();

            ParentSingleUIContainer? restoreP = null;
            if (test.SaveOption == EnumTestSaveCategory.RestoreOnly)
            {
                restoreP = new ParentSingleUIContainer()
                {
                    Name = nameof(DominosRegularMainViewModel.RestoreScreen)
                };
            }


            _bone = new BoneYardWPF<SimpleDominoInfo, ts, DominosWPF<SimpleDominoInfo>, DominosBasicShuffler<SimpleDominoInfo>>();
            _playerHandWPF = new BaseHandWPF<SimpleDominoInfo, ts, DominosWPF<SimpleDominoInfo>>();
            _score = new ScoreBoardWPF();
            _gameBoard1 = new GameBoardUI();
            _bone.Height = 300;
            _bone.Width = 800; //can adjust as needed.
            mainStack.Children.Add(_bone);
            mainStack.Children.Add(_gameBoard1);
            mainStack.Children.Add(_playerHandWPF);
            SimpleLabelGrid firstInfo = new SimpleLabelGrid();
            firstInfo.AddRow("Turn", nameof(DominosRegularMainViewModel.NormalTurn));
            firstInfo.AddRow("Status", nameof(DominosRegularMainViewModel.Status));
            mainStack.Children.Add(firstInfo.GetContent);
            _score.AddColumn("Total Score", true, nameof(DominosRegularPlayerItem.TotalScore));
            _score.AddColumn("Dominos Left", true, nameof(DominosRegularPlayerItem.ObjectCount)); // if not important, can just comment
            mainStack.Children.Add(_score);
            Button endTurn = GetGamingButton("End Turn", nameof(DominosRegularMainViewModel.EndTurnAsync));
            endTurn.HorizontalAlignment = HorizontalAlignment.Left;
            mainStack.Children.Add(endTurn);




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

            DominosRegularSaveInfo save = cons!.Resolve<DominosRegularSaveInfo>(); //usually needs this part for multiplayer games.

            _score!.LoadLists(save.PlayerList);
            _playerHandWPF!.LoadList(_model.PlayerHand1, ts.TagUsed);
            _model.BoneYard.MaxSize = new SKSize(750, 350);
            _model.BoneYard.ScatterPieces();
            _bone!.LoadList(_model.BoneYard, ts.TagUsed);
            _gameBoard1!.LoadList(_model, _resolver);
            return this.RefreshBindingsAsync(_aggregator);
        }



        Task IUIView.TryActivateAsync()
        {
            return Task.CompletedTask;
        }

        Task IUIView.TryCloseAsync()
        {
            _aggregator.Unsubscribe(this);
            return Task.CompletedTask;
        }
    }
}
