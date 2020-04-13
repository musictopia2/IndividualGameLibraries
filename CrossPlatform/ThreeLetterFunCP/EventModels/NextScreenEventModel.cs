using ThreeLetterFunCP.BeginningClasses;
namespace ThreeLetterFunCP.EventModels
{
    public class NextScreenEventModel
    {
        public NextScreenEventModel(EnumNextScreen screen)
        {
            Screen = screen;
        }

        public EnumNextScreen Screen { get; }
    }
}