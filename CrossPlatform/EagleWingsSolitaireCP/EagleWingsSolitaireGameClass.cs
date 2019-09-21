using BaseSolitaireClassesCP.BasicVMInterfaces;
using BaseSolitaireClassesCP.Cards;
using BaseSolitaireClassesCP.MainClasses;
using BasicGameFramework.Attributes;
using BasicGameFramework.DrawableListsViewModels;
using BasicGameFramework.Extensions;
using System.Linq;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace EagleWingsSolitaireCP
{
    [SingletonGame]
    public class EagleWingsSolitaireGameClass : SolitaireGameClass<EagleWingsSolitaireSaveInfo>
    {

        private readonly EagleWingsSolitaireViewModel _thisMod;
        public EagleWingsSolitaireGameClass(IBasicSolitaireVM thisMod) : base(thisMod)
        {
            _thisMod = (EagleWingsSolitaireViewModel)thisMod;
        }

        protected override SolitaireCard CardPlayed()
        {
            var thisCard = base.CardPlayed();
            if (thisCard.Deck > 0)
            {
                if (_thisMod.Heel1!.IsSelected)
                    return new SolitaireCard();
                return thisCard;
            }
            thisCard = _thisMod.Heel1!.DrawCard();
            thisCard.IsSelected = false;
            return thisCard;
        }

        protected override async Task<bool> HasOtherAsync(int pile)
        {
            if (_thisMod.Heel1!.CardsLeft() != 1 || _thisMod.Heel1.IsSelected == false)
                return await base.HasOtherAsync(pile);
            int wastes = _thisMod.WastePiles1!.OneSelected();
            if (wastes > -1)
            {
                await _thisMod.ShowGameMessageAsync("Can choose either the waste pile or from heel; but not both");
                return true;
            }
            await _thisMod.ShowGameMessageAsync("Cannot play from heel to wing since its the last card");
            return true;
        }
        public async Task HeelToMainAsync()
        {
            if (_thisMod.WastePiles1!.OneSelected() > -1)
                return;
            int index = ValidMainColumn(_thisMod.Heel1!.RevealCard());
            if (index == -1)
            {
                _thisMod.Heel1.IsSelected = false;
                return;
            }
            var thisCard = _thisMod.Heel1.DrawCard();
            await FinishAddingToMainPileAsync(index, thisCard);
        }

        protected override void AfterShuffleCards()
        {
            var thisCol = CardList.Take(13).ToRegularDeckDict();
            CardList!.RemoveRange(0, 13);
            _thisMod.Heel1!.DeckStyle = DeckViewModel<SolitaireCard>.EnumStyleType.Unknown;
            _thisMod.Heel1.OriginalList(thisCol);
            thisCol = CardList.Take(1).ToRegularDeckDict();
            CardList.RemoveRange(0, 1);
            AfterShuffle(thisCol);
            CardList.Clear(); //try this way.  hopefully i won't regret this.  because otherwise, the new game goes not work.
        }

        protected async override Task ContinueOpenSavedAsync()
        {
            //anything else you need will be here
            var newList = SaveRoot.HeelList.GetNewObjectListFromDeckList(DeckList);
            _thisMod.Heel1!.OriginalList(newList);
            if (newList.Count == 1)
                _thisMod.Heel1.DeckStyle = DeckViewModel<SolitaireCard>.EnumStyleType.AlwaysKnown;
            else
                _thisMod.Heel1.DeckStyle = DeckViewModel<SolitaireCard>.EnumStyleType.Unknown;
            await base.ContinueOpenSavedAsync();
        }
        protected async override Task FinishSaveAsync()
        {
            //anything else to finish save will be here.
            SaveRoot.HeelList = _thisMod.Heel1!.GetCardIntegers();
            await base.FinishSaveAsync();
        }

    }
}
