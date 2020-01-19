namespace MidiReader.Models.Enums {
    public enum SystemMessageEvents {

        // System Exclusives Messages
        StartOfSystemExclusive   = 240,
        EndOfSystemExclusive     = 247,

        // System Common Messgages
        MidiTimeCodeQuarterFrame = 241,
        SongPositionPointer      = 242,
        SongSelect               = 243,
        Undefined1               = 244,
        Undefined2               = 245,
        TuneRequest              = 246,

        // System RealTime Messages
        TimingClock              = 248,
        Undefined3               = 249,
        Start                    = 250,
        Continue                 = 251,
        Stop                     = 252,
        Undefined4               = 253,
        ActiveSensing            = 254,
        SystemReset              = 255,
    }
}
