using BasicGameFramework.Attributes;
using BasicGameFramework.MultiplayerClasses.SavedGameClasses;
using BasicGameFramework.RegularDeckOfCards;
using Newtonsoft.Json;
namespace CrazyEightsCP
{
    [SingletonGame]
    public class CrazyEightsSaveInfo : BasicSavedCardClass<CrazyEightsPlayerItem, RegularSimpleCard>
    { //anything needed for autoresume is here.
        private bool _ChooseSuit;
        public bool ChooseSuit
        {
            get
            {
                return _ChooseSuit;
            }

            set
            {
                if (SetProperty(ref _ChooseSuit, value) == true)
                    // code to run
                    ThisMod!.MarkSuitVisible(value); //iffy.  if i run into problems, change logic statement.
            }
        }

        [JsonIgnore]
        public CrazyEightsViewModel? ThisMod; //you have to populate manually
        public void UpdateSuits()
        {
            ThisMod!.SuitChooser!.Visible = _ChooseSuit;
        }

        public EnumSuitList CurrentSuit { get; set; }
        public EnumCardValueList CurrentNumber { get; set; } //decided to use the enum now.
    }
}