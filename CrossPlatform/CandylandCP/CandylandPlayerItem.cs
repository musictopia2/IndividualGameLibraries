using BasicGameFramework.MultiplayerClasses.BasicPlayerClasses;
using CommonBasicStandardLibraries.Messenging;
namespace CandylandCP
{
    public class CandylandPlayerItem : SimplePlayer
    {
        internal CandylandMainGameClass? ThisGame;
        internal EventAggregator? ThisE;
        private int _SpaceNumber;
        public int SpaceNumber
        {
            get { return _SpaceNumber; }
            set
            {
                if (SetProperty(ref _SpaceNumber, value))
                {
                    //can decide what to do when property changes
                    if (value > 0)
                    {
                        if (ThisGame == null)
                            return; //i think this is the way to go.
                        if (ThisE == null)
                            ThisE = ThisGame.MainContainer.Resolve<EventAggregator>();
                        ThisE.Publish(this); //i think this simple.
                    }
                }
            }
        }
    }
}