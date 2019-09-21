using BasicGameFramework.Attributes;
using BasicGameFramework.Dominos;
using BasicGameFramework.MultiplayerClasses.SavedGameClasses;
namespace DominosRegularCP
{
    [SingletonGame]
    public class DominosRegularSaveInfo : BasicSavedDominosClass<SimpleDominoInfo, DominosRegularPlayerItem>
    {
        public SimpleDominoInfo? CenterDomino { get; set; }
        public SimpleDominoInfo? FirstDomino { get; set; }
        public SimpleDominoInfo? SecondDomino { get; set; }
        public bool Beginnings { get; set; }
    }
}