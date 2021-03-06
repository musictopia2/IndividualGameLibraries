using BasicGameFrameworkLibrary.BasicDrawables.BasicClasses;
using BasicGameFrameworkLibrary.BasicDrawables.Interfaces;
using BasicGameFrameworkLibrary.BasicGameDataClasses;
using BasicGameFrameworkLibrary.CommonInterfaces;
using BasicGameFrameworkLibrary.Dice;
using BasicGameFrameworkLibrary.DrawableListsObservable;
using BasicGameFrameworkLibrary.GameGraphicsCP.Interfaces;
using BasicGameFrameworkLibrary.MiscProcesses;
using BasicGameFrameworkLibrary.MultiplayerClasses.LoadingClasses;
using BasicGameFrameworkLibrary.StandardImplementations.CrossPlatform.CommonProportionClasses;
using BasicGameFrameworkLibrary.StandardImplementations.CrossPlatform.ExtensionClasses;
using BasicGameFrameworkLibrary.ViewModels;
using BasicGamingUIWPFLibrary.Bootstrappers;
using BasicGamingUIWPFLibrary.GameGraphics.Dice;
using FillOrBustCP.Cards;
using FillOrBustCP.Data;
using FillOrBustCP.ViewModels;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace FillOrBustWPF
{
    public class Bootstrapper : MultiplayerBasicBootstrapper<FillOrBustShellViewModel, GamePage>
    {
        public Bootstrapper(IStartUp starts, EnumGamePackageMode mode) : base(starts, mode)
        {
        }

        protected override Task ConfigureAsync()
        {
            OurContainer!.RegisterNonSavedClasses<FillOrBustShellViewModel>();
            OurContainer!.RegisterType<BasicGameLoader<FillOrBustPlayerItem, FillOrBustSaveInfo>>();
            OurContainer.RegisterType<RetrieveSavedPlayers<FillOrBustPlayerItem, FillOrBustSaveInfo>>();
            OurContainer.RegisterType<MultiplayerOpeningViewModel<FillOrBustPlayerItem>>(true); //had to be set to true after all.
            OurContainer.RegisterType<DeckObservablePile<FillOrBustCardInformation>>(true);
            OurContainer.RegisterType<GenericCardShuffler<FillOrBustCardInformation>>();
            OurContainer.RegisterSingleton<IDeckCount, FillOrBustDeckCount>();
            //anything that needs to be registered will be here.

            OurContainer.RegisterSingleton<IProportionImage, ProportionWPF>("");
            OurContainer.RegisterSingleton<IProportionImage, StandardProportion>(StandardDiceWPF.GetDiceTag);
            OurContainer.RegisterSingleton<IGenerateDice<int>, SimpleDice>();
            OurContainer.RegisterType<StandardRollProcesses<SimpleDice, FillOrBustPlayerItem>>();

            return Task.CompletedTask;
        }
    }
}