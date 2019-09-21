using BaseGPXWindowsAndControlsCore.GameGraphics.Base;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.Messenging;
using SnakesAndLaddersCP;
using System.Windows;
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;
using cs = CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.SColorString;
namespace SnakesAndLaddersWPF
{
    public class GamePieceWPF : BaseGraphicsWPF<PieceGraphicsCP>
    {
        public bool NeedsSubscribe { get; set; } = true;
        private static EventAggregator? _thisE;
        public GamePieceWPF()
        {
            _thisE = Resolve<EventAggregator>();
        }
        public static readonly DependencyProperty NumberProperty = DependencyProperty.Register("Number", typeof(int), typeof(GamePieceWPF), new FrameworkPropertyMetadata(0, new PropertyChangedCallback(NumberPropertyChanged)));
        public int Number
        {
            get
            {
                return (int)GetValue(NumberProperty);
            }
            set
            {
                SetValue(NumberProperty, value);
            }
        }
        private static void NumberPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var thisItem = (GamePieceWPF)sender;
            thisItem.Mains.Number = (int)e.NewValue;
            if (thisItem.NeedsSubscribe == true)
                _thisE!.Publish(thisItem);
        }
        public static readonly DependencyProperty IndexProperty = DependencyProperty.Register("Index", typeof(int), typeof(GamePieceWPF), new FrameworkPropertyMetadata(0, new PropertyChangedCallback(IndexPropertyChanged)));
        public int Index
        {
            get
            {
                return (int)GetValue(IndexProperty);
            }
            set
            {
                SetValue(IndexProperty, value);
            }
        }
        private static void IndexPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var thisItem = (GamePieceWPF)sender;
            switch (thisItem.Index)
            {
                case 1:
                    {
                        thisItem.MainColor = cs.Blue;
                        break;
                    }

                case 2:
                    {
                        thisItem.MainColor = cs.DeepPink;
                        break;
                    }

                case 3:
                    {
                        thisItem.MainColor = cs.Orange;
                        break;
                    }

                case 4:
                    {
                        thisItem.MainColor = cs.ForestGreen;
                        break;
                    }

                default:
                    {
                        throw new BasicBlankException("Only 4 players supported");
                    }
            }
        }
    }
}