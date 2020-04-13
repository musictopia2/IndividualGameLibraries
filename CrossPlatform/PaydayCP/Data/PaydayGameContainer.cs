using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.BasicDrawables.Dictionary;
using BasicGameFrameworkLibrary.BasicGameDataClasses;
using BasicGameFrameworkLibrary.CommandClasses;
using BasicGameFrameworkLibrary.CommonInterfaces;
using BasicGameFrameworkLibrary.DIContainers;
using BasicGameFrameworkLibrary.GameGraphicsCP.GameboardPositionHelpers;
using BasicGameFrameworkLibrary.MultiplayerClasses.GameContainers;
using BasicGameFrameworkLibrary.TestUtilities;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.RandomGenerator;
using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.Messenging;
using PaydayCP.Cards;
using System;
using System.Threading.Tasks;

namespace PaydayCP.Data
{
    [SingletonGame]
    [AutoReset]
    public class PaydayGameContainer : BasicGameContainer<PaydayPlayerItem, PaydaySaveInfo>
    {
        public PaydayGameContainer(
            BasicData basicData,
            TestOptions test,
            IGameInfo gameInfo,
            IAsyncDelayer delay,
            IEventAggregator aggregator,
            CommandContainer command,
            IGamePackageResolver resolver,
            RandomGenerator random) : base(basicData,
                test,
                gameInfo,
                delay,
                aggregator,
                command,
                resolver,
                random)
        {
        }


        public CustomBasicList<GameSpace> PrivateSpaceList { get; set; } = new CustomBasicList<GameSpace>();
        public PositionPieces? Pos;
        public CardInformation GetCard(int deck)
        {
            CardInformation output;
            if (deck <= 24)
            {
                output = new DealCard();
                output.Populate(deck);
                return output;
            }
            output = new MailCard();
            output.Populate(deck);
            return output;
        }
        public void RemoveOutCards(IDeckDict<CardInformation> listToRemove)
        {
            SaveRoot!.OutCards.RemoveGivenList(listToRemove); //hopefully this simple.
        }
        public void AddOutCard(CardInformation thisCard)
        {
            SaveRoot!.OutCards.Add(thisCard);
        }

        public int OtherTurn
        {
            get
            {
                return SaveRoot!.PlayOrder.OtherTurn;
            }
            set
            {
                SaveRoot!.PlayOrder.OtherTurn = value;
            }
        }

        public Func<int, Task>? SpaceClickedAsync { get; set; }
        public Func<int, Task>? ResultsOfMoveAsync { get; set; }

        public Func<Task>? OtherTurnProgressAsync { get; set; }

    }
}
