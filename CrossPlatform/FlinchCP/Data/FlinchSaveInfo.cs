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
using FlinchCP.Cards;
using BasicGameFrameworkLibrary.MultiplePilesObservable;
//i think this is the most common things i like to do
namespace FlinchCP.Data
{
    [SingletonGame]
    public class FlinchSaveInfo : BasicSavedCardClass<FlinchPlayerItem, FlinchCardInformation>
    { //anything needed for autoresume is here.
        public CustomBasicList<BasicPileInfo<FlinchCardInformation>> PublicPileList { get; set; } = new CustomBasicList<BasicPileInfo<FlinchCardInformation>>();

        private int _cardsToShuffle;
        public int CardsToShuffle
        {
            get { return _cardsToShuffle; }
            set
            {
                if (SetProperty(ref _cardsToShuffle, value))
                {
                    //can decide what to do when property changes
                    if (_model == null)
                        return;
                    _model.CardsToShuffle = value;
                }

            }
        }

        public int PlayerFound { get; set; }
        public EnumStatusList GameStatus { get; set; }

        public void LoadMod(FlinchVMData model)
        {
            _model = model;
            _model.CardsToShuffle = CardsToShuffle;
        }

        private FlinchVMData? _model;
    }
}