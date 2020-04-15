using BasicGameFrameworkLibrary.BasicEventModels;
using BasicGameFrameworkLibrary.ColorCards;
using BasicGameFrameworkLibrary.GameGraphicsCP.GamePieces;
using BasicGamingUIXFLibrary.BasicControls.ChoicePickers;
using BasicGamingUIXFLibrary.BasicControls.GameFrames;
using BasicGamingUIXFLibrary.GameGraphics.GamePieces;
using BasicXFControlsAndPages.MVVMFramework.Controls;
using CommonBasicStandardLibraries.Messenging;
using System.Threading.Tasks;
using UnoCP.Cards;
using UnoCP.Data;
using Xamarin.Forms;
using ts = BasicGameFrameworkLibrary.GameGraphicsCP.Cards.BaseColorCardsCP;

namespace UnoXF.Views
{
    public class ChooseColorView : CustomControlBase, IHandleAsync<LoadEventModel>
    {
        private readonly IEventAggregator _aggregator;

        public ChooseColorView(IEventAggregator aggregator, UnoVMData model)
        {
            _aggregator = aggregator;
            _aggregator.Subscribe(this);
            StackLayout stack = new StackLayout();
            EnumPickerXF<CheckerChoiceCP<EnumColorTypes>,
            CheckerChooserXF<EnumColorTypes>, EnumColorTypes> picker = new EnumPickerXF<CheckerChoiceCP<EnumColorTypes>, CheckerChooserXF<EnumColorTypes>, EnumColorTypes>();
            stack.Children.Add(picker);
            BaseHandXF<UnoCardInformation, UnoGraphicsCP, CardGraphicsXF> hand = new BaseHandXF<UnoCardInformation, UnoGraphicsCP, CardGraphicsXF>();
            stack.Children.Add(hand);
            hand.Margin = new Thickness(5);
            hand.LoadList(model.PlayerHand1, ts.TagUsed);
            picker.LoadLists(model.ColorPicker);
            Content = stack;
        }


        Task IHandleAsync<LoadEventModel>.HandleAsync(LoadEventModel message)
        {
            return Task.CompletedTask;
        }
        protected override Task TryCloseAsync()
        {
            _aggregator.Unsubscribe(this);
            return base.TryCloseAsync();
        }
    }
}
