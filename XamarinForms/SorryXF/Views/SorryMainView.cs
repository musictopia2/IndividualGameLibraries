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
using SorryCP.Data;
using Xamarin.Forms;
using BasicXFControlsAndPages.MVVMFramework.Controls;
using BasicGameFrameworkLibrary.TestUtilities;
using SorryCP.ViewModels;
using BasicGamingUIXFLibrary.Helpers;
using BasicGamingUIXFLibrary.BasicControls.SimpleControls;
using BasicGamingUIXFLibrary.BasicControls.GameBoards;
using SorryCP.Graphics;
using BasicGameFrameworkLibrary.DIContainers;
using System.Reflection;

namespace SorryXF.Views
{
    public class SorryMainView : ContentView, IUIView, IHandleAsync<LoadEventModel>
    {
        private readonly IEventAggregator _aggregator;
        readonly CompleteGameBoardXF<GameBoardGraphicsCP> _board = new CompleteGameBoardXF<GameBoardGraphicsCP>();
        public SorryMainView(IEventAggregator aggregator,
            TestOptions test,
            GameBoardGraphicsCP graphicsCP, IGamePackageRegister register
            )
        {
            _aggregator = aggregator;
            _aggregator.Subscribe(this);
            register.RegisterControl(_board.ThisElement, "");
            graphicsCP.LinkBoard();
            StackLayout mainStack = new StackLayout();
            ParentSingleUIContainer? restoreP = null;
            if (test.SaveOption == EnumTestSaveCategory.RestoreOnly)
            {
                restoreP = new ParentSingleUIContainer(nameof(SorryMainViewModel.RestoreScreen));
            }


            var endButton = GetGamingButton("End Turn", nameof(SorryMainViewModel.EndTurnAsync));
            endButton.HorizontalOptions = LayoutOptions.Start;
            SimpleLabelGridXF firstInfo = new SimpleLabelGridXF();
            firstInfo.AddRow("Turn", nameof(SorryMainViewModel.NormalTurn));
            firstInfo.AddRow("Instructions", nameof(SorryMainViewModel.Instructions));
            firstInfo.AddRow("Status", nameof(SorryMainViewModel.Status));


            mainStack.Children.Add(_board);


            mainStack.Children.Add(endButton);
            mainStack.Children.Add(firstInfo.GetContent);


            AddVerticalLabelGroup("Card Details", nameof(SorryMainViewModel.CardDetails), mainStack);


            if (restoreP != null)
            {
                //todo:  figure out where to place the restore ui if there is a restore option.
                mainStack.Children.Add(restoreP); //default add to grid but does not have to.
            }
            Content = mainStack;
        }
        Task IHandleAsync<LoadEventModel>.HandleAsync(LoadEventModel message)
        {

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
