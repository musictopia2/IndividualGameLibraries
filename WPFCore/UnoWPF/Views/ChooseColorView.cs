using BasicGameFrameworkLibrary.BasicEventModels;
using BasicGameFrameworkLibrary.ColorCards;
using BasicGameFrameworkLibrary.GameGraphicsCP.GamePieces;
using BasicGamingUIWPFLibrary.BasicControls.ChoicePickers;
using BasicGamingUIWPFLibrary.BasicControls.GameFrames;
using BasicGamingUIWPFLibrary.GameGraphics.GamePieces;
using CommonBasicStandardLibraries.Messenging;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using System.Threading.Tasks;
using System.Windows.Controls;
using UnoCP.Cards;
using UnoCP.Data;
using ts = BasicGameFrameworkLibrary.GameGraphicsCP.Cards.BaseColorCardsCP;
using System.Windows;
namespace UnoWPF.Views
{
    public class ChooseColorView : UserControl, IUIView, IHandleAsync<LoadEventModel>
    {
        private readonly IEventAggregator _aggregator;

        public ChooseColorView(IEventAggregator aggregator, UnoVMData model)
        {
            _aggregator = aggregator;
            _aggregator.Subscribe(this);
            StackPanel stack = new StackPanel();
            EnumPickerWPF<CheckerChoiceCP<EnumColorTypes>,
            CheckerChooserWPF<EnumColorTypes>, EnumColorTypes> picker = new EnumPickerWPF<CheckerChoiceCP<EnumColorTypes>, CheckerChooserWPF<EnumColorTypes>, EnumColorTypes>();

            stack.Children.Add(picker);
            picker.GraphicsHeight = 300;
            picker.GraphicsWidth = 300;
            BaseHandWPF<UnoCardInformation, UnoGraphicsCP, CardGraphicsWPF> hand = new BaseHandWPF<UnoCardInformation, UnoGraphicsCP, CardGraphicsWPF>();
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
