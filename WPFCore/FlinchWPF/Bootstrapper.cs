using BasicGameFrameworkLibrary.BasicDrawables.BasicClasses;
using BasicGameFrameworkLibrary.BasicDrawables.Interfaces;
using BasicGameFrameworkLibrary.BasicGameDataClasses;
using BasicGameFrameworkLibrary.CommonInterfaces;
using BasicGameFrameworkLibrary.DrawableListsObservable;
using BasicGameFrameworkLibrary.GameGraphicsCP.Interfaces;
using BasicGameFrameworkLibrary.MiscProcesses;
using BasicGameFrameworkLibrary.MultiplayerClasses.LoadingClasses;
using BasicGameFrameworkLibrary.RegularDeckOfCards;
using BasicGameFrameworkLibrary.StandardImplementations.CrossPlatform.CommonProportionClasses;
using BasicGameFrameworkLibrary.StandardImplementations.CrossPlatform.ExtensionClasses;
using BasicGameFrameworkLibrary.ViewModels;
using BasicGamingUIWPFLibrary.Bootstrappers;
using FlinchCP.Cards;
using FlinchCP.Data;
using FlinchCP.ViewModels;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace FlinchWPF
{
    public class Bootstrapper : MultiplayerBasicBootstrapper<FlinchShellViewModel, GamePage>
    {
        public Bootstrapper(IStartUp starts, EnumGamePackageMode mode) : base(starts, mode)
        {
        }
        //protected override Task RegisterTestsAsync()
        //{
        //    TestData!.PlayCategory = BasicGameFrameworkLibrary.TestUtilities.EnumPlayCategory.NoShuffle;
        //    return base.RegisterTestsAsync();
        //}
        protected override Task ConfigureAsync()
        {
            OurContainer!.RegisterNonSavedClasses<FlinchShellViewModel>();
            OurContainer!.RegisterType<BasicGameLoader<FlinchPlayerItem, FlinchSaveInfo>>();
            OurContainer.RegisterType<RetrieveSavedPlayers<FlinchPlayerItem, FlinchSaveInfo>>();
            OurContainer.RegisterType<MultiplayerOpeningViewModel<FlinchPlayerItem>>(true); //had to be set to true after all.
            OurContainer.RegisterType<DeckObservablePile<FlinchCardInformation>>(true);
            OurContainer.RegisterType<GenericCardShuffler<FlinchCardInformation>>();
            OurContainer.RegisterSingleton<IDeckCount, FlinchDeckCount>();
            OurContainer.RegisterSingleton<IProportionImage, StandardProportion>("");
            //anything that needs to be registered will be here.



            return Task.CompletedTask;
        }
    }
}