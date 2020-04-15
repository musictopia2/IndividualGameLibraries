using BasicGameFrameworkLibrary.BasicEventModels;
using BasicGameFrameworkLibrary.GameGraphicsCP.GamePieces;
using BasicGamingUIXFLibrary.BasicControls.ChoicePickers;
using BasicGamingUIXFLibrary.GameGraphics.GamePieces;
using BasicXFControlsAndPages.MVVMFramework.Controls;
using CommonBasicStandardLibraries.Messenging;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using MastermindCP.Data;
using MastermindCP.ViewModels;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using Xamarin.Forms;
using static BasicGamingUIXFLibrary.Helpers.SharedUIFunctions; //this usually will be used too.

namespace MastermindXF.Views
{
    public class MastermindMainView : ContentView, IUIView, IHandleAsync<LoadEventModel>
    {
        private readonly IEventAggregator _aggregator;
        private readonly EnumPickerXF<CirclePieceCP<EnumColorPossibilities>, CirclePieceXF<EnumColorPossibilities>, EnumColorPossibilities> _colors;

        //hopefully does not require too much rearranging (?)

        public MastermindMainView(IEventAggregator aggregator, LevelClass level)
        {
            _aggregator = aggregator;
            _aggregator.Subscribe(this);


            StackLayout stack = new StackLayout()
            {
                Orientation = StackOrientation.Horizontal
            };

            ParentSingleUIContainer parent = new ParentSingleUIContainer(nameof(MastermindMainViewModel.ActiveViewModel));
            stack.Children.Add(parent);
            Grid grid = new Grid();
            grid.BackgroundColor = Color.Brown;
            grid.WidthRequest = 200;
            EnumPickerXF<CirclePieceCP<EnumColorPossibilities>, CirclePieceXF<EnumColorPossibilities>, EnumColorPossibilities> colors = new EnumPickerXF<CirclePieceCP<EnumColorPossibilities>, CirclePieceXF<EnumColorPossibilities>, EnumColorPossibilities>();
            colors.Rows = 10; //try this way now.
            colors.HorizontalOptions = LayoutOptions.Center;
            _colors = colors;
            var acceptBut = GetGamingButton("Accept", nameof(MastermindMainViewModel.AcceptAsync));
            var giveUpBut = GetGamingButton("Give Up", nameof(MastermindMainViewModel.GiveUpAsync));
            var label = GetDefaultLabel();
            label.FontSize = 20;
            label.Text = $"Level Chosen:  {level.LevelChosen}";
            grid.Children.Add(colors);
            grid.Margin = new Thickness(10);
            stack.Children.Add(grid);
            StackLayout other = new StackLayout();
            other.Children.Add(acceptBut);
            other.Children.Add(giveUpBut);
            other.Children.Add(label);
            stack.Children.Add(other);
            Content = stack;

        }
        Task IHandleAsync<LoadEventModel>.HandleAsync(LoadEventModel message)
        {
            MastermindMainViewModel model = (MastermindMainViewModel)BindingContext;
            _colors.LoadLists(model.Color1);
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
