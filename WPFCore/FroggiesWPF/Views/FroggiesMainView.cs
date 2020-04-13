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
using FroggiesCP.Data;
using static BasicControlsAndWindowsCore.Helpers.GridHelper;
using FroggiesCP.Logic;
using System.Windows;
using FroggiesCP.ViewModels;
using BasicGamingUIWPFLibrary.BasicControls.SimpleControls;

namespace FroggiesWPF.Views
{
    public class FroggiesMainView : UserControl, IUIView, IHandleAsync<LoadEventModel>
    {
        //TODO:  has to build the main page.  no need for new game since that is completely separated out into another class.
        private readonly IEventAggregator _aggregator;
        public FroggiesMainView(IEventAggregator aggregator, FroggiesMainGameClass game)
        {
            _aggregator = aggregator;
			_aggregator.Subscribe(this);

            Grid thisGrid = new Grid();
            AddAutoColumns(thisGrid, 2);
            StackPanel stack = new StackPanel();
            GameBoardWPF board = new GameBoardWPF(aggregator, game, this);
            board.Margin = new Thickness(5, 5, 5, 5);
            stack.Margin = new Thickness(5, 5, 5, 5);

            AddControlToGrid(thisGrid, board, 0, 0);
            Button redoButton = GetGamingButton("Redo Game", nameof(FroggiesMainViewModel.RedoAsync));
            stack.Children.Add(redoButton);
            SimpleLabelGrid thisLab = new SimpleLabelGrid();
            thisLab.AddRow("Moves Left", nameof(FroggiesMainViewModel.MovesLeft));
            thisLab.AddRow("How Many Frogs Currently", nameof(FroggiesMainViewModel.NumberOfFrogs));
            thisLab.AddRow("How Many Frogs To Start", nameof(FroggiesMainViewModel.StartingFrogs));
            stack.Children.Add(thisLab.GetContent);
            AddControlToGrid(thisGrid, stack, 0, 1);
            Content = thisGrid; //i think.

        }
        Task IHandleAsync<LoadEventModel>.HandleAsync(LoadEventModel message)
        {



            return Task.CompletedTask;
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
