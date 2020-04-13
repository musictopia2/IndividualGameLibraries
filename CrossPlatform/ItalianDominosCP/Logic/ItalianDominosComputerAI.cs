using ItalianDominosCP.Data;
namespace ItalianDominosCP.Logic
{
    public static class ItalianDominosComputerAI
    {
        public static int Play(ItalianDominosMainGameClass game)
        {
            ItalianDominosPlayerItem singleInfo = game.SingleInfo!;
            foreach (var thisDomino in singleInfo.MainHandList)
            {
                if (thisDomino.FirstNum + thisDomino.SecondNum == game.SaveRoot!.NextNumber)
                    return thisDomino.Deck;
            }
            return singleInfo.MainHandList.GetRandomItem().Deck;
        }
    }
}