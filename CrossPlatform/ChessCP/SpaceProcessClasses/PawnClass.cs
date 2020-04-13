using BasicGameFrameworkLibrary.GameGraphicsCP.CheckersChessHelpers;
using ChessCP.Data;
using ChessCP.Logic;
using CommonBasicStandardLibraries.CollectionClasses;
namespace ChessCP.SpaceProcessClasses
{
    public class PawnClass : IChessMan
    {
        public int Row { get; set; }
        public int Column { get; set; }
        public int Player { get; set; }
        public bool IsReversed { get; set; }
        public EnumPieceType PieceCategory { get; set; }

        public CustomBasicList<CheckersChessVector> GetValidMoves()
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
}
