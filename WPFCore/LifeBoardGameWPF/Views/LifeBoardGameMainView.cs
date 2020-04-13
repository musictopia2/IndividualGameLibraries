using BasicControlsAndWindowsCore.Controls.NavigationControls;
using BasicGameFrameworkLibrary.BasicEventModels;
using BasicGameFrameworkLibrary.DIContainers;
using BasicGamingUIWPFLibrary.BasicControls.SimpleControls;
using BasicGamingUIWPFLibrary.Helpers;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.Messenging;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using LifeBoardGameCP.Graphics;
using LifeBoardGameCP.ViewModels;
using System;
using System.Linq;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using System.Windows;
using System.Windows.Controls;
using static BasicControlsAndWindowsCore.Helpers.GridHelper; //usually needs this as well for grid helpers.
using static BasicGamingUIWPFLibrary.Helpers.SharedUIFunctions; //this usually will be used too.

namespace LifeBoardGameWPF.Views
{
    //public GameboardView(IEventAggregator aggregator, IGamePackageRegister register, GameBoardGraphicsCP graphicsCP)
    //{
    //    _gameBoard = new GameBoardWPF(aggregator);
    //    register.RegisterControl(_gameBoard.Element, "");
    //    graphicsCP.LinkBoard();
    //    Content = _gameBoard;
    //}
    public class LifeBoardGameMainView : UserControl, IUIView, IHandleAsync<LoadEventModel>
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
            SimpleLabelGrid firstInfo = new SimpleLabelGrid();
            firstInfo.AddRow("Turn", nameof(LifeBoardGameMainViewModel.NormalTurn));
            firstInfo.AddRow("Instructions", nameof(LifeBoardGameMainViewModel.Instructions));
            firstInfo.AddRow("Status", nameof(LifeBoardGameMainViewModel.Status));

            StackPanel stack = new StackPanel();
            stack.Margin = new Thickness(3);
            stack.Children.Add(firstInfo.GetContent);
            AddVerticalLabelGroup("Space Details", nameof(LifeBoardGameMainViewModel.GameDetails), stack);
            AddControlToGrid(grid, stack, 0, 2);
            //on tablets will be different.

            GameBoardWPF gameBoard = new GameBoardWPF(aggregator);
            gameBoard.VerticalAlignment = VerticalAlignment.Top;
            register.RegisterControl(gameBoard.Element, "");
            graphicsCP.LinkBoard();
            AddControlToGrid(grid, gameBoard, 0, 0);
            gameBoard.Margin = new Thickness(3);
            stack = new StackPanel();
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
                ParentSingleUIContainer middle = new ParentSingleUIContainer()
                {
                    Name = x,
                    HorizontalAlignment = HorizontalAlignment.Left
                    //Margin = new Thickness(5)
                };
                stack.Children.Add(middle);
            });
            AddControlToGrid(grid, stack, 0, 1);
            Content = grid;
        }
        Task IHandleAsync<LoadEventModel>.HandleAsync(LoadEventModel message)
        {

            GamePackageViewModelBinder.ManuelElements.Clear(); //often times i have to add manually.

            return Task.CompletedTask;
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
