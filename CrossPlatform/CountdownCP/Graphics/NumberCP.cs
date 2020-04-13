using BasicGameFrameworkLibrary.GameGraphicsCP.GamePieces;
using CommonBasicStandardLibraries.Exceptions;
using CountdownCP.Data;
using SkiaSharp;
using SkiaSharpGeneralLibrary.SKExtensions;
namespace CountdownCP.Graphics
{
    public class NumberCP : NumberPieceCP
    {
        private readonly SKPaint _yellowPaint;
        public SimpleNumber? Number { get; set; }
        protected override bool CanDrawNumber()
        {
            if (Number!.Used == true)
                return false;
            return true;
        }

        protected override string GetValueToPrint()
        {
            return Number!.Value.ToString();
        }
        //private readonly CountdownMainGameClass MainGame;
        public NumberCP()
        {
            _yellowPaint = MiscHelpers.GetSolidPaint(SKColors.Yellow); // hopefully can still use (?)
        }

        protected override SKPaint GetFillPaint()
        {
            if (CountdownVMData.ShowHints == false)
                return base.GetFillPaint();
            if (CountdownVMData.CanChooseNumber == null)
            {
                throw new BasicBlankException("Nobody is handling canchoosecolor.  Rethink");
            }
            if (CountdownVMData.CanChooseNumber(Number!) == false)
            {
                return base.GetFillPaint();
            }
            return _yellowPaint;
        }

        protected override void SelectProcesses()
        {
            IsSelected = Number!.IsSelected;
        }
    }
}