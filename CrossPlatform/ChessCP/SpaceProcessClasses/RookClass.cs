using BasicGameFrameworkLibrary.GameGraphicsCP.CheckersChessHelpers;
using ChessCP.Data;
using ChessCP.Logic;
using CommonBasicStandardLibraries.CollectionClasses;
namespace ChessCP.SpaceProcessClasses
{
    public class RookClass : IChessMan
    {
        public int Row { get; set; }
        public int Column { get; set; }
        public int Player { get; set; }
        public bool IsReversed { get; set; }
        public EnumPieceType PieceCategory { get; set; }

        public CustomBasicList<CheckersChessVector> GetValidMoves()
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
            CustomBasicList<CheckersChessVector> output = new CustomBasicList<CheckersChessVector>();
            do
            {
                checkRow -= 1;
                c = GameBoardGraphicsCP.GetSpace(checkRow, checkColumn);
                if (c == null)
                    break;
                if (c.PlayerOwns == Player)
                    break;
                // can add to list
                output.Add(new CheckersChessVector(checkRow, checkColumn));
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
                output.Add(new CheckersChessVector(checkRow, checkColumn));
                if (c.PlayerOwns > 0)
                    break;
            }
            while (true); // because its after the ends// can't go further
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
                output.Add(new CheckersChessVector(checkRow, checkColumn));
                if (c.PlayerOwns > 0)
                    break;
            }
            while (true);// because its after the ends// can't go further
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
                output.Add(new CheckersChessVector(checkRow, checkColumn));
                if (c.PlayerOwns > 0)
                    break;
            }
            while (true); // because its after the ends// can't go further
            return output;
        }
    }
}