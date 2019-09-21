using CommonBasicStandardLibraries.MVVMHelpers;
namespace FluxxCP
{
    public class DetailCardViewModel : ObservableObject
    {
        public FluxxCardInformation CurrentCard { get; set; } //i think for all, the data context will be this one, not the entire viewmodel.
        public IChangeCard? ThisChange;
        private void NotifyCard()
        {
            if (ThisChange == null)
                return;
            ThisChange.ShowChangedCard();
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
            //CurrentCard = new F();
            CurrentCard = FluxxDetailClass.GetNewCard(thisCard.Deck); //try this way.
            CurrentCard.Populate(thisCard.Deck); //needs to be specific so it can run the proper routines.
            NotifyCard();
        }
        public DetailCardViewModel()
        {
            CurrentCard = new FluxxCardInformation();
            CurrentCard.IsUnknown = true;
        }
    }
}