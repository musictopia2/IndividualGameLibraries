using BasicGamingUIXFLibrary.BasicControls.GameBoards;
using BowlingDiceGameCP.Data;
using BowlingDiceGameCP.Logic;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using CommonBasicStandardLibraries.Exceptions;
using System.Linq;
using Xamarin.Forms;
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;
namespace BowlingDiceGameXF
{
    public class BowlingCompleteScoresheetXF : DisplayGameBoardXF
    {
        protected override int HowManyRows => _thisGame!.PlayerList.Count();
        protected override int HowManyColumns => 10;
        private bool _canLoad;
        private BowlingDiceGameMainGameClass? _thisGame;
        public void LoadPlayerScores()
        {
            _thisGame = Resolve<BowlingDiceGameMainGameClass>();// this is okay because this can't be unit tested.
            _canLoad = true;
            CanHeadersBold = false;
            HeaderFontSize = 12;
            LeftRightMargins = 10;
            LoadGridColumnRows(); // needs to do this first
            var rowList = _thisGame.PlayerList.Select(Items => Items.NickName).ToCustomBasicList();
            CreateHeadersColumnsRows(rowList, _thisGame.ScoreSheets!.TextFrames, "Player:");
            int x = 0;
            int y = 0;
            int z = 0;
            foreach (var thisPlayer in _thisGame.PlayerList!)
            {
                if (thisPlayer.FrameList.Count != 10)
                    throw new BasicBlankException("Each player must have 10 frames");
                y = 0;
                foreach (var thisFrame in thisPlayer.FrameList.Values)
                {
                    var thisControl = GetControl(thisFrame);
                    AddControlToGrid(thisControl, x + 1, y + 1); // try this way.
                    z += 1;
                    y += 1;
                }
                x += 1;
            }
        }
        public void SavedScores() // rebindings.
        {
            int x = 0;
            int y;
            foreach (var thisPlayer in _thisGame!.PlayerList!)
            {
                if (thisPlayer.FrameList.Count != 10)
                    throw new BasicBlankException("Each player must have 10 frames");
                x += 1;
                var tempControl = FindControl(x, 0); // has to be 0 always for this one
                var thisLabel = (Label)tempControl;
                thisLabel.Text = thisPlayer.NickName;
                for (y = 1; y <= 10; y++)
                {
                    var thisControl = FindControl(x, y);
                    var thisGraphics = (CompleteFrameXF)thisControl;
                    thisGraphics.SavedFrame(thisPlayer.FrameList[y]);
                }
            }
        }
        protected override void LoadGridColumnRows()
        {
            if (_canLoad == false)
                return;
            base.LoadGridColumnRows();
        }
        private CompleteFrameXF GetControl(FrameInfoCP thisItem)
        {
            CompleteFrameXF thisControl = new CompleteFrameXF();
            thisControl.LoadFrame(thisItem); // hopefully this is all there is (well see)
            return thisControl;
        }
    }
}