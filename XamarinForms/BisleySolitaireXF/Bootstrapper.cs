using BasicGameFrameworkLibrary.BasicDrawables.Interfaces;
using BasicGameFrameworkLibrary.BasicGameDataClasses;
using BasicGameFrameworkLibrary.CommonInterfaces;
using BasicGameFrameworkLibrary.DrawableListsObservable;
using BasicGameFrameworkLibrary.GameGraphicsCP.Interfaces;
using BasicGameFrameworkLibrary.RegularDeckOfCards;
using BasicGameFrameworkLibrary.SolitaireClasses.Cards;
using BasicGameFrameworkLibrary.StandardImplementations.CrossPlatform.ExtensionClasses;
using BasicGameFrameworkLibrary.StandardImplementations.XamarinForms.Interfaces;
using BasicGamingUIXFLibrary.Bootstrappers;
using BisleySolitaireCP.Data;
using BisleySolitaireCP.Logic;
using BisleySolitaireCP.ViewModels;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using ts = BasicGameFrameworkLibrary.GameGraphicsCP.Cards.DeckOfCardsCP;

namespace BisleySolitaireXF
{
    public class Bootstrapper : SinglePlayerBootstrapper<BisleySolitaireShellViewModel, GamePage>
    {
        public Bootstrapper(IGamePlatform customPlatform, IStartUp starts, EnumGamePackageMode mode) : base(customPlatform, starts, mode)
        {
        }

        protected override Task ConfigureAsync()
        {
            OurContainer!.RegisterNonSavedClasses<BisleySolitaireShellViewModel>(); //hopefully okay.
            OurContainer!.RegisterSingleton<IProportionImage, CustomProportion>(ts.TagUsed);
            OurContainer.RegisterType<DeckObservablePile<SolitaireCard>>(true); //i think
            OurContainer.RegisterSingleton<IDeckCount, CustomDeck>(); //forgot to use a custom deck for this one.
            OurContainer.RegisterSingleton<IRegularAceCalculator, RegularLowAceCalculator>(); //most of the time, aces are low.
            //we have to resolve the IMain and IWaste.
            OurContainer.RegisterType<WastePiles>(); //can't do automatically because we don't know if we will do it or not.
            OurContainer.RegisterType<CustomMain>();
            return Task.CompletedTask;
        }
    }
}
