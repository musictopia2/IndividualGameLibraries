using BasicGameFramework.BasicDrawables.Dictionary;
using BasicGameFramework.Extensions;
using BasicGameFramework.RegularDeckOfCards;
using BasicGameFramework.SpecializedGameTypes.RummyClasses;
using BasicGameFramework.ViewModelInterfaces;
using CommonBasicStandardLibraries.Exceptions;
namespace OpetongCP
{
    public class RummySet : SetInfo<EnumSuitList, EnumColorList, RegularRummyCard, SavedSet>
    {
        private int _player;
        private readonly OpetongMainGameClass _mainGame;
        public RummySet(IBasicGameVM thisMod) : base(thisMod)
        {
            _mainGame = thisMod.MainContainer!.Resolve<OpetongMainGameClass>();
        }
        public override void LoadSet(SavedSet current)
        {
            _player = current.Player;
            HandList.ReplaceRange(current.CardList);
        }
        public override SavedSet SavedSet()
        {
            SavedSet output = new SavedSet();
            output.CardList = HandList.ToRegularDeckDict();
            output.Player = _player;
            return output;
        }
        private int GetBaseRate
        {
            get
            {
                var newList = HandList.ToRegularDeckDict();
                bool rets;
                rets = _mainGame.Rummys!.IsNewRummy(newList, HandList.Count, RummyProcesses<EnumSuitList, EnumColorList, RegularRummyCard>.EnumRummyType.Sets);
                if (rets)
                {
                    return 10;
                }
                bool suits;
                bool runs;
                suits = _mainGame.Rummys.IsNewRummy(newList, HandList.Count, RummyProcesses<EnumSuitList, EnumColorList, RegularRummyCard>.EnumRummyType.Colors);
                runs = _mainGame.Rummys.IsNewRummy(newList, HandList.Count, RummyProcesses<EnumSuitList, EnumColorList, RegularRummyCard>.EnumRummyType.Runs);
                if (suits && runs)
                    return 10;
                return 5;
            }
        }
        public void CreateNewSet(IDeckDict<RegularRummyCard> thisCol)
        {
            if (thisCol.Count < 2 || thisCol.Count > 4)
                throw new BasicBlankException("A set must have 2 to 4 cards");
            _player = _mainGame.WhoTurn;
            thisCol.ForEach(thisCard =>
            {
                thisCard.Drew = false;
                thisCard.IsSelected = false;
            });
            HandList.ReplaceRange(thisCol);
        }
        public int CalculateScore(int whichPlayer)
        {
            if (whichPlayer != _player)
                return 0;
            int bases = GetBaseRate;
            return bases * HandList.Count;
        }
    }
}