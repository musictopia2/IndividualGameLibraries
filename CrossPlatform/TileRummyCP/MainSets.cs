using BasicGameFramework.SpecializedGameTypes.RummyClasses;
using BasicGameFramework.ViewModelInterfaces;
using CommonBasicStandardLibraries.CollectionClasses;
using System.Linq;
namespace TileRummyCP
{
    public class MainSets : MainSetsViewModel<EnumColorType, EnumColorType, TileInfo, TileSet, SavedSet>
    {
        public void RemoveSet(int index)
        {
            var thisSet = SetList[index];
            RemoveSet(thisSet);
        }
        public void RedoSets()
        {
            SetList.ForEach(thisTemp =>
            {
                thisTemp.HandList.ForEach(thisTile =>
                {
                    thisTile.IsUnknown = false; //to double check.
                    thisTile.Drew = false;
                    thisTile.IsSelected = false;
                });
            });
        }
        public bool PlayedAtLeastOneFromHand()
        {
            CustomBasicList<int> tempList = new CustomBasicList<int>();
            SetList.ForEach(thisTemp =>
            {
                thisTemp.HandList.ForEach(thisTile =>
                {
                    tempList.Add(thisTile.Deck);
                });
            });
            foreach (var thisIndex in _mainGame.SaveRoot!.YourTiles)
            {
                if (tempList.Any(items => items == thisIndex))
                    return true;
            }
            return false;
        }
        private readonly TileRummyMainGameClass _mainGame;
        public MainSets(IBasicGameVM thisMod) : base(thisMod)
        {
            _mainGame = thisMod.MainContainer!.Resolve<TileRummyMainGameClass>();
        }
    }
}