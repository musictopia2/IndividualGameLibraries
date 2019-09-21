using BasicGameFramework.Attributes;
using BasicGameFramework.GameGraphicsCP.Animations;
namespace AggravationCP
{
    [SingletonGame]
    public class GlobalVariableClass
    {
        internal int MovePlayer { get; set; }
        internal int PlayerGoingBack { get; set; }
        internal AnimateSkiaSharpGameBoard Animates = new AnimateSkiaSharpGameBoard(); //hopefully this simple (?)
    }
}