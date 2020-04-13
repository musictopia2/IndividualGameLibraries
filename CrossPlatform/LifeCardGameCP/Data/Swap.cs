using CommonBasicStandardLibraries.CollectionClasses;

namespace LifeCardGameCP.Data
{
    public class Swap
    {
        public int Player { get; set; }
        public CustomBasicList<int> YourCards { get; set; } = new CustomBasicList<int>();
        public CustomBasicList<int> OtherCards { get; set; } = new CustomBasicList<int>();
    }
}
