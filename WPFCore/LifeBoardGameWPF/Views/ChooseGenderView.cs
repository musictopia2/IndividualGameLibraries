using BasicGameFrameworkLibrary.BasicEventModels;
using BasicGameFrameworkLibrary.GameGraphicsCP.GamePieces;
using BasicGamingUIWPFLibrary.BasicControls.ChoicePickers;
using BasicGamingUIWPFLibrary.BasicControls.SimpleControls;
using BasicGamingUIWPFLibrary.GameGraphics.GamePieces;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using CommonBasicStandardLibraries.Messenging;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using LifeBoardGameCP.Data;
using LifeBoardGameCP.ViewModels;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using System.Windows;
using System.Windows.Controls;
namespace LifeBoardGameWPF.Views
{
    public class ChooseGenderView : UserControl, IUIView, IHandle<NewTurnEventModel>, IHandleAsync<LoadEventModel>
    {
        private readonly IEventAggregator _aggregator;
        private readonly LifeBoardGameGameContainer _gameContainer;
        private readonly CarPieceWPF _car;
        public ChooseGenderView(IEventAggregator aggregator, LifeBoardGameVMData model, LifeBoardGameGameContainer gameContainer)
        {
            _aggregator = aggregator;
            _gameContainer = gameContainer;
            _aggregator.Subscribe(this);
            StackPanel stack = new StackPanel();
            var picker = new EnumPickerWPF<CirclePieceCP<EnumGender>, CirclePieceWPF<EnumGender>, EnumGender>();
            stack.Children.Add(picker);
            picker.LoadLists(model.GenderChooser);
            _car = new CarPieceWPF();
            _car.Height = 186;
            _car.Width = 102;
            _car.Margin = new Thickness(0, 20, 0, 0);
            _car.MainColor = _gameContainer.SingleInfo!.Color.ToColor(); //because too late the first time it runs.
            _car.Init();
            stack.Children.Add(_car);
            SimpleLabelGrid simple = new SimpleLabelGrid();
            simple.AddRow("Turn", nameof(ChooseGenderViewModel.Turn));
            simple.AddRow("Instructions", nameof(ChooseGenderViewModel.Instructions));
            stack.Children.Add(simple.GetContent);
            Content = stack;
        }

        public Task HandleAsync(LoadEventModel message)
        {
            return Task.CompletedTask;
        }

        void IHandle<NewTurnEventModel>.Handle(NewTurnEventModel message)
        {
            _car.MainColor = _gameContainer.SingleInfo!.Color.ToColor();
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
