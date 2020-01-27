namespace MidiReader.Models.Events {
    public class SysexEvent : IMidiEvent {

        #region Constructor

        public SysexEvent() {}

        public override string ToString() => EventInformation;
        #endregion

        public string EventInformation => "Some SysexEvent";
    }
}
