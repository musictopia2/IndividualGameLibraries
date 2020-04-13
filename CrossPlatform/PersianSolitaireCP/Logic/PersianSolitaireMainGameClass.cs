using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.CommonInterfaces;
using BasicGameFrameworkLibrary.DIContainers;
using BasicGameFrameworkLibrary.SolitaireClasses.Cards;
using BasicGameFrameworkLibrary.SolitaireClasses.DataClasses;
using BasicGameFrameworkLibrary.SolitaireClasses.MainClasses;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.Messenging;
using PersianSolitaireCP.Data;
using PersianSolitaireCP.ViewModels;
using System.Threading.Tasks; //most of the time, i will be using asyncs.

namespace PersianSolitaireCP.Logic
{
    [SingletonGame]
    public class PersianSolitaireMainGameClass : SolitaireGameClass<PersianSolitaireSaveInfo>, IHandle<ScoreModel>
    {
        readonly ScoreModel _model;
        public PersianSolitaireMainGameClass(ISolitaireData solitaireData1,
            ISaveSinglePlayerClass thisState,
            IEventAggregator aggregator,
            IScoreData score
            )
            : base(solitaireData1, thisState, aggregator, score)
        {
            _model = (ScoreModel)score;
            aggregator.Subscribe(this);
        }
        //rethink if i need view model.  hopefully won't happen though.

        protected async override Task ContinueOpenSavedAsync()
        {
            //anything else you need will be here
            _model.DealNumber = SaveRoot.DealNumber;
            await base.ContinueOpenSavedAsync();
        }
        protected async override Task FinishSaveAsync()
        {
            //anything else to finish save will be here.
            if (SaveRoot.DealNumber == 0)
            {
                throw new BasicBlankException("The deal cannot be 0.  Rethink");
            }
            await base.FinishSaveAsync();
        }
        protected override SolitaireCard CardPlayed()
        {
            var thisCard = base.CardPlayed();
            return thisCard;
            //if any changes, will be here.
        }
        private PersianSolitaireMainViewModel? _newVM;
        protected override void AfterShuffleCards()
        {
            _model.DealNumber = 1;
            _thisMod!.MainPiles1!.ClearBoard();
            AfterShuffle();
        }

        void IHandle<ScoreModel>.Handle(ScoreModel message)
        {
            if (_newVM == null)
            {
                _newVM = (PersianSolitaireMainViewModel)_thisMod!;
            }
            _newVM.DealNumber = message.DealNumber;
            
            
            SaveRoot.DealNumber = message.DealNumber; //hopefully this simple.
        }
    }
}