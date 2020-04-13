using BasicGameFrameworkLibrary.CommonInterfaces;
using BasicGameFrameworkLibrary.MultiplayerClasses.BasicPlayerClasses;
using BasicGameFrameworkLibrary.SpecializedGameTypes.YahtzeeStyleHelpers.Containers;
using BasicGameFrameworkLibrary.SpecializedGameTypes.YahtzeeStyleHelpers.Data;
using BasicGameFrameworkLibrary.SpecializedGameTypes.YahtzeeStyleHelpers.Logic;
using BasicGameFrameworkLibrary.TestUtilities;
using CommonBasicStandardLibraries.Exceptions;
using KismetCP.Data;
using System.Linq;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace KismetCP.Logic
{
    public class KismetMissTurn : IMissTurnClass<YahtzeePlayerItem<KismetDice>>
    {
        private readonly YahtzeeVMData<KismetDice> _model;

        //private readonly YahtzeeViewModel<KismetDice> _thisMod;
        private readonly TestOptions _thisTest;
        private readonly IAsyncDelayer _delay;
        private readonly IScoreLogic _scoreLogic;
        private readonly ScoreContainer _scoreContainer;
        private readonly YahtzeeGameContainer<KismetDice> _gameContainer;

        public KismetMissTurn(
            YahtzeeVMData<KismetDice> model,
            TestOptions thisTest,
            IAsyncDelayer delay,
            IScoreLogic scoreLogic,
            ScoreContainer scoreContainer,
            YahtzeeGameContainer<KismetDice> gameContainer
            )
        {
            _model = model;
            _thisTest = thisTest;
            _delay = delay;
            _scoreLogic = scoreLogic;
            _scoreContainer = scoreContainer;
            _gameContainer = gameContainer;
        }

        public async Task PlayerMissTurnAsync(YahtzeePlayerItem<KismetDice> player)
        {
            _model.NormalTurn = player.NickName;
            _scoreContainer.RowList = player.RowList;
            _scoreLogic.ClearRecent();
            RowInfo thisItem = player.RowList.Where(Items => Items.RowSection == EnumRow.Regular
                && Items.HasFilledIn() == false).Take(1).SingleOrDefault();
            if (thisItem != null)
            {
                thisItem.IsRecent = true;
                thisItem.PointsObtained = 0; //i doubt you can do anything because its busy anyways automatically.
                if (_thisTest.NoAnimations == false)
                {
                    //needs to show the new ui.
                    if (_gameContainer.GetNewScoreAsync == null)
                    {
                        throw new BasicBlankException("Nobody is handling the getting new scores.  Rethink");
                    }
                    await _gameContainer.GetNewScoreAsync();
                    await _delay.DelayMilli(1200);
                }
            }
        }
    }
}