using CommonBasicStandardLibraries.Messenging;
using LifeBoardGameCP.EventModels;
using SkiaSharp.Views.WPF;
namespace LifeBoardGameWPF
{
    public class SpinnerCanvasWPF : SKElement, IHandle<PaintSpinEventModel>
    {
        readonly IEventAggregator _aggregator;
        public SpinnerCanvasWPF(IEventAggregator aggregator)
        {
            _aggregator = aggregator;
            _aggregator.Subscribe(this);
        }
        //public int Position { get; set; }
        //public int HighSpeedPhase { get; set; }
        public void BeforeClosing()
        {
            _aggregator.Unsubscribe(this);
        }

        void IHandle<PaintSpinEventModel>.Handle(PaintSpinEventModel message)
        {
            InvalidateVisual();
        }
    }
}