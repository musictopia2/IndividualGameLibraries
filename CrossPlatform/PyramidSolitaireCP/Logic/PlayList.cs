using BasicGameFrameworkLibrary.CommandClasses;
using BasicGameFrameworkLibrary.DrawableListsViewModels;
using BasicGameFrameworkLibrary.SolitaireClasses.Cards;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.Messenging;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using PyramidSolitaireCP.EventModels;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace PyramidSolitaireCP.Logic
{
    public class PlayList : GameBoardObservable<SolitaireCard>
    {
        private readonly IEventAggregator _aggregator;

        public PlayList(CommandContainer container, IEventAggregator aggregator) : base(container)
        {
            Rows = 1;
            Columns = 2;
            HasFrame = true;
            Text = "Chosen Cards";
            _aggregator = aggregator;
        }
        public bool AlreadyHasTwoCards() => ObjectList.Count == 2;
        public bool HasChosenCards() => ObjectList.Count != 0;
        public void RemoveOneCard(SolitaireCard thisCard) => ObjectList.RemoveObjectByDeck(thisCard.Deck);
        public void AddCard(SolitaireCard thisCard)
        {
            if (AlreadyHasTwoCards())
                throw new BasicBlankException("Already has two cards.  Therefore, no cards can be added");
            var newCard = new SolitaireCard();
            newCard.Populate(thisCard.Deck); //to clone.
            newCard.Visible = true; //to double check.
            ObjectList.Add(newCard);
        }
        public void RemoveCards() => ObjectList.Clear();
        protected override async Task ClickProcessAsync(SolitaireCard card)
        {
            if (AlreadyHasTwoCards())
            {
                await UIPlatform.ShowMessageAsync("Sorry, 2 has already been selected");
                return;
            }
            AddCard(card);
            _aggregator.Publish(new MoveEventModel(card.Deck));
        }
    }
}
