using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.DrawableListsObservable;
using BasicGameFrameworkLibrary.MultiplayerClasses.SavedGameClasses;
using SorryCardGameCP.Cards;
namespace SorryCardGameCP.Data
{
    [SingletonGame]
    public class SorryCardGameSaveInfo : BasicSavedCardClass<SorryCardGamePlayerItem, SorryCardGameCardInformation>
    { //anything needed for autoresume is here.

        private int _upTo;
        public int UpTo
        {
            get { return _upTo; }
            set
            {
                if (SetProperty(ref _upTo, value))
                {
                    if (_model != null)
                        _model.UpTo = value;
                }
            }
        }
        private string _instructions = "";
        public string Instructions
        {
            get { return _instructions; }
            set
            {
                if (SetProperty(ref _instructions, value))
                {
                    //can decide what to do when property changes
                    if (_model != null)
                        _model.Instructions = value;
                }
            }
        }
        private SorryCardGameVMData? _model;
        public void LoadMod(SorryCardGameVMData model)
        {
            _model = model;
            _model.UpTo = UpTo;
            _model.Instructions = Instructions;
        }
        public EnumGameStatus GameStatus { get; set; }
        public bool WasTie { get; set; }
        public EnumSorry LastSorry { get; set; } = EnumSorry.None;
        public SavedDiscardPile<SorryCardGameCardInformation>? OtherPileData;

    }
}