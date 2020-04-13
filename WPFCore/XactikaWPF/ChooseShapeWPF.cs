using BasicGamingUIWPFLibrary.GameGraphics.Base;
using CommonBasicStandardLibraries.CollectionClasses;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using XactikaCP.Data;
using XactikaCP.Logic;
using XactikaCP.MiscImages;
using static BasicGamingUIWPFLibrary.Helpers.SharedUIFunctions; //this usually will be used too.

namespace XactikaWPF
{
    public class ChooseShapeWPF : UserControl
    {
        private CustomBasicCollection<PieceCP>? _shapeList;
        private ChooseShapeObservable? _tempMod;
        private void PieceBindings(XactikaPieceWPF thisGraphics, PieceCP thisPiece)
        {
            thisGraphics.Width = 90;
            thisGraphics.Height = 207; // use proportions (decided the aspect ratio as 1.5)
            thisGraphics.Margin = new Thickness(5, 0, 5, 5); // i do like the idea of margins this time as well.
            thisGraphics.Visibility = Visibility.Visible; // has to manually be set
            thisGraphics.DataContext = thisPiece;
            var thisBind = GetCommandBinding(nameof(ChooseShapeObservable.ShapeSelectedCommand));
            thisGraphics.SetBinding(GraphicsCommand.CommandProperty, thisBind);
            thisGraphics.CommandParameter = thisPiece; // i think
            thisGraphics.SetBinding(XactikaPieceWPF.ShapeUsedProperty, nameof(PieceCP.ShapeUsed));
            thisGraphics.SetBinding(XactikaPieceWPF.HowManyProperty, nameof(PieceCP.HowMany));
            thisGraphics.SetBinding(XactikaPieceWPF.IsSelectedProperty, nameof(PieceCP.IsSelected));
            thisGraphics.SetBinding(IsEnabledProperty, nameof(PieceCP.IsEnabled));
            thisGraphics.SendPiece(thisPiece); // i think
        }
        private Binding GetCommandBinding(string path)
        {
            Binding ThisBind = new Binding(path);
            ThisBind.Source = _tempMod; // i think
            return ThisBind;
        }
        private StackPanel? _thisStack;
        public void Init(XactikaVMData thisMod)
        {
            _tempMod = thisMod.ShapeChoose1;
            _shapeList = _tempMod!.PieceList;
            Margin = new Thickness(10, 10, 10, 10);
            DataContext = _tempMod; // i think
            var thisBind = GetVisibleBinding(nameof(ChooseShapeObservable.Visible));
            SetBinding(VisibilityProperty, thisBind); // i think
            _thisStack = new StackPanel();
            _thisStack.Orientation = Orientation.Horizontal; // for this time, must be horizontal.
            _shapeList.CollectionChanged += ShapeList_CollectionChanged;
            Content = _thisStack;
            PopulateList();
        }
        private void PopulateList()
        {
            _thisStack!.Children.Clear();
            foreach (var thisShape in _shapeList!)
            {
                XactikaPieceWPF thisGraphics = new XactikaPieceWPF();
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
