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
using LifeBoardGameCP.Data;
using Xamarin.Forms;
using BasicXFControlsAndPages.MVVMFramework.Controls;
using BasicGameFrameworkLibrary.TestUtilities;
using LifeBoardGameCP.ViewModels;
using BasicGamingUIXFLibrary.Helpers;
using BasicGamingUIXFLibrary.BasicControls.SimpleControls;
using BasicGameFrameworkLibrary.DIContainers;
using LifeBoardGameCP.Graphics;

namespace LifeBoardGameXF.Views
{
    public class LifeBoardGameMainView : ContentView, IUIView, IHandleAsync<LoadEventModel>
    {
        private readonly IEventAggregator _aggregator;
        public LifeBoardGameMainView(IEventAggregator aggregator,
            IGamePackageRegister register,
            GameBoardGraphicsCP graphicsCP
            )
        {
            _aggregator = aggregator;
            _aggregator.Subscribe(this);

            Grid grid = new Grid();
            AddAutoColumns(grid, 1);
            AddLeftOverColumn(grid, 70);
            AddLeftOverColumn(grid, 30);//i think split 50/50 is fine.

            //ParentSingleUIContainer? restoreP = null;

            //risk no testing.
            //because we don't have restore this time.


            //if (test.SaveOption == EnumTestSaveCategory.RestoreOnly)
            //{
            //    restoreP = new ParentSingleUIContainer(nameof(LifeBoardGameMainViewModel.RestoreScreen));
            //}



            StackLayout stack = new StackLayout();
            stack.Margin = new Thickness(3);
            AddVerticalLabelGroup("Space Details", nameof(LifeBoardGameMainViewModel.GameDetails), stack);
            AddControlToGrid(grid, stack, 0, 2);
            //on tablets will be different.

            GameBoardXF gameBoard = new GameBoardXF(aggregator);
            gameBoard.VerticalOptions = LayoutOptions.Start;
            register.RegisterControl(gameBoard.Element, "");
            graphicsCP.LinkBoard();
            StackLayout finStack = new StackLayout();
            finStack.Children.Add(gameBoard);
            AddControlToGrid(grid, finStack, 0, 0);
            var firstInfo = new SimpleLabelGridXF();
            firstInfo.AddRow("Turn", nameof(LifeBoardGameMainViewModel.NormalTurn));
            firstInfo.AddRow("Instructions", nameof(LifeBoardGameMainViewModel.Instructions));
            Grid temps = firstInfo.GetContent;
            temps.WidthRequest = gameBoard.WidthRequest;
            finStack.Children.Add(firstInfo.GetContent);
            gameBoard.Margin = new Thickness(3);
            stack = new StackLayout();
            //everything else will go to this stack.
            stack.Margin = new Thickness(3);

            Type type = typeof(LifeBoardGameMainViewModel);
            CustomBasicList<string> list = type.GetProperties(x => x.Name.EndsWith("Screen") && x.Name != "MainScreen" && x.Name != "BoardScreen").Select(x => x.Name).ToCustomBasicList();
            if (list.Count == 0)
            {
                throw new BasicBlankException("No screens found using reflection.  Rethink");
            }
            list.ForEach(x =>
            {
                ParentSingleUIContainer middle = new ParentSingleUIContainer(x)
                {
                    HorizontalOptions = LayoutOptions.Start
                    //Margin = new Thickness(5)
                };
                stack.Children.Add(middle);
            });
            AddControlToGrid(grid, stack, 0, 1);


            //ParentSingleUIContainer board = new ParentSingleUIContainer(nameof(LifeBoardGameMainViewModel.BoardScreen));


            //decided to attempt to use same format for desktop as for tablets.





            //if (restoreP != null)
            //{
            //    //todo:  figure out where to place the restore ui if there is a restore option.
            //    mainStack.Children.Add(restoreP); //default add to grid but does not have to.
            //}
            Content = grid;
        }
        Task IHandleAsync<LoadEventModel>.HandleAsync(LoadEventModel message)
        {

            GamePackageViewModelBinder.ManuelElements.Clear(); //often times i have to add manually.

            //LifeBoardGameSaveInfo save = cons!.Resolve<LifeBoardGameSaveInfo>(); //usually needs this part for multiplayer games.

            return Task.CompletedTask;
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
