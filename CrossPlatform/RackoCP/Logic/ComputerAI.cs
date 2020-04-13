using RackoCP.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RackoCP.Logic
{
    public static class ComputerAI
    {
        public static int PickUp(RackoMainGameClass game, RackoVMData model)
        {
            return ComputerUse(game, model.Pile1.GetCardInfo().Deck);
        }
        private static int ComputerUse(RackoMainGameClass game, int card)
        {
            int maxDiffs;
            maxDiffs = game.PlayerList.Count() + 2;
            int possNum = 0;
            int previousCard = 0;
            int prevPossNum = 0;
            game.SingleInfo!.MainHandList.ForEach(x =>
            {
                possNum += maxDiffs;
                if (x.Value > prevPossNum && x.Value < possNum && x.WillKeep == false)
                    x.WillKeep = true;
                prevPossNum = possNum;
            });
            possNum = 0;
            prevPossNum = 0;
            int x = 0;
            foreach (var y in game.SingleInfo.MainHandList)
            {
                x++;
                possNum += maxDiffs;
                if (y.WillKeep == false && card >= possNum && card <= prevPossNum)
                    return x;
                if (y.WillKeep == false && previousCard > possNum && card <= possNum)
                    return x;
                prevPossNum = possNum;
                previousCard = y.Value;
            }
            return 0;
        }
        public static int CardToPlay(RackoMainGameClass game, RackoVMData model)
        {
            return ComputerUse(game, model.OtherPile!.GetCardInfo().Deck);
        }
    }
}
