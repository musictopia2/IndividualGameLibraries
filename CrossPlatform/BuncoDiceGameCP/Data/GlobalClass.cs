using BasicGameFrameworkLibrary.Attributes;
namespace BuncoDiceGameCP.Data
{
    [SingletonGame]
    public class GlobalClass
    {
        public bool IsActive { get; set; }
    }
}
