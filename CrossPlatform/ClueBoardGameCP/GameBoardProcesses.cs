using BasicGameFramework.Attributes;
using BasicGameFramework.BasicEventModels;
using BasicGameFramework.GameGraphicsCP.GameboardPositionHelpers;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.Messenging;
using System;
using System.Collections.Generic;
using System.Linq;
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;
using cs = CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.SColorString;
namespace ClueBoardGameCP
{
    [SingletonGame]
    public class GameBoardProcesses
    {
        private readonly GameBoardGraphicsCP _imageBoard;
        private readonly ClueBoardGameMainGameClass _mainGame;
        private readonly GlobalClass _thisGlobal;
        private readonly EventAggregator _thisE;
        public Dictionary<int, FieldInfo> FieldList = new Dictionary<int, FieldInfo>();
        public PredictionInfo Solution
        {
            get
            {
                return _mainGame.SaveRoot!.Solution;
            }
            set
            {
                _mainGame.SaveRoot!.Solution = value;
            }
        }
        public PositionPieces ThisPos
        {
            get
            {
                return _thisGlobal.ThisPos;
            }
            set
            {
                _thisGlobal.ThisPos = value;
            }
        }
        public Dictionary<int, MoveInfo> PreviousMoves //i think i can save as dictionary now.
        {
            get
            {
                return _mainGame.SaveRoot!.PreviousMoves;
            }
            set
            {
                _mainGame.SaveRoot!.PreviousMoves = value;
            }
        }
        public GameBoardProcesses(ClueBoardGameMainGameClass mainGame, GameBoardGraphicsCP imageBoard, GlobalClass thisGlobal, EventAggregator thisE)
        {
            _mainGame = mainGame;
            _imageBoard = imageBoard;
            _thisGlobal = thisGlobal;
            _thisE = thisE;
        }
        public void LoadSavedGame()
        {
            RepaintBoard();
            NewTurn();
        }
        public void RepaintBoard()
        {
            _thisE.RepaintBoard();
        }
        public void NewTurn()
        {
            _thisE.Publish(new NewTurnEventModel());
        }
        private bool CharacterOnSpace(int space)
        {
            return _thisGlobal.CharacterList.Values.Any(items => items.Space == space);
        }
        private int FindSpace(int whatNumber)
        {
            switch (whatNumber)
            {
                case 1:
                    {
                        return 41;
                    }

                case 2:
                    {
                        return 53;
                    }

                case 3:
                    {
                        return 172;
                    }

                case 4:
                    {
                        return 171;
                    }

                case 5:
                    {
                        return 149;
                    }

                case 6:
                    {
                        return 15;
                    }
            }
            throw new BasicBlankException("Must be between 1 and 6");
        }
        public void LoadCharacters(CustomBasicList<EnumNameList> newCol)
        {
            int x;
            _thisGlobal.CharacterList.Clear();
            int newSpace;
            for (x = 1; x <= newCol.Count; x++)
            {
                newSpace = FindSpace(x);
                var thisCharacter = new CharacterInfo();
                thisCharacter.Piece = newCol[x - 1];
                thisCharacter.MainColor = thisCharacter.Piece switch
                {
                    EnumNameList.Peacock => cs.Aqua,
                    EnumNameList.Green => cs.Green,
                    EnumNameList.Plum => cs.Purple,
                    EnumNameList.Scarlet => cs.Red,
                    EnumNameList.White => cs.White,
                    EnumNameList.Mustard => cs.Yellow,
                    _ => throw new BasicBlankException("No color found when loading character list"),
                };
                thisCharacter.FirstSpace = newSpace;
                thisCharacter.FirstNumber = x;
                _thisGlobal.CharacterList.Add(thisCharacter);
            }
        }
        public void LoadCharacters()
        {
            CustomBasicList<int> output = GetIntegerList(1, 6);
            output.ShuffleList();
            LoadCharacters(output);
        }
        public void LoadCharacters(CustomBasicList<int> newCol)
        {
            var output = new CustomBasicList<EnumNameList>();
            newCol.ForEach(thisItem => output.Add(thisItem.ToEnum<EnumNameList>()));
            LoadCharacters(output);
        }
        public void LoadColorsForCharacters()
        {
            _mainGame.PlayerList!.ForEach(thisPlayer => _thisGlobal.CharacterList.PlayerChoseColor(thisPlayer));
        }
        public void PlaceWeapons()
        {
            _thisGlobal.WeaponList.PlaceWeaponsInRooms();
            if (_thisGlobal.WeaponList.Values.Any(items => items.Room == 0))
                throw new BasicBlankException("Failed to place weapons");
            RepaintBoard();
        }
        public void ClearGame(bool alsoLoadCharacters = true)
        {
            foreach (var thisCharacter in _thisGlobal.CharacterList.Values)
            {
                thisCharacter.ComputerData.CluesGiven.Clear();
                thisCharacter.ComputerData.CluesReceived.Clear();
                thisCharacter.CurrentRoom = 0;
                thisCharacter.PreviousRoom = 0;
                thisCharacter.Space = 0;
            }
            if (alsoLoadCharacters)
            {
                LoadCharacters();
                _thisGlobal.CurrentCharacter = _thisGlobal.CharacterList[1];
            }
            PlaceWeapons();
        }
        public void ResetMoves()
        {
            PreviousMoves = new Dictionary<int, MoveInfo>();
            var thisMove = new MoveInfo();
            if (_thisGlobal.CurrentCharacter!.Space > 0 || _thisGlobal.CurrentCharacter.CurrentRoom > 0)
            {
                if (_thisGlobal.CurrentCharacter.Space > 0)
                    thisMove.SpaceNumber = _thisGlobal.CurrentCharacter.Space;
                else
                    thisMove.RoomNumber = _thisGlobal.CurrentCharacter.CurrentRoom;
            }
            PreviousMoves.Add(thisMove);
        }
        public void ChooseScene(PredictionInfo thisScene)
        {
            Solution = thisScene;
        }
        public void ChooseScene()
        {
            Solution = new PredictionInfo();
            WeaponInfo thisWeapon = _thisGlobal.WeaponList.GetRandomItem();
            if (thisWeapon.Name == "")
                throw new BasicBlankException("Weapon must have name to be part of solution");
            Solution.WeaponName = thisWeapon.Name;
            var thisCharacter = _thisGlobal.CharacterList.GetRandomItem();
            Solution.CharacterName = thisCharacter.Name;
            var thisRoom = _thisGlobal.RoomList.GetRandomItem();
            Solution.RoomName = thisRoom.Name;
        }
        public void ComputerMoveToSpace(int space)
        {
            _thisGlobal.CurrentCharacter!.Space = space;
            if (_thisGlobal.CurrentCharacter.Space > 0)
            {
                return;
            }
            _thisGlobal.CurrentCharacter.PreviousRoom = 0;
            _thisGlobal.CurrentCharacter.CurrentRoom = 0;
        }
        public void ComputerMoveToRoom(int room)
        {
            _thisGlobal.CurrentCharacter!.Space = 0;
            _thisGlobal.CurrentCharacter.PreviousRoom = room;
            _thisGlobal.CurrentCharacter.CurrentRoom = room;
        }
        public CustomBasicList<FieldInfo> GetBlockList()
        {
            CustomBasicList<FieldInfo> output = new CustomBasicList<FieldInfo>();
            int x = 0;
            foreach (var thisField in FieldList.Values)
            {
                x++;
                if (CharacterOnSpace(x))
                    output.Add(thisField);
            }
            return output;
        }
        public bool CardPartOfSolution(CardInfo thisCard)
        {
            if (thisCard.Name == Solution.WeaponName)
                return true;
            if (thisCard.Name == Solution.RoomName)
                return true;
            if (thisCard.Name == Solution.CharacterName)
                return true;
            return false;
        }
        public CustomBasicList<MoveInfo> GetPossibleMoveList()
        {
            CustomBasicList<MoveInfo> output = new CustomBasicList<MoveInfo>();
            MoveInfo thisMove;
            if (_thisGlobal.CurrentCharacter!.Space > 0)
            {
                var thisField = FieldList[_thisGlobal.CurrentCharacter.Space];
                foreach (var tempMove in thisField.Neighbors.Values)
                {
                    if (tempMove.SpaceNumber > 0)
                    {
                        if (CanMoveToSpace(tempMove.SpaceNumber))
                            output.Add(tempMove);
                        else
                        {
                            if (CanMoveToRoom(tempMove.RoomNumber))
                                output.Add(tempMove);
                        }
                    }
                }
                return output;
            }
            if (_thisGlobal.CurrentCharacter.CurrentRoom == 0 && _thisGlobal.CurrentCharacter.Space == 0)
            {
                if (CanMoveToRoom(_thisGlobal.CurrentCharacter.FirstSpace))
                {
                    thisMove = new MoveInfo();
                    thisMove.SpaceNumber = _thisGlobal.CurrentCharacter.FirstSpace;
                    output.Add(thisMove);
                }
                return output;
            }
            int count = _thisGlobal.RoomList.Count;
            count.Times(y =>
            {
                if (CanMoveToRoom(y))
                {
                    thisMove = new MoveInfo();
                    thisMove.RoomNumber = y;
                    output.Add(thisMove);
                }
            });
            var thisRoom = _thisGlobal.RoomList[_thisGlobal.CurrentCharacter.CurrentRoom];
            count = thisRoom.DoorList.Count;
            count.Times(y =>
            {
                if (CanMoveToSpace(thisRoom.DoorList[y - 1]))
                {
                    thisMove = new MoveInfo();
                    thisMove.SpaceNumber = thisRoom.DoorList[y - 1];
                    output.Add(thisMove);
                }
            });
            return output;
        }
        public bool HasValidMoves()
        {
            if (_thisGlobal.CurrentCharacter!.Space > 0)
            {
                var thisField = FieldList[_thisGlobal.CurrentCharacter.Space];
                foreach (var tempMove in thisField.Neighbors.Values)
                {
                    if (tempMove.SpaceNumber > 0)
                    {
                        if (CanMoveToSpace(tempMove.SpaceNumber))
                            return true;
                    }
                    else
                    {
                        if (CanMoveToRoom(tempMove.RoomNumber))
                            return true;
                    }
                }
                return false;
            }
            if (_thisGlobal.CurrentCharacter.CurrentRoom == 0 && _thisGlobal.CurrentCharacter.Space == 0)
            {
                if (CanMoveToRoom(_thisGlobal.CurrentCharacter.FirstSpace))
                {
                    return true;
                }
                return false;
            }
            for (int x = 1; x <= _thisGlobal.RoomList.Count; x++)
            {
                if (CanMoveToRoom(x))
                    return true;
            }
            var thisRoom = _thisGlobal.RoomList[_thisGlobal.CurrentCharacter.CurrentRoom];
            for (int x = 1; x <= thisRoom.DoorList.Count; x++)
            {
                if (CanMoveToSpace(thisRoom.DoorList[x - 1]))
                    return true;
            }
            return false;
        }
        public bool CanMoveToSpace(int space)
        {
            if (space == _thisGlobal.CurrentCharacter!.Space)
                return false;
            if (PreviousMoves.Count > 0)
            {
                foreach (var thisMove in PreviousMoves.Values)
                {
                    if (thisMove.SpaceNumber == space)
                        return false;
                }
            }
            if (_thisGlobal.CurrentCharacter.CurrentRoom == 0 && _thisGlobal.CurrentCharacter.Space == 0 && _thisGlobal.CurrentCharacter.PreviousRoom == 0)
            {
                if (space == _thisGlobal.CurrentCharacter.FirstSpace)
                {
                    if (CharacterOnSpace(space) == false)
                        return true;
                }
                return false;
            }
            if (_thisGlobal.CurrentCharacter.CurrentRoom == 0 && _thisGlobal.CurrentCharacter.PreviousRoom == 0)
            {
                var thisField = FieldList[_thisGlobal.CurrentCharacter.Space];
                foreach (var thisMove in thisField.Neighbors.Values)
                {
                    if (thisMove.SpaceNumber == space)
                    {
                        return CharacterOnSpace(space) == false;
                    }
                }
                return false;
            }
            var thisRoom = _thisGlobal.RoomList[_thisGlobal.CurrentCharacter.CurrentRoom];
            for (int x = 1; x <= thisRoom.DoorList.Count; x++)
            {
                if (thisRoom.DoorList[x - 1] == space)
                {
                    return CharacterOnSpace(space) == false;
                }
            }
            return false;
        }
        public bool CanMoveToRoom(int room)
        {
            if (_mainGame.ThisTest!.AllowAnyMove)
                return true; //for testing.
            if (_thisGlobal.CurrentCharacter!.PreviousRoom == room)
                return false;
            if (PreviousMoves.Count > 0)
            {
                foreach (var thisMove in PreviousMoves.Values)
                {
                    if (thisMove.RoomNumber == room)
                        return false;
                }
            }
            var thisRoom = _thisGlobal.RoomList[room];
            if (thisRoom.RoomPassage > 0)
            {
                if (_thisGlobal.CurrentCharacter.CurrentRoom == thisRoom.RoomPassage)
                    return true;
            }
            for (int x = 1; x <= thisRoom.DoorList.Count; x++)
            {
                if (thisRoom.DoorList[x - 1] == _thisGlobal.CurrentCharacter.Space)
                {
                    return true;
                }
            }
            return false;
        }
        public void MoveToSpace(int space)
        {
            MoveInfo thisMove;
            thisMove = new MoveInfo();
            thisMove.SpaceNumber = space;
            PreviousMoves.Add(thisMove);
            int previousSpace = _thisGlobal.CurrentCharacter!.Space;
            _thisGlobal.CurrentCharacter.Space = space;
            if (_thisGlobal.CurrentCharacter.CurrentRoom == 0 && _thisGlobal.CurrentCharacter.Space == 0 && _thisGlobal.CurrentCharacter.PreviousRoom == 0)
            {
                RepaintBoard();
                return;
            }
            if (previousSpace > 0)
            {
                RepaintBoard();
                return;
            }
            int currentRoom = _thisGlobal.CurrentCharacter.CurrentRoom;
            thisMove = new MoveInfo();
            thisMove.RoomNumber = currentRoom;
            PreviousMoves.Add(thisMove);
            _thisGlobal.CurrentCharacter.PreviousRoom = 0;
            _thisGlobal.CurrentCharacter.CurrentRoom = 0;
            RepaintBoard();
        }
        public void MoveToRoom(int room)
        {
            _mainGame.SaveRoot!.MovesLeft = 0;
            PreviousMoves = new Dictionary<int, MoveInfo>();
            _thisGlobal.CurrentCharacter!.Space = 0;
            _thisGlobal.CurrentCharacter.CurrentRoom = room;
            _thisGlobal.CurrentCharacter.PreviousRoom = room;
            RepaintBoard();
        }
        #region "Loading Board Processes"
        private void PopulateRoom(CustomBasicList<int> doorList, int roomNumber, int newRoom = 0)
        {
            RoomInfo thisRoom = _thisGlobal.RoomList[roomNumber];
            thisRoom.RoomPassage = newRoom;
            thisRoom.DoorList = doorList.ToCustomBasicList();
        }
        private void PutPosition(FieldInfo thisField, int spaceNumber, EnumPositionInfo whatPosition)
        {
            MoveInfo thisNeighbor = new MoveInfo();
            thisNeighbor.Position = whatPosition;
            if (spaceNumber > 0)
                thisNeighbor.SpaceNumber = spaceNumber;
            else
                thisNeighbor.RoomNumber = Math.Abs(spaceNumber);
            thisField.Neighbors.Add(thisNeighbor);
        }
        private void SetNeighborsFor(int fieldNumber, int topField, int bottomField, int leftField, int rightField) //name is not used for room now.
        {
            var thisField = FieldList[fieldNumber];
            if (topField != 0)
                PutPosition(thisField, topField, EnumPositionInfo.Top);
            if (bottomField != 0)
                PutPosition(thisField, bottomField, EnumPositionInfo.Bottom);
            if (leftField != 0)
                PutPosition(thisField, leftField, EnumPositionInfo.Left);
            if (rightField != 0)
                PutPosition(thisField, rightField, EnumPositionInfo.Right);
        }
        private bool _spacesLoaded = false;
        public void LoadSpacesInRoom()
        {
            if (_spacesLoaded)
                return;
            int x;
            for (x = 1; x <= 9; x++)
            {
                var ThisRoom = _thisGlobal.RoomList[x];
                ThisRoom.Space = _imageBoard.RoomSpaceInfo(x);
                foreach (var ThisRect in ThisRoom.Space.ObjectList)
                {
                    try
                    {
                        ThisPos.AddRectToArea(ThisRoom.Space, ThisRect); // try this way for now.
                    }
                    catch (Exception) //try to ignore exceptions.
                    {
                    }
                }
            }
            _spacesLoaded = true;
        }
        public void LoadBoard()
        {
            180.Times(x =>
            {
                FieldList.Add(new FieldInfo());
            });
            9.Times(x =>
            {
                RoomInfo thisRoom = new RoomInfo();
                thisRoom.Name = _imageBoard.RoomName(x);
                _thisGlobal.RoomList.Add(thisRoom);
            });
            SetNeighborsFor(1, 0, 2, 3, 0);
            SetNeighborsFor(2, 0, 0, 4, 1);
            SetNeighborsFor(3, 1, 4, 6, 0);
            SetNeighborsFor(4, 2, 0, 5, 3);
            SetNeighborsFor(5, 4, 0, 7, 6);
            SetNeighborsFor(6, 3, 5, 8, 0);
            SetNeighborsFor(7, 5, -7, 22, 8); //hall
            SetNeighborsFor(8, 6, 7, 21, 9);
            SetNeighborsFor(9, -6, 8, 20, 10); //study
            SetNeighborsFor(10, 0, 9, 19, 11);
            SetNeighborsFor(11, 0, 10, 18, 12);
            SetNeighborsFor(12, 0, 11, 17, 13);
            SetNeighborsFor(13, 0, 12, 16, 14);
            SetNeighborsFor(14, 0, 13, 15, 0);
            SetNeighborsFor(15, 14, 16, 0, 0);
            SetNeighborsFor(16, 13, 17, 0, 15);
            SetNeighborsFor(17, 12, 18, 0, 16);
            SetNeighborsFor(18, 11, 19, 0, 17);
            SetNeighborsFor(19, 10, 20, 0, 18);
            SetNeighborsFor(20, 9, 21, 25, 19);
            SetNeighborsFor(21, 8, 22, 24, 20);
            SetNeighborsFor(22, 7, 0, 23, 21);
            SetNeighborsFor(23, 22, 0, 27, 24);
            SetNeighborsFor(24, 21, 23, 26, 25);
            SetNeighborsFor(25, 20, 24, 0, 0);
            SetNeighborsFor(26, 24, 27, 69, 0);
            SetNeighborsFor(27, 23, 28, 70, 26);
            SetNeighborsFor(28, 0, 29, 0, 27);
            SetNeighborsFor(29, 0, 30, 0, 28);
            SetNeighborsFor(30, 0, 31, -7, 29);//, "Hall");
            SetNeighborsFor(31, 0, 32, -7, 30);//, "Hall");
            SetNeighborsFor(32, 0, 33, 0, 31);
            SetNeighborsFor(33, 0, 34, 68, 32);
            SetNeighborsFor(34, 35, 59, 67, 33);
            SetNeighborsFor(35, 36, 46, 34, 0);
            SetNeighborsFor(36, 37, 45, 35, 0);
            SetNeighborsFor(37, 38, 44, 36, 0);
            SetNeighborsFor(38, 39, 43, 37, 0);
            SetNeighborsFor(39, 40, 42, 38, 0);
            SetNeighborsFor(40, 0, 41, 39, 0);
            SetNeighborsFor(41, 0, 0, 42, 40);
            SetNeighborsFor(42, 41, 0, 43, 39);
            SetNeighborsFor(43, 42, 0, 44, 38);
            SetNeighborsFor(44, 43, 0, 45, 37);
            SetNeighborsFor(45, 44, 0, 46, 36);
            SetNeighborsFor(46, 45, 47, 59, 35);
            SetNeighborsFor(47, -8, 48, 58, 46);//, "Lounge");
            SetNeighborsFor(48, 0, 49, 57, 47);
            SetNeighborsFor(49, 0, 50, 56, 48);
            SetNeighborsFor(50, 0, 51, 55, 49);
            SetNeighborsFor(51, 0, 52, 54, 50);
            SetNeighborsFor(52, 0, 0, 53, 51);
            SetNeighborsFor(53, 52, 0, 60, 54);
            SetNeighborsFor(54, 51, 53, 61, 55);
            SetNeighborsFor(55, 50, 54, 62, 56);
            SetNeighborsFor(56, 49, 55, 63, 57);
            SetNeighborsFor(57, 48, 56, 64, 58);
            SetNeighborsFor(58, 47, 57, 65, 59);
            SetNeighborsFor(59, 46, 58, 66, 34);
            SetNeighborsFor(60, 53, 0, 0, 61);
            SetNeighborsFor(61, 54, 60, 0, 62);
            SetNeighborsFor(62, 55, 61, 0, 63);
            SetNeighborsFor(63, 56, 62, 0, 64);
            SetNeighborsFor(64, 57, 63, 0, 65);
            SetNeighborsFor(65, 58, 64, -9, 66);//, "Dining Room");
            SetNeighborsFor(66, 59, 65, 0, 67);
            SetNeighborsFor(67, 34, 66, 85, 68);
            SetNeighborsFor(68, 33, 67, 84, 0);
            SetNeighborsFor(69, 26, 70, 72, -5);//, "Library");
            SetNeighborsFor(70, 27, 0, 71, 69);
            SetNeighborsFor(71, 70, 0, 75, 72);
            SetNeighborsFor(72, 69, 71, 74, 0);
            SetNeighborsFor(73, 0, 74, 81, 0);
            SetNeighborsFor(74, 72, 75, 82, 73);
            SetNeighborsFor(75, 71, 0, 83, 74);
            SetNeighborsFor(76, 0, 77, -4, 0);//, "Billiard Room");
            SetNeighborsFor(77, 0, 78, 0, 76);
            SetNeighborsFor(78, -5, 79, 0, 77);//, "Library");
            SetNeighborsFor(79, 0, 80, 0, 78);
            SetNeighborsFor(80, 0, 81, 0, 79);
            SetNeighborsFor(81, 73, 82, 90, 80);
            SetNeighborsFor(82, 74, 83, 91, 81);
            SetNeighborsFor(83, 75, 0, 92, 82);
            SetNeighborsFor(84, 68, 85, 86, 0);
            SetNeighborsFor(85, 67, 0, 87, 84);
            SetNeighborsFor(86, 84, 87, 88, 0);
            SetNeighborsFor(87, 85, 0, 89, 86);
            SetNeighborsFor(88, 86, 89, 99, 0);
            SetNeighborsFor(89, 87, 0, 100, 88);
            SetNeighborsFor(90, 81, 91, 93, 0);
            SetNeighborsFor(91, 82, 92, 94, 90);
            SetNeighborsFor(92, 83, 0, 95, 91);
            SetNeighborsFor(93, 90, 94, 96, 0);
            SetNeighborsFor(94, 91, 95, 97, 93);
            SetNeighborsFor(95, 92, 0, 98, 94);
            SetNeighborsFor(96, 93, 97, 105, 0);
            SetNeighborsFor(97, 94, 98, 106, 96);
            SetNeighborsFor(98, 95, 0, 107, 97);
            SetNeighborsFor(99, 88, 100, 101, 0);
            SetNeighborsFor(100, 89, -9, 102, 99);//, "Dining Room");
            SetNeighborsFor(101, 99, 102, 103, 0);
            SetNeighborsFor(102, 100, 0, 104, 101);
            SetNeighborsFor(103, 101, 104, 113, 0);
            SetNeighborsFor(104, 102, 0, 114, 103);
            SetNeighborsFor(105, 96, 106, 118, -4);//, "Billiard Room");
            SetNeighborsFor(106, 97, 107, 119, 105);
            SetNeighborsFor(107, 98, 108, 120, 106);
            SetNeighborsFor(108, 0, 109, 121, 107);
            SetNeighborsFor(109, 0, 110, 122, 108);
            SetNeighborsFor(110, 0, 111, 123, 109);
            SetNeighborsFor(111, 0, 112, 124, 110);
            SetNeighborsFor(112, 0, 113, 125, 111);
            SetNeighborsFor(113, 103, 114, 126, 112);
            SetNeighborsFor(114, 104, 115, 127, 113);
            SetNeighborsFor(115, 0, 116, 128, 114);
            SetNeighborsFor(116, 0, 117, 129, 115);
            SetNeighborsFor(117, 0, 0, 130, 116);
            SetNeighborsFor(118, 105, 119, 140, 0);
            SetNeighborsFor(119, 106, 120, 141, 118);
            SetNeighborsFor(120, 107, 121, 0, 119);
            SetNeighborsFor(121, 108, 122, -2, 120);//, "Ballroom");
            SetNeighborsFor(122, 109, 123, 0, 121);
            SetNeighborsFor(123, 110, 124, 0, 122);
            SetNeighborsFor(124, 111, 125, 0, 123);
            SetNeighborsFor(125, 112, 126, -2, 124);//, "Ballroom");
            SetNeighborsFor(126, 113, 127, 0, 125);
            SetNeighborsFor(127, 114, 128, 0, 126);
            SetNeighborsFor(128, 115, 129, 142, 127);
            SetNeighborsFor(129, 116, 130, 143, 128);
            SetNeighborsFor(130, 117, 131, 144, 129);
            SetNeighborsFor(131, 0, 132, 145, 130);
            SetNeighborsFor(132, 0, 133, 146, 131);
            SetNeighborsFor(133, 0, 134, 147, 132);
            SetNeighborsFor(134, 0, 0, 148, 133);
            SetNeighborsFor(135, 0, 136, 149, 0);
            SetNeighborsFor(136, 0, 137, 150, 135);
            SetNeighborsFor(137, 0, 138, 151, 136);
            SetNeighborsFor(138, 0, 139, 152, 137);
            SetNeighborsFor(139, 0, 140, 153, 138);
            SetNeighborsFor(140, 118, 141, 154, 139);
            SetNeighborsFor(141, 119, 0, 155, 140);
            SetNeighborsFor(142, 128, 143, 156, 0);
            SetNeighborsFor(143, 129, 144, 157, 142);
            SetNeighborsFor(144, 130, 145, 0, 143);
            SetNeighborsFor(145, 131, 146, -1, 144);//, "Kitchen");
            SetNeighborsFor(146, 132, 147, 0, 145);
            SetNeighborsFor(147, 133, 148, 0, 146);
            SetNeighborsFor(148, 134, 0, 0, 147);
            SetNeighborsFor(149, 135, 150, 0, 0);
            SetNeighborsFor(150, 136, 151, 0, 149);
            SetNeighborsFor(151, 137, 152, 0, 150);
            SetNeighborsFor(152, 138, 153, 0, 151);
            SetNeighborsFor(153, 139, 154, 158, 152);
            SetNeighborsFor(154, 140, 155, 159, 153);
            SetNeighborsFor(155, 141, 0, 160, 154);
            SetNeighborsFor(156, 142, 157, 161, 0);
            SetNeighborsFor(157, 143, 0, 162, 156);
            SetNeighborsFor(158, 153, 159, 0, -3);//, "Conservatory");
            SetNeighborsFor(159, 154, 160, 163, 158);
            SetNeighborsFor(160, 155, -2, 164, 159);//, "Ballroom");
            SetNeighborsFor(161, 156, 162, 179, -2);//, "Ballroom");
            SetNeighborsFor(162, 157, 0, 180, 161);
            SetNeighborsFor(163, 159, 164, 165, 0);
            SetNeighborsFor(164, 160, 0, 166, 163);
            SetNeighborsFor(165, 163, 166, 167, 0);
            SetNeighborsFor(166, 164, 0, 168, 165);
            SetNeighborsFor(167, 165, 168, 0, 0);
            SetNeighborsFor(168, 166, 0, 170, 167);
            SetNeighborsFor(169, 167, 170, 0, 0);
            SetNeighborsFor(170, 168, 171, 0, 169);
            SetNeighborsFor(171, 0, 0, 0, 170);
            SetNeighborsFor(172, 0, 173, 0, 0);
            SetNeighborsFor(173, 0, 174, 0, 172);
            SetNeighborsFor(174, 175, 0, 0, 173);
            SetNeighborsFor(175, 178, 176, 174, 0);
            SetNeighborsFor(176, 177, 0, 0, 175);
            SetNeighborsFor(177, 180, 0, 176, 178);
            SetNeighborsFor(178, 179, 177, 175, 0);
            SetNeighborsFor(179, 161, 180, 178, 0);
            SetNeighborsFor(180, 162, 0, 177, 179);
            CustomBasicList<int> NewCol = new CustomBasicList<int>
            {
                145
            };
            PopulateRoom(NewCol, 1, 6);
            NewCol = new CustomBasicList<int>
            {
                161,
                125,
                121,
                160
            };
            PopulateRoom(NewCol, 2);
            NewCol = new CustomBasicList<int>
            {
                158
            };
            PopulateRoom(NewCol, 3, 8);
            NewCol = new CustomBasicList<int>
            {
                105,
                76
            };
            PopulateRoom(NewCol, 4);
            NewCol = new CustomBasicList<int>
            {
                78,
                69
            };
            PopulateRoom(NewCol, 5);
            NewCol = new CustomBasicList<int>
            {
                9
            };
            PopulateRoom(NewCol, 6, 1);
            NewCol = new CustomBasicList<int>
            {
                7,
                30,
                31
            };
            PopulateRoom(NewCol, 7);
            NewCol = new CustomBasicList<int>
            {
                47
            };
            PopulateRoom(NewCol, 8, 3);
            NewCol = new CustomBasicList<int>
            {
                65,
                100
            };
            PopulateRoom(NewCol, 9);
        }
        #endregion
    }
}