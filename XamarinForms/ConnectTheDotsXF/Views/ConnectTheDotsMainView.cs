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
using ConnectTheDotsCP.Data;
using Xamarin.Forms;
using BasicXFControlsAndPages.MVVMFramework.Controls;
using BasicGameFrameworkLibrary.TestUtilities;
using ConnectTheDotsCP.ViewModels;
using BasicGamingUIXFLibrary.Helpers;
using BasicGamingUIXFLibrary.BasicControls.SimpleControls;
using BasicGamingUIXFLibrary.BasicControls.GameFrames;
using BasicGameFrameworkLibrary.DIContainers;
using ConnectTheDotsCP.Graphics;

namespace ConnectTheDotsXF.Views
{
    public class ConnectTheDotsMainView : ContentView, IUIView, IHandleAsync<LoadEventModel>
    {
        private readonly IEventAggregator _aggregator;
        readonly GameBoardXF _board = new GameBoardXF();
        private readonly ScoreBoardXF _score;
        public ConnectTheDotsMainView(IEventAggregator aggregator,
            TestOptions test, GameBoardGraphicsCP graphicsCP, IGamePackageRegister register
            )
        {
            _aggregator = aggregator;
            _aggregator.Subscribe(this);
            register.RegisterControl(_board.Element, "");
            graphicsCP.LinkBoard();
            StackLayout mainStack = new StackLayout();
            ParentSingleUIContainer? restoreP = null;
            if (test.SaveOption == EnumTestSaveCategory.RestoreOnly)
            {
                restoreP = new ParentSingleUIContainer(nameof(ConnectTheDotsMainViewModel.RestoreScreen));
            }
            _score = new ScoreBoardXF();
            _score.AddColumn("Score", true, nameof(ConnectTheDotsPlayerItem.Score));

            SimpleLabelGridXF firstInfo = new SimpleLabelGridXF();
            firstInfo.AddRow("Turn", nameof(ConnectTheDotsMainViewModel.NormalTurn));
            firstInfo.AddRow("Status", nameof(ConnectTheDotsMainViewModel.Status));


            mainStack.Children.Add(_board);
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
            ConnectTheDotsSaveInfo save = cons!.Resolve<ConnectTheDotsSaveInfo>(); //usually needs this part for multiplayer games.
            _score.LoadLists(save.PlayerList);
            _board.LoadBoard();
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
