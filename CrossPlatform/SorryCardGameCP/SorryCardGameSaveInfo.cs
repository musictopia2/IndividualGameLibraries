using BasicGameFramework.Attributes;
using BasicGameFramework.DrawableListsViewModels;
using BasicGameFramework.MultiplayerClasses.SavedGameClasses;
namespace SorryCardGameCP
{
    [SingletonGame]
    public class SorryCardGameSaveInfo : BasicSavedCardClass<SorryCardGamePlayerItem, SorryCardGameCardInformation>
    {
        private int _UpTo;
        public int UpTo
        {
            get { return _UpTo; }
            set
            {
                if (SetProperty(ref _UpTo, value))
                {
                    if (_thisMod != null)
                        _thisMod.UpTo = value;
                }
            }
        }
        private string _Instructions = "";
        public string Instructions
        {
            get { return _Instructions; }
            set
            {
                if (SetProperty(ref _Instructions, value))
                {
                    //can decide what to do when property changes
                    if (_thisMod != null)
                        _thisMod.Instructions = value;
                }
            }
        }
        private SorryCardGameViewModel? _thisMod;
        public void LoadMod(SorryCardGameViewModel thisMod)
        {
            _thisMod = thisMod;
            _thisMod.UpTo = UpTo;
            _thisMod.Instructions = Instructions;
        }
        public EnumGameStatus GameStatus { get; set; }
        public bool WasTie { get; set; }
        public EnumSorry LastSorry { get; set; } = EnumSorry.None;
        public SavedDiscardPile<SorryCardGameCardInformation>? OtherPileData;
    }
}