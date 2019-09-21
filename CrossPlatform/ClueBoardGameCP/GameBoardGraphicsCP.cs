using BasicGameFramework.Attributes;
using BasicGameFramework.DIContainers;
using BasicGameFramework.Extensions;
using BasicGameFramework.GameGraphicsCP.BasicGameBoards;
using BasicGameFramework.GameGraphicsCP.GameboardPositionHelpers;
using BasicGameFramework.GameGraphicsCP.GamePieces;
using BasicGameFramework.GameGraphicsCP.Interfaces;
using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.Exceptions;
using SkiaSharp;
using SkiaSharpGeneralLibrary.SKExtensions;
using System.Collections;
namespace ClueBoardGameCP
{
    [SingletonGame]
    public class GameBoardGraphicsCP : BaseGameBoardCP<PawnPiecesCP<EnumColorChoice>>
    {
        private struct Room
        {
            public string Name;
            public SKColor FloorColor1; // its okay because no autoresume needed for this.
            public SKColor FloorColor2;
            // Dim Area As Rectangle
            public CustomBasicList<SKRect> MiscList; // coordinates.
            public SKRect PieceArea; // this is the rectangle (coordinates only)
            public CustomBasicList<Door> Doors;
            public SKPoint TextPoint;
        }
        private struct Door
        {
            public SKPoint Point;
            public string Direction;
        }
        private CustomBasicList<Room>? arr_Rooms; // 0 based here unless i use dictionary.
        private Hashtable? arr_Squares;
        private readonly ClueBoardGameMainGameClass _mainGame;
        public GameBoardGraphicsCP(IGamePackageResolver MainContainer) : base(MainContainer)
        {
            _mainGame = MainContainer.Resolve<ClueBoardGameMainGameClass>();
            InitializeRooms();
            InitializeSquares();
        }
        public override string TagUsed => "";
        protected override SKSize OriginalSize { get; set; } = new SKSize(576, 600);
        protected override bool CanStartPaint()
        {
            return _mainGame.DidChooseColors;
        }
        private float SpaceSize() // i think private.
        {
            var bounds = GetBounds();
            return bounds.Width / 24; // i think
        }
        protected override PawnPiecesCP<EnumColorChoice> GetGamePiece(string color, SKPoint location)
        {
            var output = base.GetGamePiece(color, location);
            output.NeedsToClear = false;
            output.ActualHeight = SpaceSize();
            output.ActualWidth = SpaceSize();
            return output;
        }
        public string RoomName(int room) // i think.
        {
            Room thisRoom;
            thisRoom = arr_Rooms![room - 1];  // its minus, not plus
            return thisRoom.Name;
        }
        private int SpaceClicked(float x, float y)
        {
            var bounds = GetBounds();
            int int_Row;
            int int_Col;
            SKRect rect_Temp;
            SKPoint pt_Temp;
            string str_Name;
            float dbl_SquareWidth;
            float dbl_SquareHeight;
            dbl_SquareWidth = bounds.Width / 24f;
            dbl_SquareHeight = bounds.Height / 25f;
            for (int_Row = 1; int_Row <= 25; int_Row++)
            {
                for (int_Col = 1; int_Col <= 24; int_Col++)
                {
                    pt_Temp = new SKPoint(int_Col, int_Row);
                    if ((arr_Squares!.Contains(pt_Temp)))
                    {
                        str_Name = arr_Squares[pt_Temp].ToString(); //hopefully that works.
                        if (str_Name.Contains("b") == false)
                        {
                            rect_Temp = SKRect.Create((pt_Temp.X - 1) * (dbl_SquareWidth), ((pt_Temp.Y - 1) * (dbl_SquareHeight)), dbl_SquareWidth, dbl_SquareHeight);
                            if (MiscHelpers.DidClickRectangle(rect_Temp, (int)x, (int)y) == true)
                                return System.Convert.ToInt32(str_Name.Replace("S", "").Replace("b", ""));
                        }
                    }
                }
            }
            return 0;
        }
        private int RoomClicked(float x, float y)
        {
            var bounds = GetBounds();
            SKRect rect_Room;
            float dbl_SquareWidth;
            float dbl_SquareHeight;
            int z = 0;
            dbl_SquareWidth = bounds.Width / 24;
            dbl_SquareHeight = bounds.Height / 25;
            foreach (var obj_Room in arr_Rooms!)
            {
                z += 1;
                rect_Room = SKRect.Create((obj_Room.PieceArea.Left - 1) * (dbl_SquareWidth), ((obj_Room.PieceArea.Top - 1) * (dbl_SquareHeight)), dbl_SquareWidth * obj_Room.PieceArea.Width, dbl_SquareHeight * obj_Room.PieceArea.Height);
                if (MiscHelpers.DidClickRectangle(rect_Room, (int)x, (int)y) == true)
                    return z;
                foreach (var TempRect in obj_Room.MiscList)
                {
                    rect_Room = SKRect.Create((TempRect.Left - 1) * (dbl_SquareWidth), ((TempRect.Top - 1) * (dbl_SquareHeight)), dbl_SquareWidth * TempRect.Width, dbl_SquareHeight * TempRect.Height);
                    if (MiscHelpers.DidClickRectangle(rect_Room, (int)x, (int)y) == true)
                        return z;
                }
            }
            return 0;
        }
        protected override void ClickProcess(SKPoint thisPoint)
        {
            if (_mainGame.ThisMod!.SpaceCommand!.CanExecute(0) == false)
                return;
            int spaceIndex = SpaceClicked(thisPoint.X, thisPoint.Y);
            if (spaceIndex > 0)
            {
                _mainGame.ThisMod.SpaceCommand.Execute(spaceIndex);
                return;
            }
            int roomIndex = RoomClicked(thisPoint.X, thisPoint.Y);
            if (roomIndex > 0)
            {
                _mainGame.ThisMod.RoomCommand!.Execute(roomIndex);
                return;
            }
        }
        protected override void CreateSpaces() { }
        private SKPaint? _whitePaint;
        private SKPaint? _goldPaint;
        private SKPaint? _borderPaint;
        private SKPaint? _darkGreenPaint;
        private SKPaint? _orangePaint;
        private SKPaint? _peachPuffPaint;
        protected override void SetUpPaints()
        {
            _whitePaint = MiscHelpers.GetSolidPaint(SKColors.White);
            _goldPaint = MiscHelpers.GetSolidPaint(SKColors.Gold);
            _borderPaint = MiscHelpers.GetStrokePaint(SKColors.Black, 1);
            _darkGreenPaint = MiscHelpers.GetSolidPaint(SKColors.DarkGreen);
            _orangePaint = MiscHelpers.GetSolidPaint(SKColors.Orange);
            _peachPuffPaint = MiscHelpers.GetSolidPaint(SKColors.PeachPuff);
        }
        private void InitializeRooms()
        {
            Room obj_TempRoom;
            Door obj_TempDoor;
            SKRect TempRect;
            arr_Squares = new Hashtable();
            // *** Create the rooms
            arr_Rooms = new CustomBasicList<Room>();
            // ******************************
            // *** Create the kitchen
            obj_TempRoom = new Room();
            obj_TempRoom.Name = "Kitchen";
            obj_TempRoom.FloorColor1 = SKColors.Black;
            obj_TempRoom.FloorColor2 = SKColors.White;
            obj_TempRoom.MiscList = new CustomBasicList<SKRect>();
            obj_TempRoom.PieceArea = SKRect.Create(19, 19, 6, 7);
            TempRect = SKRect.Create(24, 18, 1, 1);
            obj_TempRoom.MiscList.Add(TempRect);
            TempRect = SKRect.Create(18, 24, 1, 2);
            obj_TempRoom.MiscList.Add(TempRect);
            TempRect = SKRect.Create(16, 25, 2, 1);
            obj_TempRoom.MiscList.Add(TempRect);
            obj_TempRoom.TextPoint = new SKPoint(20.5f, 22);
            obj_TempRoom.Doors = new CustomBasicList<Door>();
            obj_TempDoor.Direction = "Down";
            obj_TempDoor.Point = new SKPoint(20, 19);
            obj_TempRoom.Doors.Add(obj_TempDoor);
            arr_Rooms.Add(obj_TempRoom);
            // ******************************
            // *** Create the Ball Room
            obj_TempRoom = new Room();
            obj_TempRoom.Name = "Ball Room";
            obj_TempRoom.FloorColor1 = SKColors.SaddleBrown;
            obj_TempRoom.FloorColor2 = SKColors.SaddleBrown;
            obj_TempRoom.MiscList = new CustomBasicList<SKRect>();
            obj_TempRoom.PieceArea = SKRect.Create(9, 18, 8, 6);
            TempRect = SKRect.Create(10, 24, 5, 2);
            obj_TempRoom.MiscList.Add(TempRect);
            obj_TempRoom.Doors = new CustomBasicList<Door>();
            obj_TempDoor.Direction = "Left";
            obj_TempDoor.Point = new SKPoint(16, 20);
            obj_TempRoom.Doors.Add(obj_TempDoor);
            obj_TempDoor.Direction = "Down";
            obj_TempDoor.Point = new SKPoint(14, 18);
            obj_TempRoom.Doors.Add(obj_TempDoor);
            obj_TempDoor.Direction = "Down";
            obj_TempDoor.Point = new SKPoint(10, 18);
            obj_TempRoom.Doors.Add(obj_TempDoor);
            obj_TempDoor.Direction = "Right";
            obj_TempDoor.Point = new SKPoint(9, 20);
            obj_TempRoom.Doors.Add(obj_TempDoor);
            arr_Rooms.Add(obj_TempRoom);
            // ******************************
            // *** Create the conservatory
            obj_TempRoom = new Room();
            obj_TempRoom.Name = "Conservatory";
            obj_TempRoom.FloorColor1 = SKColors.Green;
            obj_TempRoom.FloorColor2 = SKColors.White;
            obj_TempRoom.MiscList = new CustomBasicList<SKRect>();
            obj_TempRoom.PieceArea = SKRect.Create(1, 20, 5, 6);
            TempRect = SKRect.Create(6, 21, 1, 5);
            obj_TempRoom.MiscList.Add(TempRect);
            TempRect = SKRect.Create(7, 25, 2, 1);
            obj_TempRoom.MiscList.Add(TempRect);
            obj_TempRoom.TextPoint = new SKPoint(1, 23);
            obj_TempRoom.Doors = new CustomBasicList<Door>();
            obj_TempDoor.Direction = "Left";
            obj_TempDoor.Point = new SKPoint(5, 20);
            obj_TempRoom.Doors.Add(obj_TempDoor);
            arr_Rooms.Add(obj_TempRoom);
            // ******************************
            // *** Create the billiard room
            obj_TempRoom = new Room();
            obj_TempRoom.Name = "Billiard Room";
            obj_TempRoom.FloorColor1 = SKColors.DarkGreen;
            obj_TempRoom.FloorColor2 = SKColors.DarkGreen;
            obj_TempRoom.MiscList = new CustomBasicList<SKRect>();
            obj_TempRoom.PieceArea = SKRect.Create(1, 13, 6, 5);
            TempRect = SKRect.Create(1, 12, 1, 1);
            obj_TempRoom.MiscList.Add(TempRect);
            TempRect = SKRect.Create(1, 18, 1, 1);
            obj_TempRoom.MiscList.Add(TempRect);
            obj_TempRoom.Doors = new CustomBasicList<Door>();
            obj_TempDoor.Direction = "Left";
            obj_TempDoor.Point = new SKPoint(6, 16);
            obj_TempRoom.Doors.Add(obj_TempDoor);
            obj_TempDoor.Direction = "Down";
            obj_TempDoor.Point = new SKPoint(2, 13);
            obj_TempRoom.Doors.Add(obj_TempDoor);
            arr_Rooms.Add(obj_TempRoom);
            // ******************************
            // *** Create the library
            obj_TempRoom = new Room();
            obj_TempRoom.Name = "Library";
            obj_TempRoom.FloorColor1 = SKColors.Blue;
            obj_TempRoom.FloorColor2 = SKColors.Blue;
            obj_TempRoom.MiscList = new CustomBasicList<SKRect>();
            obj_TempRoom.PieceArea = SKRect.Create(1, 7, 6, 5);
            TempRect = SKRect.Create(7, 8, 1, 3);
            obj_TempRoom.MiscList.Add(TempRect);
            obj_TempRoom.Doors = new CustomBasicList<Door>();
            obj_TempDoor.Direction = "Up";
            obj_TempDoor.Point = new SKPoint(4, 11);
            obj_TempRoom.Doors.Add(obj_TempDoor);
            obj_TempDoor.Direction = "Left";
            obj_TempDoor.Point = new SKPoint(7, 9);
            obj_TempRoom.Doors.Add(obj_TempDoor);
            arr_Rooms.Add(obj_TempRoom);
            // ******************************
            // *** Create the study
            obj_TempRoom = new Room();
            obj_TempRoom.Name = "Study";
            obj_TempRoom.FloorColor1 = SKColors.Green;
            obj_TempRoom.FloorColor2 = SKColors.Green;
            obj_TempRoom.MiscList = new CustomBasicList<SKRect>();
            obj_TempRoom.PieceArea = SKRect.Create(1, 1, 7, 4);
            TempRect = SKRect.Create(8, 1, 1, 1);
            obj_TempRoom.MiscList.Add(TempRect);
            TempRect = SKRect.Create(1, 5, 1, 1);
            obj_TempRoom.MiscList.Add(TempRect);
            obj_TempDoor.Direction = "Up";
            obj_TempDoor.Point = new SKPoint(7, 4);
            obj_TempRoom.Doors = new CustomBasicList<Door>
            {
                obj_TempDoor
            };
            obj_TempRoom.TextPoint = new SKPoint(3, 3);
            arr_Rooms.Add(obj_TempRoom);
            // ******************************
            // *** Create the hall
            obj_TempRoom = new Room();
            obj_TempRoom.Name = "Hall";
            obj_TempRoom.FloorColor1 = SKColors.Maroon;
            obj_TempRoom.FloorColor2 = SKColors.Maroon;
            obj_TempRoom.MiscList = new CustomBasicList<SKRect>();
            obj_TempRoom.PieceArea = SKRect.Create(10, 1, 6, 7);
            TempRect = SKRect.Create(9, 1, 1, 1);
            obj_TempRoom.MiscList.Add(TempRect);
            TempRect = SKRect.Create(16, 1, 1, 1);
            obj_TempRoom.MiscList.Add(TempRect);
            obj_TempRoom.Doors = new CustomBasicList<Door>();
            obj_TempDoor.Direction = "Right";
            obj_TempDoor.Point = new SKPoint(10, 5);
            obj_TempRoom.Doors.Add(obj_TempDoor);
            obj_TempDoor.Direction = "Up";
            obj_TempDoor.Point = new SKPoint(12, 7);
            obj_TempRoom.Doors.Add(obj_TempDoor);
            obj_TempDoor.Direction = "Up";
            obj_TempDoor.Point = new SKPoint(13, 7);
            obj_TempRoom.Doors.Add(obj_TempDoor);
            arr_Rooms.Add(obj_TempRoom);
            // ******************************
            // *** Create the lounge
            obj_TempRoom = new Room();
            obj_TempRoom.Name = "Lounge";
            obj_TempRoom.FloorColor1 = SKColors.SeaGreen;
            obj_TempRoom.FloorColor2 = SKColors.SeaGreen;
            obj_TempRoom.MiscList = new CustomBasicList<SKRect>();
            obj_TempRoom.PieceArea = SKRect.Create(18, 1, 7, 6);
            TempRect = SKRect.Create(24, 7, 1, 1);
            obj_TempRoom.MiscList.Add(TempRect);
            obj_TempRoom.Doors = new CustomBasicList<Door>();
            obj_TempDoor.Direction = "Up";
            obj_TempDoor.Point = new SKPoint(18, 6);
            obj_TempRoom.Doors.Add(obj_TempDoor);
            arr_Rooms.Add(obj_TempRoom);
            // ******************************
            // *** Create the dining room
            obj_TempRoom = new Room();
            obj_TempRoom.Name = "Dining Room";
            obj_TempRoom.FloorColor1 = SKColors.Brown;
            obj_TempRoom.FloorColor2 = SKColors.Brown;
            obj_TempRoom.MiscList = new CustomBasicList<SKRect>();
            obj_TempRoom.PieceArea = SKRect.Create(17, 10, 8, 6);
            TempRect = SKRect.Create(24, 9, 1, 1);
            obj_TempRoom.MiscList.Add(TempRect);
            TempRect = SKRect.Create(24, 17, 1, 1);
            obj_TempRoom.MiscList.Add(TempRect);
            TempRect = SKRect.Create(20, 16, 5, 1);
            obj_TempRoom.MiscList.Add(TempRect);
            obj_TempRoom.Doors = new CustomBasicList<Door>();
            obj_TempDoor.Direction = "Down";
            obj_TempDoor.Point = new SKPoint(18, 10);
            obj_TempRoom.Doors.Add(obj_TempDoor);
            obj_TempDoor.Direction = "Right";
            obj_TempDoor.Point = new SKPoint(17, 13);
            obj_TempRoom.Doors.Add(obj_TempDoor);
            arr_Rooms.Add(obj_TempRoom);
        }
        private void InitializeSquares()
        {
            int int_Row;
            int int_Col;
            // ******************************
            // *** Create the spaces
            arr_Squares = new Hashtable();
            arr_Squares.Add(new SKPoint(8, 2), "S" + System.Convert.ToString((arr_Squares.Count + 1)));
            arr_Squares.Add(new SKPoint(9, 2), "S" + System.Convert.ToString((arr_Squares.Count + 1)));
            arr_Squares.Add(new SKPoint(8, 3), "S" + System.Convert.ToString((arr_Squares.Count + 1)));
            arr_Squares.Add(new SKPoint(9, 3), "S" + System.Convert.ToString((arr_Squares.Count + 1)));
            arr_Squares.Add(new SKPoint(9, 4), "S" + System.Convert.ToString((arr_Squares.Count + 1)));
            arr_Squares.Add(new SKPoint(8, 4), "S" + System.Convert.ToString((arr_Squares.Count + 1)));
            int_Col = 9;
            while (int_Col >= 2)
            {
                arr_Squares.Add(new SKPoint(int_Col, 5), "S" + System.Convert.ToString((arr_Squares.Count + 1)));
                int_Col -= 1;
            }
            for (int_Col = 2; int_Col <= 9; int_Col++)
                arr_Squares.Add(new SKPoint(int_Col, 6), "S" + System.Convert.ToString((arr_Squares.Count + 1)));
            arr_Squares.Add(new SKPoint(9, 7), "S" + System.Convert.ToString((arr_Squares.Count + 1)));
            arr_Squares.Add(new SKPoint(8, 7), "S" + System.Convert.ToString((arr_Squares.Count + 1)));
            arr_Squares.Add(new SKPoint(7, 7), "S" + System.Convert.ToString((arr_Squares.Count + 1)));
            for (int_Col = 8; int_Col <= 16; int_Col++)
                arr_Squares.Add(new SKPoint(int_Col, 8), "S" + System.Convert.ToString((arr_Squares.Count + 1)));
            int_Row = 7;
            while (int_Row >= 2)
            {
                arr_Squares.Add(new SKPoint(16, int_Row), "S" + System.Convert.ToString((arr_Squares.Count + 1)));
                int_Row -= 1;
            }
            for (int_Row = 2; int_Row <= 7; int_Row++)
                arr_Squares.Add(new SKPoint(17, int_Row), "S" + System.Convert.ToString((arr_Squares.Count + 1)));
            for (int_Col = 18; int_Col <= 23; int_Col++)
                arr_Squares.Add(new SKPoint(int_Col, 7), "S" + System.Convert.ToString((arr_Squares.Count + 1)));
            int_Col = 23;
            while (int_Col >= 17)
            {
                arr_Squares.Add(new SKPoint(int_Col, 8), "S" + System.Convert.ToString((arr_Squares.Count + 1)));
                int_Col -= 1;
            }
            int_Col = 23;
            while (int_Col >= 15)
            {
                arr_Squares.Add(new SKPoint(int_Col, 9), "S" + System.Convert.ToString((arr_Squares.Count + 1)));
                int_Col -= 1;
            }
            // 69-75
            arr_Squares.Add(new SKPoint(8, 9), "S" + System.Convert.ToString((arr_Squares.Count + 1)));
            arr_Squares.Add(new SKPoint(9, 9), "S" + System.Convert.ToString((arr_Squares.Count + 1)));
            arr_Squares.Add(new SKPoint(9, 10), "S" + System.Convert.ToString((arr_Squares.Count + 1)));
            arr_Squares.Add(new SKPoint(8, 10), "S" + System.Convert.ToString((arr_Squares.Count + 1)));
            arr_Squares.Add(new SKPoint(7, 11), "S" + System.Convert.ToString((arr_Squares.Count + 1)));
            arr_Squares.Add(new SKPoint(8, 11), "S" + System.Convert.ToString((arr_Squares.Count + 1)));
            arr_Squares.Add(new SKPoint(9, 11), "S" + System.Convert.ToString((arr_Squares.Count + 1)));
            // 76-83
            for (int_Col = 2; int_Col <= 9; int_Col++)
                arr_Squares.Add(new SKPoint(int_Col, 12), "S" + System.Convert.ToString((arr_Squares.Count + 1)));
            // 84-89
            for (int_Row = 10; int_Row <= 12; int_Row++)
            {
                for (int_Col = 15; int_Col <= 16; int_Col++)
                    arr_Squares.Add(new SKPoint(int_Col, int_Row), "S" + System.Convert.ToString((arr_Squares.Count + 1)));
            }
            // 90-98
            for (int_Row = 13; int_Row <= 15; int_Row++)
            {
                for (int_Col = 7; int_Col <= 9; int_Col++)
                    arr_Squares.Add(new SKPoint(int_Col, int_Row), "S" + System.Convert.ToString((arr_Squares.Count + 1)));
            }
            // 99-104
            for (int_Row = 13; int_Row <= 15; int_Row++)
            {
                for (int_Col = 15; int_Col <= 16; int_Col++)
                    arr_Squares.Add(new SKPoint(int_Col, int_Row), "S" + System.Convert.ToString((arr_Squares.Count + 1)));
            }
            // 105-117
            for (int_Col = 7; int_Col <= 19; int_Col++)
                arr_Squares.Add(new SKPoint(int_Col, 16), "S" + System.Convert.ToString((arr_Squares.Count + 1)));
            // 118-134
            for (int_Col = 7; int_Col <= 23; int_Col++)
                arr_Squares.Add(new SKPoint(int_Col, 17), "S" + System.Convert.ToString((arr_Squares.Count + 1)));
            // 135-141
            for (int_Col = 2; int_Col <= 8; int_Col++)
                arr_Squares.Add(new SKPoint(int_Col, 18), "S" + System.Convert.ToString((arr_Squares.Count + 1)));
            // 142-148
            for (int_Col = 17; int_Col <= 23; int_Col++)
                arr_Squares.Add(new SKPoint(int_Col, 18), "S" + System.Convert.ToString((arr_Squares.Count + 1)));
            // 149-155
            for (int_Col = 2; int_Col <= 8; int_Col++)
                arr_Squares.Add(new SKPoint(int_Col, 19), "S" + System.Convert.ToString((arr_Squares.Count + 1)));
            // 156-162
            arr_Squares.Add(new SKPoint(17, 19), "S" + System.Convert.ToString((arr_Squares.Count + 1)));
            arr_Squares.Add(new SKPoint(18, 19), "S" + System.Convert.ToString((arr_Squares.Count + 1)));
            arr_Squares.Add(new SKPoint(6, 20), "S" + System.Convert.ToString((arr_Squares.Count + 1)));
            arr_Squares.Add(new SKPoint(7, 20), "S" + System.Convert.ToString((arr_Squares.Count + 1)));
            arr_Squares.Add(new SKPoint(8, 20), "S" + System.Convert.ToString((arr_Squares.Count + 1)));
            arr_Squares.Add(new SKPoint(17, 20), "S" + System.Convert.ToString((arr_Squares.Count + 1)));
            arr_Squares.Add(new SKPoint(18, 20), "S" + System.Convert.ToString((arr_Squares.Count + 1)));
            // 163-171
            for (int_Row = 21; int_Row <= 24; int_Row++)
            {
                for (int_Col = 7; int_Col <= 8; int_Col++)
                    arr_Squares.Add(new SKPoint(int_Col, int_Row), "S" + System.Convert.ToString((arr_Squares.Count + 1)));
            }
            arr_Squares.Add(new SKPoint(9, 24), "S" + System.Convert.ToString((arr_Squares.Count + 1)));
            arr_Squares.Add(new SKPoint(15, 24), "S" + System.Convert.ToString((arr_Squares.Count + 1)));
            arr_Squares.Add(new SKPoint(16, 24), "S" + System.Convert.ToString((arr_Squares.Count + 1)));
            arr_Squares.Add(new SKPoint(17, 24), "S" + System.Convert.ToString((arr_Squares.Count + 1)));
            arr_Squares.Add(new SKPoint(17, 23), "S" + System.Convert.ToString((arr_Squares.Count + 1)));
            arr_Squares.Add(new SKPoint(18, 23), "S" + System.Convert.ToString((arr_Squares.Count + 1)));
            arr_Squares.Add(new SKPoint(18, 22), "S" + System.Convert.ToString((arr_Squares.Count + 1)));
            arr_Squares.Add(new SKPoint(17, 22), "S" + System.Convert.ToString((arr_Squares.Count + 1)));
            arr_Squares.Add(new SKPoint(17, 21), "S" + System.Convert.ToString((arr_Squares.Count + 1)));
            arr_Squares.Add(new SKPoint(18, 21), "S" + System.Convert.ToString((arr_Squares.Count + 1)));
            // *** Create the entryways
            arr_Squares.Add(new SKPoint(17, 1), "b1");
            arr_Squares.Add(new SKPoint(24, 8), "b2");
            arr_Squares.Add(new SKPoint(15, 25), "b3");
            arr_Squares.Add(new SKPoint(9, 25), "b4");
            arr_Squares.Add(new SKPoint(1, 19), "b5");
            arr_Squares.Add(new SKPoint(1, 6), "b6");
        }
        private SKPoint PositionForStartSpace(int startNumber) // i think make private since this should be drawing the stuff on the board as well
        {
            var bounds = GetBounds();
            int int_Row;
            int int_Col;
            SKPoint pt_Temp;
            string str_Name;
            float dbl_SquareWidth;
            float dbl_SquareHeight;
            dbl_SquareWidth = bounds.Width / 24;
            dbl_SquareHeight = bounds.Height / 25;
            for (int_Row = 1; int_Row <= 25; int_Row++)
            {
                for (int_Col = 1; int_Col <= 24; int_Col++)
                {
                    pt_Temp = new SKPoint(int_Col, int_Row);
                    if ((arr_Squares!.Contains(pt_Temp)))
                    {
                        str_Name = arr_Squares[pt_Temp].ToString();
                        if (((str_Name ?? "") == (("b" + startNumber) ?? "")))
                            return new SKPoint((pt_Temp.X - 1) * (dbl_SquareWidth), ((pt_Temp.Y - 1) * (dbl_SquareHeight)));
                    }
                }
            }
            return default;
        }
        private SKPoint PositionForBoardPiece(int space)
        {
            int int_Row;
            int int_Col;
            SKPoint pt_Temp;
            string str_Name;
            float dbl_SquareWidth;
            float dbl_SquareHeight;
            dbl_SquareHeight = SpaceSize();
            dbl_SquareWidth = SpaceSize();
            for (int_Row = 1; int_Row <= 25; int_Row++)
            {
                for (int_Col = 1; int_Col <= 24; int_Col++)
                {
                    pt_Temp = new SKPoint(int_Col, int_Row);
                    if ((arr_Squares!.Contains(pt_Temp)))
                    {
                        str_Name = arr_Squares[pt_Temp].ToString();
                        if (((str_Name ?? "") == (("S" + space) ?? "")))
                            return new SKPoint((pt_Temp.X - 1) * (dbl_SquareWidth), ((pt_Temp.Y - 1) * (dbl_SquareHeight)));
                    }
                }
            }
            return default;
        }
        public GameSpace RoomSpaceInfo(int room)
        {
            GameSpace thisRoom = new GameSpace();
            Room obj_Room;
            var bounds = GetBounds();
            if (bounds.Height == 0 || bounds.Width == 0)
            {
                if (_didDraw)
                    throw new BasicBlankException("Did draw was true but still cannot get bounds");
                else
                    throw new BasicBlankException("The height and width cannot be 0 for roomspaceinfo");
            }
            float dbl_SquareWidth;
            float dbl_SquareHeight;
            dbl_SquareWidth = bounds.Width / 24;
            dbl_SquareHeight = bounds.Height / 25;
            int y;
            obj_Room = arr_Rooms![room - 1]; // because 0 based.
            thisRoom.Area = SKRect.Create((obj_Room.PieceArea.Left - 1) * (dbl_SquareWidth), (obj_Room.PieceArea.Top - 1) * (dbl_SquareHeight), dbl_SquareWidth * obj_Room.PieceArea.Width, dbl_SquareHeight * obj_Room.PieceArea.Height);
            var loopTo = obj_Room.Doors.Count - 1;
            for (y = 0; y <= loopTo; y++)
                thisRoom.ObjectList.Add(DoorRectangle(obj_Room.Doors[y], dbl_SquareWidth, dbl_SquareHeight));
            return thisRoom;
        }
        private SKRect DoorRectangle(Door obj_Door, float dbl_SquareWidth, float dbl_SquareHeight)
        {
            SKPoint pt_Temp;
            pt_Temp = obj_Door.Point;
            switch (obj_Door.Direction)
            {
                case "Up":
                    {
                        return SKRect.Create((pt_Temp.X - 1) * (dbl_SquareWidth), ((pt_Temp.Y - 1) * (dbl_SquareHeight)) + (dbl_SquareHeight * 0.4f), dbl_SquareWidth, dbl_SquareHeight * 0.6f);
                    }

                case "Down":
                    {
                        return SKRect.Create((pt_Temp.X - 1) * (dbl_SquareWidth), ((pt_Temp.Y - 1) * (dbl_SquareHeight)), dbl_SquareWidth, dbl_SquareHeight * 0.6f);
                    }

                case "Left":
                    {
                        return SKRect.Create((pt_Temp.X - 1) * (dbl_SquareWidth) + (dbl_SquareWidth * 0.4f), ((pt_Temp.Y - 1) * (dbl_SquareHeight)), dbl_SquareWidth * 0.6f, dbl_SquareHeight);
                    }

                case "Right":
                    {
                        return SKRect.Create((pt_Temp.X - 1) * (dbl_SquareWidth), ((pt_Temp.Y - 1) * (dbl_SquareHeight)), dbl_SquareWidth * 0.6f, dbl_SquareHeight);
                    }
            }

            return default;
        }
        private bool _didDraw;
        protected override void DrawBoard(SKCanvas canvas)
        {
            _didDraw = true;
            canvas.Clear();
            PrivateDrawBoard(canvas);
            if (_mainGame.ThisGlobal!.RoomList.Count == 0)
                return; //because we don't have the room list yet.
            if (_mainGame.ThisGlobal.RoomList.Count != 9)
                throw new BasicBlankException("Must have 9 rooms when painting board");
            int x = 0;
            CustomBasicList<WeaponInfo> tempCol1;
            CustomBasicList<CharacterInfo> tempCol2;
            SKPoint newPoint;
            IProportionImage thisP = _mainGame.MainContainer.Resolve<IProportionImage>();
            foreach (var thisRoom in _mainGame.ThisGlobal.RoomList.Values)
            {
                x++;
                tempCol1 = _mainGame.ThisGlobal.WeaponsInRoom(x);
                if (thisRoom.Space == null)
                    return;
                _mainGame.ThisGlobal.ThisPos.ClearArea(thisRoom.Space);
                tempCol1.ForEach(thisWeapon =>
                {
                    SKSize weaponSize = thisWeapon.DefaultSize.GetSizeUsed(thisP.Proportion);
                    newPoint = _mainGame.ThisGlobal.ThisPos.GetPosition(thisRoom.Space, weaponSize.Width, weaponSize.Height);
                    WeaponCP otherWeapon = new WeaponCP();
                    otherWeapon.ActualHeight = weaponSize.Height;
                    otherWeapon.ActualWidth = weaponSize.Width;
                    otherWeapon.Weapon = thisWeapon.Weapon;
                    thisRoom.Space.PieceList.Add(otherWeapon);
                    _mainGame.ThisGlobal.ThisPos.AddRectToArea(thisRoom.Space, SKRect.Create(newPoint.X, newPoint.Y, (float)otherWeapon.ActualWidth, (float)otherWeapon.ActualHeight));
                    otherWeapon.Location = newPoint;
                    otherWeapon.NeedsToClear = false;
                    otherWeapon.DrawImage(canvas);
                });
                tempCol2 = _mainGame.ThisGlobal.CharactersInRoom(x);
                tempCol2.ForEach(thisCharacter =>
                {
                    PawnPiecesCP<EnumColorChoice> otherCharacter = new PawnPiecesCP<EnumColorChoice>();
                    otherCharacter.ActualHeight = SpaceSize();
                    otherCharacter.ActualWidth = SpaceSize();
                    newPoint = _mainGame.ThisGlobal.ThisPos.GetPosition(thisRoom.Space, (float)otherCharacter.ActualWidth, (float)otherCharacter.ActualHeight);
                    otherCharacter.MainColor = thisCharacter.MainColor;
                    thisRoom.Space.PieceList.Add(otherCharacter);
                    _mainGame.ThisGlobal.ThisPos.AddRectToArea(thisRoom.Space, SKRect.Create(newPoint.X, newPoint.Y, (float)otherCharacter.ActualWidth, (float)otherCharacter.ActualHeight));
                    otherCharacter.Location = newPoint;
                    otherCharacter.NeedsToClear = false;
                    otherCharacter.DrawImage(canvas);
                });
            }
            tempCol2 = _mainGame.ThisGlobal.CharactersOnStart();
            tempCol2.ForEach(thisCharacter =>
            {
                var otherCharacter = GetGamePiece(thisCharacter.MainColor, PositionForStartSpace(thisCharacter.FirstNumber));
                otherCharacter.DrawImage(canvas);
            });
            tempCol2 = _mainGame.ThisGlobal.CharactersOnBoard();
            tempCol2.ForEach(thisCharacter =>
            {
                var otherCharacter = GetGamePiece(thisCharacter.MainColor, PositionForBoardPiece(thisCharacter.Space));
                otherCharacter.DrawImage(canvas);
            });
        }
        private void DrawEnvelope(SKCanvas canvas, SKRect rect_Envelope)
        {
            var pn_Envelope = MiscHelpers.GetStrokePaint(SKColors.Black, rect_Envelope.Width / 30);
            SKPath gp_Temp = new SKPath();
            canvas.DrawRect(rect_Envelope, _whitePaint);
            canvas.DrawRect(rect_Envelope, pn_Envelope);
            canvas.DrawLine(rect_Envelope.Left, rect_Envelope.Top, System.Convert.ToInt32((rect_Envelope.Left + ((rect_Envelope.Width * 2) / 3))), System.Convert.ToInt32((rect_Envelope.Top + (rect_Envelope.Height / 2))), pn_Envelope);
            canvas.DrawLine(rect_Envelope.Left, rect_Envelope.Top + rect_Envelope.Height, System.Convert.ToInt32((rect_Envelope.Left + ((rect_Envelope.Width * 2) / 3))), System.Convert.ToInt32((rect_Envelope.Top + (rect_Envelope.Height / 2))), pn_Envelope);
            gp_Temp.AddLine(rect_Envelope.Left + rect_Envelope.Width, rect_Envelope.Top, System.Convert.ToInt32((rect_Envelope.Left + (rect_Envelope.Width / 3))), System.Convert.ToInt32((rect_Envelope.Top + (rect_Envelope.Height / 2))), true); // i think its true
            gp_Temp.AddLine(System.Convert.ToInt32((rect_Envelope.Left + (rect_Envelope.Width / 3))), System.Convert.ToInt32((rect_Envelope.Top + (rect_Envelope.Height / 2))), rect_Envelope.Left + rect_Envelope.Width, rect_Envelope.Top + rect_Envelope.Height);
            gp_Temp.Close();
            canvas.DrawPath(gp_Temp, _whitePaint);
            canvas.DrawPath(gp_Temp, pn_Envelope);
        }
        private void DrawDoor(SKCanvas canvas, Door obj_Door, float dbl_SquareWidth, float dbl_SquareHeight)
        {
            SKPoint pt_Temp;
            SKRect rect_Temp;
            pt_Temp = obj_Door.Point;
            switch (obj_Door.Direction)
            {
                case "Up":
                    {
                        rect_Temp = SKRect.Create((pt_Temp.X - 1) * (dbl_SquareWidth), ((pt_Temp.Y - 1) * (dbl_SquareHeight)) + (dbl_SquareHeight * 0.4f), dbl_SquareWidth, dbl_SquareHeight * 0.6f);
                        canvas.DrawRect(rect_Temp, _goldPaint);
                        canvas.DrawRect(rect_Temp, _borderPaint);
                        break;
                    }

                case "Down":
                    {
                        rect_Temp = SKRect.Create((pt_Temp.X - 1) * (dbl_SquareWidth), ((pt_Temp.Y - 1) * (dbl_SquareHeight)), dbl_SquareWidth, dbl_SquareHeight * 0.6f);
                        canvas.DrawRect(rect_Temp, _goldPaint);
                        canvas.DrawRect(rect_Temp, _borderPaint);
                        break;
                    }

                case "Left":
                    {
                        rect_Temp = SKRect.Create((pt_Temp.X - 1) * (dbl_SquareWidth) + (dbl_SquareWidth * 0.4f), ((pt_Temp.Y - 1) * (dbl_SquareHeight)), dbl_SquareWidth * 0.6f, dbl_SquareHeight);
                        canvas.DrawRect(rect_Temp, _goldPaint);
                        canvas.DrawRect(rect_Temp, _borderPaint);
                        break;
                    }

                case "Right":
                    {
                        rect_Temp = SKRect.Create((pt_Temp.X - 1) * (dbl_SquareWidth), ((pt_Temp.Y - 1) * (dbl_SquareHeight)), dbl_SquareWidth * 0.6f, dbl_SquareHeight);
                        canvas.DrawRect(rect_Temp, _goldPaint);
                        canvas.DrawRect(rect_Temp, _borderPaint);
                        break;
                    }
            }
        }
        private void PrivateDrawBoard(SKCanvas canvas)
        {
            var rect_Board = GetBounds(); // i think
            SKPoint pt_Temp;
            float dbl_SquareWidth;
            float dbl_SquareHeight;
            string str_Name;
            SKRect rect_Temp;
            SKRect rect_Text;
            SKRect rect_Room;
            int int_Col;
            int int_Row;
            int int_Count;
            SKPaint br_1;
            SKPaint br_2;
            dbl_SquareWidth = rect_Board.Width / 24;
            dbl_SquareHeight = rect_Board.Height / 25;
            float fontRoomSize = dbl_SquareHeight * 0.8f;
            rect_Temp = SKRect.Create(9 * dbl_SquareWidth, 8 * dbl_SquareHeight, 5 * dbl_SquareWidth, 7 * dbl_SquareHeight);
            canvas.DrawRect(rect_Temp, _whitePaint);
            rect_Temp = SKRect.Create(9 * dbl_SquareWidth, 11.5f * dbl_SquareHeight, 5 * dbl_SquareWidth, 7 * dbl_SquareHeight);
            canvas.DrawRect(rect_Temp, _darkGreenPaint);
            rect_Temp = SKRect.Create(10 * dbl_SquareWidth, 9 * dbl_SquareHeight, 3 * dbl_SquareWidth, 5 * dbl_SquareHeight);
            DrawEnvelope(canvas, rect_Temp);
            // *** Draw then rooms
            foreach (var obj_Room in arr_Rooms!)
            {
                rect_Room = SKRect.Create((obj_Room.PieceArea.Left - 1) * dbl_SquareWidth, ((obj_Room.PieceArea.Top - 1) * dbl_SquareHeight), dbl_SquareWidth * obj_Room.PieceArea.Width, dbl_SquareHeight * obj_Room.PieceArea.Height);
                if (obj_Room.FloorColor1 == obj_Room.FloorColor2)
                {
                    var thisPaint = MiscHelpers.GetSolidPaint(obj_Room.FloorColor1);
                    canvas.DrawRect(rect_Room, thisPaint); // i don't think i need the borders that are the same anyways.
                    foreach (var tempRect in obj_Room.MiscList)
                    {
                        rect_Room = SKRect.Create((tempRect.Left - 1) * dbl_SquareWidth, ((tempRect.Top - 1) * dbl_SquareHeight), dbl_SquareWidth * tempRect.Width, dbl_SquareHeight * tempRect.Height);
                        canvas.DrawRect(rect_Room, thisPaint); // i don't think i need the borders that are the same anyways.
                    }
                    br_1 = _whitePaint!;
                    br_2 = thisPaint;
                }
                else
                {
                    br_1 = MiscHelpers.GetSolidPaint(obj_Room.FloorColor1);
                    br_2 = MiscHelpers.GetSolidPaint(obj_Room.FloorColor2);
                    var loopTo = obj_Room.PieceArea.Top + obj_Room.PieceArea.Height - 1;
                    for (int_Row = (int)obj_Room.PieceArea.Top; int_Row <= loopTo; int_Row++)
                    {
                        var loopTo1 = obj_Room.PieceArea.Left + obj_Room.PieceArea.Width - 1;
                        for (int_Col = (int)obj_Room.PieceArea.Left; int_Col <= loopTo1; int_Col++)
                        {
                            pt_Temp = new SKPoint(int_Col, int_Row);
                            rect_Temp = SKRect.Create((pt_Temp.X - 1) * dbl_SquareWidth, ((pt_Temp.Y - 1) * dbl_SquareHeight), dbl_SquareWidth, dbl_SquareHeight);
                            if (((int_Row + int_Col) % 2) == 0)
                                canvas.DrawRect(rect_Temp, br_1);
                            else
                                canvas.DrawRect(rect_Temp, br_2);
                        }
                    }
                    foreach (var tempRect in obj_Room.MiscList)
                    {
                        var loopTo2 = tempRect.Top + tempRect.Height - 1;
                        for (int_Row = (int)tempRect.Top; int_Row <= loopTo2; int_Row++)
                        {
                            var loopTo3 = tempRect.Left + tempRect.Width - 1;
                            for (int_Col = (int)tempRect.Left; int_Col <= loopTo3; int_Col++)
                            {
                                pt_Temp = new SKPoint(int_Col, int_Row);
                                rect_Temp = SKRect.Create((pt_Temp.X - 1) * dbl_SquareWidth, ((pt_Temp.Y - 1) * dbl_SquareHeight), dbl_SquareWidth, dbl_SquareHeight);
                                if (((int_Row + int_Col) % 2) == 0)
                                    canvas.DrawRect(rect_Temp, br_1);
                                else
                                    canvas.DrawRect(rect_Temp, br_2);
                            }
                        }
                    }
                }
                rect_Room = SKRect.Create((obj_Room.PieceArea.Left - 1) * dbl_SquareWidth, ((obj_Room.PieceArea.Top - 1) * dbl_SquareHeight), dbl_SquareWidth * obj_Room.PieceArea.Width, dbl_SquareHeight * obj_Room.PieceArea.Height);
                var textPaint = MiscHelpers.GetTextPaint(obj_Room.FloorColor2, fontRoomSize);
                textPaint.FakeBoldText = true;
                var newRect = obj_Room.Name.GetTextRectangle(textPaint);
                var tmp_Size = newRect.Size;
                tmp_Size = new SKSize(tmp_Size.Width * 1.2f, tmp_Size.Height * 2);
                if (obj_Room.TextPoint.X == 0)
                    rect_Text = SKRect.Create((rect_Room.Left + (rect_Room.Width / 2)) - (tmp_Size.Width / 2), (rect_Room.Top + (rect_Room.Height / 2)) - (tmp_Size.Height / 2), tmp_Size.Width, tmp_Size.Height);
                else
                    rect_Text = SKRect.Create((obj_Room.TextPoint.X - 1) * dbl_SquareWidth, (obj_Room.TextPoint.Y - 1) * dbl_SquareHeight, tmp_Size.Width, tmp_Size.Height);
                var loopTo4 = obj_Room.Doors.Count - 1;
                // *** Draw the doors
                for (int_Count = 0; int_Count <= loopTo4; int_Count++)
                    DrawDoor(canvas, obj_Room.Doors[int_Count], dbl_SquareWidth, dbl_SquareHeight);
                canvas.DrawOval(rect_Text, br_1);
                canvas.DrawCustomText(obj_Room.Name, TextExtensions.EnumLayoutOptions.Center, TextExtensions.EnumLayoutOptions.Center, textPaint, rect_Text, out _);
            }
            // *** Draw the squares
            for (int_Row = 1; int_Row <= 25; int_Row++)
            {
                for (int_Col = 1; int_Col <= 24; int_Col++)
                {
                    pt_Temp = new SKPoint(int_Col, int_Row);

                    if ((arr_Squares!.Contains(pt_Temp)))
                    {
                        str_Name = arr_Squares[pt_Temp].ToString();

                        rect_Temp = SKRect.Create((pt_Temp.X - 1) * dbl_SquareWidth, ((pt_Temp.Y - 1) * dbl_SquareHeight), dbl_SquareWidth, dbl_SquareHeight);

                        if (str_Name.Contains("b"))
                            canvas.DrawRect(rect_Temp, _orangePaint);
                        else
                            canvas.DrawRect(rect_Temp, _peachPuffPaint);
                        canvas.DrawRect(rect_Temp, _borderPaint);
                    }
                }
            }
        }
    }
}