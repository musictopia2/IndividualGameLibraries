using BasicGameFramework.GameGraphicsCP.GamePieces;
using SkiaSharp;
using SkiaSharpGeneralLibrary.SKExtensions;
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;
namespace CountdownCP
{
    public class NumberCP : NumberPieceCP
    {
        private readonly SKPaint YellowPaint;
        public SimpleNumber? ThisNumber { get; set; }
        protected override bool CanDrawNumber()
        {
            if (ThisNumber!.Used == true)
                return false;
            return true;
        }

        protected override string GetValueToPrint()
        {
            return ThisNumber!.Value.ToString();
        }
        private readonly CountdownMainGameClass MainGame;
        public NumberCP()
        {
            YellowPaint = MiscHelpers.GetSolidPaint(SKColors.Yellow); // hopefully can still use (?)
            MainGame = Resolve<CountdownMainGameClass>();
        }

        protected override SKPaint GetFillPaint()
        {
            if (CountdownViewModel.ShowHints == false)
                return base.GetFillPaint();
            if (MainGame.CanChooseNumber(ThisNumber!) == false)
                return base.GetFillPaint();
            return YellowPaint;
        }

        protected override void SelectProcesses()
        {
            IsSelected = ThisNumber!.IsSelected;
        }
    }
}
