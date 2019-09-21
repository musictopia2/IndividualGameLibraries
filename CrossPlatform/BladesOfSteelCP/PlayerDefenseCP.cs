using BasicGameFramework.DrawableListsViewModels;
using BasicGameFramework.MultiplayerClasses.BasicPlayerClasses;
using BasicGameFramework.RegularDeckOfCards;
using BasicGameFramework.ViewModelInterfaces;
namespace BladesOfSteelCP
{
    public class PlayerDefenseCP : HandViewModel<RegularSimpleCard>
    {
        private EnumPlayerCategory PlayerCategory { get; set; }
        protected override bool CanEverEnable()
        {
            return PlayerCategory == EnumPlayerCategory.Self;
        }
        public void LoadBoard(BladesOfSteelPlayerItem thisPlayer)
        {
            PlayerCategory = thisPlayer.PlayerCategory;
            HandList = thisPlayer.DefenseList;
            Maximum = 3;
            AutoSelect = HandViewModel<RegularSimpleCard>.EnumAutoType.None;
            Text = thisPlayer.NickName + " Defense";
        }
        protected override void AddObjectToHand(RegularSimpleCard payLoad)
        {
            payLoad.Drew = false;
            if (PlayerCategory == EnumPlayerCategory.Self)
                payLoad.IsUnknown = false;
            else
            {
                payLoad.IsUnknown = true;
                payLoad.IsSelected = false;
            }
        }
        protected override void AfterPopulateObjects()
        {
            HandList.ForEach(thisCard =>
            {
                AddObjectToHand(thisCard); //even better.
            });
        }
        public PlayerDefenseCP(IBasicGameVM thisMod) : base(thisMod) { }
    }
}