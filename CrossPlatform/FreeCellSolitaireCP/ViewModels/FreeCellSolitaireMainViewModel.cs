using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.CommandClasses;
using BasicGameFrameworkLibrary.DIContainers;
using BasicGameFrameworkLibrary.MultiplePilesObservable;
using BasicGameFrameworkLibrary.SolitaireClasses.Cards;
using BasicGameFrameworkLibrary.SolitaireClasses.MainClasses;
using CommonBasicStandardLibraries.Messenging;
using FreeCellSolitaireCP.Data;
using FreeCellSolitaireCP.Logic;
using System.Threading.Tasks; //most of the time, i will be using asyncs.

namespace FreeCellSolitaireCP.ViewModels
{
    [InstanceGame]
    public class FreeCellSolitaireMainViewModel : SolitaireMainViewModel<FreeCellSolitaireSaveInfo>
    {

        public FreePiles FreePiles1;
        public FreeCellSolitaireMainViewModel(IEventAggregator aggregator,
            CommandContainer command,
            IGamePackageResolver resolver
            )
            : base(aggregator, command, resolver)
        {
            FreePiles1 = new FreePiles(command, aggregator);
            FreePiles1.PileClickedAsync += FreePiles1_PileClickedAsync;
        }
        private FreeCellSolitaireMainGameClass? _mainGame;
        protected override SolitaireGameClass<FreeCellSolitaireSaveInfo> GetGame(IGamePackageResolver resolver)
        {
            _mainGame = resolver.ReplaceObject<FreeCellSolitaireMainGameClass>();
            return _mainGame;
        }
        private async Task FreePiles1_PileClickedAsync(int Index, BasicPileInfo<SolitaireCard> ThisPile)
        {
            await _mainGame!.FreeSelectedAsync(Index);
        }
    }
}
