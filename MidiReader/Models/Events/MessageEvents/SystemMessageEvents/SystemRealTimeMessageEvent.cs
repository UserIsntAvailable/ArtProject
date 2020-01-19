using System;

namespace MidiReader.Models.Events.MessageEvents.SystemMessageEvents {
    internal class SystemRealTimeMessageEvent : ISystemMessageEvent {

        #region Constructor

        public SystemRealTimeMessageEvent(Enums.SystemMessageEvents eventType) {

            SystemMessageEventType = eventType;
        }

        public override string ToString() => SystemMessageEventType.ToString();
        #endregion

        #region Public Properties

        public Enums.SystemMessageEvents SystemMessageEventType { get; set; }

        [Obsolete]
        public string SystemMessageEventData { get; set; }
        #endregion
    }
}
