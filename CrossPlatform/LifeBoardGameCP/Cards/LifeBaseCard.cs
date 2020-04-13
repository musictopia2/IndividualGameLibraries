using BasicGameFrameworkLibrary.BasicDrawables.BasicClasses;
using BasicGameFrameworkLibrary.BasicDrawables.Interfaces;
using LifeBoardGameCP.Data;
using SkiaSharp;
namespace LifeBoardGameCP.Cards
{
    public class LifeBaseCard : SimpleDeckObject, IDeckObject //can't be abstract or can't autosave cards.
    {
        public LifeBaseCard()
        {
            DefaultSize = new SKSize(80, 100);
        }
        private EnumCardCategory _cardCategory;
        public EnumCardCategory CardCategory
        {
            get { return _cardCategory; }
            set
            {
                if (SetProperty(ref _cardCategory, value))
                {
                    //can decide what to do when property changes
                }
            }
        }
        public void Populate(int chosen) { }
        public void Reset() { }
    }
}