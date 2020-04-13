using BasicGameFrameworkLibrary.BasicDrawables.BasicClasses;
using BasicGameFrameworkLibrary.BasicDrawables.Interfaces;

namespace ClueBoardGameCP.Cards
{
    public abstract class MainInfo : SimpleDeckObject, IDeckObject
    {
        public string Name { get; set; } = "";
        public abstract void Populate(int Chosen);
        public abstract void Reset();
    }
}