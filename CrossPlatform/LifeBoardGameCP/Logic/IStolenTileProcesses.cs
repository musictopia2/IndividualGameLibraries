using System.Threading.Tasks;

namespace LifeBoardGameCP.Logic
{
    public interface IStolenTileProcesses
    {
        Task TilesStolenAsync(string player);
        Task ComputerStealTileAsync();
        void LoadOtherPlayerTiles();
    }
}