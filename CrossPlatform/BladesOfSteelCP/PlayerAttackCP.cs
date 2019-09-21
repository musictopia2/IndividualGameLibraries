using BasicGameFramework.DrawableListsViewModels;
using BasicGameFramework.MultiplayerClasses.BasicPlayerClasses;
using BasicGameFramework.RegularDeckOfCards;
using BasicGameFramework.ViewModelInterfaces;
namespace BladesOfSteelCP
{
    public class PlayerAttackCP : HandViewModel<RegularSimpleCard>
    {
        private EnumPlayerCategory PlayerCategory { get; set; }
        protected override bool CanEverEnable()
        {
            return PlayerCategory == EnumPlayerCategory.Self;
        }
        protected override void AddObjectToHand(RegularSimpleCard payLoad)
        {
            payLoad.Drew = false;
        }
        public void LoadBoard(BladesOfSteelPlayerItem thisPlayer)
        {
            PlayerCategory = thisPlayer.PlayerCategory;
            HandList = thisPlayer.AttackList;
            Maximum = 3;
            AutoSelect = EnumAutoType.None;
            Text = thisPlayer.NickName + " Attack";
        }
        public PlayerAttackCP(IBasicGameVM thisMod) : base(thisMod) { }
    }
}