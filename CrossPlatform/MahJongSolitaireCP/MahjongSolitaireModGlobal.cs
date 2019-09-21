using BaseMahjongTilesCP;
using BasicGameFramework.Attributes;
namespace MahJongSolitaireCP
{
    [SingletonGame]
    public class MahJongSolitaireModGlobal
    {
        public int SecondSelected { get; set; } //decided to create new class afterall.
        public MahjongShuffler TileList = new MahjongShuffler(); //try this way.  could be even better (?)
    }
}
