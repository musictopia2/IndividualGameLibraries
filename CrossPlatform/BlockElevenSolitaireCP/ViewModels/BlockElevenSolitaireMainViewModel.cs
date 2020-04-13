using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.CommandClasses;
using BasicGameFrameworkLibrary.DIContainers;
using BasicGameFrameworkLibrary.SolitaireClasses.MainClasses;
using BlockElevenSolitaireCP.Data;
using BlockElevenSolitaireCP.Logic;
using CommonBasicStandardLibraries.Messenging;

namespace BlockElevenSolitaireCP.ViewModels
{
    [InstanceGame]
    public class BlockElevenSolitaireMainViewModel : SolitaireMainViewModel<BlockElevenSolitaireSaveInfo>
    {
        public BlockElevenSolitaireMainViewModel(IEventAggregator aggregator,
            CommandContainer command,
            IGamePackageResolver resolver
            )
            : base(aggregator, command, resolver)
        {
            GlobalClass.MainMod = this; //hopefully this works (?)
        }

        protected override SolitaireGameClass<BlockElevenSolitaireSaveInfo> GetGame(IGamePackageResolver resolver)
        {
            return resolver.ReplaceObject<BlockElevenSolitaireMainGameClass>();
        }


        private int _cardsLeft;

        public int CardsLeft
        {
            get { return _cardsLeft; }
            set
            {
                if (SetProperty(ref _cardsLeft, value))
                {
                    //can decide what to do when property changes
                }

            }
        }
        protected override void CommandExecutingChanged()
        {
            CardsLeft = DeckPile!.CardsLeft();
            //DeckPile.Visible = false;
            //DeckPile.IsEnabled = false;
        }
    }
}
