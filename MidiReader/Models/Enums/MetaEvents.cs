namespace MidiReader.Models.Enums {
    public enum MetaEvents {

        SequenceNumber         = 0,
        TextEvent              = 1,
        CopyrightNotice        = 2,
        TrackName              = 3,
        InstrumentName         = 4,
        Lyric                  = 5,
        Marker                 = 6,
        CuePoint               = 7,
        MidiChannelPrefix      = 32,
        EndOfTrack             = 47,
        SetTempo               = 81,
        SMTPEOffset            = 84,
        TimeSignature          = 88,
        KeySignature           = 89,
        SystemExclusiveMessage = 127,
    }
}
