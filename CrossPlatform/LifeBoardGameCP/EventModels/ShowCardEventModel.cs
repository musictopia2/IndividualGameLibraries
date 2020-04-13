using LifeBoardGameCP.Data;
namespace LifeBoardGameCP.EventModels
{
    public class ShowCardEventModel
    {
        public ShowCardEventModel(EnumWhatStatus gameStatus)
        {
            GameStatus = gameStatus;
        }

        public EnumWhatStatus GameStatus { get; }
    }
}