using BasicGameFramework.BasicDrawables.Interfaces;
namespace MonopolyCardGameCP
{
    public class MonopolyCardGameDeckCount : IDeckCount
    {
        public int GetDeckCount()
        {
            return 60; //change to what it really is.
        }
    }
}