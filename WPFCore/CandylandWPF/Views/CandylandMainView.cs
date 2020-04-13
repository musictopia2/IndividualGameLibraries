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
using CandylandCP.Data;
using BasicGameFrameworkLibrary.TestUtilities;
using BasicControlsAndWindowsCore.Controls.NavigationControls;
using CandylandCP.ViewModels;
using static BasicControlsAndWindowsCore.Helpers.GridHelper; //usually needs this as well for grid helpers.
using BasicGamingUIWPFLibrary.Helpers;
using System.Windows;
using System.Windows.Media;
using BasicGameFrameworkLibrary.DIContainers;
using BasicControlsAndWindowsCore.Helpers;

namespace CandylandWPF.Views
{
    public class CandylandMainView : UserControl, IUIView, IHandleAsync<LoadEventModel>, IHandle<CandylandCardData>, IHandle<NewTurnEventModel>
    {
        private readonly IEventAggregator _aggregator;
        private readonly CardGraphicsWPF _ourCard;
        private readonly GameboardWPF _ourBoard;
        private readonly PieceWPF _ourPiece;
        public CandylandMainView(IEventAggregator aggregator,
            TestOptions test,
            IGamePackageRegister register //unless i extend the contract, will be done this way.
            )
        {
            Background = Brushes.White;
            _aggregator = aggregator;
            _aggregator.Subscribe(this);
            StackPanel mainStack = new StackPanel();
            ParentSingleUIContainer? restoreP = null;
            if (test.SaveOption == EnumTestSaveCategory.RestoreOnly)
            {
                restoreP = new ParentSingleUIContainer()
                {
                    Name = nameof(CandylandMainViewModel.RestoreScreen)
                };
            }

            StackPanel otherStack = new StackPanel();
            _ourBoard = new GameboardWPF();
            register.RegisterControl(_ourBoard.Element1, "main");
            //register!.RegisterSingleton(_ourBoard.Element1, "main"); //i think
            _ourCard = new CardGraphicsWPF(); // bindings are finished
            _ourCard.SendSize("main", new CandylandCardData());
            otherStack.Margin = new Thickness(5, 5, 5, 5);
            otherStack.Orientation = Orientation.Horizontal;
            StackPanel firstStack = new StackPanel();
            otherStack.Children.Add(firstStack);
            firstStack.Children.Add(_ourCard); //you already subscribed.  just hook up another event for this.
            _ourPiece = new PieceWPF();
            _ourPiece.Margin = new Thickness(0, 5, 0, 0);
            _ourPiece.SetSizes();
            BaseLabelGrid firstInfo = new BaseLabelGrid();
            firstInfo.AddRow("Turn", nameof(CandylandMainViewModel.NormalTurn));
            firstInfo.AddRow("Status", nameof(CandylandMainViewModel.Status));
            firstStack.Children.Add(firstInfo.GetContent);
            firstStack.Children.Add(_ourPiece);
            _ourBoard.HorizontalAlignment = HorizontalAlignment.Left;
            _ourBoard.VerticalAlignment = VerticalAlignment.Top;
            _ourBoard.Margin = new Thickness(5, 0, 0, 0);
            otherStack.Children.Add(_ourBoard);
            mainStack.Children.Add(otherStack);
            if (restoreP != null)
            {
                //todo:  figure out where to place the restore ui if there is a restore option.
                mainStack.Children.Add(restoreP); //default add to grid but does not have to.
            }
            Content = mainStack;

        }

        void IHandle<CandylandCardData>.Handle(CandylandCardData message)
        {
            _ourCard!.DataContext = null;
            _ourCard.DataContext = message;
        }

        void IHandle<NewTurnEventModel>.Handle(NewTurnEventModel message)
        {
            _ourPiece!.MainColor = _ourBoard!.PieceForCurrentPlayer(); //hopefully that is it.
        }

        Task IHandleAsync<LoadEventModel>.HandleAsync(LoadEventModel message)
        {
            _ourBoard!.LoadBoard();
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
