using BasicGameFrameworkLibrary.CommandClasses;
using BasicGameFrameworkLibrary.DrawableListsObservable;
using BasicGameFrameworkLibrary.MultiplayerClasses.BasicPlayerClasses;
using BasicGameFrameworkLibrary.RegularDeckOfCards;
using BladesOfSteelCP.Data;

namespace BladesOfSteelCP.CustomPiles
{
    public class PlayerDefenseCP : HandObservable<RegularSimpleCard>
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
            AutoSelect = HandObservable<RegularSimpleCard>.EnumAutoType.None;
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
        public PlayerDefenseCP(CommandContainer command) : base(command) { }
    }
}
