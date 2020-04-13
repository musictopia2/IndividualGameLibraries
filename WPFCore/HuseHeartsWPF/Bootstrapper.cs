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
using HuseHeartsCP.Cards;
using HuseHeartsCP.Data;
using HuseHeartsCP.ViewModels;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using ts = BasicGameFrameworkLibrary.GameGraphicsCP.Cards.DeckOfCardsCP;

namespace HuseHeartsWPF
{
    public class Bootstrapper : MultiplayerBasicBootstrapper<HuseHeartsShellViewModel, GamePage>
    {
        public Bootstrapper(IStartUp starts, EnumGamePackageMode mode) : base(starts, mode)
        {
        }

        protected override Task ConfigureAsync()
        {
            OurContainer!.RegisterNonSavedClasses<HuseHeartsShellViewModel>();
            OurContainer!.RegisterType<BasicGameLoader<HuseHeartsPlayerItem, HuseHeartsSaveInfo>>();
            OurContainer.RegisterType<RetrieveSavedPlayers<HuseHeartsPlayerItem, HuseHeartsSaveInfo>>();
            OurContainer.RegisterType<MultiplayerOpeningViewModel<HuseHeartsPlayerItem>>(true); //had to be set to true after all.
            OurContainer.RegisterType<DeckObservablePile<HuseHeartsCardInformation>>(true);
            //OurContainer.RegisterType<GenericCardShuffler<HuseHeartsCardInformation>>();
            OurContainer.RegisterType<RegularCardsBasicShuffler<HuseHeartsCardInformation>>();
            OurContainer.RegisterSingleton<IRegularAceCalculator, RegularAceHighCalculator>(); //most of the time, ace is high for trick taking games.

            OurContainer.RegisterSingleton<IDeckCount, RegularAceHighSimpleDeck>();
            OurContainer.RegisterSingleton<IProportionImage, StandardProportion>(ts.TagUsed);
            //anything that needs to be registered will be here.

            bool rets = OurContainer.RegistrationExist<ISortCategory>();
            if (rets == true)
            {
                ISortCategory cat = OurContainer.Resolve<ISortCategory>();
                SortSimpleCards<HuseHeartsCardInformation> sort = new SortSimpleCards<HuseHeartsCardInformation>();
                sort.SuitForSorting = cat.SortCategory;
                OurContainer.RegisterSingleton(sort); //if we have a custom one, will already be picked up.
            }
            return Task.CompletedTask;
        }
    }
}