using CommonBasicStandardLibraries.Exceptions;
using System.Linq;
namespace BackgammonCP
{
    public static class ComputerAI
    {
        public static (int spaceFrom, int spaceTo) GetComputerMove(BackgammonMainGameClass mainGame)
        {
            int tempFrom;
            int tempTo;
            if (mainGame.ThisGlobal!.MoveList.Count == 0)
                throw new BasicBlankException("No move was possible");
            if (mainGame.ThisGlobal.MoveList.Count == 1)
            {
                tempFrom = mainGame.ThisGlobal.MoveList.First().SpaceFrom;
                tempTo = mainGame.ThisGlobal.MoveList.First().SpaceTo;
                return (tempFrom, tempTo);
            }
            var thisMove = mainGame.ThisGlobal.MoveList.GetRandomItem();
            return (thisMove.SpaceFrom, thisMove.SpaceTo);
        }
    }
}