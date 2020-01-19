namespace MidiReader.Models.Events {
    public struct MidiEvent {

        #region Constructor

        public MidiEvent(long relativeTime, long absoluteTime, IMidiEvent midiEvent) {

            RelativeTime     = relativeTime;
            AbsoluteTime     = absoluteTime;
            Event            = midiEvent;
        }

        public override string ToString() => $"{AbsoluteTime}: {RelativeTime} | {EventInformation}";
        #endregion

        #region Public Properties

        /// <summary>
        /// Time difference between the previous event
        /// </summary>
        public long RelativeTime { get; set; }

        /// <summary>
        /// Total time since the first event
        /// </summary>
        public long AbsoluteTime { get; set; }

        /// <summary>
        /// MidiEvent of this instance
        /// </summary>
        public IMidiEvent Event { get; set; }

        /// <summary>
        /// The type + the data of the event
        /// </summary>
        public string EventInformation => Event.EventInformation;

        /// <summary>
        /// The Enum that define the Event
        /// </summary>
        public string EventType => EventInformation.Split(" ")[0].Replace(":", "");

        /// <summary>
        /// The data that contains that Event
        /// </summary>
        public string EventData => string.Join(" ", EventInformation.Split(" ")[1..^2]);
        #endregion
    }
}
