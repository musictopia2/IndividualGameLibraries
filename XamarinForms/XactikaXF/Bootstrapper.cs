using BasicGameFrameworkLibrary.BasicDrawables.BasicClasses;
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
using BasicGameFrameworkLibrary.StandardImplementations.XamarinForms.BasicClasses;
using BasicGameFrameworkLibrary.StandardImplementations.XamarinForms.Interfaces;
using BasicGameFrameworkLibrary.ViewModels;
using BasicGamingUIXFLibrary.Bootstrappers;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using XactikaCP.Cards;
using XactikaCP.Data;
using XactikaCP.ViewModels;

namespace XactikaXF
{
    public class Bootstrapper : MultiplayerBasicBootstrapper<XactikaShellViewModel, GamePage>
    {
        public Bootstrapper(IGamePlatform customPlatform, IStartUp starts, EnumGamePackageMode mode) : base(customPlatform, starts, mode)
        {
        }

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
            //risk without.  if it works, then not needed for number pickers.
            //OurContainer.RegisterType<StandardPickerSizeClass>(); //i think this too.
            return Task.CompletedTask;
        }
    }
}
