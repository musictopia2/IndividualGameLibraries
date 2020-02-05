using BasicGameFramework.BasicDrawables.Interfaces;
using BasicGameFramework.TestUtilities;

namespace TeeItUpCP
{
    public class TeeItUpDeckCount : IDeckCount
    {
        private readonly TestOptions _testConfig;

        public TeeItUpDeckCount(TestOptions testConfig)
        {
            _testConfig = testConfig;
        }

        public int GetDeckCount()
        {
            if (_testConfig.DoubleCheck == false)
                return 87; //change to what it really is.
            return 110;
        }
    }
}