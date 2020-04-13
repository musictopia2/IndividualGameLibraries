using BasicGameFrameworkLibrary.BasicDrawables.Interfaces;

namespace LifeCardGameCP.Cards
{
    public class LifeCardGameDeckCount : IDeckCount
    {
        public int GetDeckCount()
        {
            return 108; //change to what it really is.
        }
    }
}