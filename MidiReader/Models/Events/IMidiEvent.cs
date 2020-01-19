namespace MidiReader.Models.Events {
    public interface IMidiEvent {

        /// <summary>
        /// EventType + EventData
        /// </summary>
        public string EventInformation { get; }
    }
}
