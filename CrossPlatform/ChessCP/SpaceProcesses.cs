using BasicGameFramework.GameGraphicsCP.CheckersChessHelpers;
using CommonBasicStandardLibraries.CollectionClasses;
namespace ChessCP
{
    public abstract class ChessMan
    {
        public int Row { get; set; }
        public int Column { get; set; }
        public int Player { get; set; } // this is the player who owns it if any.
        public bool IsReversed { get; set; }
        public EnumPieceType PieceCategory { get; set; }
        public abstract CustomBasicList<CheckersChessVector> GetValidMoves(); // i think the point will represent a valid move.
    }

    public class PawnClass : ChessMan
    {
        public override CustomBasicList<CheckersChessVector> GetValidMoves()
        {
            SpaceCP? c;
            SpaceCP c2;
            CustomBasicList<CheckersChessVector> thisList = new CustomBasicList<CheckersChessVector>();
            if (IsReversed == false)
            {
                // diagonal left

                c = GameBoardGraphicsCP.GetSpace(Row - 1, Column - 1)!;
                if (c != null)
                {
                    // this means there is something there.

                    if (c.PlayerOwns > 0 && c.PlayerOwns != Player)
                        // this means you can do this one.
                        thisList.Add(new CheckersChessVector(Row - 1, Column - 1));
                }

                // diagonal right

                c = GameBoardGraphicsCP.GetSpace(Row - 1, Column + 1)!;
                if (c != null)
                {
                    if (c.PlayerOwns > 0 && c.PlayerOwns != Player)
                        thisList.Add(new CheckersChessVector(Row - 1, Column + 1));
                }

                // middle
                c = GameBoardGraphicsCP.GetSpace(Row - 1, Column)!;
                if (c != null)
                {
                    if (c.PlayerOwns == 0)
                        // if anybody is in the middle, you can't do it.
                        thisList.Add(new CheckersChessVector(Row - 1, Column));
                    // first turn
                    if (Row == 7)
                    {
                        // this means first turn so its possible.
                        if (c.PlayerOwns == 0)
                        {
                            c2 = GameBoardGraphicsCP.GetSpace(Row - 2, Column)!;
                            // has to find something this time.
                            if (c2.PlayerOwns == 0)
                                thisList.Add(new CheckersChessVector(Row - 2, Column));
                        }
                    }
                }
                
            }
            else
            {
                c = GameBoardGraphicsCP.GetSpace(Row + 1, Column - 1)!;
                if (c != null)
                {
                    // this means there is something there.

                    if (c.PlayerOwns > 0 && c.PlayerOwns != Player)
                        // this means you can do this one.
                        thisList.Add(new CheckersChessVector(Row + 1, Column - 1));
                }

                // diagonal right

                c = GameBoardGraphicsCP.GetSpace(Row + 1, Column + 1)!;
                if (c != null)
                {
                    if (c.PlayerOwns > 0 && c.PlayerOwns != Player)
                        thisList.Add(new CheckersChessVector(Row + 1, Column + 1));
                }

                // middle
                c = GameBoardGraphicsCP.GetSpace(Row + 1, Column)!;
                if (c != null)
                {
                    if (c.PlayerOwns == 0)
                        // if anybody is in the middle, you can't do it.
                        thisList.Add(new CheckersChessVector(Row + 1, Column));
                    // first turn
                    if (Row == 2)
                    {
                        // this means first turn so its possible.
                        if (c.PlayerOwns == 0)
                        {
                            c2 = GameBoardGraphicsCP.GetSpace(Row + 2, Column)!;
                            // has to find something this time.
                            if (c2.PlayerOwns == 0)
                                thisList.Add(new CheckersChessVector(Row + 2, Column));
                        }
                    }
                }
                
            }
            return thisList;
        }
    }

    public class KnightClass : ChessMan// this is the horse.
    {
        public override CustomBasicList<CheckersChessVector> GetValidMoves()
        {
            CustomBasicList<CheckersChessVector> PossibleList = new CustomBasicList<CheckersChessVector>
                {
                    new CheckersChessVector(Row + 1, Column + 2),
                    new CheckersChessVector(Row - 1, Column + 2),
                    new CheckersChessVector(Row + 1, Column - 2),
                    new CheckersChessVector(Row - 1, Column - 2),
                    new CheckersChessVector(Row + 2, Column + 1),
                    new CheckersChessVector(Row - 2, Column + 1),
                    new CheckersChessVector(Row + 2, Column - 1),
                    new CheckersChessVector(Row - 2, Column - 1)
                };
            CustomBasicList<CheckersChessVector> RealList = new CustomBasicList<CheckersChessVector>();
            SpaceCP? c;
            foreach (var ThisPossible in PossibleList)
            {
                c = GameBoardGraphicsCP.GetSpace(ThisPossible.Row, ThisPossible.Column);
                if (c != null)
                {
                    if (c.PlayerOwns == 0 || c.PlayerOwns != Player)
                        RealList.Add(ThisPossible);
                }
            }
            return RealList;
        }
    }

    public class RookClass : ChessMan
    {
        public override CustomBasicList<CheckersChessVector> GetValidMoves()
        {

            // has to check 4 directions.  the moment it finds one, it stops.
            // try up first

            int CheckRow;
            int CheckColumn;

            void UpdateChecker()
            {
                CheckRow = Row;
                CheckColumn = Column;
            };
            SpaceCP? c;
            UpdateChecker();
            // up processes
            CustomBasicList<CheckersChessVector> FinalList = new CustomBasicList<CheckersChessVector>();
            do
            {
                CheckRow -= 1;
                c = GameBoardGraphicsCP.GetSpace(CheckRow, CheckColumn);
                if (c == null)
                    break;
                if (c.PlayerOwns == Player)
                    break;
                // can add to list
                FinalList.Add(new CheckersChessVector(CheckRow, CheckColumn));
                if (c.PlayerOwns > 0)
                    break;
            }
            while (true); // because its after the ends// can't go further
            UpdateChecker();
            do
            {
                CheckRow += 1;
                c = GameBoardGraphicsCP.GetSpace(CheckRow, CheckColumn);
                if (c == null)
                    break;
                if (c.PlayerOwns == Player)
                    break;
                // can add to list
                FinalList.Add(new CheckersChessVector(CheckRow, CheckColumn));
                if (c.PlayerOwns > 0)
                    break;
            }
            while (true); // because its after the ends// can't go further
            UpdateChecker();
            do
            {
                CheckColumn -= 1;
                c = GameBoardGraphicsCP.GetSpace(CheckRow, CheckColumn);
                if (c == null)
                    break;
                if (c.PlayerOwns == Player)
                    break;
                // can add to list
                FinalList.Add(new CheckersChessVector(CheckRow, CheckColumn));
                if (c.PlayerOwns > 0)
                    break;
            }
            while (true);// because its after the ends// can't go further
            UpdateChecker();
            do
            {
                CheckColumn += 1;
                c = GameBoardGraphicsCP.GetSpace(CheckRow, CheckColumn);
                if (c == null)
                    break;
                if (c.PlayerOwns == Player)
                    break;
                // can add to list
                FinalList.Add(new CheckersChessVector(CheckRow, CheckColumn));
                if (c.PlayerOwns > 0)
                    break;
            }
            while (true); // because its after the ends// can't go further
            return FinalList;
        }
    }

    public class BishopClass : ChessMan
    {
        public override CustomBasicList<CheckersChessVector> GetValidMoves()
        {
            // has to check 4 directions.  the moment it finds one, it stops.
            // try up first

            int checkRow;
            int checkColumn;
            void UpdateChecker()
            {
                checkRow = Row;
                checkColumn = Column;
            };
            SpaceCP? c;
            UpdateChecker();
            // up processes
            CustomBasicList<CheckersChessVector> finalList = new CustomBasicList<CheckersChessVector>();
            do
            {
                checkRow -= 1;
                checkColumn -= 1;
                c = GameBoardGraphicsCP.GetSpace(checkRow, checkColumn);
                if (c == null)
                    break;
                if (c.PlayerOwns == Player)
                    break;
                // can add to list
                finalList.Add(new CheckersChessVector(checkRow, checkColumn));
                if (c.PlayerOwns > 0)
                    break;
            }
            while (true); // because its after the ends// can't go further
            UpdateChecker();
            do
            {
                checkRow += 1;
                checkColumn += 1;
                c = GameBoardGraphicsCP.GetSpace(checkRow, checkColumn);
                if (c == null)
                    break;
                if (c.PlayerOwns == Player)
                    break;
                // can add to list
                finalList.Add(new CheckersChessVector(checkRow, checkColumn));
                if (c.PlayerOwns > 0)
                    break;
            }
            while (true); // because its after the ends// can't go further
            UpdateChecker();
            do
            {
                checkRow += 1;
                checkColumn -= 1;
                c = GameBoardGraphicsCP.GetSpace(checkRow, checkColumn);
                if (c == null)
                    break;
                if (c.PlayerOwns == Player)
                    break;
                // can add to list
                finalList.Add(new CheckersChessVector(checkRow, checkColumn));
                if (c.PlayerOwns > 0)
                    break;
            }
            while (true); // because its after the ends// can't go further
            UpdateChecker();
            do
            {
                checkRow -= 1;
                checkColumn += 1;
                c = GameBoardGraphicsCP.GetSpace(checkRow, checkColumn);
                if (c == null)
                    break;
                if (c.PlayerOwns == Player)
                    break;
                // can add to list
                finalList.Add(new CheckersChessVector(checkRow, checkColumn));
                if (c.PlayerOwns > 0)
                    break;
            }
            while (true);// because its after the ends// can't go further
            return finalList;
        }
    }

    public class QueenClass : ChessMan
    {
        public override CustomBasicList<CheckersChessVector> GetValidMoves()
        {
            int checkRow;
            int checkColumn;

            void UpdateChecker()
            {
                checkRow = Row;
                checkColumn = Column;
            };
            SpaceCP? c;
            UpdateChecker();
            // up processes
            CustomBasicList<CheckersChessVector> finalList = new CustomBasicList<CheckersChessVector>();
            do
            {
                checkRow -= 1;
                c = GameBoardGraphicsCP.GetSpace(checkRow, checkColumn);
                if (c == null)
                    break;
                if (c.PlayerOwns == Player)
                    break;
                // can add to list
                finalList.Add(new CheckersChessVector(checkRow, checkColumn));
                if (c.PlayerOwns > 0)
                    break;
            }
            while (true); // because its after the ends// can't go further
            UpdateChecker();
            do
            {
                checkRow += 1;
                c = GameBoardGraphicsCP.GetSpace(checkRow, checkColumn);
                if (c == null)
                    break;
                if (c.PlayerOwns == Player)
                    break;
                // can add to list
                finalList.Add(new CheckersChessVector(checkRow, checkColumn));
                if (c.PlayerOwns > 0)
                    break;
            }
            while (true);

            UpdateChecker();
            do
            {
                checkColumn -= 1;
                c = GameBoardGraphicsCP.GetSpace(checkRow, checkColumn);
                if (c == null)
                    break;
                if (c.PlayerOwns == Player)
                    break;
                // can add to list
                finalList.Add(new CheckersChessVector(checkRow, checkColumn));
                if (c.PlayerOwns > 0)
                    break;
            }
            while (true);
            UpdateChecker();
            do
            {
                checkColumn += 1;
                c = GameBoardGraphicsCP.GetSpace(checkRow, checkColumn);
                if (c == null)
                    break;
                if (c.PlayerOwns == Player)
                    break;
                // can add to list
                finalList.Add(new CheckersChessVector(checkRow, checkColumn));
                if (c.PlayerOwns > 0)
                    break;
            }
            while (true);

            UpdateChecker();
            do
            {
                checkRow -= 1;
                checkColumn -= 1;
                c = GameBoardGraphicsCP.GetSpace(checkRow, checkColumn);
                if (c == null)
                    break;
                if (c.PlayerOwns == Player)
                    break;
                // can add to list
                finalList.Add(new CheckersChessVector(checkRow, checkColumn));
                if (c.PlayerOwns > 0)
                    break;
            }
            while (true);
            UpdateChecker();
            do
            {
                checkRow += 1;
                checkColumn += 1;
                c = GameBoardGraphicsCP.GetSpace(checkRow, checkColumn);
                if (c == null)
                    break;
                if (c.PlayerOwns == Player)
                    break;
                // can add to list
                finalList.Add(new CheckersChessVector(checkRow, checkColumn));
                if (c.PlayerOwns > 0)
                    break;
            }
            while (true);
            UpdateChecker();
            do
            {
                checkRow += 1;
                checkColumn -= 1;
                c = GameBoardGraphicsCP.GetSpace(checkRow, checkColumn);
                if (c == null)
                    break;
                if (c.PlayerOwns == Player)
                    break;
                // can add to list
                finalList.Add(new CheckersChessVector(checkRow, checkColumn));
                if (c.PlayerOwns > 0)
                    break;
            }
            while (true);
            UpdateChecker();
            do
            {
                checkRow -= 1;
                checkColumn += 1;
                c = GameBoardGraphicsCP.GetSpace(checkRow, checkColumn);
                if (c == null)
                    break;
                if (c.PlayerOwns == Player)
                    break;
                // can add to list
                finalList.Add(new CheckersChessVector(checkRow, checkColumn));
                if (c.PlayerOwns > 0)
                    break;
            }
            while (true);
            return finalList;
        }
    }
    public class KingClass : ChessMan// later has to worry about the checks part of it.
    {
        public override CustomBasicList<CheckersChessVector> GetValidMoves()
        {
            CustomBasicList<CheckersChessVector> possibleList = new CustomBasicList<CheckersChessVector>
            {
                new CheckersChessVector(Row + 1, Column + 1),
                new CheckersChessVector(Row - 1, Column - 1),
                new CheckersChessVector(Row + 1, Column),
                new CheckersChessVector(Row - 1, Column),
                new CheckersChessVector(Row, Column + 1),
                new CheckersChessVector(Row, Column - 1),
                new CheckersChessVector(Row - 1, Column + 1),
                new CheckersChessVector(Row + 1, Column - 1)
            };
            CustomBasicList<CheckersChessVector> finalList = new CustomBasicList<CheckersChessVector>();
            SpaceCP? c;
            foreach (var thisPos in possibleList)
            {
                c = GameBoardGraphicsCP.GetSpace(thisPos.Row, thisPos.Column);
                if (c != null)
                {
                    if (c.PlayerOwns != Player)
                        // can't be yourself period.
                        finalList.Add(thisPos);
                }
            }
            return finalList;
        }
    }
}