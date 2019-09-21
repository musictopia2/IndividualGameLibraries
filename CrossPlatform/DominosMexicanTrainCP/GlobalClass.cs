using BasicGameFramework.Attributes;
using BasicGameFramework.Extensions;
using BasicGameFramework.GameGraphicsCP.Animations;
using BasicGameFramework.GameGraphicsCP.BaseGraphics;
using BasicGameFramework.GameGraphicsCP.Interfaces;
using CommonBasicStandardLibraries.Exceptions;
using SkiaSharp;
using ts = BasicGameFramework.GameGraphicsCP.Dominos.DominosCP;
namespace DominosMexicanTrainCP
{
    [SingletonGame]
    public class GlobalClass
    {
        public AnimateSkiaSharpGameBoard? Animates;
        internal ts? MovingDomino;
        private DominosMexicanTrainMainGameClass? _maingame;
        private IProportionImage? _thisP;
        public void Init(DominosMexicanTrainMainGameClass mainGame)
        {
            _maingame = mainGame;
            _thisP = _maingame.MainContainer.Resolve<IProportionImage>(ts.TagUsed);
        }
        internal ts GetDominoPiece(MexicanDomino thisDomino)
        {
            if (_thisP == null)
                throw new BasicBlankException("Forgot to init global class.  Rethink");
            ts newDomino = new ts();
            newDomino.MainGraphics = new BaseDeckGraphicsCP(); //i think has to be first so it can use to set paints.
            newDomino.MainGraphics.ThisGraphics = newDomino; //this is needed too;
            newDomino.Init();
            newDomino.CurrentFirst = thisDomino.CurrentFirst;
            newDomino.CurrentSecond = thisDomino.CurrentSecond;
            newDomino.HighestDomino = thisDomino.HighestDomino;
            newDomino.MainGraphics.NeedsToClear = false; //i think
            newDomino.MainGraphics.Angle = thisDomino.Angle;
            SKSize thisSize = thisDomino.DefaultSize.GetSizeUsed(_thisP.Proportion);
            newDomino.MainGraphics.ActualHeight = thisSize.Height;
            newDomino.MainGraphics.ActualWidth = thisSize.Width;
            newDomino.MainGraphics.Location = thisDomino.Location;
            return newDomino;
        }
    }
}
