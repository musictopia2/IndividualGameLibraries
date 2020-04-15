using BasicGameFrameworkLibrary.BasicDrawables.BasicClasses;
using BasicGameFrameworkLibrary.BasicDrawables.Interfaces;
using BasicGameFrameworkLibrary.BasicGameDataClasses;
using BasicGameFrameworkLibrary.ColorCards;
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
using Phase10CP.Cards;
using Phase10CP.Data;
using Phase10CP.ViewModels;
using System.Threading.Tasks; //most of the time, i will be using asyncs.

namespace Phase10XF
{
    public class Bootstrapper : MultiplayerBasicBootstrapper<Phase10ShellViewModel, GamePage>
    {
        public Bootstrapper(IGamePlatform customPlatform, IStartUp starts, EnumGamePackageMode mode) : base(customPlatform, starts, mode)
        {
        }

        protected override Task ConfigureAsync()
        {
            OurContainer!.RegisterNonSavedClasses<Phase10ShellViewModel>();
            OurContainer!.RegisterType<BasicGameLoader<Phase10PlayerItem, Phase10SaveInfo>>();
            OurContainer.RegisterType<RetrieveSavedPlayers<Phase10PlayerItem, Phase10SaveInfo>>();
            OurContainer.RegisterType<MultiplayerOpeningViewModel<Phase10PlayerItem>>(true); //had to be set to true after all.
            OurContainer.RegisterType<DeckObservablePile<Phase10CardInformation>>(true);
            OurContainer.RegisterType<GenericCardShuffler<Phase10CardInformation>>();
            OurContainer.RegisterSingleton<IDeckCount, Phase10UnoDeck>();
            OurContainer.RegisterSingleton<IProportionImage, SmallDrawableProportion>(""); //has to be small this time.
            return Task.CompletedTask;
        }
    }
}
