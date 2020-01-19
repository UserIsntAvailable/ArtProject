namespace MidiReader.Models.Events.MessageEvents {

    /// <summary>
    /// Interface for Channel Message Events. <see cref="ChannelVoiceMessageEvent"/> & <see cref="ChannelModeMessageEvent"/>
    /// </summary>
    /// <typeparam name="T">Enum that repesents the Event</typeparam>
    public interface IChannelMessageEvent<T> {

        public T ChannelMessageEventType { get; set; }

        public string ChannelMessageEventData { get; set; }
    }
}
