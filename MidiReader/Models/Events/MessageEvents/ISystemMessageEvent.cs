namespace MidiReader.Models.Events.MessageEvents {
    public interface ISystemMessageEvent{

        public Enums.SystemMessageEvents SystemMessageEventType { get; set; }

        public string SystemMessageEventData { get; set; }
    }
}
