using CommonBasicStandardLibraries.MVVMFramework.ViewModels;
using FluxxCP.Cards;
using FluxxCP.Data;

namespace FluxxCP.UICP
{
    public class DetailCardObservable : ObservableObject
    {
        public FluxxCardInformation CurrentCard { get; set; } //i think for all, the data context will be this one, not the entire viewmodel.
        public IChangeCard? Card;
        private void NotifyCard()
        {
            if (Card == null)
                return;
            Card.ShowChangedCard();
        }
        public void ResetCard()
        {
            CurrentCard = new FluxxCardInformation();
            CurrentCard.IsUnknown = true;
            NotifyCard();
        }
        public void ShowCard<F>(F thisCard)
            where F : FluxxCardInformation, new()
        {
            if (thisCard.Deck == CurrentCard.Deck)
                return;
            CurrentCard = FluxxDetailClass.GetNewCard(thisCard.Deck); //try this way.
            CurrentCard.Populate(thisCard.Deck); //needs to be specific so it can run the proper routines.
            NotifyCard();
        }
        public DetailCardObservable()
        {
            CurrentCard = new FluxxCardInformation();
            CurrentCard.IsUnknown = true;
        }
    }
}
