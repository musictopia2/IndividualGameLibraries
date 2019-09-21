using BasicGameFramework.Attributes;
using BasicGameFramework.BasicEventModels;
using BasicGameFramework.CommonInterfaces;
using BasicGameFramework.GameGraphicsCP.GameboardPositionHelpers;
using BasicGameFramework.GameGraphicsCP.GamePieces;
using BasicGameFramework.MultiplayerClasses.Extensions;
using BasicGameFramework.NetworkingClasses.Extensions;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.Messenging;
using System;
using System.Linq;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace PaydayCP
{
    [SingletonGame]
    public class GameBoardProcesses
    {
        public EnumDay GetDayDetails(int day)
        {
            if (day > 30)
                throw new BasicBlankException("Only goes upto 30");
            return DayList[day - 1];
        }
        public int NextBuyerSpace()
        {
            return FindNextSpace(EnumDay.Buyer);
        }
        private int FindNextSpace(EnumDay whichOne)
        {
            var currentDay = _mainGame.SingleInfo!.DayNumber;
            var tempList = DayList.Skip(currentDay);
            int x = 0;
            foreach (var thisItem in tempList)
            {
                x += 1;
                if ((int)thisItem == (int)whichOne)
                    return x + currentDay;
            }
            throw new Exception("Can't find the next space on " + whichOne.ToString());
        }
        public int NextDealSpace()
        {
            return FindNextSpace(EnumDay.Deal);
        }
        readonly CustomBasicList<EnumDay> DayList = new CustomBasicList<EnumDay>();
        readonly PaydayMainGameClass _mainGame;
        readonly GlobalFunctions _thisGlobals;
        readonly EventAggregator _thisE;
        readonly IAsyncDelayer _delay;
        internal GameBoardGraphicsCP? GraphicsBoard; //hopefully this simple.
        public GameBoardProcesses(PaydayMainGameClass mainGame, GlobalFunctions thisGlobals, IAsyncDelayer delay, EventAggregator thisE)
        {
            _mainGame = mainGame;
            _thisGlobals = thisGlobals;
            _delay = delay;
            _thisE = thisE;
            DayList = new CustomBasicList<EnumDay>
            {
                EnumDay.Mail,
                EnumDay.SweepStakes,
                EnumDay.Mail,
                EnumDay.Deal,
                EnumDay.Mail,
                EnumDay.Lottery,
                EnumDay.SkiWeekEnd,
                EnumDay.RadioContest,
                EnumDay.Buyer,
                EnumDay.HappyBirthday,
                EnumDay.Mail,
                EnumDay.Deal,
                EnumDay.Lottery,
                EnumDay.CharityConcert,
                EnumDay.Deal,
                EnumDay.Mail,
                EnumDay.Buyer,
                EnumDay.Food,
                EnumDay.Mail,
                EnumDay.Lottery,
                EnumDay.YardSale,
                EnumDay.Mail,
                EnumDay.Buyer,
                EnumDay.Mail,
                EnumDay.Deal,
                EnumDay.Buyer,
                EnumDay.Lottery,
                EnumDay.ShoppingSpree,
                EnumDay.Buyer,
                EnumDay.WalkForCharity
            };
        }
        public void MoveToLastSpace()
        {
            _mainGame.SingleInfo!.InGame = false;
            PositionPieces();
        }
        private void PositionPieces()
        {
            _thisGlobals.PrivateSpaceList.ForEach(thisSpace =>
            {
                _thisGlobals.Pos!.ClearArea(thisSpace);
            });
            PawnPiecesCP<EnumColorChoice> thisPiece;
            GameSpace tempSpace;
            foreach (var thisPlayer in _mainGame.PlayerList!)
            {
                if (thisPlayer.InGame == true)
                    tempSpace = _thisGlobals.PrivateSpaceList[thisPlayer.DayNumber];
                else
                    tempSpace = _thisGlobals.PrivateSpaceList[32];// very last
                thisPiece = new PawnPiecesCP<EnumColorChoice>();
                thisPiece.MainColor = thisPlayer.Color.ToColor();
                thisPiece.NeedsToClear = false;
                if (thisPlayer.InGame == true)
                    thisPiece.ActualWidth = tempSpace.Area.Width / 3.2; // try this way
                else
                    thisPiece.ActualWidth = (double)tempSpace.Area.Width / (float)7;
                thisPiece.ActualHeight = thisPiece.ActualWidth; // i think
                tempSpace.PieceList.Add(thisPiece);
                var thisPos = _thisGlobals.Pos!.GetPosition(tempSpace, (float)thisPiece.ActualWidth, (float)thisPiece.ActualHeight);
                thisPiece.Location = thisPos;
                _thisGlobals.Pos.AddPieceToArea(tempSpace, thisPiece);
            }
            _thisE.RepaintBoard();
        }
        public async Task ResetBoardAsync()
        {
            int x = 0;
            if (_thisGlobals.Pos == null == true || _thisGlobals.PrivateSpaceList.Count == 0)
            {
                _thisE.RepaintBoard(); // has to repaint so it can do what it needs to.
                do
                {
                    await Task.Delay(10); //do need to do it this time.
                    if (GraphicsBoard!.DidPaint() == true)
                        break;
                    x++;
                    if (x > 5000)
                        throw new BasicBlankException("Did not repaint.  Rethink");
                }
                while (true);
                LoadBoard();
            }
            foreach (var thisPlayer in _mainGame.PlayerList!)
            {
                thisPlayer.MoneyHas = 3500;
                thisPlayer.CurrentMonth = 1; // start out with month 1
                thisPlayer.DayNumber = 0; // start with day 0 because has not started yet
                thisPlayer.Loans = 0;
                thisPlayer.Hand.Clear(); // clear out the cards
                thisPlayer.ChoseNumber = 0;
            }
            PositionPieces();
        }
        private void LoadBoard()
        {
            _thisGlobals.Pos = new PositionPieces();
            int x;
            for (x = 0; x <= 32; x++)
            {
                GameSpace thisSpace = new GameSpace();
                if (x >= 1 && x <= 31)
                    thisSpace.Area = GraphicsBoard!.SpaceRectangle(x);
                else if (x == 0)
                    thisSpace.Area = GraphicsBoard!.StartingRectangle;
                else if (x == 32)
                    thisSpace.Area = GraphicsBoard!.FinishRectangle;
                else
                    throw new BasicBlankException("No rectangle found");
                if (thisSpace.Area.Width == 0 || thisSpace.Area.Height == 0)
                    throw new BasicBlankException("No rectangle.  This means needs to rethink drawing or when creating spaces.");
                _thisGlobals.PrivateSpaceList.Add(thisSpace);
            }
        }
        internal async Task ReloadSavedStateAsync()
        {
            _thisE.RepaintBoard();
            int x = 0;
            do
            {
                await Task.Delay(10);
                if (GraphicsBoard!.DidPaint() == true)
                    break;
                x++;
                if (x > 5000)
                    throw new BasicBlankException("Did not repaint.  Rethink");
            }
            while (true);

            if (_thisGlobals.Pos == null == true || _thisGlobals.PrivateSpaceList.Count == 0)
                LoadBoard();
            PositionPieces();
        }
        public void NewTurn()
        {
            _thisE.Publish(new NewTurnEventModel()); //i think
            _thisE.RepaintBoard();
            if (_thisGlobals.Pos == null == true || _thisGlobals.PrivateSpaceList.Count == 0)
                LoadBoard();
        }
        public void ClearJackPot()
        {
            _mainGame.SaveRoot!.LotteryAmount = 0;
            _thisE.RepaintBoard();
        }
        public void AddToJackPot(decimal Amount)
        {
            _mainGame.SaveRoot!.LotteryAmount += Amount;
            _thisE.RepaintBoard();
        }
        public void HighlightDay(int day)
        {
            _mainGame.SaveRoot!.NumberHighlighted = day;
            _thisE.RepaintBoard();
        }
        public void UnhighlightDay()
        {
            _mainGame.SaveRoot!.NumberHighlighted = 0;
            _thisE.RepaintBoard();
        }
        public async Task AnimateMoveAsync(int newDay)
        {
            if (newDay > 31)
                throw new BasicBlankException("Must be upto 31.  If 32 is needed; then rethinking is required");
            if (_mainGame.SingleInfo!.CanSendMessage(_mainGame.ThisData!))
                await _mainGame.ThisNet!.SendMoveAsync(newDay);
            int x;
            x = _mainGame.SingleInfo!.DayNumber;
            do
            {
                x += 1;
                if (x == 32 && newDay < 32)
                    x = 1;
                _mainGame.SingleInfo.DayNumber = x;
                PositionPieces();
                if (_mainGame.ThisTest!.NoAnimations == false)
                    await _delay.DelayMilli(100);
                if ((newDay >= 32 && x == 32) || x == newDay)
                    break;
            }
            while (true);
            UnhighlightDay();
            await _mainGame.ResultsOfMoveAsync(newDay);
        }
    }
}