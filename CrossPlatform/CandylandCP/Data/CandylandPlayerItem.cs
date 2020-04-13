using BasicGameFrameworkLibrary.MultiplayerClasses.BasicPlayerClasses;
using CommonBasicStandardLibraries.Messenging;
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;
namespace CandylandCP.Data
{
    public class CandylandPlayerItem : SimplePlayer
    { //anything needed is here
        internal IEventAggregator? Aggregator { get; set; }
        private int _spaceNumber;
        public int SpaceNumber
        {
            get { return _spaceNumber; }
            set
            {
                if (SetProperty(ref _spaceNumber, value))
                {
                    //can decide what to do when property changes
                    if (value > 0)
                    {
                        if (Aggregator == null)
                            Aggregator = cons!.Resolve<EventAggregator>();
                        Aggregator.Publish(this); //i think this simple.
                    }
                }
            }
        }
    }
}