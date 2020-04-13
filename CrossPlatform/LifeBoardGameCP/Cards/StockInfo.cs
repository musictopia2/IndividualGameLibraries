using LifeBoardGameCP.Data;
namespace LifeBoardGameCP.Cards
{
    public class StockInfo : LifeBaseCard
    {
        public StockInfo()
        {
            CardCategory = EnumCardCategory.Stock;
        }
        private int _value;
        public int Value
        {
            get { return _value; }
            set
            {
                if (SetProperty(ref _value, value))
                {
                    //can decide what to do when property changes
                }
            }
        }
    }
}
