using MidiReader.Models.Enums;

namespace MidiReader.Models.Events {
    public class MetaEvent : IMidiEvent {

        #region Constructor

        public MetaEvent(MetaEvents eventType, string eventData) {

            EventType = eventType; EventData = eventData;
        }

        public override string ToString() => EventInformation;
        #endregion

        #region Public Properties

        public MetaEvents EventType { get; set; }

        public string EventData { get; set; }

        public string EventInformation => $"{EventType}: {EventData}";
        #endregion
    }
}
