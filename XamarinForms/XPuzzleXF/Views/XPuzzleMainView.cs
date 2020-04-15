using BasicGameFrameworkLibrary.BasicEventModels;
using CommonBasicStandardLibraries.Messenging;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using Xamarin.Forms;
using XPuzzleCP.Data;
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;

namespace XPuzzleXF.Views
{
    public class XPuzzleMainView : ContentView, IUIView, IHandleAsync<LoadEventModel>
    {
        object IUIView.DataContext
        {
            get => BindingContext;
            set => BindingContext = value;
        }

        private readonly XPuzzleGameBoard? _thisBoard;
        private readonly IEventAggregator _aggregator;

        public XPuzzleMainView(IEventAggregator aggregator)
        {
            //looks like the board is all we have this time.



            _thisBoard = new XPuzzleGameBoard();
            aggregator.Subscribe(this);
            _thisBoard.HorizontalOptions = LayoutOptions.Center;
            Content = _thisBoard;
            _aggregator = aggregator;
        }

        Task IHandleAsync<LoadEventModel>.HandleAsync(LoadEventModel message)
        {



            XPuzzleSaveInfo thisSave = cons!.Resolve<XPuzzleSaveInfo>();


            _thisBoard!.CreateControls(thisSave.SpaceList);
            return this.RefreshBindingsAsync(_aggregator);
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
