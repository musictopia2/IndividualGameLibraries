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
using CommonBasicStandardLibraries.MVVMFramework.ViewModels;
using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.BasicGameDataClasses;
using BasicGameFrameworkLibrary.MultiplayerClasses.SavedGameClasses;
using BasicGameFrameworkLibrary.RegularDeckOfCards;
using SixtySix2PlayerCP.Cards;
using BasicGameFrameworkLibrary.BasicDrawables.Dictionary;
//i think this is the most common things i like to do
namespace SixtySix2PlayerCP.Data
{
    [SingletonGame]
    public class SixtySix2PlayerSaveInfo : BasicSavedTrickGamesClass<EnumSuitList, SixtySix2PlayerCardInformation, SixtySix2PlayerPlayerItem>
    { //anything needed for autoresume is here.
        public DeckRegularDict<SixtySix2PlayerCardInformation> CardList { get; set; } = new DeckRegularDict<SixtySix2PlayerCardInformation>();
        public CustomBasicList<int> CardsForMarriage { get; set; } = new CustomBasicList<int>();
        public int LastTrickWon { get; set; }
        private SixtySix2PlayerVMData? _model;
        public void LoadMod(SixtySix2PlayerVMData model)
        {
            _model = model;
            _model.BonusPoints = BonusPoints;
        }
        private int _bonusPoints;
        public int BonusPoints
        {
            get { return _bonusPoints; }
            set
            {
                if (SetProperty(ref _bonusPoints, value))
                {
                    if (_model != null)
                        _model.BonusPoints = value;
                }
            }
        }
    }
}