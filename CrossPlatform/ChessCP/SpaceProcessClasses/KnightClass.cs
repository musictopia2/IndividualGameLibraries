using BasicGameFrameworkLibrary.GameGraphicsCP.CheckersChessHelpers;
using ChessCP.Data;
using ChessCP.Logic;
using CommonBasicStandardLibraries.CollectionClasses;
namespace ChessCP.SpaceProcessClasses
{
    public class KnightClass : IChessMan
    {
        public int Row { get; set; }
        public int Column { get; set; }
        public int Player { get; set; }
        public bool IsReversed { get; set; }
        public EnumPieceType PieceCategory { get; set; }

        public CustomBasicList<CheckersChessVector> GetValidMoves()
        {
            CustomBasicList<CheckersChessVector> possibleList = new CustomBasicList<CheckersChessVector>
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
            CustomBasicList<CheckersChessVector> output = new CustomBasicList<CheckersChessVector>();
            SpaceCP? c;
            foreach (var possible in possibleList)
            {
                c = GameBoardGraphicsCP.GetSpace(possible.Row, possible.Column);
                if (c != null)
                {
                    if (c.PlayerOwns == 0 || c.PlayerOwns != Player)
                        output.Add(possible);
                }
            }
            return output;
        }
    }
}
