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
using MonopolyCardGameCP.Cards;
using Newtonsoft.Json;
using MonopolyCardGameCP.Logic;

namespace MonopolyCardGameCP.Data
{
    public class MonopolyCardGamePlayerItem : PlayerSingleHand<MonopolyCardGameCardInformation>
    { //anything needed is here
        [JsonIgnore]
        public TradePile? TradePile;
        public string TradeString { get; set; } = ""; //iffy.
        private decimal _previousMoney;
        public decimal PreviousMoney
        {
            get { return _previousMoney; }
            set
            {
                if (SetProperty(ref _previousMoney, value))
                {
                    //can decide what to do when property changes
                }

            }
        }
        private decimal _totalMoney;
        public decimal TotalMoney
        {
            get { return _totalMoney; }
            set
            {
                if (SetProperty(ref _totalMoney, value))
                {
                    //can decide what to do when property changes
                }

            }
        }
    }
}
