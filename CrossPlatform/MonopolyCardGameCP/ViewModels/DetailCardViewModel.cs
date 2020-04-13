using CommonBasicStandardLibraries.MVVMFramework.ViewModels;
using MonopolyCardGameCP.Cards;
using MonopolyCardGameCP.Logic;

namespace MonopolyCardGameCP.ViewModels
{
    public class DetailCardViewModel : ObservableObject
    {
        public MonopolyCardGameCardInformation CurrentCard { get; set; } //i think for all, the data context will be this one, not the entire viewmodel.
        public IChangeCard? Change;
        private void NotifyCard()
        {
            if (Change == null)
                return;
            Change.ShowChangedCard();
        }
        public void Clear()
        {
            if (CurrentCard.Deck == 0)
                return;
            CurrentCard = new MonopolyCardGameCardInformation();
            NotifyCard();
        }
        public void AdditionalInfo(int deck)
        {
            if (deck == CurrentCard.Deck)
                return;
            CurrentCard = new MonopolyCardGameCardInformation();
            CurrentCard.Populate(deck); //needs to be specific so it can run the proper routines.
            NotifyCard();
        }
        public DetailCardViewModel()
        {
            CurrentCard = new MonopolyCardGameCardInformation();
            CurrentCard.IsUnknown = true;
        }
    }
}
