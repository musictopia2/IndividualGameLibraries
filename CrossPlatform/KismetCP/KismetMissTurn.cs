using BasicGameFramework.CommonInterfaces;
using BasicGameFramework.MultiplayerClasses.BasicPlayerClasses;
using BasicGameFramework.TestUtilities;
using BasicGameFramework.YahtzeeStyleHelpers;
using System.Linq;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace KismetCP
{
    public class KismetMissTurn : IMissTurnClass<YahtzeePlayerItem<KismetDice>>
    {
        private readonly YahtzeeViewModel<KismetDice> _thisMod;
        private readonly TestOptions _thisTest;
        private readonly IAsyncDelayer _delay;
        public KismetMissTurn(YahtzeeViewModel<KismetDice> thisMod, TestOptions thisTest, IAsyncDelayer delay)
        {
            _thisMod = thisMod;
            _thisTest = thisTest;
            _delay = delay;
        }

        public async Task PlayerMissTurnAsync(YahtzeePlayerItem<KismetDice> player)
        {
            _thisMod.NormalTurn = player.NickName;
            player.Scoresheet!.Visible = true;
            player.Scoresheet.ClearRecent();
            RowInfo thisItem = player.Scoresheet.RowList.Where(Items => Items.RowSection == EnumRowEnum.Regular
                && Items.HasFilledIn() == false).Take(1).SingleOrDefault();
            if (thisItem != null)
            {
                thisItem.IsRecent = true;
                thisItem.PointsObtained = 0; //i doubt you can do anything because its busy anyways automatically.
                if (_thisTest.NoAnimations == false)
                    await _delay.DelayMilli(1200);
            }
            player.Scoresheet.Visible = false;
        }
    }
}