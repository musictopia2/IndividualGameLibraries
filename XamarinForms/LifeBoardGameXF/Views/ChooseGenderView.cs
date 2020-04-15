using BasicGameFrameworkLibrary.BasicEventModels;
using BasicGameFrameworkLibrary.GameGraphicsCP.GamePieces;
using BasicGamingUIXFLibrary.BasicControls.ChoicePickers;
using BasicGamingUIXFLibrary.BasicControls.SimpleControls;
using BasicGamingUIXFLibrary.GameGraphics.GamePieces;
using BasicXFControlsAndPages.MVVMFramework.Controls;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using CommonBasicStandardLibraries.Messenging;
using LifeBoardGameCP.Data;
using LifeBoardGameCP.ViewModels;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using Xamarin.Forms;

namespace LifeBoardGameXF.Views
{
    public class ChooseGenderView : CustomControlBase, IHandle<NewTurnEventModel>, IHandleAsync<LoadEventModel>
    {
        private readonly IEventAggregator _aggregator;
        private readonly LifeBoardGameGameContainer _gameContainer;
        private readonly CarPieceXF _car;
        public ChooseGenderView(IEventAggregator aggregator, LifeBoardGameVMData model, LifeBoardGameGameContainer gameContainer)
        {
            _aggregator = aggregator;
            _gameContainer = gameContainer;
            _aggregator.Subscribe(this);
            StackLayout stack = new StackLayout();
            var picker = new EnumPickerXF<CirclePieceCP<EnumGender>, CirclePieceXF<EnumGender>, EnumGender>();
            stack.Children.Add(picker);
            picker.LoadLists(model.GenderChooser);
            _car = new CarPieceXF();
            _car.HeightRequest = 93;
            _car.WidthRequest = 51;
            _car.Margin = new Thickness(0, 20, 0, 0);
            _car.MainColor = _gameContainer.SingleInfo!.Color.ToColor(); //because too late the first time it runs.
            _car.Init();
            stack.Children.Add(_car);
            SimpleLabelGridXF simple = new SimpleLabelGridXF();
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
        protected override Task TryCloseAsync()
        {
            _aggregator.Unsubscribe(this);
            return base.TryCloseAsync();
        }
    }
}