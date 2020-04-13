using AgnesSolitaireCP.Data;
using AgnesSolitaireCP.Logic;
using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.CommandClasses;
using BasicGameFrameworkLibrary.DIContainers;
using BasicGameFrameworkLibrary.SolitaireClasses.MainClasses;
using CommonBasicStandardLibraries.Messenging;

namespace AgnesSolitaireCP.ViewModels
{
    [InstanceGame]
    public class AgnesSolitaireMainViewModel : SolitaireMainViewModel<AgnesSolitaireSaveInfo>
    {
        public AgnesSolitaireMainViewModel(IEventAggregator aggregator,
            CommandContainer command,
            IGamePackageResolver resolver
            )
            : base(aggregator, command, resolver)
        {
        }

        protected override SolitaireGameClass<AgnesSolitaireSaveInfo> GetGame(IGamePackageResolver resolver)
        {
            return resolver.ReplaceObject<AgnesSolitaireMainGameClass>();
        }
        //anything else needed is here.

        private int _startingNumber;

        public int StartingNumber
        {
            get { return _startingNumber; }
            set
            {
                if (SetProperty(ref _startingNumber, value))
                {
                    //can decide what to do when property changes
                }

            }
        }
        protected override void CommandExecutingChanged()
        {
            StartingNumber = MainPiles1!.StartNumber();
        }
    }
}
