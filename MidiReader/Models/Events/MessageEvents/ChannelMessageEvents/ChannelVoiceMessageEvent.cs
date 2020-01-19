using MidiReader.Models.Enums;

namespace MidiReader.Models.Events.MessageEvents.ChannelMessageEvents {
    internal class ChannelVoiceMessageEvent : IChannelMessageEvent<ChannelVoiceMessageEvents> {

        #region Constructor

        public ChannelVoiceMessageEvent(ChannelVoiceMessageEvents eventType, string eventData) {

            ChannelMessageEventType = eventType; ChannelMessageEventData = eventData;
        }

        public override string ToString() => $"{ChannelMessageEventType}: {ChannelMessageEventData}";
        #endregion

        #region Public Properties

        public ChannelVoiceMessageEvents ChannelMessageEventType { get; set; }

        public string ChannelMessageEventData { get; set; }
        #endregion
    }
}
