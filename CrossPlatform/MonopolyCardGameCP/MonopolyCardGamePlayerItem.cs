using BasicGameFramework.MultiplayerClasses.BasicPlayerClasses;
using Newtonsoft.Json;
namespace MonopolyCardGameCP
{
    public class MonopolyCardGamePlayerItem : PlayerSingleHand<MonopolyCardGameCardInformation>
    {
        [JsonIgnore]
        public TradePile? TradePile;
        public string TradeString { get; set; } = ""; //iffy.
        private decimal _PreviousMoney;
        public decimal PreviousMoney
        {
            get { return _PreviousMoney; }
            set
            {
                if (SetProperty(ref _PreviousMoney, value))
                {
                    //can decide what to do when property changes
                }

            }
        }
        private decimal _TotalMoney;
        public decimal TotalMoney
        {
            get { return _TotalMoney; }
            set
            {
                if (SetProperty(ref _TotalMoney, value))
                {
                    //can decide what to do when property changes
                }

            }
        }
    }
}