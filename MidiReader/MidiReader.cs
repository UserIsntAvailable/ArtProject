using System;
using System.IO;
using MidiReader.Utils;
using MidiReader.Models;
using MidiReader.Models.Events;

namespace MidiReader {
    public static class MidiReader {

        /// <summary>
        /// Read a midi file
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        internal static Midi Read(string path) {

            // Opening midiFile as BinaryReader
            BinaryReader midiReader = new BinaryReader(File.OpenRead(path));

            // Object to return
            Midi midi = new Midi();

            /* MThd ( ASCII Header Chunk )
             * Length */
            midiReader.ReadBytes(8);

            midi.Version = midiReader.ReadBytes(2).GetMostSignificantBit();

            int numberOfTracks = midiReader.ReadBytes(2).GetMostSignificantBit();

            var midiTiming = midiReader.ReadBytes(2);

            if (midiTiming.GetMostSignificantBit() == 0) {
                midi.PulsesPerQuarterNote = midiTiming.HexToInt();
            }
            else {
                throw new NotImplementedException("MidiTiming v2 isn't supported yet");
            }

            var midiTracks = new MTrk[numberOfTracks];

            for (int i = 0; i < numberOfTracks; i++) {

                MTrk mTrk = new MTrk();

                // MTrk ( ASCII Header Chunk )
                midiReader.ReadBytes(4);

                var trackDataReader = new BinaryReader(
                    new MemoryStream(
                        // Track data
                        midiReader.ReadBytes(
                            // Length of the track chunk
                            midiReader.ReadBytes(4).HexToInt()
                            )
                        )
                    );

                int lastDeltaTime = 0;

                // This will be an infinite loop that will be break if
                // trackEvent == MetaEvents.EndOfTrack
                for (int j = 0; j > -1; j++) {

                    var deltaTime = (int)trackDataReader.ReadVariableLengthQuantity();

                    var trackEvent = trackDataReader.ReadMidiEvent();

                    mTrk.Events.Add(new MidiEvent(deltaTime, lastDeltaTime += deltaTime, trackEvent));

                    if (trackEvent is MetaEvent metaEvent) {

                        if (metaEvent.EventType == Models.Enums.MetaEvents.EndOfTrack) break;
                    }
                }

                midiTracks[i] = mTrk;
            }

            midi.Tracks = midiTracks;

            return midi;
        }
    }
}
