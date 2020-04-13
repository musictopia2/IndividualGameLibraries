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
using BasicGameFrameworkLibrary.MultiplayerClasses.BasicPlayerClasses;
using BasicGameFrameworkLibrary.RegularDeckOfCards;
using Newtonsoft.Json;
using BasicGameFrameworkLibrary.BasicDrawables.Dictionary;
using BladesOfSteelCP.CustomPiles;
//i think this is the most common things i like to do
namespace BladesOfSteelCP.Data
{
    public class BladesOfSteelPlayerItem : PlayerSingleHand<RegularSimpleCard>
    { //anything needed is here
        private RegularSimpleCard? _faceOff;
        public RegularSimpleCard? FaceOff
        {
            get { return _faceOff; }
            set
            {
                if (SetProperty(ref _faceOff, value))
                {
                    //can decide what to do when property changes
                }
            }
        }
        private int _score;
        public int Score
        {
            get { return _score; }
            set
            {
                if (SetProperty(ref _score, value))
                {
                    //can decide what to do when property changes
                }
            }
        }
        public DeckObservableDict<RegularSimpleCard> AttackList { get; set; } = new DeckObservableDict<RegularSimpleCard>();
        public DeckObservableDict<RegularSimpleCard> DefenseList { get; set; } = new DeckObservableDict<RegularSimpleCard>();
        [JsonIgnore]
        public PlayerAttackCP? AttackPile;
        [JsonIgnore]
        public PlayerDefenseCP? DefensePile;
    }
}