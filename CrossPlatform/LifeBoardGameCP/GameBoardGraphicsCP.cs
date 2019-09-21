using BasicGameFramework.Attributes;
using BasicGameFramework.DIContainers;
using BasicGameFramework.GameGraphicsCP.BasicGameBoards;
using BasicGameFramework.GameGraphicsCP.GameboardPositionHelpers;
using BasicGameFramework.MultiplayerClasses.BasicPlayerClasses;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.FileFunctions;
using CommonBasicStandardLibraries.Exceptions;
using Newtonsoft.Json;
using SkiaSharp;
using SkiaSharpGeneralLibrary.SKExtensions;
using System.Linq;
using System.Reflection;
using System.Windows.Input;
namespace LifeBoardGameCP
{
    [SingletonGame]
    public class GameBoardGraphicsCP : BaseGameBoardCP<CarPieceCP>
    {
        private const int _ourHeight = 1200;
        private const int _ourWidth = 900;
        public TempInfo? TempData;
        internal int GoingTo;
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
            var thisData = _thisA.ResourcesAllTextFromFile("lifeboardgame.json");
            TempData = JsonConvert.DeserializeObject<TempInfo>(thisData);
        }
        private readonly GlobalVariableClass _thisGlobal;
        private readonly LifeBoardGameMainGameClass _mainGame;
        private readonly ISpacePosition _SpacePos;
        public GameBoardGraphicsCP(IGamePackageResolver MainContainer) : base(MainContainer)
        {
            _thisGlobal = MainContainer.Resolve<GlobalVariableClass>();
            _mainGame = MainContainer.Resolve<LifeBoardGameMainGameClass>();
            _SpacePos = MainContainer.Resolve<ISpacePosition>();
        }

        public override string TagUsed => "";
        protected override SKSize OriginalSize { get; set; } = new SKSize(800, 800);
        protected override bool CanStartPaint()
        {
            if (_mainGame.DidChooseColors == false)
                return false;
            return _thisGlobal.GameStatus != EnumWhatStatus.NeedChooseGender;
        }
        private Assembly? _thisA;
        protected override void CreateSpaces()
        {
            CreatePositions(); // i think this is needed here instead.
            _thisGlobal.CountrySpace = new GameSpace();
            _thisGlobal.CountrySpace.Area = GetCountrySideRect();
            _thisGlobal.MillionSpace = new GameSpace();
            _thisGlobal.MillionSpace.Area = GetMillionRect();
            _thisGlobal.ExtraSpace = new GameSpace();
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
            index = 300 + (int)_thisGlobal.CurrentView;
            var thisPos = (from items in TempData!.PositionList
                           where items.SpaceNumber == index
                           select items).Single();
            var firstPoint = GetActualPoint(thisPos.SpacePoint); // i think
            var thisSize = GetActualSize(400, 300); // i think
            return SKRect.Create(firstPoint.X, firstPoint.Y, thisSize.Width, thisSize.Height);
        }
        private ButtonCP GetButton(string text, ICommand command)
        {
            ButtonCP thisBut = new ButtonCP();
            var thisSize = GetActualSize(150, 50);
            thisBut.Text = text;
            thisBut.Command = command;
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
            switch (_thisGlobal.CurrentView)
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
        protected override void ClickProcess(SKPoint thisPoint)
        {
            if (_thisGlobal.GameStatus == EnumWhatStatus.NeedChooseFirstOption)
            {
                if (_mainGame.ThisMod!.OpeningCommand!.CanExecute(0) == false)
                    return;
                var firstOption = GetCareerRectangle();
                var secondOption = GetCollegeRectangle();
                if (MiscHelpers.DidClickRectangle(firstOption, thisPoint))
                {
                    _mainGame.ThisMod.OpeningCommand.Execute(EnumStart.Career);
                    return;
                }
                if (MiscHelpers.DidClickRectangle(secondOption, thisPoint))
                {
                    _mainGame.ThisMod.OpeningCommand.Execute(EnumStart.College);
                    return;
                }
                return;
            }
            if (_thisGlobal.GameStatus == EnumWhatStatus.NeedChooseRetirement)
            {
                if (_mainGame.ThisMod!.RetirementCommand!.CanExecute(0) == false)
                    return;
                var country = GetCountrySideRect();
                var million = GetMillionRect();
                if (MiscHelpers.DidClickRectangle(country, thisPoint))
                {
                    _mainGame.ThisMod.RetirementCommand.Execute(EnumFinal.CountrySideAcres);
                    return;
                }
                if (MiscHelpers.DidClickRectangle(million, thisPoint))
                {
                    _mainGame.ThisMod.RetirementCommand.Execute(EnumFinal.MillionaireEstates);
                    return;
                }
                return;
            }
            if (_thisGlobal.GameStatus == EnumWhatStatus.NeedToChooseSpace)
            {
                int firstOption = _SpacePos.FirstPossiblePosition;
                int secondOption = _SpacePos.SecondPossiblePosition;
                if (firstOption == 0 || secondOption == 0)
                    throw new BasicBlankException("Must truly have 2 options.  Otherwise, can't do it.");
                var firstRect = GetSpaceRect(firstOption);
                var secondRect = GetSpaceRect(secondOption);
                if (MiscHelpers.DidClickRectangle(firstRect, thisPoint))
                {
                    _mainGame.ThisMod!.SpaceCommand!.Execute(firstOption);
                    return;
                }
                if (MiscHelpers.DidClickRectangle(secondRect, thisPoint))
                {
                    _mainGame.ThisMod!.SpaceCommand!.Execute(secondOption);
                    return;
                }
            }

            foreach (var thisPiece in _thisGlobal!.ExtraSpace!.PieceList)
            {
                ButtonCP? thisBut = thisPiece as ButtonCP;
                if (thisBut!.DidClickButton(thisPoint))
                {
                    thisBut.Command!.Execute(null);
                    return;
                }
            }
        }
        protected override void DrawBoard(SKCanvas canvas)
        {
            canvas.Clear();
            var cropRect = GetBounds();
            var (locationX, LocationY) = CalculateView();
            _currentPoint = new SKPoint(locationX, LocationY);
            var testRect = SKRect.Create(locationX, LocationY, _ourWidth, _ourHeight);
            canvas.DrawBitmap(_thisBit, testRect, cropRect, _bitPaint);
            var thisSize = GetActualSize(51, 93);
            int index = 400 + (int)_thisGlobal.CurrentView;
            var thisPos = TempData!.PositionList.Where(items => items.SpaceNumber == index).Single();
            var thisPoint = GetActualPoint(thisPos.SpacePoint);
            CarPieceCP thisPiece = GetGamePiece(_mainGame.SingleInfo!.Color.ToColor(), thisPoint);
            thisPiece.ActualHeight = thisSize.Height;
            thisPiece.ActualWidth = thisSize.Width;
            thisPiece.AddPegs(_mainGame.SingleInfo);
            thisPiece.DrawImage(canvas);  //to show whose turn it is.
            thisSize = GetActualSize(34, 62);
            _mainGame.PlayerList!.ForEach(thisPlayer =>
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
            if (_mainGame.SingleInfo.PlayerCategory == EnumPlayerCategory.Self)
            {
                _thisGlobal.ExtraSpace!.Area = GetExtraRectangle();
                _thisGlobal.ExtraSpace.PieceList.Clear();
                ButtonCP thisBut;
                SKPoint newPos;
                if (_mainGame.ThisMod!.CarInsuranceCommand!.CanExecute(null!) == true)
                {
                    thisBut = GetButton("Insure Car", _mainGame.ThisMod.CarInsuranceCommand);
                    _thisGlobal.ExtraSpace.PieceList.Add(thisBut);
                    newPos = _thisGlobal.Pos!.GetPosition(_thisGlobal.ExtraSpace, (float)thisBut.ActualWidth, (float)thisBut.ActualHeight);
                    thisBut.Location = newPos;
                    _thisGlobal.Pos.AddPieceToArea(_thisGlobal.ExtraSpace, thisBut);
                }
                if (_mainGame.ThisMod.HouseInsuranceCommand!.CanExecute(null!) == true)
                {
                    thisBut = GetButton("Insure House", _mainGame.ThisMod.HouseInsuranceCommand);
                    _thisGlobal.ExtraSpace.PieceList.Add(thisBut);
                    newPos = _thisGlobal.Pos!.GetPosition(_thisGlobal.ExtraSpace, (float)thisBut.ActualWidth, (float)thisBut.ActualHeight);
                    thisBut.Location = newPos;
                    _thisGlobal.Pos.AddPieceToArea(_thisGlobal.ExtraSpace, thisBut);
                }
                if (_mainGame.ThisMod.PurchaseStockCommand!.CanExecute(null!) == true)
                {
                    thisBut = GetButton("Buy Stock", _mainGame.ThisMod.PurchaseStockCommand);
                    _thisGlobal.ExtraSpace.PieceList.Add(thisBut);
                    newPos = _thisGlobal.Pos!.GetPosition(_thisGlobal.ExtraSpace, (float)thisBut.ActualWidth, (float)thisBut.ActualHeight);
                    thisBut.Location = newPos;
                    _thisGlobal.Pos.AddPieceToArea(_thisGlobal.ExtraSpace, thisBut);
                }
                if (_mainGame.ThisMod.SellHouseCommand!.CanExecute(null!) == true)
                {
                    thisBut = GetButton("Sell House", _mainGame.ThisMod.SellHouseCommand);
                    _thisGlobal.ExtraSpace.PieceList.Add(thisBut);
                    newPos = _thisGlobal.Pos!.GetPosition(_thisGlobal.ExtraSpace, (float)thisBut.ActualWidth, (float)thisBut.ActualHeight);
                    thisBut.Location = newPos;
                    _thisGlobal.Pos.AddPieceToArea(_thisGlobal.ExtraSpace, thisBut);
                }
                if (_mainGame.ThisMod.AttendNightSchoolCommand!.CanExecute(null!) == true)
                {
                    thisBut = GetButton("Night School", _mainGame.ThisMod.AttendNightSchoolCommand);
                    _thisGlobal.ExtraSpace.PieceList.Add(thisBut);
                    newPos = _thisGlobal.Pos!.GetPosition(_thisGlobal.ExtraSpace, (float)thisBut.ActualWidth, (float)thisBut.ActualHeight);
                    thisBut.Location = newPos;
                    _thisGlobal.Pos.AddPieceToArea(_thisGlobal.ExtraSpace, thisBut);
                }
                if (_mainGame.ThisMod.TradeLifeForSalaryCommand!.CanExecute(null!) == true)
                {
                    thisBut = GetButton("Trade 4 Tiles", _mainGame.ThisMod.TradeLifeForSalaryCommand);
                    _thisGlobal.ExtraSpace.PieceList.Add(thisBut);
                    newPos = _thisGlobal.Pos!.GetPosition(_thisGlobal.ExtraSpace, (float)thisBut.ActualWidth, (float)thisBut.ActualHeight);
                    thisBut.Location = newPos;
                    _thisGlobal.Pos.AddPieceToArea(_thisGlobal.ExtraSpace, thisBut);
                }
                if (_thisGlobal.GameStatus == EnumWhatStatus.NeedToChooseSpace)
                {
                    thisBut = GetButton("Submit Space", _mainGame.ThisMod.SubmitCommand!);
                    _thisGlobal.ExtraSpace.PieceList.Add(thisBut);
                    newPos = _thisGlobal.Pos!.GetPosition(_thisGlobal.ExtraSpace, (float)thisBut.ActualWidth, (float)thisBut.ActualHeight);
                    thisBut.Location = newPos;
                    _thisGlobal.Pos.AddPieceToArea(_thisGlobal.ExtraSpace, thisBut);
                }
                if (_mainGame.ThisMod.EndTurnCommand.CanExecute(null!) == true)
                {
                    thisBut = GetButton("End Turn", _mainGame.ThisMod.EndTurnCommand);
                    _thisGlobal.ExtraSpace.PieceList.Add(thisBut);
                    newPos = _thisGlobal.Pos!.GetPosition(_thisGlobal.ExtraSpace, (float)thisBut.ActualWidth, (float)thisBut.ActualHeight);
                    thisBut.Location = newPos;
                    _thisGlobal.Pos.AddPieceToArea(_thisGlobal.ExtraSpace, thisBut);
                }
                _thisGlobal.ExtraSpace.PieceList.ForEach(EndBut =>
                {
                    EndBut.DrawImage(canvas);
                });
            }
            if (_thisGlobal.CurrentView == EnumViewCategory.EndGame)
            {
                _thisGlobal.Pos!.ClearArea(_thisGlobal.CountrySpace!);
                _thisGlobal.Pos.ClearArea(_thisGlobal.MillionSpace!);
                _thisGlobal.CountrySpace!.PieceList.Clear();
                _thisGlobal.MillionSpace!.PieceList.Clear();
                var tempList = (from items in _mainGame.PlayerList
                                where items.LastMove == EnumFinal.CountrySideAcres
                                select items).ToCustomBasicList();
                foreach (var thisPlayer in tempList)
                {
                    thisPiece = new CarPieceCP();
                    thisPiece.ActualHeight = _thisGlobal.CountrySpace.Area.Height;
                    thisPiece.ActualWidth = thisSize.Width;
                    thisPiece.NeedsToClear = false;
                    thisPiece.MainColor = thisPlayer.Color.ToColor();
                    thisPiece.AddPegs(thisPlayer);
                    _thisGlobal.CountrySpace.PieceList.Add(thisPiece);
                    var newPos = _thisGlobal.Pos.GetPosition(_thisGlobal.CountrySpace, (float)thisPiece.ActualWidth, (float)thisPiece.ActualHeight);
                    thisPiece.Location = newPos;
                    _thisGlobal.Pos.AddPieceToArea(_thisGlobal.CountrySpace, thisPiece);
                }
                tempList = (from Items in _mainGame.PlayerList
                            where Items.LastMove == EnumFinal.MillionaireEstates
                            select Items).ToCustomBasicList();
                foreach (var thisPlayer in tempList)
                {
                    thisPiece = new CarPieceCP();
                    thisPiece.ActualHeight = _thisGlobal.MillionSpace.Area.Height; // try this way.  even though the proportions won't quite be right.
                    thisPiece.ActualWidth = thisSize.Width;
                    thisPiece.NeedsToClear = false;
                    thisPiece.MainColor = thisPlayer.Color.ToColor();
                    thisPiece.AddPegs(thisPlayer);
                    _thisGlobal.MillionSpace.PieceList.Add(thisPiece);
                    var newPos = _thisGlobal.Pos.GetPosition(_thisGlobal.MillionSpace, (float)thisPiece.ActualWidth, (float)thisPiece.ActualHeight);
                    thisPiece.Location = newPos;
                    _thisGlobal.Pos.AddPieceToArea(_thisGlobal.MillionSpace, thisPiece);
                }
                _thisGlobal.CountrySpace.PieceList.ForEach(EndPiece =>
                {
                    EndPiece.DrawImage(canvas);
                });

                _thisGlobal.MillionSpace.PieceList.ForEach(EndPiece =>
                {
                    EndPiece.DrawImage(canvas);
                });
            }
            if (_thisGlobal.GameStatus == EnumWhatStatus.NeedToChooseSpace)
            {
                var firstPos = _SpacePos.FirstPossiblePosition;
                var secondPos = _SpacePos.SecondPossiblePosition;
                if (_thisGlobal.CurrentSelected > 0 && _thisGlobal.CurrentSelected != firstPos && _thisGlobal.CurrentSelected != secondPos)
                    throw new BasicBlankException("The current selected must be 0, " + firstPos + " or " + secondPos + ", not " + _thisGlobal.CurrentSelected);
                SKPaint thisPaint;
                if (firstPos == _thisGlobal.CurrentSelected)
                    thisPaint = _aquaBorder!;
                else
                    thisPaint = _blackBorder!;
                thisPos = (from Items in TempData.PositionList
                           where Items.PointView == _currentPoint && Items.SpaceNumber == firstPos
                           select Items).Single();
                var thisRect = GetClickableRectangle(thisPos, false);
                canvas.DrawRect(thisRect, thisPaint);
                if (secondPos == _thisGlobal.CurrentSelected)
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
            if (_thisGlobal.GameStatus == EnumWhatStatus.NeedChooseFirstOption)
            {
                var firstRect = GetCareerRectangle();
                var secondRect = GetCollegeRectangle();
                if (_thisGlobal.CurrentView != EnumViewCategory.StartGame)
                    throw new BasicBlankException("If you need to choose between college and career, the current view must be start game");
                canvas.DrawRect(firstRect, _blackBorder);
                canvas.DrawRect(secondRect, _blackBorder);
            }
            if (_thisGlobal.GameStatus == EnumWhatStatus.NeedChooseRetirement)
            {
                var firstRect = GetCountrySideRect();
                var secondRect = GetMillionRect();
                canvas.DrawRect(firstRect, _blackBorder);
                canvas.DrawRect(secondRect, _blackBorder);
            }
        }
    }
}