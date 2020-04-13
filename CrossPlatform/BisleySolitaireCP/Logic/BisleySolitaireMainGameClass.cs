using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.CommonInterfaces;
using BasicGameFrameworkLibrary.SolitaireClasses.Cards;
using BasicGameFrameworkLibrary.SolitaireClasses.DataClasses;
using BasicGameFrameworkLibrary.SolitaireClasses.MainClasses;
using BisleySolitaireCP.Data;
using CommonBasicStandardLibraries.Messenging;
using System.Threading.Tasks; //most of the time, i will be using asyncs.

namespace BisleySolitaireCP.Logic
{
    [SingletonGame]
    public class BisleySolitaireMainGameClass : SolitaireGameClass<BisleySolitaireSaveInfo>
    {
        public BisleySolitaireMainGameClass(ISolitaireData solitaireData1,
            ISaveSinglePlayerClass thisState,
            IEventAggregator aggregator,
            IScoreData score
            )
            : base(solitaireData1, thisState, aggregator, score)
        {
        }
        //rethink if i need view model.  hopefully won't happen though.

        protected async override Task ContinueOpenSavedAsync()
        {
            //anything else you need will be here
            await base.ContinueOpenSavedAsync();
        }
        protected async override Task FinishSaveAsync()
        {
            //anything else to finish save will be here.
            await base.FinishSaveAsync();
        }
        protected override SolitaireCard CardPlayed()
        {
            var thisCard = base.CardPlayed();
            return thisCard;
            //if any changes, will be here.
        }
        protected override void AfterShuffleCards()
        {
            var aceList = GetAceList();
            AfterShuffle(aceList);
        }
        protected override int WhichAutoMoveIsValid()
        {
            var tempList = _thisMod!.WastePiles1!.GetAllCards();
            for (int x = 0; x < tempList.Count; x++)
            {
                if (_thisMod.WastePiles1.CanSelectUnselectPile(x))
                {
                    var thisCard = tempList[x];
                    if (ValidMainColumn(thisCard) > -1)
                        return x;
                }
            }
            return -1;
        }

    }
}
