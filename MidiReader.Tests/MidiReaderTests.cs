using Xunit;
using System.Linq;
using MidiReader.Models;
using MidiReader.Models.Events;
using static MidiReader.Tests.Resources.ResourcesHelper;

namespace MidiReader.Tests {
    public class MidiReaderTests {

        [Fact]
        public void ReadASingleFile() {

            var midiPath = GetResourcePathFile("Twinkle.mid");

            var midi = Midi.Read(midiPath);

            // MThd Information
            Assert.Equal(0, midi.Version);
            Assert.Single(midi.Tracks);
            Assert.Equal(128, midi.PulsesPerQuarterNote);

            // TotalEvents Information
            var allEvents = midi.Tracks[0].Events;

            Assert.Equal(31, allEvents.Count());

            // MetaEvents Information
            var metaEvents = allEvents.Where(midiEvent => midiEvent.Event is MetaEvent).ToArray();

            Assert.Equal(3, metaEvents.Count());
            Assert.Equal("TimeSignature: 4/4 48 8", metaEvents[0].EventInformation);
            Assert.Equal("KeySignature: C Major", metaEvents[1].EventInformation);
            Assert.Equal("EndOfTrack: End of track", metaEvents[2].EventInformation);

            // MessageEvents Information
            var messageEvents = allEvents.Where(midiEvent => midiEvent.Event is MessageEvent).ToArray();

            Assert.Equal(28, messageEvents.Count());
            Assert.Equal("NoteOn: 1 C4 40", messageEvents[0].EventInformation);
            Assert.Equal("NoteOn: 1 G4 45", messageEvents[4].EventInformation);
            Assert.Equal("NoteOn: 1 C4 0", messageEvents[27].EventInformation);
        }
    }
}
