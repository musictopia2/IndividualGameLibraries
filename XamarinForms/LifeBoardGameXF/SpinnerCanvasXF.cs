using CommonBasicStandardLibraries.Messenging;
using LifeBoardGameCP.EventModels;
using SkiaSharp.Views.Forms;
namespace LifeBoardGameXF
{
    public class SpinnerCanvasXF : SKCanvasView, IHandle<PaintSpinEventModel>
    {
        readonly IEventAggregator _aggregator;
        public SpinnerCanvasXF(IEventAggregator aggregator)
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
            InvalidateSurface();
        }
    }
}