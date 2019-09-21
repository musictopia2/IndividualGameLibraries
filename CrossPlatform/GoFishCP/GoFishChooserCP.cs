using BasicGameFramework.ChooserClasses;
using BasicGameFramework.CommonInterfaces;
using BasicGameFramework.GameGraphicsCP.GamePieces;
using BasicGameFramework.RegularDeckOfCards;
using BasicGameFramework.ViewModelInterfaces;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using CommonBasicStandardLibraries.CollectionClasses;
using System.Linq;
namespace GoFishCP
{
    public class GoFishChooserCP : SimpleEnumPickerVM<EnumCardValueList, NumberPieceCP, CardValueListChooser>
    {
        public void LoadFromHandCardValues(GoFishPlayerItem thisPlayer) //its smart enough to take their hand part
        {
            var thisList = thisPlayer.MainHandList.GroupBy(items => items.Value).Select(Items => Items.Key).ToCustomBasicList();
            CustomBasicList<NumberPieceCP> TempList = new CustomBasicList<NumberPieceCP>();
            thisList.ForEach(items =>
            {
                IEnumPiece<EnumCardValueList> thisPiece = new NumberPieceCP();
                thisPiece.EnumValue = items;
                thisPiece.IsEnabled = IsEnabled;
                thisPiece.IsSelected = false;
                TempList.Add((NumberPieceCP)thisPiece);
            });
            ItemList.ReplaceRange(TempList);
        }
        public GoFishChooserCP(IBasicGameVM thisMod) : base(thisMod) { }
    }
}