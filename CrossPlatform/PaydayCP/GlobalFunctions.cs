using BasicGameFramework.Attributes;
using BasicGameFramework.BasicDrawables.Dictionary;
using BasicGameFramework.GameGraphicsCP.GameboardPositionHelpers;
using CommonBasicStandardLibraries.CollectionClasses;
namespace PaydayCP
{
    [SingletonGame]
    public class GlobalFunctions
    {
        readonly PaydayMainGameClass _mainGame;
        public GlobalFunctions(PaydayMainGameClass mainGame)
        {
            _mainGame = mainGame;
        }
        public CustomBasicList<GameSpace> PrivateSpaceList { get; set; } = new CustomBasicList<GameSpace>();
        public PositionPieces? Pos;
        public CardInformation GetCard(int deck)
        {
            CardInformation output;
            if (deck <= 24)
            {
                output = new DealCard();
                output.Populate(deck);
                return output;
            }
            output = new MailCard();
            output.Populate(deck);
            return output;
        }
        public void RemoveOutCards(IDeckDict<CardInformation> listToRemove)
        {
            _mainGame.SaveRoot!.OutCards.RemoveGivenList(listToRemove); //hopefully this simple.
        }
        public void AddOutCard(CardInformation thisCard)
        {
            _mainGame.SaveRoot!.OutCards.Add(thisCard);
        }
    }
}