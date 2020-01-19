namespace MidiReader.Models.Events {
    internal class MessageEvent : IMidiEvent {

        #region Constructor

        public MessageEvent(object messageEvent) {
            EventInformation = messageEvent.ToString();
        }

        public override string ToString() => EventInformation;
        #endregion

        #region Public Properties

        public string EventInformation { get; }
        #endregion
    }
}
