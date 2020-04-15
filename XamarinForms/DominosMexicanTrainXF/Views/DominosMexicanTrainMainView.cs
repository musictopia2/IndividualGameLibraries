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
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using static BasicGamingUIXFLibrary.Helpers.SharedUIFunctions; //this usually will be used too.
using static BasicXFControlsAndPages.Helpers.GridHelper;
using CommonBasicStandardLibraries.Messenging;
using BasicGameFrameworkLibrary.BasicEventModels;
using DominosMexicanTrainCP.Data;
using Xamarin.Forms;
using BasicXFControlsAndPages.MVVMFramework.Controls;
using BasicGameFrameworkLibrary.TestUtilities;
using DominosMexicanTrainCP.ViewModels;
using BasicGamingUIXFLibrary.Helpers;
using BasicGamingUIXFLibrary.BasicControls.GameFrames;
using ts = BasicGameFrameworkLibrary.GameGraphicsCP.Dominos.DominosCP;
using BasicGameFrameworkLibrary.Dominos;
using BasicGamingUIXFLibrary.GameGraphics.Dominos;
using SkiaSharp;

namespace DominosMexicanTrainXF.Views
{
    public class DominosMexicanTrainMainView : ContentView, IUIView, IHandleAsync<LoadEventModel>
    {
        //TODO:  has to build the main page.  no need for new game since that is completely separated out into another class.
        private readonly IEventAggregator _aggregator;
        //private readonly DominosMexicanTrainVMData _model;
        private readonly BoneYardXF<SimpleDominoInfo, ts, DominosXF<SimpleDominoInfo>,
            DominosBasicShuffler<SimpleDominoInfo>> _bone;
        private readonly BaseHandXF<SimpleDominoInfo, ts, DominosXF<SimpleDominoInfo>> _playerHandWPF;
        //private readonly ScoreBoardXF _score;
        public DominosMexicanTrainMainView(IEventAggregator aggregator,
            TestOptions test
            //DominosMexicanTrainVMData model
            )
        {
            _aggregator = aggregator;
            //_model = model;
            _aggregator.Subscribe(this);
            StackLayout mainStack = new StackLayout();

            ParentSingleUIContainer? restoreP = null;
            if (test.SaveOption == EnumTestSaveCategory.RestoreOnly)
            {
                restoreP = new ParentSingleUIContainer(nameof(DominosMexicanTrainMainViewModel.RestoreScreen));
            }


            _bone = new BoneYardXF<SimpleDominoInfo, ts, DominosXF<SimpleDominoInfo>, DominosBasicShuffler<SimpleDominoInfo>>();
            _playerHandWPF = new BaseHandXF<SimpleDominoInfo, ts, DominosXF<SimpleDominoInfo>>();
            //_score = new ScoreBoardXF();
            _bone.HeightRequest = 130;
            _bone.WidthRequest = 600;
            mainStack.Children.Add(_bone);
            mainStack.Children.Add(_playerHandWPF);

            //anything else to add can be done as well.


            if (restoreP != null)
            {
                //todo:  figure out where to place the restore ui if there is a restore option.
                mainStack.Children.Add(restoreP); //default add to grid but does not have to.
            }
            Content = mainStack;
        }
        Task IHandleAsync<LoadEventModel>.HandleAsync(LoadEventModel message)
        {

            //GamePackageViewModelBinder.ManuelElements.Clear(); //often times i have to add manually.

            //DominosMexicanTrainSaveInfo save = cons!.Resolve<DominosMexicanTrainSaveInfo>(); //usually needs this part for multiplayer games.

            //_score!.LoadLists(save.PlayerList);
            //_playerHandWPF!.LoadList(_model.PlayerHand1, ts.TagUsed);
            //_model.BoneYard.MaxSize = new SKSize(550, 250);
            //_model.BoneYard.ScatterPieces();
            //_bone!.LoadList(_model.BoneYard, ts.TagUsed);

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
