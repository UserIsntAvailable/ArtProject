namespace MidiReader.Models.Events.MessageEvents.SystemMessageEvents {
    internal class SystemExclusiveMessageEvent : ISystemMessageEvent {

        #region Constructor

        public SystemExclusiveMessageEvent(Enums.SystemMessageEvents eventType, string eventData) {

            SystemMessageEventType = eventType; SystemMessageEventData = eventData;
        }

        public override string ToString() => $"{SystemMessageEventType}: {SystemMessageEventData}";
        #endregion

        #region Public Properties

        public Enums.SystemMessageEvents SystemMessageEventType { get; set; }

        public string SystemMessageEventData { get; set; }
        #endregion
    }
}
