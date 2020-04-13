using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.DIContainers;
using BasicGameFrameworkLibrary.Dominos;
using BasicGameFrameworkLibrary.Extensions;
using BasicGameFrameworkLibrary.GameGraphicsCP.Animations;
using BasicGameFrameworkLibrary.GameGraphicsCP.BaseGraphics;
using BasicGameFrameworkLibrary.GameGraphicsCP.Interfaces;
using DominosMexicanTrainCP.Data;
using SkiaSharp;
using System;
using System.Threading.Tasks;
using ts = BasicGameFrameworkLibrary.GameGraphicsCP.Dominos.DominosCP;
namespace DominosMexicanTrainCP.Logic
{
    [SingletonGame]
    [AutoReset]
    public class GlobalClass
    {

        private readonly IProportionImage _prop;
        public AnimateSkiaSharpGameBoard? Animates;
        internal ts? MovingDomino { get; set; }
        internal DominosBoneYardClass<MexicanDomino>? BoneYard { get; set; }

        internal TrainStationBoardProcesses? TrainStation1 { get; set; }

        public GlobalClass(IGamePackageResolver resolver)
        {
            _prop = resolver.Resolve<IProportionImage>(ts.TagUsed);
        }

        public static Func<int, Task>? TrainClickedAsync { get; set; }
        public static Func<SKPoint, int>? GetTrainClicked { get; set; }
        internal ts GetDominoPiece(MexicanDomino thisDomino)
        {
            ts newDomino = new ts();
            newDomino.MainGraphics = new BaseDeckGraphicsCP(); //i think has to be first so it can use to set paints.
            newDomino.MainGraphics.ThisGraphics = newDomino; //this is needed too;
            newDomino.Init();
            newDomino.CurrentFirst = thisDomino.CurrentFirst;
            newDomino.CurrentSecond = thisDomino.CurrentSecond;
            newDomino.HighestDomino = thisDomino.HighestDomino;
            newDomino.MainGraphics.NeedsToClear = false; //i think
            newDomino.MainGraphics.Angle = thisDomino.Angle;
            SKSize thisSize = thisDomino.DefaultSize.GetSizeUsed(_prop.Proportion);
            newDomino.MainGraphics.ActualHeight = thisSize.Height;
            newDomino.MainGraphics.ActualWidth = thisSize.Width;
            newDomino.MainGraphics.Location = thisDomino.Location;
            return newDomino;
        }
    }
}
