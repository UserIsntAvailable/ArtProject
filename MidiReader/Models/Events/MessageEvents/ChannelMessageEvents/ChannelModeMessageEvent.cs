using MidiReader.Models.Enums;

namespace MidiReader.Models.Events.MessageEvents.ChannelMessageEvents {
    internal class ChannelModeMessageEvent : IChannelMessageEvent<ChannelModeMessageEvents> {

        #region Constructor

        public ChannelModeMessageEvent(ChannelModeMessageEvents eventType, string eventData) {

            ChannelMessageEventType = eventType; ChannelMessageEventData = eventData;
        }

        public override string ToString() => $"{ChannelMessageEventType}: {ChannelMessageEventData}";
        #endregion

        #region Public Properties

        public ChannelModeMessageEvents ChannelMessageEventType { get; set; }

        public string ChannelMessageEventData { get; set; }
        #endregion
    }
}
