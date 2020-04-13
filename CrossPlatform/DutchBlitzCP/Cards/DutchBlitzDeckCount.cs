using BasicGameFrameworkLibrary.BasicDrawables.Interfaces;

namespace DutchBlitzCP.Cards
{
    public class DutchBlitzDeckCount : IDeckCount
    {
        public static bool DoubleDeck;
        public int GetDeckCount()
        {
            if (DoubleDeck)
                return 320;
            return 160;
        }
    }
}