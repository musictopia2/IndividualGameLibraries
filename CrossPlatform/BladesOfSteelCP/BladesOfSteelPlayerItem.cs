using BasicGameFramework.BasicDrawables.Dictionary;
using BasicGameFramework.MultiplayerClasses.BasicPlayerClasses;
using BasicGameFramework.RegularDeckOfCards;
using Newtonsoft.Json;
namespace BladesOfSteelCP
{
    public class BladesOfSteelPlayerItem : PlayerSingleHand<RegularSimpleCard>
    { //anything needed is here
        private RegularSimpleCard? _FaceOff;
        public RegularSimpleCard? FaceOff
        {
            get { return _FaceOff; }
            set
            {
                if (SetProperty(ref _FaceOff, value))
                {
                    //can decide what to do when property changes
                }
            }
        }
        private int _Score;
        public int Score
        {
            get { return _Score; }
            set
            {
                if (SetProperty(ref _Score, value))
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