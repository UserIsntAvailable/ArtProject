namespace MidiReader.Models.Events {

    /// <summary>
    /// I don't really need SysexEvents. This is a template if you would need to use them
    /// </summary>
    public class SysexEvent : IMidiEvent {

        #region Constructor

        public SysexEvent(byte[] data) {

            EventData = data;
        }

        public override string ToString() => EventInformation;
        #endregion

        #region Public Properties

        /// <summary>
        /// The Data of the sysex event ( you will need to read it by yourself )
        /// </summary>
        public byte[] EventData { get; set; }

        /// <summary>
        /// Information Template
        /// </summary>
        public string EventInformation => "Some SysexEvent";
        #endregion
    }
}
