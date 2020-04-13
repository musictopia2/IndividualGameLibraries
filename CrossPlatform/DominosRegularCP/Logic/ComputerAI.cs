using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.RandomGenerator;
using CommonBasicStandardLibraries.CollectionClasses;
using System;
using System.Linq;
namespace DominosRegularCP.Logic
{
    public static class ComputerAI
    {
        public struct MoveInfo : IComparable<MoveInfo>
        {
            public int Deck;
            public int WhichOne;
            public int Points;
            int IComparable<MoveInfo>.CompareTo(MoveInfo other)
            {
                return other.Points.CompareTo(Points); //to sort descending.
            }
        }
        public static int DominoToPlay(out int whichOne, DominosRegularMainGameClass mainGame, GameBoardCP gameboard)
        {
            whichOne = 0; //until proven otherwise.
            CustomBasicList<MoveInfo> moveList = new CustomBasicList<MoveInfo>();
            mainGame.SingleInfo!.MainHandList.ForEach(thisDomino =>
            {
                for (int x = 1; x <= 2; x++)
                {
                    if (gameboard.IsValidMove(x, thisDomino))
                    {
                        MoveInfo thisMove = new MoveInfo();
                        thisMove.Deck = thisDomino.Deck;
                        thisMove.WhichOne = x;
                        thisMove.Points = thisDomino.Points;
                        moveList.Add(thisMove);
                    }
                }
            });
            if (moveList.Count == 0)
                return 0;
            moveList.Sort();
            var finalMove = moveList.First();
            int howMany = moveList.Count(items => items.Deck == finalMove.Deck);
            if (howMany == 1)
            {
                whichOne = finalMove.WhichOne;
                return finalMove.Deck;
            }
            RandomGenerator rs = mainGame.MainContainer.Resolve<RandomGenerator>();
            whichOne = rs.GetRandomNumber(2);
            return finalMove.Deck;
        }
    }
}
