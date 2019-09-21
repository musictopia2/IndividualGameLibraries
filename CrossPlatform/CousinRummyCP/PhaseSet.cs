using BasicGameFramework.BasicDrawables.Dictionary;
using BasicGameFramework.Extensions;
using BasicGameFramework.RegularDeckOfCards;
using BasicGameFramework.SpecializedGameTypes.RummyClasses;
using BasicGameFramework.ViewModelInterfaces;
using System.Linq;
namespace CousinRummyCP
{
    public class PhaseSet : SetInfo<EnumSuitList, EnumColorList, RegularRummyCard, SavedSet>
    {
        private readonly CousinRummyMainGameClass _mainGame;
        public PhaseSet(IBasicGameVM thisMod) : base(thisMod)
        {
            _mainGame = thisMod.MainContainer!.Resolve<CousinRummyMainGameClass>();
        }
        public override void LoadSet(SavedSet payLoad)
        {
            HandList.ReplaceRange(payLoad.CardList);
        }
        public override SavedSet SavedSet()
        {
            SavedSet output = new SavedSet();
            output.CardList = HandList.ToRegularDeckDict();
            return output;
        }
        public void AddCard(RegularRummyCard thisCard)
        {
            UpdateCard(thisCard);
            HandList.Add(thisCard);
        }
        private void UpdateCard(RegularRummyCard thisCard)
        {
            thisCard.IsSelected = false;
            thisCard.Drew = false;
            thisCard.Player = _mainGame.WhoTurn;
        }
        public void CreateSet(IDeckDict<RegularRummyCard> thisList)
        {
            thisList.ForEach(thisCard => UpdateCard(thisCard));
            HandList.ReplaceRange(thisList);
        }
        public int PointsReceived(int player)
        {
            _mainGame.ModifyCards(HandList);
            return HandList.Where(items => items.Player == player).Sum(items => items.Points);
        }
        public bool CanExpand(RegularRummyCard thisCard)
        {
            if (thisCard.IsObjectWild == true)
                return true;
            EnumCardValueList numberNeeded = HandList.First(items => items.IsObjectWild == false).Value;
            return thisCard.Value == numberNeeded;
        }
    }
}