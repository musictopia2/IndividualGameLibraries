using BasicGameFramework.RegularDeckOfCards;
using CommonBasicStandardLibraries.CollectionClasses;
namespace GoFishCP
{
    public class GoFishComputerAI
    {
        public EnumCardValueList NumberToAsk(GoFishSaveInfo saveRoot)
        {
            GoFishPlayerItem singleInfo = saveRoot.PlayerList.GetWhoPlayer();
            return singleInfo.MainHandList.GetRandomItem().Value;
        }
        public CustomBasicList<RegularSimpleCard> PairToPlay(GoFishSaveInfo saveRoot)
        {
            GoFishPlayerItem singleInfo = saveRoot.PlayerList.GetWhoPlayer();
            CustomBasicList<RegularSimpleCard> output = new CustomBasicList<RegularSimpleCard>();
            foreach (var firstCard in singleInfo.MainHandList)
            {
                foreach (var secondCard in singleInfo.MainHandList)
                {
                    if (firstCard.Deck != secondCard.Deck)
                    {
                        if (firstCard.Value == secondCard.Value)
                        {
                            output.Add(firstCard);
                            output.Add(secondCard);
                            return output;
                        }
                    }
                }
            }
            return new CustomBasicList<RegularSimpleCard>();
        }
    }
}
