using BaseGPXPagesAndControlsXF.GameGraphics.Base;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.Messenging;
using SnakesAndLaddersCP;
using Xamarin.Forms;
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;
using cs = CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.SColorString;
namespace SnakesAndLaddersXF
{
    public class GamePieceXF : BaseGraphicsXF<PieceGraphicsCP>
    {
        public bool NeedsSubscribe { get; set; } = true;
        private static EventAggregator? _thisE;
        public GamePieceXF()
        {
            _thisE = Resolve<EventAggregator>();
        }
        public static readonly BindableProperty NumberProperty = BindableProperty.Create(propertyName: "Number", returnType: typeof(int), declaringType: typeof(GamePieceXF), defaultValue: 0, defaultBindingMode: BindingMode.TwoWay, propertyChanged: NumberPropertyChanged);
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
        private static void NumberPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var thisItem = (GamePieceXF)bindable;
            thisItem.Mains.Number = (int)newValue;
            if (thisItem.NeedsSubscribe == true)
                _thisE!.Publish(thisItem);
        }
        public static readonly BindableProperty IndexProperty = BindableProperty.Create(propertyName: "Index", returnType: typeof(int), declaringType: typeof(GamePieceXF), defaultValue: 0, defaultBindingMode: BindingMode.TwoWay, propertyChanged: IndexPropertyChanged);
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
        private static void IndexPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var thisItem = (GamePieceXF)bindable;
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