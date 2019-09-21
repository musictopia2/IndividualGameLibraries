using BasicGameFramework.BasicDrawables.BasicClasses;
using BasicGameFramework.BasicDrawables.Interfaces;
namespace PaydayCP
{
    public class CardInformation : SimpleDeckObject, IDeckObject
    {
        private EnumCardCategory _CardCategory;
        public EnumCardCategory CardCategory
        {
            get { return _CardCategory; }
            set
            {
                if (SetProperty(ref _CardCategory, value))
                {
                    //can decide what to do when property changes
                }
            }
        }
        private int _Index;
        public int Index
        {
            get { return _Index; }
            set
            {
                if (SetProperty(ref _Index, value))
                {
                    //can decide what to do when property changes
                }
            }
        }
        public virtual void Populate(int chosen) { }

        public virtual void Reset() { }
    }
}