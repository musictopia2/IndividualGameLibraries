using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.CommandClasses;
using BasicGameFrameworkLibrary.DIContainers;
using BasicGameFrameworkLibrary.SolitaireClasses.ClockClasses;
using BasicGameFrameworkLibrary.SolitaireClasses.MainClasses;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.Messenging;
using GrandfathersClockCP.Data;
using GrandfathersClockCP.Logic;
using System.Threading.Tasks; //most of the time, i will be using asyncs.

namespace GrandfathersClockCP.ViewModels
{
    [InstanceGame]
    public class GrandfathersClockMainViewModel : SolitaireMainViewModel<GrandfathersClockSaveInfo>
    {
        public GrandfathersClockMainViewModel(IEventAggregator aggregator,
            CommandContainer command,
            IGamePackageResolver resolver
            )
            : base(aggregator, command, resolver)
        {
        }
        
        protected override SolitaireGameClass<GrandfathersClockSaveInfo> GetGame(IGamePackageResolver resolver)
        {
            return resolver.ReplaceObject<GrandfathersClockMainGameClass>();
        }
        //anything else needed is here.
    }
}
