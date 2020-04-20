using BasicGameFrameworkLibrary.BasicEventModels;
using BasicGameFrameworkLibrary.CommandClasses;
using BasicGameFrameworkLibrary.ViewModels;
using BasicGamingUIXFLibrary.BasicControls.ChoicePickers;
using BasicXFControlsAndPages.MVVMFramework.Controls;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.Messenging;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using LifeBoardGameCP.Data;
using System.Threading.Tasks;
using Xamarin.Forms;
using static BasicGamingUIXFLibrary.Helpers.SharedUIFunctions; //this usually will be used too.

namespace LifeBoardGameXF
{
    public abstract class BasicPlayerPicker : CustomControlBase
    {
        private readonly IEventAggregator _aggregator;
        private readonly LifeBoardGameGameContainer _gameContainer;
        private readonly Button _button;
        public BasicPlayerPicker(LifeBoardGameVMData model, IEventAggregator aggregator, LifeBoardGameGameContainer gameContainer)
        {
            //i think just the listview and submit
            //hopefully this one does not need end turn (?)
            //this is desktop anyways.
            ListChooserXF list = new ListChooserXF();
            list.ItemHeight = 50; // try this.
            StackLayout stack = new StackLayout();
            //{
            //    Orientation = StackOrientation.Horizontal
            //};
            stack.Children.Add(list);
            list.LoadLists(model.PlayerPicker);
            _button = GetGamingButton("Submit", nameof(BasicSubmitViewModel.SubmitAsync));
            _button.FontSize = 80; //could be iffy.
            stack.Children.Add(_button); //can always adjust as needed anyways.
            Content = stack;
            _aggregator = aggregator;
            _gameContainer = gameContainer;
        }
        protected override async Task TryActivateAsync()
        {
            await this.RefreshBindingsAsync(_aggregator);
            //if (_button.Command == null)
            //{
            //    UIPlatform.ShowError("There are no buttons now.  Rethink");
            //    return;
            //}
            //_button.IsEnabled = true; //try this way now.
            //await UIPlatform.ShowMessageAsync("Getting Button");
            //_gameContainer.SubmitPlayerCommand = (PlainCommand) _button.Command;
            //if (_gameContainer.SubmitPlayerCommand.CanExecute(null!) == false)
            //{
            //    var command = _gameContainer.Command;   
            //    await UIPlatform.ShowMessageAsync("Still disabled.  Really rethink");
            //}
            //else
            //{
            //    await UIPlatform.ShowMessageAsync("Help.  Should be enabled");
            //}
            //_gameContainer.SubmitPlayerCommand.ReportCanExecuteChange(); //try this way.
        }
    }
}