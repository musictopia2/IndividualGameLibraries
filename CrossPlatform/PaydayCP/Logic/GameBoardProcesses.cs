using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.BasicEventModels;
using BasicGameFrameworkLibrary.GameGraphicsCP.GameboardPositionHelpers;
using BasicGameFrameworkLibrary.GameGraphicsCP.GamePieces;
using BasicGameFrameworkLibrary.MultiplayerClasses.Extensions;
using BasicGameFrameworkLibrary.NetworkingClasses.Extensions;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.Exceptions;
using PaydayCP.Data;
using PaydayCP.Graphics;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace PaydayCP.Logic
{
    [SingletonGame]
    [AutoReset]
    public class GameBoardProcesses
    {
        public EnumDay GetDayDetails(int day)
        {
            if (day > 30)
                throw new BasicBlankException("Only goes upto 30");
            return _dayList[day - 1];
        }
        public int NextBuyerSpace()
        {
            return FindNextSpace(EnumDay.Buyer);
        }
        private int FindNextSpace(EnumDay whichOne)
        {
            var currentDay = _gameContainer.SingleInfo!.DayNumber;
            var tempList = _dayList.Skip(currentDay);
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
        readonly CustomBasicList<EnumDay> _dayList = new CustomBasicList<EnumDay>();
        readonly PaydayGameContainer _gameContainer;
        private readonly GameBoardGraphicsCP _graphicsBoard; //hopefully this simple.
        public GameBoardProcesses(PaydayGameContainer gameContainer, GameBoardGraphicsCP boardGraphicsCP)
        {
            _gameContainer = gameContainer;
            _graphicsBoard = boardGraphicsCP;
            _dayList = new CustomBasicList<EnumDay>
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
            _gameContainer.SingleInfo!.InGame = false;
            PositionPieces();
        }
        private void PositionPieces()
        {
            _gameContainer.PrivateSpaceList.ForEach(thisSpace =>
            {
                _gameContainer.Pos!.ClearArea(thisSpace);
            });
            PawnPiecesCP<EnumColorChoice> thisPiece;
            GameSpace tempSpace;
            foreach (var thisPlayer in _gameContainer.PlayerList!)
            {
                if (thisPlayer.InGame == true)
                    tempSpace = _gameContainer.PrivateSpaceList[thisPlayer.DayNumber];
                else
                    tempSpace = _gameContainer.PrivateSpaceList[32];// very last
                thisPiece = new PawnPiecesCP<EnumColorChoice>();
                thisPiece.MainColor = thisPlayer.Color.ToColor();
                thisPiece.NeedsToClear = false;
                if (thisPlayer.InGame == true)
                    thisPiece.ActualWidth = tempSpace.Area.Width / 3.2; // try this way
                else
                    thisPiece.ActualWidth = (double)tempSpace.Area.Width / (float)7;
                thisPiece.ActualHeight = thisPiece.ActualWidth; // i think
                tempSpace.PieceList.Add(thisPiece);
                var thisPos = _gameContainer.Pos!.GetPosition(tempSpace, (float)thisPiece.ActualWidth, (float)thisPiece.ActualHeight);
                thisPiece.Location = thisPos;
                _gameContainer.Pos.AddPieceToArea(tempSpace, thisPiece);
            }
            _gameContainer.Aggregator.RepaintBoard();
        }
        public async Task ResetBoardAsync()
        {
            int x = 0;
            if (_gameContainer.Pos == null == true || _gameContainer.PrivateSpaceList.Count == 0)
            {
                _gameContainer.Aggregator.RepaintBoard(); // has to repaint so it can do what it needs to.
                do
                {
                    await Task.Delay(10); //do need to do it this time.
                    if (_graphicsBoard!.DidPaint() == true)
                        break;
                    x++;
                    if (x > 5000)
                        throw new BasicBlankException("Did not repaint.  Rethink");
                }
                while (true);
                LoadBoard();
            }
            foreach (var thisPlayer in _gameContainer.PlayerList!)
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
            _gameContainer.Pos = new PositionPieces();
            int x;
            for (x = 0; x <= 32; x++)
            {
                GameSpace thisSpace = new GameSpace();
                if (x >= 1 && x <= 31)
                    thisSpace.Area = _graphicsBoard!.SpaceRectangle(x);
                else if (x == 0)
                    thisSpace.Area = _graphicsBoard!.StartingRectangle;
                else if (x == 32)
                    thisSpace.Area = _graphicsBoard!.FinishRectangle;
                else
                    throw new BasicBlankException("No rectangle found");
                if (thisSpace.Area.Width == 0 || thisSpace.Area.Height == 0)
                    throw new BasicBlankException("No rectangle.  This means needs to rethink drawing or when creating spaces.");
                _gameContainer.PrivateSpaceList.Add(thisSpace);
            }
        }
        internal async Task ReloadSavedStateAsync()
        {
            _gameContainer.Aggregator.RepaintBoard();
            int x = 0;
            do
            {
                await Task.Delay(10);
                if (_graphicsBoard!.DidPaint() == true)
                    break;
                x++;
                if (x > 5000)
                    throw new BasicBlankException("Did not repaint.  Rethink");
            }
            while (true);

            if (_gameContainer.Pos == null == true || _gameContainer.PrivateSpaceList.Count == 0)
                LoadBoard();
            PositionPieces();
        }
        public void NewTurn()
        {
            _gameContainer.Aggregator.Publish(new NewTurnEventModel()); //i think
            _gameContainer.Aggregator.RepaintBoard();
            if (_gameContainer.Pos == null == true || _gameContainer.PrivateSpaceList.Count == 0)
                LoadBoard();
        }
        public void ClearJackPot()
        {
            _gameContainer.SaveRoot!.LotteryAmount = 0;
            _gameContainer.Aggregator.RepaintBoard();
        }
        public void AddToJackPot(decimal Amount)
        {
            _gameContainer.SaveRoot!.LotteryAmount += Amount;
            _gameContainer.Aggregator.RepaintBoard();
        }
        public void HighlightDay(int day)
        {
            _gameContainer.SaveRoot!.NumberHighlighted = day;
            _gameContainer.Aggregator.RepaintBoard();
        }
        public void UnhighlightDay()
        {
            _gameContainer.SaveRoot!.NumberHighlighted = 0;
            _gameContainer.Aggregator.RepaintBoard();
        }
        public async Task AnimateMoveAsync(int newDay)
        {
            if (newDay > 31)
                throw new BasicBlankException("Must be upto 31.  If 32 is needed; then rethinking is required");
            if (_gameContainer.SingleInfo!.CanSendMessage(_gameContainer.BasicData!))
                await _gameContainer.Network!.SendMoveAsync(newDay);
            int x;
            x = _gameContainer.SingleInfo!.DayNumber;
            do
            {
                x += 1;
                if (x == 32 && newDay < 32)
                    x = 1;
                _gameContainer.SingleInfo.DayNumber = x;
                PositionPieces();
                if (_gameContainer.Test!.NoAnimations == false)
                    await _gameContainer.Delay.DelayMilli(100);
                if ((newDay >= 32 && x == 32) || x == newDay)
                    break;
            }
            while (true);
            UnhighlightDay();
            if (_gameContainer.ResultsOfMoveAsync == null)
            {
                throw new BasicBlankException("Nobody is handling the resultsofmoveasync.  Rethink");
            }
            await _gameContainer.ResultsOfMoveAsync.Invoke(newDay);
        }
    }
}