using BasicGameFrameworkLibrary.BasicDrawables.BasicClasses;
using BasicGameFrameworkLibrary.BasicDrawables.Interfaces;
using BasicGameFrameworkLibrary.BasicGameDataClasses;
using BasicGameFrameworkLibrary.CommonInterfaces;
using BasicGameFrameworkLibrary.DrawableListsObservable;
using BasicGameFrameworkLibrary.GameGraphicsCP.Interfaces;
using BasicGameFrameworkLibrary.MiscProcesses;
using BasicGameFrameworkLibrary.MultiplayerClasses.LoadingClasses;
using BasicGameFrameworkLibrary.StandardImplementations.CrossPlatform.CommonProportionClasses;
using BasicGameFrameworkLibrary.StandardImplementations.CrossPlatform.ExtensionClasses;
using BasicGameFrameworkLibrary.StandardImplementations.XamarinForms.Interfaces;
using BasicGameFrameworkLibrary.ViewModels;
using BasicGamingUIXFLibrary.Bootstrappers;
using DutchBlitzCP.Cards;
using DutchBlitzCP.Data;
using DutchBlitzCP.ViewModels;
using System.Threading.Tasks; //most of the time, i will be using asyncs.

namespace DutchBlitzXF
{
    public class Bootstrapper : MultiplayerBasicBootstrapper<DutchBlitzShellViewModel, GamePage>
    {
        public Bootstrapper(IGamePlatform customPlatform, IStartUp starts, EnumGamePackageMode mode) : base(customPlatform, starts, mode)
        {
        }

        protected override Task ConfigureAsync()
        {
            OurContainer!.RegisterNonSavedClasses<DutchBlitzShellViewModel>();
            OurContainer!.RegisterType<BasicGameLoader<DutchBlitzPlayerItem, DutchBlitzSaveInfo>>();
            OurContainer.RegisterType<RetrieveSavedPlayers<DutchBlitzPlayerItem, DutchBlitzSaveInfo>>();
            OurContainer.RegisterType<MultiplayerOpeningViewModel<DutchBlitzPlayerItem>>(true); //had to be set to true after all.
            OurContainer.RegisterType<DeckObservablePile<DutchBlitzCardInformation>>(true);
            OurContainer.RegisterType<GenericCardShuffler<DutchBlitzCardInformation>>();
            OurContainer.RegisterSingleton<IDeckCount, DutchBlitzDeckCount>();
            OurContainer.RegisterSingleton<IProportionImage, StandardProportion>("");
            return Task.CompletedTask;
        }
    }
}
