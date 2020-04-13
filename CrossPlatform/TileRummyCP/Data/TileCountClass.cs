using BasicGameFrameworkLibrary.BasicDrawables.Interfaces;

namespace TileRummyCP.Data
{
    public class TileCountClass : IDeckCount
    {
        int IDeckCount.GetDeckCount()
        {
            return 106;
        }
    }
}