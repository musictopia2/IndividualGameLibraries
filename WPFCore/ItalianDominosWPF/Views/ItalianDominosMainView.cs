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
using ItalianDominosCP.Data;
using BasicGameFrameworkLibrary.TestUtilities;
using BasicControlsAndWindowsCore.Controls.NavigationControls;
using ItalianDominosCP.ViewModels;
using static BasicControlsAndWindowsCore.Helpers.GridHelper; //usually needs this as well for grid helpers.
using BasicGamingUIWPFLibrary.Helpers;
using System.Windows;
using ts = BasicGameFrameworkLibrary.GameGraphicsCP.Dominos.DominosCP;
using BasicGamingUIWPFLibrary.BasicControls.GameFrames;
using BasicGameFrameworkLibrary.Dominos;
using BasicGamingUIWPFLibrary.GameGraphics.Dominos;
using SkiaSharp;
using BasicGamingUIWPFLibrary.BasicControls.SimpleControls;

namespace ItalianDominosWPF.Views
{
    public class ItalianDominosMainView : UserControl, IUIView, IHandleAsync<LoadEventModel>
    {
        //TODO:  has to build the main page.  no need for new game since that is completely separated out into another class.
        private readonly IEventAggregator _aggregator;
        private readonly ItalianDominosVMData _model;
        private readonly BoneYardWPF<SimpleDominoInfo, ts, DominosWPF<SimpleDominoInfo>,
            DominosBasicShuffler<SimpleDominoInfo>> _bone;
        private readonly BaseHandWPF<SimpleDominoInfo, ts, DominosWPF<SimpleDominoInfo>> _playerHandWPF;
        private readonly ScoreBoardWPF _score;

        public ItalianDominosMainView(IEventAggregator aggregator,
            TestOptions test,
            ItalianDominosVMData model
            )
        {
            _aggregator = aggregator;
            _model = model;
            _aggregator.Subscribe(this);
            StackPanel mainStack = new StackPanel();
            
            ParentSingleUIContainer? restoreP = null;
            if (test.SaveOption == EnumTestSaveCategory.RestoreOnly)
            {
                restoreP = new ParentSingleUIContainer()
                {
                    Name = nameof(ItalianDominosMainViewModel.RestoreScreen)
                };
            }


            _bone = new BoneYardWPF<SimpleDominoInfo, ts, DominosWPF<SimpleDominoInfo>, DominosBasicShuffler<SimpleDominoInfo>>();
            _playerHandWPF = new BaseHandWPF<SimpleDominoInfo, ts, DominosWPF<SimpleDominoInfo>>();
            _score = new ScoreBoardWPF();
            _bone.Height = 400;
            _bone.Width = 800; //can adjust as needed.
            mainStack.Children.Add(_bone);
            mainStack.Children.Add(_playerHandWPF);

            //anything else to add can be done as well.
            SimpleLabelGrid firstInfo = new SimpleLabelGrid();
            firstInfo.AddRow("Turn", nameof(ItalianDominosMainViewModel.NormalTurn));
            firstInfo.AddRow("Status", nameof(ItalianDominosMainViewModel.Status));
            firstInfo.AddRow("Up To", nameof(ItalianDominosMainViewModel.UpTo));
            firstInfo.AddRow("Next #", nameof(ItalianDominosMainViewModel.NextNumber));
            StackPanel otherStack = new StackPanel();
            otherStack.Orientation = Orientation.Horizontal;
            var thisBut = GetGamingButton("Play", nameof(ItalianDominosMainViewModel.PlayAsync));
            otherStack.Children.Add(thisBut);
            _score.AddColumn("Total Score", true, nameof(ItalianDominosPlayerItem.TotalScore), rightMargin: 10);
            _score.AddColumn("Dominos Left", true, nameof(ItalianDominosPlayerItem.ObjectCount), rightMargin: 10); // if not important, can just comment
            _score.AddColumn("Drew Yet", true, nameof(ItalianDominosPlayerItem.DrewYet), useTrueFalse: true);
            otherStack.Children.Add(firstInfo.GetContent);
            otherStack.Children.Add(_score);
            mainStack.Children.Add(otherStack); // this may be missing as well.

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
           
            ItalianDominosSaveInfo save = cons!.Resolve<ItalianDominosSaveInfo>(); //usually needs this part for multiplayer games.

            _score!.LoadLists(save.PlayerList);
            _playerHandWPF!.LoadList(_model.PlayerHand1, ts.TagUsed);
            _model.BoneYard.MaxSize = new SKSize(750, 350);
            _model.BoneYard.ScatterPieces();
            _bone!.LoadList(_model.BoneYard, ts.TagUsed);

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
