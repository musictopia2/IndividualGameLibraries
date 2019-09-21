using BasicGameFramework.Attributes;
using BasicGameFramework.BasicDrawables.Dictionary;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using CommonBasicStandardLibraries.Exceptions;
namespace LifeCardGameCP
{
    [SingletonGame]
    public class GlobalClass
    {
        public LifeCardGameCardInformation? CardChosen { get; set; }
        public int PlayerChosen { get; set; }
        public DeckRegularDict<LifeCardGameCardInformation>? TradeList { get; set; } = new DeckRegularDict<LifeCardGameCardInformation>();
        private readonly LifeCardGameMainGameClass _mainGame;
        public GlobalClass(LifeCardGameMainGameClass mainGame)
        {
            _mainGame = mainGame;
        }
        public LifeCardGamePlayerItem PlayerWithCard(LifeCardGameCardInformation thisCard)
        {
            var tempList = _mainGame.PlayerList.ToCustomBasicList();
            tempList.RemoveSpecificItem(_mainGame.SingleInfo!); //try this way
            foreach (var thisPlayer in tempList)
            {
                if (thisPlayer.LifeStory!.HandList.ObjectExist(thisCard.Deck))
                    return thisPlayer;
            }
            throw new BasicBlankException("No player has the card");
        }
        public void CreateLifeStoryPile(LifeCardGameViewModel thisMod, LifeCardGamePlayerItem thisPlayer)
        {
            thisPlayer.LifeStory = new LifeStoryHand(thisMod, thisPlayer.Id);
            thisPlayer.LifeStory.Text = thisPlayer.NickName; //hopefully no need for enabled part (?)
        }
        public int OtherCardSelected()
        {
            var tempList = _mainGame.PlayerList!.AllPlayersExceptForCurrent();
            int decks = 0;
            int tempDeck;
            foreach (var thisPlayer in tempList)
            {
                tempDeck = thisPlayer.LifeStory!.ObjectSelected();
                if (tempDeck > 0)
                {
                    if (decks > 0)
                        return 0;
                }
                decks = tempDeck;
            }
            return decks;
        }
    }
}