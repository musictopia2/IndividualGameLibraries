using BasicGameFramework.Attributes;
using BasicGameFramework.Dominos;
using BasicGameFramework.MultiplayerClasses.SavedGameClasses;
namespace ItalianDominosCP
{
    [SingletonGame]
    public class ItalianDominosSaveInfo : BasicSavedDominosClass<SimpleDominoInfo, ItalianDominosPlayerItem>
    {
        private int _NextNumber;

        public int NextNumber
        {
            get { return _NextNumber; }
            set
            {
                if (SetProperty(ref _NextNumber, value))
                {
                    //can decide what to do when property changes
                    if (_thisMod != null)
                        _thisMod.NextNumber = value;
                }

            }
        }
        private int _UpTo;

        public int UpTo
        {
            get { return _UpTo; }
            set
            {
                if (SetProperty(ref _UpTo, value))
                {
                    //can decide what to do when property changes
                    if (_thisMod != null)
                        _thisMod.UpTo = value;
                }

            }
        }

        private ItalianDominosViewModel? _thisMod;
        public void LoadMod(ItalianDominosViewModel thisMod)
        {
            _thisMod = thisMod;
            thisMod.UpTo = UpTo;
            thisMod.NextNumber = NextNumber;
        }

    }
}