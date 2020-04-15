using BasicGamingUIXFLibrary.GameGraphics.Base;
using ThreeLetterFunCP.Data;
using ThreeLetterFunCP.GraphicsCP;
using Xamarin.Forms;
namespace ThreeLetterFunXF
{
    public class TileCardXF : BaseDeckGraphicsXF<TileInformation, TileCP>
    {
        public static readonly BindableProperty IsMovedProperty = BindableProperty.Create(propertyName: "IsMoved", returnType: typeof(bool), declaringType: typeof(TileCardXF), defaultValue: false, defaultBindingMode: BindingMode.TwoWay, propertyChanged: IsMovedPropertyChanged);
        public bool IsMoved
        {
            get
            {
                return (bool)GetValue(IsMovedProperty);
            }
            set
            {
                SetValue(IsMovedProperty, value);
            }
        }

        private static void IsMovedPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var thisItem = (TileCardXF)bindable;
            thisItem.MainObject!.IsMoved = (bool)newValue;
        }
        public static readonly BindableProperty LetterProperty = BindableProperty.Create(propertyName: "Letter", returnType: typeof(char), declaringType: typeof(TileCardXF), defaultValue: new char(), defaultBindingMode: BindingMode.TwoWay, propertyChanged: LetterPropertyChanged);
        public char Letter
        {
            get
            {
                return (char)GetValue(LetterProperty);
            }
            set
            {
                SetValue(LetterProperty, value);
            }
        }
        private static void LetterPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var thisItem = (TileCardXF)bindable;
            thisItem.MainObject!.Letter = (char)newValue;
        }
        protected override void DoCardBindings()
        {
            SetBinding(IsMovedProperty, new Binding(nameof(TileInformation.IsMoved)));
            SetBinding(LetterProperty, new Binding(nameof(TileInformation.Letter)));
        }
        protected override void PopulateInitObject() //this is needed too.
        {
            MainObject!.Init();
        }
    }
}