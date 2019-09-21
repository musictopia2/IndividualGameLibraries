using BasicGameFramework.Attributes;
using BasicGameFramework.MiscProcesses;
using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.Exceptions;
using SkiaSharp;
using SkiaSharpGeneralLibrary.SKExtensions;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using cs = CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.SColorString;
namespace BattleshipCP
{
    [SingletonGame] //decided to risk making this singleton.
    public class GameBoardCP
    {
        public Dictionary<int, string>? RowList;
        public Dictionary<int, string>? ColumnList; //has to be string if possible
        public BattleshipCollection? HumanList;
        public BattleshipCollection? ComputerList;
        public float SpaceSize { get; set; }
        private bool _isWaiting;
        private Assembly? _thisAssembly;
        public void StartGame()
        {
            _isWaiting = false;
            foreach (var item in HumanList!)
            {
                item.FillColor = cs.Blue; //i think this is all that it needs to when it starts game.
            }
            _thisAssembly = Assembly.GetAssembly(this.GetType());

        } // don't think i need to refresh
        public void HumanWaiting()
        {
            if (_isWaiting == true)
                return;
            HumanList!.PlayerWaiting();
            _isWaiting = true;
        }
        public void ClearBoard()
        {
            HumanList!.Clear();
            ComputerList!.Clear(); //i think both this time.
        }
        public GameBoardCP(BattleshipMainGameClass mainGame)
        {
            _mainGame = mainGame;
            PrivateInit(); //if i am wrong, rethink.
        }
        private SKPaint? _penPaint;
        private SKPaint? BitPaint;
        private readonly BattleshipMainGameClass _mainGame;
        private void PrivateInit() //this is it i think.
        {
            CustomBasicList<string> TempList = new CustomBasicList<string>
            {
                "A",
                "B",
                "C",
                "D",
                "E",
                "F",
                "G",
                "H",
                "I",
                "J"
            };
            RowList = new Dictionary<int, string>();
            var loopTo = TempList.Count;
            int x;
            for (x = 1; x <= loopTo; x++)
                RowList.Add(RowList.Count + 1, TempList[x - 1]);// because 0 based
            ColumnList = new Dictionary<int, string>();
            for (x = 1; x <= 10; x++)
                ColumnList.Add(ColumnList.Count + 1, x.ToString());
            int y = 0;
            HumanList = new BattleshipCollection();
            ComputerList = new BattleshipCollection();
            x = 0; //i think.
            foreach (var thisRow in RowList.Values)
            {
                x += 1;
                foreach (var thisColumn in ColumnList.Values)
                {
                    y += 1;
                    HumanList[x, int.Parse(thisColumn)].Letter = thisRow.ToLower();
                    ComputerList[x, int.Parse(thisColumn)].Letter = thisRow.ToLower();
                }
            }
            _penPaint = MiscHelpers.GetStrokePaint(SKColors.Black, 2); // try 2 instead of 1
            BitPaint = MiscHelpers.GetBitmapPaint();
        }
        public bool CanChooseSpace(Vector space)
        {
            return HumanList![space].Hit == EnumWhatHit.None;
        }
        public void MarkField(Vector space, EnumWhatHit hit)
        {
            if (_mainGame.StatusOfGame != EnumStatusList.InGame)
                throw new BasicBlankException("Can only mark a field if its in the game");
            FieldInfoCP thisField = HumanList![space];
            thisField.Hit = hit;
            if (hit == EnumWhatHit.Miss)
                thisField.FillColor = cs.Lime;
        }
        public void PlaceShip(Vector space, int nextShip, out string label)
        {
            FieldInfoCP thisField;
            thisField = HumanList![space];
            thisField.FillColor = cs.Gray;
            thisField.ShipNumber = nextShip;
            label = $"{thisField.Letter.ToUpper()}:{space.Column}";
        }
        public bool HumanWon()
        {
            return HumanList.Count(items => items.Hit == EnumWhatHit.Hit) == 16;
        }
        public void DrawSpace(SKCanvas thisCanvas, FieldInfoCP thisSpace, float width, float height) // i think its fieldinfo (well see)
        {
            var thisRect = SKRect.Create(0, 0, width, height);
            var thisPaint = MiscHelpers.GetSolidPaint(thisSpace.FillColor.ToSKColor());
            thisCanvas.DrawRect(thisRect, thisPaint);
            thisCanvas.DrawRect(thisRect, _penPaint);
            if ((int)thisSpace.Hit == (int)EnumWhatHit.Hit)
            {
                var thisBit = ImageExtensions.GetSkBitmap(_thisAssembly, "battleshipfire.png");
                thisCanvas.DrawBitmap(thisBit, thisRect, BitPaint);
            }
        }
    }
}