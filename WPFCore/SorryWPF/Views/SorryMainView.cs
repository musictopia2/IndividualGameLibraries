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
using SorryCP.Data;
using BasicGameFrameworkLibrary.TestUtilities;
using BasicControlsAndWindowsCore.Controls.NavigationControls;
using SorryCP.ViewModels;
using static BasicControlsAndWindowsCore.Helpers.GridHelper; //usually needs this as well for grid helpers.
using BasicGamingUIWPFLibrary.Helpers;
using System.Windows;
using BasicGamingUIWPFLibrary.BasicControls.SimpleControls;
using BasicGamingUIWPFLibrary.BasicControls.GameBoards;
using SorryCP.Graphics;
using BasicGameFrameworkLibrary.DIContainers;

namespace SorryWPF.Views
{
    public class SorryMainView : UserControl, IUIView, IHandleAsync<LoadEventModel>
    {
        private readonly IEventAggregator _aggregator;

        readonly CompleteGameBoard<GameBoardGraphicsCP> _board = new CompleteGameBoard<GameBoardGraphicsCP>();

        public SorryMainView(IEventAggregator aggregator,
            TestOptions test,
            GameBoardGraphicsCP graphicsCP, IGamePackageRegister register
            )
        {
            _aggregator = aggregator;
			_aggregator.Subscribe(this);
            register.RegisterControl(_board.ThisElement, "");
            graphicsCP.LinkBoard();
            StackPanel mainStack = new StackPanel();
            ParentSingleUIContainer? restoreP = null;
            if (test.SaveOption == EnumTestSaveCategory.RestoreOnly)
            {
                restoreP = new ParentSingleUIContainer()
                {
                    Name = nameof(SorryMainViewModel.RestoreScreen)
                };
            }

            StackPanel tempStack = new StackPanel();
            Grid tempGrid = new Grid();
            AddAutoRows(tempGrid, 1);
            AddPixelColumn(tempGrid, 500);
            AddAutoColumns(tempGrid, 1);
            AddControlToGrid(tempGrid, tempStack, 0, 0);
            AddControlToGrid(tempGrid, _board, 0, 1);
            mainStack.Children.Add(tempGrid);
            var endButton = GetGamingButton("End Turn", nameof(SorryMainViewModel.EndTurnAsync));
            endButton.HorizontalAlignment = HorizontalAlignment.Left;
            SimpleLabelGrid firstInfo = new SimpleLabelGrid();
            firstInfo.AddRow("Turn", nameof(SorryMainViewModel.NormalTurn));
            firstInfo.AddRow("Instructions", nameof(SorryMainViewModel.Instructions));
            firstInfo.AddRow("Status", nameof(SorryMainViewModel.Status));
            tempStack.Children.Add(firstInfo.GetContent);
            AddVerticalLabelGroup("Card Details", nameof(SorryMainViewModel.CardDetails), tempStack);
            tempStack.Children.Add(endButton);
            //if we need to put to main, just change to main (?)
            if (restoreP != null)
            {
                mainStack.Children.Add(restoreP); //default add to grid but does not have to.
            }
            Content = mainStack;

        }
        Task IHandleAsync<LoadEventModel>.HandleAsync(LoadEventModel message)
        {
            _board.LoadBoard();
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
