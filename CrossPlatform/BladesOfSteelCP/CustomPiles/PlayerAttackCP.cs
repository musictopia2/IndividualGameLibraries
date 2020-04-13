using BasicGameFrameworkLibrary.CommandClasses;
using BasicGameFrameworkLibrary.DrawableListsObservable;
using BasicGameFrameworkLibrary.MultiplayerClasses.BasicPlayerClasses;
using BasicGameFrameworkLibrary.RegularDeckOfCards;
using BladesOfSteelCP.Data;

namespace BladesOfSteelCP.CustomPiles
{
    public class PlayerAttackCP : HandObservable<RegularSimpleCard>
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
        public PlayerAttackCP(CommandContainer command) : base(command) { }
    }
}