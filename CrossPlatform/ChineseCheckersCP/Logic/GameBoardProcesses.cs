using System;
using System.Text;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using System.Linq;
using CommonBasicStandardLibraries.BasicDataSettingsAndProcesses;
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;
using CommonBasicStandardLibraries.CollectionClasses;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using fs = CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.JsonSerializers.FileHelpers;
using js = CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.JsonSerializers.NewtonJsonStrings; //just in case i need those 2.
using BasicGameFrameworkLibrary.Attributes;
using ChineseCheckersCP.Data;
using System.Collections.Generic;
using BasicGameFrameworkLibrary.BasicEventModels;
using BasicGameFrameworkLibrary.MultiplayerClasses.BasicPlayerClasses;
//i think this is the most common things i like to do
namespace ChineseCheckersCP.Logic
{
    [SingletonGame]
    [AutoReset]
    public class GameBoardProcesses
    {
        private CustomBasicList<MoveInfo> PreviousMoves
        {
            get
            {
                return _gameContainer.SaveRoot!.PreviousMoves;
            }
            set
            {
                _gameContainer.SaveRoot!.PreviousMoves = value;
            }
        }
        private int WhoTurn
        {
            get
            {
                return _gameContainer.SaveRoot!.PlayOrder.WhoTurn;
            }
            set
            {
                _gameContainer.SaveRoot!.PlayOrder.WhoTurn = value;
            }
        }
        readonly ChineseCheckersGameContainer _gameContainer;

        public GameBoardProcesses(ChineseCheckersGameContainer gameContainer)
        {
            _gameContainer = gameContainer;
            LoadBoard();
        }
        private bool _turnContinued;
        private int _currentSpace; // i think i need this (?)
        private GameBoardGraphicsCP? _graphicsBoard;
        private Dictionary<int, SpaceInfo>? _spaceList; // i think dictionary is fine
        public async Task MakeMoveAsync(int index)
        {
            if (_graphicsBoard is null)
            {
                _graphicsBoard = _gameContainer.Resolver.Resolve<GameBoardGraphicsCP>(); //i think.  hopefully this simple this time.
            }
            _currentSpace = index;
            if (WhoTurn > _gameContainer.PlayerList.Count())
                throw new BasicBlankException("Number cannot be higher than the total available players");
            _gameContainer.SingleInfo!.PieceList.RemoveSpecificItem(_gameContainer.SaveRoot!.PreviousSpace); // needs to remove.  will add later after animations
            _gameContainer.Animates!.LocationFrom = _graphicsBoard.LocationOfSpace(_gameContainer.SaveRoot.PreviousSpace);
            _gameContainer.Animates.LocationTo = _graphicsBoard.LocationOfSpace(index);
            await _gameContainer.Animates.DoAnimateAsync();
            _gameContainer.SingleInfo.PieceList.Add(index); // you are now here instead
            _gameContainer.Aggregator.RepaintBoard();
            PopulateSpacesFromPlayers();
            await AdditionalProcesses(index);
        }
        private void PopulateSpacesFromPlayers()
        {
            foreach (var thisSpace in _spaceList!.Values)
                thisSpace.Player = 0;// has to clear out now.  otherwise, the old never gets removed.
            foreach (var thisPlayer in _gameContainer.PlayerList!)
            {
                foreach (var thisPiece in thisPlayer.PieceList)
                {
                    var thisSpace = (from items in _spaceList.Values
                                     where items.SpaceNumber == thisPiece
                                     select items).Single();
                    thisSpace.Player = thisPlayer.Id; //i think this is fine now.
                }
            }
        }
        private async Task AdditionalProcesses(int index)
        {
            if (CanWin() == true)
            {
                _gameContainer.SaveRoot!.PreviousSpace = 0;
                _gameContainer.Aggregator.RepaintBoard();
                await _gameContainer.ShowWinAsync!.Invoke();
                return;
            }
            if (DidJump() == false)
            {
                // turn is finished
                await _gameContainer.EndTurnAsync!.Invoke();
                return;
            }
            _turnContinued = true;
            MoveInfo thisMove = new MoveInfo();
            thisMove.SpaceTo = _gameContainer.SaveRoot!.PreviousSpace;
            thisMove.SpaceFrom = _gameContainer.SaveRoot.PreviousSpace;
            PreviousMoves.Add(thisMove);
            CustomBasicList<MoveInfo> thisList;
            var thisSpace = _spaceList![index];
            thisList = PossibleMovesWithPiece(thisSpace);
            if (thisList.Count > 0)
            {
                _gameContainer.SaveRoot.PreviousSpace = _currentSpace;
                _currentSpace = 0;
                _gameContainer.Aggregator.RepaintBoard();
                if (_gameContainer.SingleInfo!.PlayerCategory == EnumPlayerCategory.Self)
                {
                    _gameContainer.SaveRoot.Instructions = "Choose to either end turn or make another move with the same piece"; //could not take a risk this time.
                }
                await _gameContainer.ContinueTurnAsync!.Invoke();
                return;
            }
            _turnContinued = false;
            await _gameContainer.EndTurnAsync!.Invoke();
        }
        public void LoadSavedGame()
        {
            PopulateSpacesFromPlayers(); // i think needs to be done here (?)
        }
        public async Task HighlightItemAsync(int index)
        {
            _gameContainer.SaveRoot!.PreviousSpace = index;
            _gameContainer.Aggregator.RepaintBoard();
            await _gameContainer.ContinueTurnAsync!.Invoke(); // will continue turn from there.
        }
        private bool DidJump()
        {
            SpaceInfo thisSpace;
            SpaceInfo newSpace;
            thisSpace = _spaceList![_gameContainer.SaveRoot!.PreviousSpace];
            if (thisSpace.LeftOnly > 0)
            {
                newSpace = _spaceList[thisSpace.LeftOnly];
                if (newSpace.SpaceNumber == _currentSpace)
                    return false;
            }
            if (thisSpace.RightOnly > 0)
            {
                newSpace = _spaceList[thisSpace.RightOnly];
                if (newSpace.SpaceNumber == _currentSpace)
                    return false;
            }
            if (thisSpace.DownLeft > 0)
            {
                newSpace = _spaceList[thisSpace.DownLeft];
                if (newSpace.SpaceNumber == _currentSpace)
                    return false;
            }
            if (thisSpace.DownRight > 0)
            {
                newSpace = _spaceList[thisSpace.DownRight];
                if (newSpace.SpaceNumber == _currentSpace)
                    return false;
            }
            if (thisSpace.UpLeft > 0)
            {
                newSpace = _spaceList[thisSpace.UpLeft];
                if (newSpace.SpaceNumber == _currentSpace)
                    return false;
            }
            if (thisSpace.UpRight > 0)
            {
                newSpace = _spaceList[thisSpace.UpRight];
                if (newSpace.SpaceNumber == _currentSpace)
                    return false;
            }
            return true;
        }
        public bool WillContinueTurn()
        {
            return _turnContinued;
        }
        private CustomBasicList<MoveInfo> PossibleMovesWithPiece(SpaceInfo thisSpace)
        {
            CustomBasicList<MoveInfo> output = new CustomBasicList<MoveInfo>();
            SpaceInfo newSpace;
            SpaceInfo furtherSpace;
            bool haveItem;
            MoveInfo thisMove;
            EnumColorChoice ourColor;
            ourColor = _gameContainer.SingleInfo!.Color;
            if (thisSpace.LeftOnly > 0)
            {
                newSpace = _spaceList![thisSpace.LeftOnly];
                if ((newSpace.Player == 0) & (_turnContinued == false))
                {
                    if (newSpace.WhatColor == EnumColorChoice.None || newSpace.DestinationColor == ourColor || newSpace.WhatColor == ourColor)
                    {
                        thisMove = new MoveInfo();
                        thisMove.SpaceFrom = thisSpace.SpaceNumber;
                        thisMove.SpaceTo = thisSpace.LeftOnly;
                        output.Add(thisMove);
                    }
                }
                haveItem = DidHave(PreviousMoves, newSpace.LeftOnly);
                if ((newSpace.Player > 0) & (newSpace.LeftOnly > 0) & (haveItem == false))
                {
                    furtherSpace = _spaceList[newSpace.LeftOnly];
                    if (furtherSpace.Player == 0)
                    {
                        if (furtherSpace.WhatColor == EnumColorChoice.None || furtherSpace.DestinationColor == ourColor || furtherSpace.WhatColor == ourColor)
                        {
                            thisMove = new MoveInfo();
                            thisMove.SpaceFrom = thisSpace.SpaceNumber;
                            thisMove.SpaceTo = newSpace.LeftOnly;
                            output.Add(thisMove);
                        }
                    }
                }
            }
            if (thisSpace.RightOnly > 0)
            {
                newSpace = _spaceList![thisSpace.RightOnly];
                if ((newSpace.Player == 0) & (_turnContinued == false))
                {
                    if (newSpace.WhatColor == EnumColorChoice.None || newSpace.DestinationColor == ourColor || newSpace.WhatColor == ourColor)
                    {
                        thisMove = new MoveInfo();
                        thisMove.SpaceFrom = thisSpace.SpaceNumber;
                        thisMove.SpaceTo = thisSpace.RightOnly;
                        output.Add(thisMove);
                    }
                }
                haveItem = DidHave(PreviousMoves, newSpace.RightOnly);
                if ((newSpace.Player > 0) & (newSpace.RightOnly > 0) & (haveItem == false))
                {
                    furtherSpace = _spaceList[newSpace.RightOnly];
                    if (furtherSpace.Player == 0)
                    {
                        if (furtherSpace.WhatColor == EnumColorChoice.None || furtherSpace.DestinationColor == ourColor || furtherSpace.WhatColor == ourColor)
                        {
                            thisMove = new MoveInfo();
                            thisMove.SpaceFrom = thisSpace.SpaceNumber;
                            thisMove.SpaceTo = newSpace.RightOnly;
                            output.Add(thisMove);
                        }
                    }
                }
            }
            if (thisSpace.UpLeft > 0)
            {
                newSpace = _spaceList![thisSpace.UpLeft];
                if ((newSpace.Player == 0) & (_turnContinued == false))
                {
                    if (newSpace.WhatColor == EnumColorChoice.None || newSpace.DestinationColor == ourColor || newSpace.WhatColor == ourColor)
                    {
                        thisMove = new MoveInfo();
                        thisMove.SpaceFrom = thisSpace.SpaceNumber;
                        thisMove.SpaceTo = thisSpace.UpLeft;
                        output.Add(thisMove);
                    }
                }
                haveItem = DidHave(PreviousMoves, newSpace.UpLeft);
                if ((newSpace.Player > 0) & (newSpace.UpLeft > 0) & (haveItem == false))
                {
                    furtherSpace = _spaceList[newSpace.UpLeft];
                    if (furtherSpace.Player == 0)
                    {
                        if (furtherSpace.WhatColor == EnumColorChoice.None || furtherSpace.DestinationColor == ourColor || furtherSpace.WhatColor == ourColor)
                        {
                            thisMove = new MoveInfo();
                            thisMove.SpaceFrom = thisSpace.SpaceNumber;
                            thisMove.SpaceTo = newSpace.UpLeft;
                            output.Add(thisMove);
                        }
                    }
                }
            }
            if (thisSpace.UpRight > 0)
            {
                newSpace = _spaceList![thisSpace.UpRight];
                if ((newSpace.Player == 0) & (_turnContinued == false))
                {
                    if (newSpace.WhatColor == EnumColorChoice.None || newSpace.DestinationColor == ourColor || newSpace.WhatColor == ourColor)
                    {
                        thisMove = new MoveInfo();
                        thisMove.SpaceFrom = thisSpace.SpaceNumber;
                        thisMove.SpaceTo = thisSpace.UpRight;
                        output.Add(thisMove);
                    }
                }
                haveItem = DidHave(PreviousMoves, newSpace.UpRight);
                if ((newSpace.Player > 0) & (newSpace.UpRight > 0) & (haveItem == false))
                {
                    furtherSpace = _spaceList[newSpace.UpRight];
                    if (furtherSpace.Player == 0)
                    {
                        if (furtherSpace.WhatColor == EnumColorChoice.None || furtherSpace.DestinationColor == ourColor || furtherSpace.WhatColor == ourColor)
                        {
                            thisMove = new MoveInfo();
                            thisMove.SpaceFrom = thisSpace.SpaceNumber;
                            thisMove.SpaceTo = newSpace.UpRight;
                            output.Add(thisMove);
                        }
                    }
                }
            }
            if (thisSpace.DownLeft > 0)
            {
                newSpace = _spaceList![thisSpace.DownLeft];
                if ((newSpace.Player == 0) & (_turnContinued == false))
                {
                    if (newSpace.WhatColor == EnumColorChoice.None || newSpace.DestinationColor == ourColor || newSpace.WhatColor == ourColor)
                    {
                        thisMove = new MoveInfo();
                        thisMove.SpaceFrom = thisSpace.SpaceNumber;
                        thisMove.SpaceTo = thisSpace.DownLeft;
                        output.Add(thisMove);
                    }
                }
                haveItem = DidHave(PreviousMoves, newSpace.DownLeft);
                if ((newSpace.Player > 0) & (newSpace.DownLeft > 0) & (haveItem == false))
                {
                    furtherSpace = _spaceList[newSpace.DownLeft];
                    if (furtherSpace.Player == 0)
                    {
                        if (furtherSpace.WhatColor == EnumColorChoice.None || furtherSpace.DestinationColor == ourColor || furtherSpace.WhatColor == ourColor)
                        {
                            thisMove = new MoveInfo();
                            thisMove.SpaceFrom = thisSpace.SpaceNumber;
                            thisMove.SpaceTo = newSpace.DownLeft;
                            output.Add(thisMove);
                        }
                    }
                }
            }
            if (thisSpace.DownRight > 0)
            {
                newSpace = _spaceList![thisSpace.DownRight];
                if ((newSpace.Player == 0) & (_turnContinued == false))
                {
                    if (newSpace.WhatColor == EnumColorChoice.None || newSpace.DestinationColor == ourColor || newSpace.WhatColor == ourColor)
                    {
                        thisMove = new MoveInfo();
                        thisMove.SpaceFrom = thisSpace.SpaceNumber;
                        thisMove.SpaceTo = thisSpace.DownRight;
                        output.Add(thisMove);
                    }
                }
                haveItem = DidHave(PreviousMoves, newSpace.DownRight);
                if ((newSpace.Player > 0) & (newSpace.DownRight > 0) & (haveItem == false))
                {
                    furtherSpace = _spaceList[newSpace.DownRight];
                    if (furtherSpace.Player == 0)
                    {
                        if (furtherSpace.WhatColor == EnumColorChoice.None || furtherSpace.DestinationColor == ourColor || furtherSpace.WhatColor == ourColor)
                        {
                            thisMove = new MoveInfo();
                            thisMove.SpaceFrom = thisSpace.SpaceNumber;
                            thisMove.SpaceTo = newSpace.DownRight;
                            output.Add(thisMove);
                        }
                    }
                }
            }
            return output;
        }
        private CustomBasicList<MoveInfo> MoveList()
        {
            CustomBasicList<MoveInfo> output = new CustomBasicList<MoveInfo>();
            CustomBasicList<MoveInfo> newCol;
            if (_gameContainer.SaveRoot!.PreviousSpace == 0)
            {
                var thisList = (from items in _spaceList!.Values
                                where items.Player == WhoTurn
                                select items).ToList();
                foreach (SpaceInfo thisSpace in thisList)
                {
                    newCol = PossibleMovesWithPiece(thisSpace);
                    if (newCol.Count > 0)
                    {
                        foreach (MoveInfo thisMove in newCol)
                            output.Add(thisMove);
                    }
                }
                return output;
            }
            var fins = _spaceList![_gameContainer.SaveRoot.PreviousSpace];
            output = PossibleMovesWithPiece(fins);
            return output;
        }
        private bool DidHave(CustomBasicList<MoveInfo> thisCol, int whatItem)
        {
            if (_gameContainer.SaveRoot!.PreviousSpace == 0)
                return thisCol.Any(Items => Items.SpaceFrom == whatItem);
            else
                return thisCol.Any(Items => Items.SpaceTo == whatItem);
        }
        public bool IsValidMove(int space)
        {
            CustomBasicList<MoveInfo> thisList;
            thisList = MoveList();
            return DidHave(thisList, space);
        }
        public void StartTurn()
        {
            _turnContinued = false;
            _gameContainer.SaveRoot!.PreviousSpace = 0;
            _currentSpace = 0;
            PreviousMoves = new CustomBasicList<MoveInfo>();
            _gameContainer.Aggregator.RepaintBoard();
        }
        private bool CanWin()
        {
            if (_gameContainer.Test.ImmediatelyEndGame)
            {
                return true; //because we are ending game to see how new game goes.
            }
            var thisCol = _spaceList!.Values.Where(items => items.Player == WhoTurn).ToCustomBasicList();
            EnumColorChoice ourColor;
            ourColor = _gameContainer.SingleInfo!.Color;
            return !thisCol.Any(Items => Items.DestinationColor != ourColor);
        }
        public void ClearBoard() // this will mean it will populate both lists.
        {
            int x = default;
            foreach (var thisPlayer in _gameContainer.PlayerList!)
                thisPlayer.PieceList.Clear();// because new game.
            foreach (var thisSpace in _spaceList!.Values)
            {
                if (thisSpace.WhatColor != EnumColorChoice.None)
                {
                    ChineseCheckersPlayerItem tempPlayer = _gameContainer.PlayerList.SingleOrDefault(Items => Items.Color == thisSpace.WhatColor);
                    if (tempPlayer != null)
                    {
                        x += 1;
                        thisSpace.Player = tempPlayer.Id;
                        tempPlayer.PieceList.Add(thisSpace.SpaceNumber);
                    }
                }
            }
            _gameContainer.Aggregator.RepaintBoard();
        }
        private void LoadBoard() // hopefully this simple.
        {
            _gameContainer.Animates!.LongestTravelTime = 80; // try 80.  50 could be too short.
            PopulateSpaceList();
        }
        public async Task UnselectPieceAsync() // well see 
        {
            _gameContainer.SaveRoot!.PreviousSpace = 0;
            _gameContainer.Aggregator.RepaintBoard();
            await _gameContainer.ContinueTurnAsync!.Invoke();
        }
        private void PopulateSpaceList()
        {
            _spaceList = new Dictionary<int, SpaceInfo>();
            SpaceInfo thisSpace;
            int x;
            for (x = 1; x <= 121; x++)
            {
                thisSpace = new SpaceInfo();
                thisSpace.WhatColor = EnumColorChoice.None;
                thisSpace.DestinationColor = EnumColorChoice.None;
                switch (x)
                {
                    case 1:
                        {
                            thisSpace.UpLeft = 2;
                            thisSpace.UpRight = 3;
                            thisSpace.WhatColor = EnumColorChoice.Green;
                            thisSpace.DestinationColor = EnumColorChoice.Blue;
                            break;
                        }

                    case 2:
                        {
                            thisSpace.DownRight = 1;
                            thisSpace.RightOnly = 3;
                            thisSpace.UpLeft = 4;
                            thisSpace.UpRight = 5;
                            thisSpace.WhatColor = EnumColorChoice.Green;
                            thisSpace.DestinationColor = EnumColorChoice.Blue;
                            break;
                        }

                    case 3:
                        {
                            thisSpace.LeftOnly = 2;
                            thisSpace.DownLeft = 1;
                            thisSpace.UpLeft = 5;
                            thisSpace.UpRight = 6;
                            thisSpace.WhatColor = EnumColorChoice.Green;
                            thisSpace.DestinationColor = EnumColorChoice.Blue;
                            break;
                        }

                    case 4:
                        {
                            thisSpace.RightOnly = 5;
                            thisSpace.DownRight = 2;
                            thisSpace.UpLeft = 7;
                            thisSpace.UpRight = 8;
                            thisSpace.WhatColor = EnumColorChoice.Green;
                            thisSpace.DestinationColor = EnumColorChoice.Blue;
                            break;
                        }

                    case 5:
                        {
                            thisSpace.LeftOnly = 4;
                            thisSpace.RightOnly = 6;
                            thisSpace.UpLeft = 8;
                            thisSpace.UpRight = 9;
                            thisSpace.DownLeft = 2;
                            thisSpace.DownRight = 3;
                            thisSpace.WhatColor = EnumColorChoice.Green;
                            thisSpace.DestinationColor = EnumColorChoice.Blue;
                            break;
                        }

                    case 6:
                        {
                            thisSpace.LeftOnly = 5;
                            thisSpace.UpLeft = 9;
                            thisSpace.UpRight = 10;
                            thisSpace.DownLeft = 3;
                            thisSpace.WhatColor = EnumColorChoice.Green;
                            thisSpace.DestinationColor = EnumColorChoice.Blue;
                            break;
                        }

                    case 7:
                        {
                            thisSpace.RightOnly = 8;
                            thisSpace.DownRight = 4;
                            thisSpace.UpLeft = 15;
                            thisSpace.UpRight = 16;
                            thisSpace.WhatColor = EnumColorChoice.Green;
                            thisSpace.DestinationColor = EnumColorChoice.Blue;
                            break;
                        }

                    case 8:
                        {
                            thisSpace.LeftOnly = 7;
                            thisSpace.RightOnly = 9;
                            thisSpace.UpLeft = 16;
                            thisSpace.UpRight = 17;
                            thisSpace.DownLeft = 4;
                            thisSpace.DownRight = 5;
                            thisSpace.WhatColor = EnumColorChoice.Green;
                            thisSpace.DestinationColor = EnumColorChoice.Blue;
                            break;
                        }

                    case 9:
                        {
                            thisSpace.LeftOnly = 8;
                            thisSpace.RightOnly = 10;
                            thisSpace.UpLeft = 17;
                            thisSpace.UpRight = 18;
                            thisSpace.DownLeft = 5;
                            thisSpace.DownRight = 6;
                            thisSpace.WhatColor = EnumColorChoice.Green;
                            thisSpace.DestinationColor = EnumColorChoice.Blue;
                            break;
                        }

                    case 10:
                        {
                            thisSpace.LeftOnly = 9;
                            thisSpace.DownLeft = 6;
                            thisSpace.UpLeft = 18;
                            thisSpace.UpRight = 19;
                            thisSpace.WhatColor = EnumColorChoice.Green;
                            thisSpace.DestinationColor = EnumColorChoice.Blue;
                            break;
                        }

                    case 11:
                        {
                            thisSpace.RightOnly = 12;
                            thisSpace.UpRight = 24;
                            thisSpace.WhatColor = EnumColorChoice.Purple;
                            thisSpace.DestinationColor = EnumColorChoice.Yellow;
                            break;
                        }

                    case 12:
                        {
                            thisSpace.LeftOnly = 11;
                            thisSpace.RightOnly = 13;
                            thisSpace.UpLeft = 24;
                            thisSpace.UpRight = 25;
                            thisSpace.WhatColor = EnumColorChoice.Purple;
                            thisSpace.DestinationColor = EnumColorChoice.Yellow;
                            break;
                        }

                    case 13:
                        {
                            thisSpace.LeftOnly = 12;
                            thisSpace.RightOnly = 14;
                            thisSpace.UpLeft = 25;
                            thisSpace.UpRight = 26;
                            thisSpace.WhatColor = EnumColorChoice.Purple;
                            thisSpace.DestinationColor = EnumColorChoice.Yellow;
                            break;
                        }

                    case 14:
                        {
                            thisSpace.LeftOnly = 13;
                            thisSpace.RightOnly = 15;
                            thisSpace.UpLeft = 26;
                            thisSpace.UpRight = 27;
                            thisSpace.WhatColor = EnumColorChoice.Purple;
                            thisSpace.DestinationColor = EnumColorChoice.Yellow;
                            break;
                        }

                    case 15:
                        {
                            thisSpace.LeftOnly = 14;
                            thisSpace.RightOnly = 16;
                            thisSpace.UpLeft = 27;
                            thisSpace.UpRight = 28;
                            thisSpace.DownRight = 7;
                            break;
                        }

                    case 16:
                        {
                            thisSpace.LeftOnly = 15;
                            thisSpace.RightOnly = 17;
                            thisSpace.UpLeft = 28;
                            thisSpace.UpRight = 29;
                            thisSpace.DownLeft = 7;
                            thisSpace.DownRight = 8;
                            break;
                        }

                    case 17:
                        {
                            thisSpace.LeftOnly = 16;
                            thisSpace.RightOnly = 18;
                            thisSpace.UpLeft = 29;
                            thisSpace.UpRight = 30;
                            thisSpace.DownLeft = 8;
                            thisSpace.DownRight = 9;
                            break;
                        }

                    case 18:
                        {
                            thisSpace.LeftOnly = 17;
                            thisSpace.RightOnly = 19;
                            thisSpace.UpLeft = 30;
                            thisSpace.UpRight = 31;
                            thisSpace.DownLeft = 9;
                            thisSpace.DownRight = 10;
                            break;
                        }

                    case 19:
                        {
                            thisSpace.LeftOnly = 18;
                            thisSpace.RightOnly = 20;
                            thisSpace.UpLeft = 31;
                            thisSpace.UpRight = 32;
                            thisSpace.DownLeft = 10;
                            break;
                        }

                    case 20:
                        {
                            thisSpace.LeftOnly = 19;
                            thisSpace.RightOnly = 21;
                            thisSpace.UpLeft = 32;
                            thisSpace.UpRight = 33;
                            thisSpace.WhatColor = EnumColorChoice.Gray;
                            thisSpace.DestinationColor = EnumColorChoice.Red;
                            break;
                        }

                    case 21:
                        {
                            thisSpace.LeftOnly = 20;
                            thisSpace.RightOnly = 22;
                            thisSpace.UpLeft = 33;
                            thisSpace.UpRight = 34;
                            thisSpace.WhatColor = EnumColorChoice.Gray;
                            thisSpace.DestinationColor = EnumColorChoice.Red;
                            break;
                        }

                    case 22:
                        {
                            thisSpace.LeftOnly = 21;
                            thisSpace.RightOnly = 23;
                            thisSpace.UpLeft = 34;
                            thisSpace.UpRight = 35;
                            thisSpace.WhatColor = EnumColorChoice.Gray;
                            thisSpace.DestinationColor = EnumColorChoice.Red;
                            break;
                        }

                    case 23:
                        {
                            thisSpace.LeftOnly = 22;
                            thisSpace.UpLeft = 35;
                            thisSpace.WhatColor = EnumColorChoice.Gray;
                            thisSpace.DestinationColor = EnumColorChoice.Red;
                            break;
                        }

                    case 24:
                        {
                            thisSpace.RightOnly = 25;
                            thisSpace.UpRight = 36;
                            thisSpace.DownLeft = 11;
                            thisSpace.DownRight = 12;
                            thisSpace.WhatColor = EnumColorChoice.Purple;
                            thisSpace.DestinationColor = EnumColorChoice.Yellow;
                            break;
                        }

                    case 25:
                        {
                            thisSpace.LeftOnly = 24;
                            thisSpace.RightOnly = 26;
                            thisSpace.UpLeft = 36;
                            thisSpace.UpRight = 37;
                            thisSpace.DownLeft = 12;
                            thisSpace.DownRight = 13;
                            thisSpace.WhatColor = EnumColorChoice.Purple;
                            thisSpace.DestinationColor = EnumColorChoice.Yellow;
                            break;
                        }

                    case 26:
                        {
                            thisSpace.LeftOnly = 25;
                            thisSpace.RightOnly = thisSpace.LeftOnly + 2;
                            thisSpace.UpLeft = 37;
                            thisSpace.UpRight = thisSpace.UpLeft + 1;
                            thisSpace.DownLeft = 13;
                            thisSpace.DownRight = thisSpace.DownLeft + 1;
                            thisSpace.WhatColor = EnumColorChoice.Purple;
                            thisSpace.DestinationColor = EnumColorChoice.Yellow;
                            break;
                        }

                    case 27:
                        {
                            thisSpace.LeftOnly = 26;
                            thisSpace.RightOnly = thisSpace.LeftOnly + 2;
                            thisSpace.UpLeft = 38;
                            thisSpace.UpRight = thisSpace.UpLeft + 1;
                            thisSpace.DownLeft = 14;
                            thisSpace.DownRight = thisSpace.DownLeft + 1;
                            break;
                        }

                    case 28:
                        {
                            thisSpace.LeftOnly = 27;
                            thisSpace.RightOnly = thisSpace.LeftOnly + 2;
                            thisSpace.UpLeft = 39;
                            thisSpace.UpRight = thisSpace.UpLeft + 1;
                            thisSpace.DownLeft = 15;
                            thisSpace.DownRight = thisSpace.DownLeft + 1;
                            break;
                        }

                    case 29:
                        {
                            thisSpace.LeftOnly = 28;
                            thisSpace.RightOnly = thisSpace.LeftOnly + 2;
                            thisSpace.UpLeft = 40;
                            thisSpace.UpRight = thisSpace.UpLeft + 1;
                            thisSpace.DownLeft = 16;
                            thisSpace.DownRight = thisSpace.DownLeft + 1;
                            break;
                        }

                    case 30:
                        {
                            thisSpace.LeftOnly = 29;
                            thisSpace.RightOnly = thisSpace.LeftOnly + 2;
                            thisSpace.UpLeft = 41;
                            thisSpace.UpRight = thisSpace.UpLeft + 1;
                            thisSpace.DownLeft = 17;
                            thisSpace.DownRight = thisSpace.DownLeft + 1;
                            break;
                        }

                    case 31:
                        {
                            thisSpace.LeftOnly = x - 1;
                            thisSpace.RightOnly = thisSpace.LeftOnly + 2;
                            thisSpace.UpLeft = x + 11;
                            thisSpace.UpRight = thisSpace.UpLeft + 1;
                            thisSpace.DownLeft = x - 13;
                            thisSpace.DownRight = thisSpace.DownLeft + 1;
                            break;
                        }

                    case 32:
                        {
                            thisSpace.LeftOnly = x - 1;
                            thisSpace.RightOnly = thisSpace.LeftOnly + 2;
                            thisSpace.UpLeft = x + 11;
                            thisSpace.UpRight = thisSpace.UpLeft + 1;
                            thisSpace.DownLeft = x - 13;
                            thisSpace.DownRight = thisSpace.DownLeft + 1;
                            break;
                        }

                    case 33:
                        {
                            thisSpace.LeftOnly = x - 1;
                            thisSpace.RightOnly = thisSpace.LeftOnly + 2;
                            thisSpace.UpLeft = x + 11;
                            thisSpace.UpRight = thisSpace.UpLeft + 1;
                            thisSpace.DownLeft = x - 13;
                            thisSpace.DownRight = thisSpace.DownLeft + 1;
                            thisSpace.WhatColor = EnumColorChoice.Gray;
                            thisSpace.DestinationColor = EnumColorChoice.Red;
                            break;
                        }

                    case 34:
                        {
                            thisSpace.LeftOnly = x - 1;
                            thisSpace.RightOnly = thisSpace.LeftOnly + 2;
                            thisSpace.UpLeft = x + 11;
                            thisSpace.UpRight = thisSpace.UpLeft + 1;
                            thisSpace.DownLeft = x - 13;
                            thisSpace.DownRight = thisSpace.DownLeft + 1;
                            thisSpace.WhatColor = EnumColorChoice.Gray;
                            thisSpace.DestinationColor = EnumColorChoice.Red;
                            break;
                        }

                    case 35:
                        {
                            thisSpace.LeftOnly = x - 1;
                            thisSpace.UpLeft = x + 11;
                            thisSpace.DownLeft = x - 13;
                            thisSpace.DownRight = x - 12;
                            thisSpace.WhatColor = EnumColorChoice.Gray;
                            thisSpace.DestinationColor = EnumColorChoice.Red;
                            break;
                        }

                    case 36:
                        {
                            thisSpace.RightOnly = x + 1;
                            thisSpace.UpRight = 47;
                            thisSpace.DownLeft = x - 12;
                            thisSpace.DownRight = x - 11;
                            thisSpace.WhatColor = EnumColorChoice.Purple;
                            thisSpace.DestinationColor = EnumColorChoice.Yellow;
                            break;
                        }

                    case 37:
                        {
                            thisSpace.LeftOnly = x - 1;
                            thisSpace.RightOnly = x + 1;
                            thisSpace.UpLeft = x + 10;
                            thisSpace.UpRight = x + 11;
                            thisSpace.DownLeft = x - 12;
                            thisSpace.DownRight = x - 11;
                            thisSpace.WhatColor = EnumColorChoice.Purple;
                            thisSpace.DestinationColor = EnumColorChoice.Yellow;
                            break;
                        }

                    case 38:
                        {
                            thisSpace.LeftOnly = x - 1;
                            thisSpace.RightOnly = x + 1;
                            thisSpace.UpLeft = x + 10;
                            thisSpace.UpRight = x + 11;
                            thisSpace.DownLeft = x - 12;
                            thisSpace.DownRight = x - 11;
                            break;
                        }

                    case 39:
                        {
                            thisSpace.LeftOnly = x - 1;
                            thisSpace.RightOnly = x + 1;
                            thisSpace.UpLeft = x + 10;
                            thisSpace.UpRight = x + 11;
                            thisSpace.DownLeft = x - 12;
                            thisSpace.DownRight = x - 11;
                            break;
                        }

                    case 40:
                        {
                            thisSpace.LeftOnly = x - 1;
                            thisSpace.RightOnly = x + 1;
                            thisSpace.UpLeft = x + 10;
                            thisSpace.UpRight = x + 11;
                            thisSpace.DownLeft = x - 12;
                            thisSpace.DownRight = x - 11;
                            break;
                        }

                    case 41:
                        {
                            thisSpace.LeftOnly = x - 1;
                            thisSpace.RightOnly = x + 1;
                            thisSpace.UpLeft = x + 10;
                            thisSpace.UpRight = x + 11;
                            thisSpace.DownLeft = x - 12;
                            thisSpace.DownRight = x - 11;
                            break;
                        }

                    case 42:
                        {
                            thisSpace.LeftOnly = x - 1;
                            thisSpace.RightOnly = x + 1;
                            thisSpace.UpLeft = x + 10;
                            thisSpace.UpRight = x + 11;
                            thisSpace.DownLeft = x - 12;
                            thisSpace.DownRight = x - 11;
                            break;
                        }

                    case 43:
                        {
                            thisSpace.LeftOnly = x - 1;
                            thisSpace.RightOnly = x + 1;
                            thisSpace.UpLeft = x + 10;
                            thisSpace.UpRight = x + 11;
                            thisSpace.DownLeft = x - 12;
                            thisSpace.DownRight = x - 11;
                            break;
                        }

                    case 44:
                        {
                            thisSpace.LeftOnly = x - 1;
                            thisSpace.RightOnly = x + 1;
                            thisSpace.UpLeft = x + 10;
                            thisSpace.UpRight = x + 11;
                            thisSpace.DownLeft = x - 12;
                            thisSpace.DownRight = x - 11;
                            break;
                        }

                    case 45:
                        {
                            thisSpace.LeftOnly = x - 1;
                            thisSpace.RightOnly = x + 1;
                            thisSpace.UpLeft = x + 10;
                            thisSpace.UpRight = x + 11;
                            thisSpace.DownLeft = x - 12;
                            thisSpace.DownRight = x - 11;
                            thisSpace.WhatColor = EnumColorChoice.Gray;
                            thisSpace.DestinationColor = EnumColorChoice.Red;
                            break;
                        }

                    case 46:
                        {
                            thisSpace.LeftOnly = x - 1;
                            thisSpace.UpLeft = x + 10;
                            thisSpace.DownLeft = x - 12;
                            thisSpace.DownRight = x - 11;
                            thisSpace.WhatColor = EnumColorChoice.Gray;
                            thisSpace.DestinationColor = EnumColorChoice.Red;
                            break;
                        }

                    case 47:
                        {
                            thisSpace.RightOnly = x + 1;
                            thisSpace.UpRight = x + 10;
                            thisSpace.DownLeft = x - 11;
                            thisSpace.DownRight = x - 10;
                            thisSpace.WhatColor = EnumColorChoice.Purple;
                            thisSpace.DestinationColor = EnumColorChoice.Yellow;
                            break;
                        }

                    case 48:
                        {
                            thisSpace.LeftOnly = x - 1;
                            thisSpace.RightOnly = x + 1;
                            thisSpace.UpLeft = x + 9;
                            thisSpace.UpRight = x + 10;
                            thisSpace.DownLeft = x - 11;
                            thisSpace.DownRight = x - 10;
                            break;
                        }

                    case 49:
                        {
                            thisSpace.LeftOnly = x - 1;
                            thisSpace.RightOnly = x + 1;
                            thisSpace.UpLeft = x + 9;
                            thisSpace.UpRight = x + 10;
                            thisSpace.DownLeft = x - 11;
                            thisSpace.DownRight = x - 10;
                            break;
                        }

                    case 50:
                        {
                            thisSpace.LeftOnly = x - 1;
                            thisSpace.RightOnly = x + 1;
                            thisSpace.UpLeft = x + 9;
                            thisSpace.UpRight = x + 10;
                            thisSpace.DownLeft = x - 11;
                            thisSpace.DownRight = x - 10;
                            break;
                        }

                    case 51:
                        {
                            thisSpace.LeftOnly = x - 1;
                            thisSpace.RightOnly = x + 1;
                            thisSpace.UpLeft = x + 9;
                            thisSpace.UpRight = x + 10;
                            thisSpace.DownLeft = x - 11;
                            thisSpace.DownRight = x - 10;
                            break;
                        }

                    case 52:
                        {
                            thisSpace.LeftOnly = x - 1;
                            thisSpace.RightOnly = x + 1;
                            thisSpace.UpLeft = x + 9;
                            thisSpace.UpRight = x + 10;
                            thisSpace.DownLeft = x - 11;
                            thisSpace.DownRight = x - 10;
                            break;
                        }

                    case 53:
                        {
                            thisSpace.LeftOnly = x - 1;
                            thisSpace.RightOnly = x + 1;
                            thisSpace.UpLeft = x + 9;
                            thisSpace.UpRight = x + 10;
                            thisSpace.DownLeft = x - 11;
                            thisSpace.DownRight = x - 10;
                            break;
                        }

                    case 54:
                        {
                            thisSpace.LeftOnly = x - 1;
                            thisSpace.RightOnly = x + 1;
                            thisSpace.UpLeft = x + 9;
                            thisSpace.UpRight = x + 10;
                            thisSpace.DownLeft = x - 11;
                            thisSpace.DownRight = x - 10;
                            break;
                        }

                    case 55:
                        {
                            thisSpace.LeftOnly = x - 1;
                            thisSpace.RightOnly = x + 1;
                            thisSpace.UpLeft = x + 9;
                            thisSpace.UpRight = x + 10;
                            thisSpace.DownLeft = x - 11;
                            thisSpace.DownRight = x - 10;
                            break;
                        }

                    case 56:
                        {
                            thisSpace.LeftOnly = x - 1;
                            thisSpace.UpLeft = x + 9;
                            thisSpace.DownLeft = x - 11;
                            thisSpace.DownRight = x - 10;
                            thisSpace.WhatColor = EnumColorChoice.Gray;
                            thisSpace.DestinationColor = EnumColorChoice.Red;
                            break;
                        }

                    case 57:
                        {
                            thisSpace.RightOnly = x + 1;
                            thisSpace.UpLeft = x + 9;
                            thisSpace.UpRight = x + 10;
                            thisSpace.DownLeft = x - 10;
                            thisSpace.DownRight = x - 9;
                            break;
                        }

                    case 58:
                        {
                            thisSpace.LeftOnly = x - 1;
                            thisSpace.RightOnly = x + 1;
                            thisSpace.UpLeft = x + 9;
                            thisSpace.UpRight = x + 10;
                            thisSpace.DownLeft = x - 10;
                            thisSpace.DownRight = x - 9;
                            break;
                        }

                    case 59:
                        {
                            thisSpace.LeftOnly = x - 1;
                            thisSpace.RightOnly = x + 1;
                            thisSpace.UpLeft = x + 9;
                            thisSpace.UpRight = x + 10;
                            thisSpace.DownLeft = x - 10;
                            thisSpace.DownRight = x - 9;
                            break;
                        }

                    case 60:
                        {
                            thisSpace.LeftOnly = x - 1;
                            thisSpace.RightOnly = x + 1;
                            thisSpace.UpLeft = x + 9;
                            thisSpace.UpRight = x + 10;
                            thisSpace.DownLeft = x - 10;
                            thisSpace.DownRight = x - 9;
                            break;
                        }

                    case 61:
                        {
                            thisSpace.LeftOnly = x - 1;
                            thisSpace.RightOnly = x + 1;
                            thisSpace.UpLeft = x + 9;
                            thisSpace.UpRight = x + 10;
                            thisSpace.DownLeft = x - 10;
                            thisSpace.DownRight = x - 9;
                            break;
                        }

                    case 62:
                        {
                            thisSpace.LeftOnly = x - 1;
                            thisSpace.RightOnly = x + 1;
                            thisSpace.UpLeft = x + 9;
                            thisSpace.UpRight = x + 10;
                            thisSpace.DownLeft = x - 10;
                            thisSpace.DownRight = x - 9;
                            break;
                        }

                    case 63:
                        {
                            thisSpace.LeftOnly = x - 1;
                            thisSpace.RightOnly = x + 1;
                            thisSpace.UpLeft = x + 9;
                            thisSpace.UpRight = x + 10;
                            thisSpace.DownLeft = x - 10;
                            thisSpace.DownRight = x - 9;
                            break;
                        }

                    case 64:
                        {
                            thisSpace.LeftOnly = x - 1;
                            thisSpace.RightOnly = x + 1;
                            thisSpace.UpLeft = x + 9;
                            thisSpace.UpRight = x + 10;
                            thisSpace.DownLeft = x - 10;
                            thisSpace.DownRight = x - 9;
                            break;
                        }

                    case 65:
                        {
                            thisSpace.LeftOnly = x - 1;
                            thisSpace.UpLeft = x + 9;
                            thisSpace.UpRight = x + 10;
                            thisSpace.DownLeft = x - 10;
                            thisSpace.DownRight = x - 9;
                            break;
                        }

                    case 66:
                        {
                            thisSpace.RightOnly = x + 1;
                            thisSpace.UpLeft = x + 10;
                            thisSpace.UpRight = x + 11;
                            thisSpace.DownLeft = x - 10;
                            thisSpace.DownRight = x - 9;
                            thisSpace.WhatColor = EnumColorChoice.Red;
                            thisSpace.DestinationColor = EnumColorChoice.Gray;
                            break;
                        }

                    case 67:
                        {
                            thisSpace.LeftOnly = x - 1;
                            thisSpace.RightOnly = x + 1;
                            thisSpace.UpLeft = x + 10;
                            thisSpace.UpRight = x + 11;
                            thisSpace.DownLeft = x - 10;
                            thisSpace.DownRight = x - 9;
                            break;
                        }

                    case 68:
                        {
                            thisSpace.LeftOnly = x - 1;
                            thisSpace.RightOnly = x + 1;
                            thisSpace.UpLeft = x + 10;
                            thisSpace.UpRight = x + 11;
                            thisSpace.DownLeft = x - 10;
                            thisSpace.DownRight = x - 9;
                            break;
                        }

                    case 69:
                        {
                            thisSpace.LeftOnly = x - 1;
                            thisSpace.RightOnly = x + 1;
                            thisSpace.UpLeft = x + 10;
                            thisSpace.UpRight = x + 11;
                            thisSpace.DownLeft = x - 10;
                            thisSpace.DownRight = x - 9;
                            break;
                        }

                    case 70:
                        {
                            thisSpace.LeftOnly = x - 1;
                            thisSpace.RightOnly = x + 1;
                            thisSpace.UpLeft = x + 10;
                            thisSpace.UpRight = x + 11;
                            thisSpace.DownLeft = x - 10;
                            thisSpace.DownRight = x - 9;
                            break;
                        }

                    case 71:
                        {
                            thisSpace.LeftOnly = x - 1;
                            thisSpace.RightOnly = x + 1;
                            thisSpace.UpLeft = x + 10;
                            thisSpace.UpRight = x + 11;
                            thisSpace.DownLeft = x - 10;
                            thisSpace.DownRight = x - 9;
                            break;
                        }

                    case 72:
                        {
                            thisSpace.LeftOnly = x - 1;
                            thisSpace.RightOnly = x + 1;
                            thisSpace.UpLeft = x + 10;
                            thisSpace.UpRight = x + 11;
                            thisSpace.DownLeft = x - 10;
                            thisSpace.DownRight = x - 9;
                            break;
                        }

                    case 73:
                        {
                            thisSpace.LeftOnly = x - 1;
                            thisSpace.RightOnly = x + 1;
                            thisSpace.UpLeft = x + 10;
                            thisSpace.UpRight = x + 11;
                            thisSpace.DownLeft = x - 10;
                            thisSpace.DownRight = x - 9;
                            break;
                        }

                    case 74:
                        {
                            thisSpace.LeftOnly = x - 1;
                            thisSpace.RightOnly = x + 1;
                            thisSpace.UpLeft = x + 10;
                            thisSpace.UpRight = x + 11;
                            thisSpace.DownLeft = x - 10;
                            thisSpace.DownRight = x - 9;
                            break;
                        }

                    case 75:
                        {
                            thisSpace.LeftOnly = x - 1;
                            thisSpace.UpLeft = x + 10;
                            thisSpace.UpRight = x + 11;
                            thisSpace.DownLeft = x - 10;
                            thisSpace.WhatColor = EnumColorChoice.Yellow;
                            thisSpace.DestinationColor = EnumColorChoice.Purple;
                            break;
                        }

                    case 76:
                        {
                            thisSpace.RightOnly = x + 1;
                            thisSpace.UpLeft = x + 11;
                            thisSpace.UpRight = x + 12;
                            thisSpace.DownRight = x - 10;
                            thisSpace.WhatColor = EnumColorChoice.Red;
                            thisSpace.DestinationColor = EnumColorChoice.Gray;
                            break;
                        }

                    case 77:
                        {
                            thisSpace.LeftOnly = x - 1;
                            thisSpace.RightOnly = x + 1;
                            thisSpace.UpLeft = x + 11;
                            thisSpace.UpRight = x + 12;
                            thisSpace.DownLeft = x - 11;
                            thisSpace.DownRight = x - 10;
                            thisSpace.WhatColor = EnumColorChoice.Red;
                            thisSpace.DestinationColor = EnumColorChoice.Gray;
                            break;
                        }

                    case 78:
                        {
                            thisSpace.LeftOnly = x - 1;
                            thisSpace.RightOnly = x + 1;
                            thisSpace.UpLeft = x + 11;
                            thisSpace.UpRight = x + 12;
                            thisSpace.DownLeft = x - 11;
                            thisSpace.DownRight = x - 10;
                            break;
                        }

                    case 79:
                        {
                            thisSpace.LeftOnly = x - 1;
                            thisSpace.RightOnly = x + 1;
                            thisSpace.UpLeft = x + 11;
                            thisSpace.UpRight = x + 12;
                            thisSpace.DownLeft = x - 11;
                            thisSpace.DownRight = x - 10;
                            break;
                        }

                    case 80:
                        {
                            thisSpace.LeftOnly = x - 1;
                            thisSpace.RightOnly = x + 1;
                            thisSpace.UpLeft = x + 11;
                            thisSpace.UpRight = x + 12;
                            thisSpace.DownLeft = x - 11;
                            thisSpace.DownRight = x - 10;
                            break;
                        }

                    case 81:
                        {
                            thisSpace.LeftOnly = x - 1;
                            thisSpace.RightOnly = x + 1;
                            thisSpace.UpLeft = x + 11;
                            thisSpace.UpRight = x + 12;
                            thisSpace.DownLeft = x - 11;
                            thisSpace.DownRight = x - 10;
                            break;
                        }

                    case 82:
                        {
                            thisSpace.LeftOnly = x - 1;
                            thisSpace.RightOnly = x + 1;
                            thisSpace.UpLeft = x + 11;
                            thisSpace.UpRight = x + 12;
                            thisSpace.DownLeft = x - 11;
                            thisSpace.DownRight = x - 10;
                            break;
                        }

                    case 83:
                        {
                            thisSpace.LeftOnly = x - 1;
                            thisSpace.RightOnly = x + 1;
                            thisSpace.UpLeft = x + 11;
                            thisSpace.UpRight = x + 12;
                            thisSpace.DownLeft = x - 11;
                            thisSpace.DownRight = x - 10;
                            break;
                        }

                    case 84:
                        {
                            thisSpace.LeftOnly = x - 1;
                            thisSpace.RightOnly = x + 1;
                            thisSpace.UpLeft = x + 11;
                            thisSpace.UpRight = x + 12;
                            thisSpace.DownLeft = x - 11;
                            thisSpace.DownRight = x - 10;
                            break;
                        }

                    case 85:
                        {
                            thisSpace.LeftOnly = x - 1;
                            thisSpace.RightOnly = x + 1;
                            thisSpace.UpLeft = x + 11;
                            thisSpace.UpRight = x + 12;
                            thisSpace.DownLeft = x - 11;
                            thisSpace.DownRight = x - 10;
                            thisSpace.WhatColor = EnumColorChoice.Yellow;
                            thisSpace.DestinationColor = EnumColorChoice.Purple;
                            break;
                        }

                    case 86:
                        {
                            thisSpace.LeftOnly = x - 1;
                            thisSpace.UpLeft = x + 11;
                            thisSpace.UpRight = x + 12;
                            thisSpace.DownLeft = x - 11;
                            thisSpace.WhatColor = EnumColorChoice.Yellow;
                            thisSpace.DestinationColor = EnumColorChoice.Purple;
                            break;
                        }

                    case 87:
                        {
                            thisSpace.RightOnly = x + 1;
                            thisSpace.UpLeft = x + 12;
                            thisSpace.UpRight = x + 13;
                            thisSpace.DownRight = x - 11;
                            thisSpace.WhatColor = EnumColorChoice.Red;
                            thisSpace.DestinationColor = EnumColorChoice.Gray;
                            break;
                        }

                    case 88:
                        {
                            thisSpace.LeftOnly = x - 1;
                            thisSpace.RightOnly = x + 1;
                            thisSpace.UpLeft = x + 12;
                            thisSpace.UpRight = x + 13;
                            thisSpace.DownLeft = x - 12;
                            thisSpace.DownRight = x - 11;
                            thisSpace.WhatColor = EnumColorChoice.Red;
                            thisSpace.DestinationColor = EnumColorChoice.Gray;
                            break;
                        }

                    case 89:
                        {
                            thisSpace.LeftOnly = x - 1;
                            thisSpace.RightOnly = x + 1;
                            thisSpace.UpLeft = x + 12;
                            thisSpace.UpRight = x + 13;
                            thisSpace.DownLeft = x - 12;
                            thisSpace.DownRight = x - 11;
                            thisSpace.WhatColor = EnumColorChoice.Red;
                            thisSpace.DestinationColor = EnumColorChoice.Gray;
                            break;
                        }

                    case 90:
                        {
                            thisSpace.LeftOnly = x - 1;
                            thisSpace.RightOnly = x + 1;
                            thisSpace.UpLeft = x + 12;
                            thisSpace.UpRight = x + 13;
                            thisSpace.DownLeft = x - 12;
                            thisSpace.DownRight = x - 11;
                            break;
                        }

                    case 91:
                        {
                            thisSpace.LeftOnly = x - 1;
                            thisSpace.RightOnly = x + 1;
                            thisSpace.UpLeft = x + 12;
                            thisSpace.UpRight = x + 13;
                            thisSpace.DownLeft = x - 12;
                            thisSpace.DownRight = x - 11;
                            break;
                        }

                    case 92:
                        {
                            thisSpace.LeftOnly = x - 1;
                            thisSpace.RightOnly = x + 1;
                            thisSpace.UpLeft = x + 12;
                            thisSpace.UpRight = x + 13;
                            thisSpace.DownLeft = x - 12;
                            thisSpace.DownRight = x - 11;
                            break;
                        }

                    case 93:
                        {
                            thisSpace.LeftOnly = x - 1;
                            thisSpace.RightOnly = x + 1;
                            thisSpace.UpLeft = x + 12;
                            thisSpace.UpRight = x + 13;
                            thisSpace.DownLeft = x - 12;
                            thisSpace.DownRight = x - 11;
                            break;
                        }

                    case 94:
                        {
                            thisSpace.LeftOnly = x - 1;
                            thisSpace.RightOnly = x + 1;
                            thisSpace.UpLeft = x + 12;
                            thisSpace.UpRight = x + 13;
                            thisSpace.DownLeft = x - 12;
                            thisSpace.DownRight = x - 11;
                            break;
                        }

                    case 95:
                        {
                            thisSpace.LeftOnly = x - 1;
                            thisSpace.RightOnly = x + 1;
                            thisSpace.UpLeft = x + 12;
                            thisSpace.UpRight = x + 13;
                            thisSpace.DownLeft = x - 12;
                            thisSpace.DownRight = x - 11;
                            break;
                        }

                    case 96:
                        {
                            thisSpace.LeftOnly = x - 1;
                            thisSpace.RightOnly = x + 1;
                            thisSpace.UpLeft = x + 12;
                            thisSpace.UpRight = x + 13;
                            thisSpace.DownLeft = x - 12;
                            thisSpace.DownRight = x - 11;
                            thisSpace.WhatColor = EnumColorChoice.Yellow;
                            thisSpace.DestinationColor = EnumColorChoice.Purple;
                            break;
                        }

                    case 97:
                        {
                            thisSpace.LeftOnly = x - 1;
                            thisSpace.RightOnly = x + 1;
                            thisSpace.UpLeft = x + 12;
                            thisSpace.UpRight = x + 13;
                            thisSpace.DownLeft = x - 12;
                            thisSpace.DownRight = x - 11;
                            thisSpace.WhatColor = EnumColorChoice.Yellow;
                            thisSpace.DestinationColor = EnumColorChoice.Purple;
                            break;
                        }

                    case 98:
                        {
                            thisSpace.LeftOnly = x - 1;
                            thisSpace.UpLeft = x + 12;
                            thisSpace.UpRight = x + 13;
                            thisSpace.DownLeft = x - 12;
                            thisSpace.WhatColor = EnumColorChoice.Yellow;
                            thisSpace.DestinationColor = EnumColorChoice.Purple;
                            break;
                        }

                    case 99:
                        {
                            thisSpace.RightOnly = x + 1;
                            thisSpace.DownRight = x - 12;
                            thisSpace.WhatColor = EnumColorChoice.Red;
                            thisSpace.DestinationColor = EnumColorChoice.Gray;
                            break;
                        }

                    case 100:
                        {
                            thisSpace.LeftOnly = x - 1;
                            thisSpace.RightOnly = x + 1;
                            thisSpace.DownLeft = x - 13;
                            thisSpace.DownRight = x - 12;
                            thisSpace.WhatColor = EnumColorChoice.Red;
                            thisSpace.DestinationColor = EnumColorChoice.Gray;
                            break;
                        }

                    case 101:
                        {
                            thisSpace.LeftOnly = x - 1;
                            thisSpace.RightOnly = x + 1;
                            thisSpace.DownLeft = x - 13;
                            thisSpace.DownRight = x - 12;
                            thisSpace.WhatColor = EnumColorChoice.Red;
                            thisSpace.DestinationColor = EnumColorChoice.Gray;
                            break;
                        }

                    case 102:
                        {
                            thisSpace.LeftOnly = x - 1;
                            thisSpace.RightOnly = x + 1;
                            thisSpace.DownLeft = x - 13;
                            thisSpace.DownRight = x - 12;
                            thisSpace.WhatColor = EnumColorChoice.Red;
                            thisSpace.DestinationColor = EnumColorChoice.Gray;
                            break;
                        }

                    case 103:
                        {
                            thisSpace.LeftOnly = x - 1;
                            thisSpace.RightOnly = x + 1;
                            thisSpace.UpRight = x + 9;
                            thisSpace.DownLeft = x - 13;
                            thisSpace.DownRight = x - 12;
                            break;
                        }

                    case 104:
                        {
                            thisSpace.LeftOnly = x - 1;
                            thisSpace.RightOnly = x + 1;
                            thisSpace.UpLeft = x + 8;
                            thisSpace.UpRight = x + 9;
                            thisSpace.DownLeft = x - 13;
                            thisSpace.DownRight = x - 12;
                            break;
                        }

                    case 105:
                        {
                            thisSpace.LeftOnly = x - 1;
                            thisSpace.RightOnly = x + 1;
                            thisSpace.UpLeft = x + 8;
                            thisSpace.UpRight = x + 9;
                            thisSpace.DownLeft = x - 13;
                            thisSpace.DownRight = x - 12;
                            break;
                        }

                    case 106:
                        {
                            thisSpace.LeftOnly = x - 1;
                            thisSpace.RightOnly = x + 1;
                            thisSpace.UpLeft = x + 8;
                            thisSpace.UpRight = x + 9;
                            thisSpace.DownLeft = x - 13;
                            thisSpace.DownRight = x - 12;
                            break;
                        }

                    case 107:
                        {
                            thisSpace.LeftOnly = x - 1;
                            thisSpace.RightOnly = x + 1;
                            thisSpace.UpLeft = x + 8;
                            thisSpace.DownLeft = x - 13;
                            thisSpace.DownRight = x - 12;
                            break;
                        }

                    case 108:
                        {
                            thisSpace.LeftOnly = x - 1;
                            thisSpace.RightOnly = x + 1;
                            thisSpace.DownLeft = x - 13;
                            thisSpace.DownRight = x - 12;
                            thisSpace.WhatColor = EnumColorChoice.Yellow;
                            thisSpace.DestinationColor = EnumColorChoice.Purple;
                            break;
                        }

                    case 109:
                        {
                            thisSpace.LeftOnly = x - 1;
                            thisSpace.RightOnly = x + 1;
                            thisSpace.DownLeft = x - 13;
                            thisSpace.DownRight = x - 12;
                            thisSpace.WhatColor = EnumColorChoice.Yellow;
                            thisSpace.DestinationColor = EnumColorChoice.Purple;
                            break;
                        }

                    case 110:
                        {
                            thisSpace.LeftOnly = x - 1;
                            thisSpace.RightOnly = x + 1;
                            thisSpace.DownLeft = x - 13;
                            thisSpace.DownRight = x - 12;
                            thisSpace.WhatColor = EnumColorChoice.Yellow;
                            thisSpace.DestinationColor = EnumColorChoice.Purple;
                            break;
                        }

                    case 111:
                        {
                            thisSpace.LeftOnly = x - 1;
                            thisSpace.DownLeft = x - 13;
                            thisSpace.WhatColor = EnumColorChoice.Yellow;
                            thisSpace.DestinationColor = EnumColorChoice.Purple;
                            break;
                        }

                    case 112:
                        {
                            thisSpace.RightOnly = x + 1;
                            thisSpace.DownLeft = x - 9;
                            thisSpace.DownRight = x - 8;
                            thisSpace.UpRight = x + 4;
                            thisSpace.WhatColor = EnumColorChoice.Blue;
                            thisSpace.DestinationColor = EnumColorChoice.Green;
                            break;
                        }

                    case 113:
                        {
                            thisSpace.LeftOnly = x - 1;
                            thisSpace.RightOnly = x + 1;
                            thisSpace.UpLeft = x + 3;
                            thisSpace.UpRight = x + 4;
                            thisSpace.DownLeft = x - 9;
                            thisSpace.DownRight = x - 8;
                            thisSpace.WhatColor = EnumColorChoice.Blue;
                            thisSpace.DestinationColor = EnumColorChoice.Green;
                            break;
                        }

                    case 114:
                        {
                            thisSpace.LeftOnly = x - 1;
                            thisSpace.RightOnly = x + 1;
                            thisSpace.UpLeft = x + 3;
                            thisSpace.UpRight = x + 4;
                            thisSpace.DownLeft = x - 9;
                            thisSpace.DownRight = x - 8;
                            thisSpace.WhatColor = EnumColorChoice.Blue;
                            thisSpace.DestinationColor = EnumColorChoice.Green;
                            break;
                        }

                    case 115:
                        {
                            thisSpace.LeftOnly = x - 1;
                            thisSpace.UpLeft = x + 3;
                            thisSpace.DownLeft = x - 9;
                            thisSpace.DownRight = x - 8;
                            thisSpace.WhatColor = EnumColorChoice.Blue;
                            thisSpace.DestinationColor = EnumColorChoice.Green;
                            break;
                        }

                    case 116:
                        {
                            thisSpace.RightOnly = x + 1;
                            thisSpace.UpRight = x + 3;
                            thisSpace.DownLeft = x - 4;
                            thisSpace.DownRight = x - 3;
                            thisSpace.WhatColor = EnumColorChoice.Blue;
                            thisSpace.DestinationColor = EnumColorChoice.Green;
                            break;
                        }

                    case 117:
                        {
                            thisSpace.LeftOnly = x - 1;
                            thisSpace.RightOnly = x + 1;
                            thisSpace.UpLeft = x + 2;
                            thisSpace.UpRight = x + 3;
                            thisSpace.DownLeft = x - 4;
                            thisSpace.DownRight = x - 3;
                            thisSpace.WhatColor = EnumColorChoice.Blue;
                            thisSpace.DestinationColor = EnumColorChoice.Green;
                            break;
                        }

                    case 118:
                        {
                            thisSpace.LeftOnly = x - 1;
                            thisSpace.UpLeft = x + 2;
                            thisSpace.DownLeft = x - 4;
                            thisSpace.DownRight = x - 3;
                            thisSpace.WhatColor = EnumColorChoice.Blue;
                            thisSpace.DestinationColor = EnumColorChoice.Green;
                            break;
                        }

                    case 119:
                        {
                            thisSpace.RightOnly = x + 1;
                            thisSpace.UpRight = x + 2;
                            thisSpace.DownLeft = x - 3;
                            thisSpace.DownRight = x - 2;
                            thisSpace.WhatColor = EnumColorChoice.Blue;
                            thisSpace.DestinationColor = EnumColorChoice.Green;
                            break;
                        }

                    case 120:
                        {
                            thisSpace.LeftOnly = x - 1;
                            thisSpace.UpLeft = x + 1;
                            thisSpace.DownLeft = x - 3;
                            thisSpace.DownRight = x - 2;
                            thisSpace.WhatColor = EnumColorChoice.Blue;
                            thisSpace.DestinationColor = EnumColorChoice.Green;
                            break;
                        }

                    case 121:
                        {
                            thisSpace.DownLeft = x - 2;
                            thisSpace.DownRight = x - 1;
                            thisSpace.WhatColor = EnumColorChoice.Blue;
                            thisSpace.DestinationColor = EnumColorChoice.Green;
                            break;
                        }
                }
                thisSpace.SpaceNumber = x;
                _spaceList.Add(x, thisSpace);
            }
        }
    }
}
