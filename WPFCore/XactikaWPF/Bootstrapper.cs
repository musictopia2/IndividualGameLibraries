using BasicGameFrameworkLibrary.BasicDrawables.Interfaces;
using BasicGameFrameworkLibrary.BasicGameDataClasses;
using BasicGameFrameworkLibrary.CommonInterfaces;
using BasicGameFrameworkLibrary.DrawableListsObservable;
using BasicGameFrameworkLibrary.GameGraphicsCP.Interfaces;
using BasicGameFrameworkLibrary.MiscProcesses;
using BasicGameFrameworkLibrary.MultiplayerClasses.LoadingClasses;
using BasicGameFrameworkLibrary.SpecializedGameTypes.TrickClasses;
using BasicGameFrameworkLibrary.StandardImplementations.CrossPlatform.CommonProportionClasses;
using BasicGameFrameworkLibrary.StandardImplementations.CrossPlatform.ExtensionClasses;
using BasicGameFrameworkLibrary.ViewModels;
using BasicGamingUIWPFLibrary.Bootstrappers;
using XactikaCP.Cards;
using XactikaCP.Data;
using XactikaCP.ViewModels;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using BasicGameFrameworkLibrary.BasicDrawables.BasicClasses;
using BasicGamingUIWPFLibrary.BasicControls.ChoicePickers;

namespace XactikaWPF
{
    public class Bootstrapper : MultiplayerBasicBootstrapper<XactikaShellViewModel, GamePage>
    {
        public Bootstrapper(IStartUp starts, EnumGamePackageMode mode) : base(starts, mode)
        {
        }
        //protected override Task RegisterTestsAsync()
        //{
        //    TestData!.ImmediatelyEndGame = true;
        //    TestData.SaveOption = BasicGameFrameworkLibrary.TestUtilities.EnumTestSaveCategory.RestoreOnly;
        //    return base.RegisterTestsAsync();
        //}
        protected override Task ConfigureAsync()
        {
            OurContainer!.RegisterNonSavedClasses<XactikaShellViewModel>();
            OurContainer!.RegisterType<BasicGameLoader<XactikaPlayerItem, XactikaSaveInfo>>();
            OurContainer.RegisterType<RetrieveSavedPlayers<XactikaPlayerItem, XactikaSaveInfo>>();
            OurContainer.RegisterType<MultiplayerOpeningViewModel<XactikaPlayerItem>>(true); //had to be set to true after all.
            OurContainer.RegisterType<DeckObservablePile<XactikaCardInformation>>(true);
            OurContainer.RegisterType<GenericCardShuffler<XactikaCardInformation>>();

            OurContainer.RegisterSingleton<IDeckCount, XactikaDeckCount>();
            //anything that needs to be registered will be here.

            //change view model for area if not using 2 player.
            OurContainer.RegisterSingleton<IProportionImage, CustomProportion>("");
            OurContainer.RegisterType<SeveralPlayersTrickObservable<EnumShapes, XactikaCardInformation, XactikaPlayerItem, XactikaSaveInfo>>();
            OurContainer.RegisterSingleton<IProportionBoard, StandardProportion>("main"); //here too.
            //OurContainer.RegisterSingleton(_stats1.ThisElement, "main");
            OurContainer.RegisterType<StandardWidthHeight>(); //i think i forgot this too.            
            return Task.CompletedTask;
        }
    }
}