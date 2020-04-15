using BasicGameFrameworkLibrary.DIContainers;
using BasicGameFrameworkLibrary.Extensions;
using BasicGameFrameworkLibrary.GameGraphicsCP.Interfaces;
using BasicGamingUIXFLibrary.GameGraphics.Base;
using CommonBasicStandardLibraries.CollectionClasses;
using SkiaSharp;
using System.Collections.Specialized;
using XactikaCP;
using XactikaCP.Data;
using XactikaCP.Logic;
using XactikaCP.MiscImages;
using Xamarin.Forms;
namespace XactikaXF
{
    public class ChooseShapeXF : ContentView
    {
        private CustomBasicCollection<PieceCP>? _shapeList;
        private ChooseShapeObservable? _tempMod;
        private void PieceBindings(XactikaPieceXF thisGraphics, PieceCP thisPiece)
        {
            thisGraphics.WidthRequest = _sizeUsed.Width;
            thisGraphics.HeightRequest = _sizeUsed.Height;
            thisGraphics.Margin = new Thickness(5, 0, 5, 5); // i do like the idea of margins this time as well.
            thisGraphics.IsVisible = true; // has to manually be set
            thisGraphics.BindingContext = thisPiece;
            var thisBind = GetCommandBinding(nameof(ChooseShapeObservable.ShapeSelectedCommand));
            thisGraphics.SetBinding(GraphicsCommand.CommandProperty, thisBind);
            thisGraphics.CommandParameter = thisPiece; // i think
            thisGraphics.SetBinding(XactikaPieceXF.ShapeUsedProperty, nameof(PieceCP.ShapeUsed));
            thisGraphics.SetBinding(XactikaPieceXF.HowManyProperty, nameof(PieceCP.HowMany));
            thisGraphics.SetBinding(XactikaPieceXF.IsSelectedProperty, nameof(PieceCP.IsSelected));
            thisGraphics.SetBinding(IsEnabledProperty, nameof(PieceCP.IsEnabled));
            thisGraphics.SendPiece(thisPiece); // i think
        }
        private Binding GetCommandBinding(string path)
        {
            Binding ThisBind = new Binding(path);
            ThisBind.Source = _tempMod; // i think
            return ThisBind;
        }
        private StackLayout? _thisStack;
        private SKSize _sizeUsed;
        public void Init(XactikaVMData thisMod, IGamePackageResolver resolver)
        {
            SKSize firstSize = new SKSize(60, 138);
            IProportionImage thisP = resolver.Resolve<IProportionImage>("");
            _sizeUsed = firstSize.GetSizeUsed(thisP.Proportion);
            _tempMod = thisMod.ShapeChoose1;
            _shapeList = _tempMod!.PieceList;
            Margin = new Thickness(10, 10, 10, 10);
            BindingContext = _tempMod; // i think
            var thisBind = new Binding(nameof(ChooseShapeObservable.Visible));
            SetBinding(IsVisibleProperty, thisBind); // i think
            _thisStack = new StackLayout();
            _thisStack.Orientation = StackOrientation.Horizontal; // for this time, must be horizontal.
            _shapeList.CollectionChanged += ShapeList_CollectionChanged;
            Content = _thisStack;
            PopulateList();
        }
        private void PopulateList()
        {
            _thisStack!.Children.Clear();
            foreach (var thisShape in _shapeList!)
            {
                XactikaPieceXF thisGraphics = new XactikaPieceXF();
                PieceBindings(thisGraphics, thisShape);
                _thisStack.Children.Add(thisGraphics);
            }
        }
        private void ShapeList_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            PopulateList(); // i think this simple.
        }
    }
}