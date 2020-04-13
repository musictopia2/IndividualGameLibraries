using System;
using System.Text;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using System.Linq;
using CommonBasicStandardLibraries.BasicDataSettingsAndProcesses;
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;
using CommonBasicStandardLibraries.CollectionClasses;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using fs = CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.JsonSerializers.FileHelpers;
using js = CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.JsonSerializers.NewtonJsonStrings; //just in case i need those 2.
using BasicGamingUIWPFLibrary.Bootstrappers;
using DeadDie96CP.ViewModels;
using BasicGameFrameworkLibrary.CommonInterfaces;
using BasicGameFrameworkLibrary.BasicGameDataClasses;
using BasicGameFrameworkLibrary.StandardImplementations.CrossPlatform.ExtensionClasses;
using BasicGameFrameworkLibrary.MultiplayerClasses.LoadingClasses;
using DeadDie96CP.Data;
using BasicGameFrameworkLibrary.MiscProcesses;
using BasicGameFrameworkLibrary.ViewModels;
using BasicGameFrameworkLibrary.Dice;
using BasicGameFrameworkLibrary.GameGraphicsCP.Interfaces;
using BasicGameFrameworkLibrary.StandardImplementations.CrossPlatform.CommonProportionClasses;
using BasicGamingUIWPFLibrary.GameGraphics.Dice;
//i think this is the most common things i like to do
namespace DeadDie96WPF
{
    public class Bootstrapper : MultiplayerBasicBootstrapper <DeadDie96ShellViewModel, GamePage>
    {
        public Bootstrapper(IStartUp starts, EnumGamePackageMode mode) : base(starts, mode)
        {
        }
        //protected override Task RegisterTestsAsync()
        //{
        //    TestData!.ImmediatelyEndGame = true;
        //    return base.RegisterTestsAsync();
        //}
        protected override Task ConfigureAsync()
        {
            OurContainer!.RegisterNonSavedClasses<DeadDie96ShellViewModel>();
            OurContainer!.RegisterType<BasicGameLoader<DeadDie96PlayerItem, DeadDie96SaveInfo>>();
            OurContainer.RegisterType<RetrieveSavedPlayers<DeadDie96PlayerItem, DeadDie96SaveInfo>>();
            OurContainer.RegisterType<MultiplayerOpeningViewModel<DeadDie96PlayerItem>>(true); //had to be set to true after all.
            OurContainer.RegisterType<StandardRollProcesses<TenSidedDice, DeadDie96PlayerItem>>();
            OurContainer.RegisterSingleton<IProportionImage, StandardProportion>(StandardDiceWPF.GetDiceTag);
            OurContainer.RegisterSingleton<IGenerateDice<int>, TenSidedDice>();

            //anything that needs to be registered will be here.
            return Task.CompletedTask;
        }
    }
}