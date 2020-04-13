using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.CommonInterfaces;
using CommonBasicStandardLibraries.MVVMFramework.ViewModels;
namespace LifeBoardGameCP.ViewModels
{
    [InstanceGame]
    public class ShowCardViewModel : Screen, IMainScreen
    {
        public ShowCardViewModel()
        {
            //not sure what is needed here though.  somehow needs to decide what card will be shown.

        }
    }
}
