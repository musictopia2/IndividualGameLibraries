using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.Dominos;
using BasicGameFrameworkLibrary.MultiplayerClasses.SavedGameClasses;
namespace DominosRegularCP.Data
{
    [SingletonGame]
    public class DominosRegularSaveInfo : BasicSavedDominosClass<SimpleDominoInfo, DominosRegularPlayerItem>
    { //anything needed for autoresume is here.
        public SimpleDominoInfo? CenterDomino { get; set; }
        public SimpleDominoInfo? FirstDomino { get; set; }
        public SimpleDominoInfo? SecondDomino { get; set; }
        public bool Beginnings { get; set; }
    }
}