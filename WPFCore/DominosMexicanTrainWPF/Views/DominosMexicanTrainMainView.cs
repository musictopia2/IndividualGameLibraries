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
using DominosMexicanTrainCP.Data;
using BasicGameFrameworkLibrary.TestUtilities;
using BasicControlsAndWindowsCore.Controls.NavigationControls;
using DominosMexicanTrainCP.ViewModels;
using static BasicControlsAndWindowsCore.Helpers.GridHelper; //usually needs this as well for grid helpers.
using BasicGamingUIWPFLibrary.Helpers;
using System.Windows;
using ts = BasicGameFrameworkLibrary.GameGraphicsCP.Dominos.DominosCP;
using BasicGamingUIWPFLibrary.BasicControls.GameFrames;
using BasicGameFrameworkLibrary.Dominos;
using BasicGamingUIWPFLibrary.GameGraphics.Dominos;
using SkiaSharp;
using BasicGameFrameworkLibrary.DIContainers;
using BasicGamingUIWPFLibrary.BasicControls.SimpleControls;
using DominosMexicanTrainCP.Logic;

namespace DominosMexicanTrainWPF.Views
{
    public class DominosMexicanTrainMainView : UserControl, IUIView, IHandleAsync<LoadEventModel>
    {
        private readonly IEventAggregator _aggregator;
        private readonly DominosMexicanTrainVMData _model;
        private readonly BoneYardWPF<MexicanDomino, ts, DominosWPF<MexicanDomino>,
            DominosBasicShuffler<MexicanDomino>> _bone;
        private readonly BaseHandWPF<MexicanDomino, ts, DominosWPF<MexicanDomino>> _playerHandWPF;
        private readonly ScoreBoardWPF _score;
        private readonly BaseHandWPF<MexicanDomino, ts, DominosWPF<MexicanDomino>> _playerTrain;
        private readonly TrainStationWPF _trainG = new TrainStationWPF(); //has to be new because of linking issues.
        public DominosMexicanTrainMainView(IEventAggregator aggregator,
            TestOptions test,
            DominosMexicanTrainVMData model,
            IGamePackageRegister register,
            TrainStationGraphicsCP graphics
            )
        {
            _aggregator = aggregator;
            _model = model;
            _aggregator.Subscribe(this);
            StackPanel mainStack = new StackPanel();
            register.RegisterControl(_trainG.Element, "");
            graphics.LinkBoard();
            ParentSingleUIContainer? restoreP = null;
            if (test.SaveOption == EnumTestSaveCategory.RestoreOnly)
            {
                restoreP = new ParentSingleUIContainer()
                {
                    Name = nameof(DominosMexicanTrainMainViewModel.RestoreScreen)
                };
            }


            _bone = new BoneYardWPF<MexicanDomino, ts, DominosWPF<MexicanDomino>, DominosBasicShuffler<MexicanDomino>>();
            _playerHandWPF = new BaseHandWPF<MexicanDomino, ts, DominosWPF<MexicanDomino>>();
            _score = new ScoreBoardWPF();
            _playerTrain = new BaseHandWPF<MexicanDomino, ts, DominosWPF<MexicanDomino>>();
            _bone.Height = 500;
            _bone.Width = 1050; //can adjust as needed.
            
            mainStack.Children.Add(_playerHandWPF);


            Grid otherGrid = new Grid();
            AddLeftOverColumn(otherGrid, 1);
            AddAutoColumns(otherGrid, 1);
            mainStack.Children.Add(otherGrid);
            StackPanel finalStack = new StackPanel();
            AddControlToGrid(otherGrid, finalStack, 0, 0);
            _trainG.Margin = new Thickness(5, 5, 5, 5);
            _trainG.HorizontalAlignment = HorizontalAlignment.Left;
            _trainG.VerticalAlignment = VerticalAlignment.Top;
            AddControlToGrid(otherGrid, _trainG, 0, 1);
            finalStack.Children.Add(_bone);
            _playerTrain.HorizontalAlignment = HorizontalAlignment.Left;
            finalStack.Children.Add(_playerTrain);
            StackPanel tempstack = new StackPanel();
            tempstack.Orientation = Orientation.Horizontal;
            finalStack.Children.Add(tempstack);
            Button endbutton = GetGamingButton("End Turn", nameof(DominosMexicanTrainMainViewModel.EndTurnAsync));
            endbutton.HorizontalAlignment = HorizontalAlignment.Left;
            tempstack.Children.Add(endbutton);
            Button thisButton = GetGamingButton("Longest Train", nameof(DominosMexicanTrainMainViewModel.LongestTrain));
            tempstack.Children.Add(thisButton);
            SimpleLabelGrid firstInfo = new SimpleLabelGrid();
            firstInfo.AddRow("Turn", nameof(DominosMexicanTrainMainViewModel.NormalTurn));
            firstInfo.AddRow("Status", nameof(DominosMexicanTrainMainViewModel.Status));
            _score.AddColumn("Dominos Left", true, nameof(DominosMexicanTrainPlayerItem.ObjectCount));
            _score.AddColumn("Total Score", true, nameof(DominosMexicanTrainPlayerItem.TotalScore));
            _score.AddColumn("Previous Score", true, nameof(DominosMexicanTrainPlayerItem.PreviousScore));
            _score.AddColumn("# Previous", true, nameof(DominosMexicanTrainPlayerItem.PreviousLeft));
            finalStack.Children.Add(_score);
            finalStack.Children.Add(firstInfo.GetContent);




            if (restoreP != null)
            {
                //todo:  figure out where to place the restore ui if there is a restore option.
                mainStack.Children.Add(restoreP); //default add to grid but does not have to.
            }

            Content = mainStack; //hopefully this is still it (?)
        }
        Task IHandleAsync<LoadEventModel>.HandleAsync(LoadEventModel message)
        {

            GamePackageViewModelBinder.ManuelElements.Clear(); //often times i have to add manually.
           
            DominosMexicanTrainSaveInfo save = cons!.Resolve<DominosMexicanTrainSaveInfo>(); //usually needs this part for multiplayer games.

            _score!.LoadLists(save.PlayerList);
            _playerHandWPF!.LoadList(_model.PlayerHand1, ts.TagUsed);
            _model.BoneYard.MaxSize = new SKSize(1000, 1000);
            _model.BoneYard.ScatterPieces();
            _bone!.LoadList(_model.BoneYard, ts.TagUsed);

            _playerTrain!.LoadList(_model.PrivateTrain1!, ts.TagUsed);
            _trainG.LoadBoard();

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
