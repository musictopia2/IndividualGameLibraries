using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.BasicEventModels;
using BasicGameFrameworkLibrary.CommandClasses;
using BasicGameFrameworkLibrary.DIContainers;
using BasicGameFrameworkLibrary.SolitaireClasses.MainClasses;
using CommonBasicStandardLibraries.Messenging;
using SpiderSolitaireCP.Data;
using SpiderSolitaireCP.Logic;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace SpiderSolitaireCP.ViewModels
{
    [InstanceGame]
    public class SpiderSolitaireMainViewModel : SolitaireMainViewModel<SpiderSolitaireSaveInfo>
    {

        [Command(EnumCommandCategory.Plain)]
        public async Task EndGameAsync()
        {
            await _mainGame!.SendGameOverAsync();
        }

        public SpiderSolitaireMainViewModel(IEventAggregator aggregator,
            CommandContainer command,
            IGamePackageResolver resolver
            )
            : base(aggregator, command, resolver)
        {
        }
        private SpiderSolitaireMainGameClass? _mainGame;
        protected override SolitaireGameClass<SpiderSolitaireSaveInfo> GetGame(IGamePackageResolver resolver)
        {
            _mainGame = resolver.ReplaceObject<SpiderSolitaireMainGameClass>();
            return _mainGame;
        }




    }
}
