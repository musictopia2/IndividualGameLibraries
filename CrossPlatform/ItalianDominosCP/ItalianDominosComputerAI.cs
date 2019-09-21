namespace ItalianDominosCP
{
    public static class ItalianDominosComputerAI
    {
        public static int Play(ItalianDominosMainGameClass thisGame)
        {
            ItalianDominosPlayerItem singleInfo = thisGame.SingleInfo!;
            foreach (var thisDomino in singleInfo.MainHandList)
            {
                if (thisDomino.FirstNum + thisDomino.SecondNum == thisGame.SaveRoot!.NextNumber)
                    return thisDomino.Deck;
            }
            return singleInfo.MainHandList.GetRandomItem().Deck;
        }
    }
}
