using BasicGameFramework.Attributes;
namespace XPuzzleCP
{
    [SingletonGame]
    public class XPuzzleGlobalMod
    {
        //not sure if i need the view model or not (?)
        public bool GameLoaded { get; set; }
    }
}