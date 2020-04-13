using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.GameGraphicsCP.BasicGameBoards;
using BasicGameFrameworkLibrary.GameGraphicsCP.GameboardPositionHelpers;
using BasicGameFrameworkLibrary.MultiplayerClasses.BasicPlayerClasses;
using BasicGameFrameworkLibrary.MultiplayerClasses.Extensions;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.FileFunctions;
using CommonBasicStandardLibraries.Exceptions;
using LifeBoardGameCP.Data;
using LifeBoardGameCP.Logic;
using Newtonsoft.Json;
using SkiaSharp;
using SkiaSharpGeneralLibrary.SKExtensions;
using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;
namespace LifeBoardGameCP.Graphics
{
    [SingletonGame]
    [AutoReset]
    public class GameBoardGraphicsCP : BaseGameBoardCP<CarPieceCP>
    {
        private const int _ourHeight = 1200;
        private const int _ourWidth = 900;
        public TempInfo? TempData;
        internal static int GoingTo { get; set; } //was going to put into container but it fits here more as static one.
        private SKRect GetClickableRectangle(PositionInfo thisPos, bool isEnding)
        {
            var firstSize = GetActualSize(10, 10);
            SKSize clickSize;
            if (isEnding == false)
                clickSize = GetActualSize(80, 80); // can always adjust as needed
            else
                clickSize = GetActualSize(140, 60);
            var tempPoint = GetActualPoint(thisPos.SpacePoint);
            var thisPoint = new SKPoint(tempPoint.X - firstSize.Width, tempPoint.Y - firstSize.Height);
            return SKRect.Create(thisPoint.X, thisPoint.Y, clickSize.Width, clickSize.Height);
        }
        private void CreatePositions()
        {
            var thisData = _thisA!.ResourcesAllTextFromFile("lifeboardgame.json");
            TempData = JsonConvert.DeserializeObject<TempInfo>(thisData);
        }
        private readonly LifeBoardGameGameContainer _gameContainer;
        private readonly ISpacePosition _spacePos;
        private readonly IBoardProcesses _options;

        public GameBoardGraphicsCP(LifeBoardGameGameContainer gameContainer,
            ISpacePosition space,
            IBoardProcesses options
            ) : base(gameContainer.Resolver)
        {
            _gameContainer = gameContainer;
            _spacePos = space; //iffy.  could cause overflow (?)
            _options = options;
        }

        public override string TagUsed => "";
        protected override SKSize OriginalSize { get; set; } = new SKSize(800, 800);
        protected override bool CanStartPaint()
        {
            return _gameContainer.GameStatus != EnumWhatStatus.NeedChooseGender;
        }
        private Assembly? _thisA;
        protected override void CreateSpaces()
        {
            CreatePositions(); // i think this is needed here instead.
            _gameContainer.CountrySpace = new GameSpace();
            _gameContainer.CountrySpace.Area = GetCountrySideRect();
            _gameContainer.MillionSpace = new GameSpace();
            _gameContainer.MillionSpace.Area = GetMillionRect();
            _gameContainer.ExtraSpace = new GameSpace();
        }
        private SKRect GetCareerRectangle()
        {
            var thisPos = (from items in TempData!.PositionList
                           where items.SpaceNumber == 201
                           select items).Single();
            return GetClickableRectangle(thisPos, false);
        }
        private SKRect GetCollegeRectangle()
        {
            var thisPos = (from items in TempData!.PositionList
                           where items.SpaceNumber == 202
                           select items).Single();
            return GetClickableRectangle(thisPos, false);
        }
        internal SKRect GetCountrySideRect()
        {
            var thisPos = (from items in TempData!.PositionList
                           where items.SpaceNumber == 203
                           select items).Single();
            return GetClickableRectangle(thisPos, true);
        }
        internal SKRect GetMillionRect()
        {
            var thisPos = (from items in TempData!.PositionList
                           where items.SpaceNumber == 204
                           select items).Single();
            return GetClickableRectangle(thisPos, true);
        }
        private SKRect GetExtraRectangle()
        {
            int index;
            index = 300 + (int)_gameContainer.CurrentView;
            var thisPos = (from items in TempData!.PositionList
                           where items.SpaceNumber == index
                           select items).Single();
            var firstPoint = GetActualPoint(thisPos.SpacePoint); // i think
            var thisSize = GetActualSize(400, 300); // i think
            return SKRect.Create(firstPoint.X, firstPoint.Y, thisSize.Width, thisSize.Height);
        }
        private ButtonCP GetButton(string text, Func<Task> func)
        {
            ButtonCP thisBut = new ButtonCP();
            var thisSize = GetActualSize(150, 50);
            thisBut.Text = text;
            thisBut.ExecuteAsync = func;
            thisBut.FontSize = (int)thisSize.Height / 3;
            thisBut.ActualHeight = thisSize.Height;
            thisBut.ActualWidth = thisSize.Width;
            return thisBut;
        }
        private SKPaint? _bitPaint;
        private SKBitmap? _thisBit;
        private SKPaint? _blackBorder;
        private SKPaint? _aquaBorder;
        private SKPaint? _limeBorder;
        protected override void SetUpPaints()
        {
            _thisA = Assembly.GetAssembly(GetType());
            _bitPaint = MiscHelpers.GetBitmapPaint();
            _thisBit = ImageExtensions.GetSkBitmap(_thisA, "gameboard.png");
            _blackBorder = MiscHelpers.GetStrokePaint(SKColors.Black, 4);
            _aquaBorder = MiscHelpers.GetStrokePaint(SKColors.Aqua, 4);
            _limeBorder = MiscHelpers.GetStrokePaint(SKColors.LimeGreen, 4);
        }
        protected override CarPieceCP GetGamePiece(string color, SKPoint location)
        {
            var output = base.GetGamePiece(color, location);
            output.NeedsToClear = false;
            return output;
        }
        private (int locationX, int locationY) CalculateView()
        {
            switch (_gameContainer.CurrentView)
            {
                case EnumViewCategory.StartGame:
                    {
                        return (30, 0);
                    }

                case EnumViewCategory.SpinAfterHouse:
                    {
                        return (230, 350);
                    }

                case EnumViewCategory.AfterFirstSetChoices:
                    {
                        return (40, 330);
                    }

                case EnumViewCategory.EndGame:
                    {
                        return (300, 0);
                    }

                default:
                    {
                        throw new BasicBlankException("Not sure");
                    }
            }
        }
        private (int locationX, int locationY) CalculateView(int currentPosition)
        {
            switch (currentPosition)
            {
                case object _ when currentPosition <= 36:
                    {
                        return CalculateView((int)EnumViewCategory.StartGame);
                    }

                case object _ when currentPosition <= 61:
                    {
                        return CalculateView((int)EnumViewCategory.SpinAfterHouse);
                    }

                case object _ when currentPosition <= 120:
                    {
                        return CalculateView((int)EnumViewCategory.AfterFirstSetChoices);
                    }

                default:
                    {
                        return CalculateView((int)EnumViewCategory.EndGame);
                    }
            }
        }
        private SKPoint _currentPoint;
        private SKRect GetSpaceRect(int thisSpace)
        {
            var thisPos = (from items in TempData!.PositionList
                           where items.PointView == _currentPoint && items.SpaceNumber == thisSpace
                           select items).Single();
            return GetClickableRectangle(thisPos, false);
        }
        protected override async Task ClickProcessAsync(SKPoint thisPoint)
        {
            if (_gameContainer.Command.IsExecuting)
            {
                return;
            }
            if (_gameContainer.GameStatus == EnumWhatStatus.NeedChooseFirstOption)
            {
                var firstOption = GetCareerRectangle();
                var secondOption = GetCollegeRectangle();
                if (MiscHelpers.DidClickRectangle(firstOption, thisPoint))
                {
                    _gameContainer.Command.StartExecuting();
                    await _options.OpeningOptionAsync(EnumStart.Career);
                    return;
                }
                if (MiscHelpers.DidClickRectangle(secondOption, thisPoint))
                {
                    _gameContainer.Command.StartExecuting();
                    await _options.OpeningOptionAsync(EnumStart.College);
                    return;
                }
                return;
            }
            if (_gameContainer.GameStatus == EnumWhatStatus.NeedChooseRetirement)
            {
                var country = GetCountrySideRect();
                var million = GetMillionRect();
                if (MiscHelpers.DidClickRectangle(country, thisPoint))
                {
                    _gameContainer.Command.StartExecuting();
                    await _options.RetirementAsync(EnumFinal.CountrySideAcres);
                    return;
                }
                if (MiscHelpers.DidClickRectangle(million, thisPoint))
                {
                    _gameContainer.Command.StartExecuting();
                    await _options.RetirementAsync(EnumFinal.MillionaireEstates);
                    return;
                }
                return;
            }
            if (_gameContainer.GameStatus == EnumWhatStatus.NeedToChooseSpace)
            {
                int firstOption = _spacePos.FirstPossiblePosition;
                int secondOption = _spacePos.SecondPossiblePosition;
                if (firstOption == 0 || secondOption == 0)
                    throw new BasicBlankException("Must truly have 2 options.  Otherwise, can't do it.");
                var firstRect = GetSpaceRect(firstOption);
                var secondRect = GetSpaceRect(secondOption);
                if (MiscHelpers.DidClickRectangle(firstRect, thisPoint))
                {
                    _options.SpaceDescription(firstOption);
                    return;
                }
                if (MiscHelpers.DidClickRectangle(secondRect, thisPoint))
                {
                    _options.SpaceDescription(secondOption);
                    return;
                }
            }

            foreach (var thisPiece in _gameContainer!.ExtraSpace!.PieceList)
            {
                ButtonCP? thisBut = thisPiece as ButtonCP;
                if (thisBut!.DidClickButton(thisPoint))
                {
                    if (thisBut.ExecuteAsync == null)
                    {
                        throw new BasicBlankException($"No execute async was done for button with text of {thisBut.Text}.  Rethink.");
                    }
                    _gameContainer.Command.StartExecuting();
                    await thisBut.ExecuteAsync();
                    return;
                }
            }
        }
        protected override void DrawBoard(SKCanvas canvas)
        {
            canvas.Clear();
            if (_gameContainer.SingleInfo!.Gender == EnumGender.None)
            {
                throw new BasicBlankException("The gender cannot be none for current turn.  Rethink");
            }
            var cropRect = GetBounds();
            var (locationX, LocationY) = CalculateView();
            _currentPoint = new SKPoint(locationX, LocationY);
            var testRect = SKRect.Create(locationX, LocationY, _ourWidth, _ourHeight);
            canvas.DrawBitmap(_thisBit, testRect, cropRect, _bitPaint);
            var thisSize = GetActualSize(51, 93);
            int index = 400 + (int)_gameContainer.CurrentView;
            var thisPos = TempData!.PositionList.Where(items => items.SpaceNumber == index).Single();
            var thisPoint = GetActualPoint(thisPos.SpacePoint);
            CarPieceCP thisPiece = GetGamePiece(_gameContainer.SingleInfo!.Color.ToColor(), thisPoint);
            thisPiece.ActualHeight = thisSize.Height;
            thisPiece.ActualWidth = thisSize.Width;
            thisPiece.AddPegs(_gameContainer.SingleInfo);
            thisPiece.DrawImage(canvas);  //to show whose turn it is.
            thisSize = GetActualSize(34, 62);
            _gameContainer.PlayerList!.ForEach(thisPlayer =>
            {
                if (thisPlayer.LastMove == EnumFinal.None && thisPlayer.OptionChosen != EnumStart.None && thisPlayer.Position > 0)
                    thisPos = TempData.PositionList.SingleOrDefault(items => items.PointView == _currentPoint && items.SpaceNumber == thisPlayer.Position);
                else if (thisPlayer.OptionChosen == EnumStart.Career && thisPlayer.LastMove == EnumFinal.None)
                    thisPos = TempData.PositionList.Single(items => items.SpaceNumber == 201);
                else if (thisPlayer.OptionChosen == EnumStart.College && thisPlayer.LastMove == EnumFinal.None)
                    thisPos = TempData.PositionList.Single(items => items.SpaceNumber == 202);
                else
                    thisPos = null!;
                if (thisPos != null && thisPos.SpaceNumber > 0)
                {
                    thisPoint = GetActualPoint(thisPos.SpacePoint);
                    thisPiece = GetGamePiece(thisPlayer.Color.ToColor(), thisPoint);
                    thisPiece.ActualHeight = thisSize.Height;
                    thisPiece.ActualWidth = thisSize.Width;
                    thisPiece.AddPegs(thisPlayer); // i think
                    thisPiece.DrawImage(canvas);
                }
            });
            if (_gameContainer.SingleInfo.PlayerCategory == EnumPlayerCategory.Self)
            {
                _gameContainer.ExtraSpace!.Area = GetExtraRectangle();
                _gameContainer.ExtraSpace.PieceList.Clear();
                ButtonCP thisBut;
                SKPoint newPos;
                if (_options.CanPurchaseCarInsurance)
                {
                    thisBut = GetButton("Insure Car", _options.PurchaseCarInsuranceAsync);
                    _gameContainer.ExtraSpace.PieceList.Add(thisBut);
                    newPos = _gameContainer.Pos!.GetPosition(_gameContainer.ExtraSpace, (float)thisBut.ActualWidth, (float)thisBut.ActualHeight);
                    thisBut.Location = newPos;
                    _gameContainer.Pos.AddPieceToArea(_gameContainer.ExtraSpace, thisBut);
                }
                if (_options.CanPurchaseHouseInsurance)
                {
                    thisBut = GetButton("Insure House", _options.PurchaseHouseInsuranceAsync);
                    _gameContainer.ExtraSpace.PieceList.Add(thisBut);
                    newPos = _gameContainer.Pos!.GetPosition(_gameContainer.ExtraSpace, (float)thisBut.ActualWidth, (float)thisBut.ActualHeight);
                    thisBut.Location = newPos;
                    _gameContainer.Pos.AddPieceToArea(_gameContainer.ExtraSpace, thisBut);
                }
                if (_options.CanPurchaseStock)
                {
                    thisBut = GetButton("Buy Stock", _options.PurchaseStockAsync);
                    _gameContainer.ExtraSpace.PieceList.Add(thisBut);
                    newPos = _gameContainer.Pos!.GetPosition(_gameContainer.ExtraSpace, (float)thisBut.ActualWidth, (float)thisBut.ActualHeight);
                    thisBut.Location = newPos;
                    _gameContainer.Pos.AddPieceToArea(_gameContainer.ExtraSpace, thisBut);
                }
                if (_options.CanSellHouse)
                {
                    thisBut = GetButton("Sell House", _options.SellHouseAsync);
                    _gameContainer.ExtraSpace.PieceList.Add(thisBut);
                    newPos = _gameContainer.Pos!.GetPosition(_gameContainer.ExtraSpace, (float)thisBut.ActualWidth, (float)thisBut.ActualHeight);
                    thisBut.Location = newPos;
                    _gameContainer.Pos.AddPieceToArea(_gameContainer.ExtraSpace, thisBut);
                }
                if (_options.CanAttendNightSchool)
                {
                    thisBut = GetButton("Night School", _options.AttendNightSchoolAsync);
                    _gameContainer.ExtraSpace.PieceList.Add(thisBut);
                    newPos = _gameContainer.Pos!.GetPosition(_gameContainer.ExtraSpace, (float)thisBut.ActualWidth, (float)thisBut.ActualHeight);
                    thisBut.Location = newPos;
                    _gameContainer.Pos.AddPieceToArea(_gameContainer.ExtraSpace, thisBut);
                }
                if (_options.CanTrade4Tiles)
                {
                    thisBut = GetButton("Trade 4 Tiles", _options.Trade4TilesAsync);
                    _gameContainer.ExtraSpace.PieceList.Add(thisBut);
                    newPos = _gameContainer.Pos!.GetPosition(_gameContainer.ExtraSpace, (float)thisBut.ActualWidth, (float)thisBut.ActualHeight);
                    thisBut.Location = newPos;
                    _gameContainer.Pos.AddPieceToArea(_gameContainer.ExtraSpace, thisBut);
                }
                if (_gameContainer.GameStatus == EnumWhatStatus.NeedToChooseSpace)
                {
                    thisBut = GetButton("Submit Space", _options.HumanChoseSpaceAsync);
                    _gameContainer.ExtraSpace.PieceList.Add(thisBut);
                    newPos = _gameContainer.Pos!.GetPosition(_gameContainer.ExtraSpace, (float)thisBut.ActualWidth, (float)thisBut.ActualHeight);
                    thisBut.Location = newPos;
                    _gameContainer.Pos.AddPieceToArea(_gameContainer.ExtraSpace, thisBut);
                }
                if (_options.CanEndTurn)
                {
                    thisBut = GetButton("End Turn", _gameContainer.UIEndTurnAsync);
                    _gameContainer.ExtraSpace.PieceList.Add(thisBut);
                    newPos = _gameContainer.Pos!.GetPosition(_gameContainer.ExtraSpace, (float)thisBut.ActualWidth, (float)thisBut.ActualHeight);
                    thisBut.Location = newPos;
                    _gameContainer.Pos.AddPieceToArea(_gameContainer.ExtraSpace, thisBut);
                }
                _gameContainer.ExtraSpace.PieceList.ForEach(EndBut =>
                {
                    EndBut.DrawImage(canvas);
                });
            }
            if (_gameContainer.CurrentView == EnumViewCategory.EndGame)
            {
                _gameContainer.Pos!.ClearArea(_gameContainer.CountrySpace!);
                _gameContainer.Pos.ClearArea(_gameContainer.MillionSpace!);
                _gameContainer.CountrySpace!.PieceList.Clear();
                _gameContainer.MillionSpace!.PieceList.Clear();
                var tempList = (from items in _gameContainer.PlayerList
                                where items.LastMove == EnumFinal.CountrySideAcres
                                select items).ToCustomBasicList();
                foreach (var thisPlayer in tempList)
                {
                    thisPiece = new CarPieceCP();
                    thisPiece.ActualHeight = _gameContainer.CountrySpace.Area.Height;
                    thisPiece.ActualWidth = thisSize.Width;
                    thisPiece.NeedsToClear = false;
                    thisPiece.MainColor = thisPlayer.Color.ToColor();
                    thisPiece.AddPegs(thisPlayer);
                    _gameContainer.CountrySpace.PieceList.Add(thisPiece);
                    var newPos = _gameContainer.Pos.GetPosition(_gameContainer.CountrySpace, (float)thisPiece.ActualWidth, (float)thisPiece.ActualHeight);
                    thisPiece.Location = newPos;
                    _gameContainer.Pos.AddPieceToArea(_gameContainer.CountrySpace, thisPiece);
                }
                tempList = (from Items in _gameContainer.PlayerList
                            where Items.LastMove == EnumFinal.MillionaireEstates
                            select Items).ToCustomBasicList();
                foreach (var thisPlayer in tempList)
                {
                    thisPiece = new CarPieceCP();
                    thisPiece.ActualHeight = _gameContainer.MillionSpace.Area.Height; // try this way.  even though the proportions won't quite be right.
                    thisPiece.ActualWidth = thisSize.Width;
                    thisPiece.NeedsToClear = false;
                    thisPiece.MainColor = thisPlayer.Color.ToColor();
                    thisPiece.AddPegs(thisPlayer);
                    _gameContainer.MillionSpace.PieceList.Add(thisPiece);
                    var newPos = _gameContainer.Pos.GetPosition(_gameContainer.MillionSpace, (float)thisPiece.ActualWidth, (float)thisPiece.ActualHeight);
                    thisPiece.Location = newPos;
                    _gameContainer.Pos.AddPieceToArea(_gameContainer.MillionSpace, thisPiece);
                }
                _gameContainer.CountrySpace.PieceList.ForEach(EndPiece =>
                {
                    EndPiece.DrawImage(canvas);
                });

                _gameContainer.MillionSpace.PieceList.ForEach(EndPiece =>
                {
                    EndPiece.DrawImage(canvas);
                });
            }
            if (_gameContainer.GameStatus == EnumWhatStatus.NeedToChooseSpace)
            {
                var firstPos = _spacePos.FirstPossiblePosition;
                var secondPos = _spacePos.SecondPossiblePosition;
                if (_gameContainer.CurrentSelected > 0 && _gameContainer.CurrentSelected != firstPos && _gameContainer.CurrentSelected != secondPos)
                    throw new BasicBlankException("The current selected must be 0, " + firstPos + " or " + secondPos + ", not " + _gameContainer.CurrentSelected);
                SKPaint thisPaint;
                if (firstPos == _gameContainer.CurrentSelected)
                    thisPaint = _aquaBorder!;
                else
                    thisPaint = _blackBorder!;
                thisPos = (from Items in TempData.PositionList
                           where Items.PointView == _currentPoint && Items.SpaceNumber == firstPos
                           select Items).Single();
                var thisRect = GetClickableRectangle(thisPos, false);
                canvas.DrawRect(thisRect, thisPaint);
                if (secondPos == _gameContainer.CurrentSelected)
                    thisPaint = _aquaBorder!;
                else
                    thisPaint = _blackBorder!;
                thisPos = (from Items in TempData.PositionList
                           where Items.PointView == _currentPoint && Items.SpaceNumber == secondPos
                           select Items).Single();
                thisRect = GetClickableRectangle(thisPos, false);
                canvas.DrawRect(thisRect, thisPaint);
            }
            if (GoingTo > 0)
            {
                thisPos = (from Items in TempData.PositionList
                           where Items.PointView == _currentPoint && Items.SpaceNumber == GoingTo
                           select Items).SingleOrDefault();
                if (thisPos == null == false)
                {
                    var ThisRect = GetClickableRectangle(thisPos!, false);
                    canvas.DrawRect(ThisRect, _limeBorder); // may later show another screen.
                }
            }
            if (_gameContainer.GameStatus == EnumWhatStatus.NeedChooseFirstOption)
            {
                var firstRect = GetCareerRectangle();
                var secondRect = GetCollegeRectangle();
                if (_gameContainer.CurrentView != EnumViewCategory.StartGame)
                    throw new BasicBlankException("If you need to choose between college and career, the current view must be start game");
                canvas.DrawRect(firstRect, _blackBorder);
                canvas.DrawRect(secondRect, _blackBorder);
            }
            if (_gameContainer.GameStatus == EnumWhatStatus.NeedChooseRetirement)
            {
                var firstRect = GetCountrySideRect();
                var secondRect = GetMillionRect();
                canvas.DrawRect(firstRect, _blackBorder);
                canvas.DrawRect(secondRect, _blackBorder);
            }
        }
    }
}
