using BasicGameFrameworkLibrary.BasicDrawables.Interfaces;

namespace MonopolyCardGameCP.Cards
{
    public class MonopolyCardGameDeckCount : IDeckCount
    {
        public int GetDeckCount()
        {
            return 60; //change to what it really is.
        }
    }
}
