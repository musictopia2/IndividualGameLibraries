namespace ThreeLetterFunCP.EventModels
{
    public class AdvancedSettingsEventModel
    {
        public AdvancedSettingsEventModel(string message)
        {
            Message = message;
        }

        public string Message { get; }
    }
}