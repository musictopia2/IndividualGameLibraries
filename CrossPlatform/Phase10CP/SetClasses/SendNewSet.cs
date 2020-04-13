using Phase10CP.Data;

namespace Phase10CP.SetClasses
{
    public class SendNewSet
    {
        public string CardListData { get; set; } = ""; // so it can use the extension
        public EnumWhatSets WhatSet { get; set; }
        public int FirstNumber { get; set; }
        public int SecondNumber { get; set; } // not sure why it showed the second number.  but trust its needed.
    }
}