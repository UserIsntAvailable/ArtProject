using System;
using System.IO;
using System.Text;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using MidiReader.Utils;

namespace MidiReader.Models {
    public class Midi : IEnumerable<MTrk> {

        #region IEnumerable Implementation

        public IEnumerator<MTrk> GetEnumerator() {

            foreach (var track in Tracks) {

                yield return track;
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        #endregion

        #region Public Properties

        /// <summary>
        /// The version of the Midi file ( v0, v1 or v2 )
        /// </summary>
        public int Version { get; set; }

        /// <summary>
        /// The resolution of the MIDI controller ( Division Format 0 )
        /// </summary>
        public int PulsesPerQuarterNote {
            get {
                return Division.GetMostSignificantBit() == 0
                    ? Division.HexToInt()
                    : 0;
            }
        }

        /// <summary>
        /// The base BMP of the Midi file
        /// </summary>
        public int BeatsPerMinute {
            get {

                var tempoMetaEvents = Tracks[0].Events
                    .Where(metaEvent => metaEvent.EventType == "SetTempo");

                return tempoMetaEvents.Count() == 0

                    // The default BPM if is not specified
                    ? 120
                    : int.Parse(tempoMetaEvents.First().EventData);
            }
        }

        /// <summary>
        /// The resolution of the MIDI controller ( Division Format 1 )
        /// </summary>
        public (int, int) SMTPEFrame {
            get {
                return Division.GetMostSignificantBit() == 1
                    ? Division.GetSMTPEFrames()
                    : (0, 0);
            }
        }

        /// <summary>
        /// The default unit of delta-time for this MIDI file
        /// </summary>
        public byte[] Division { get; set; }

        /// <summary>
        /// Each track of the Midi file ( 1 if version = 0 )
        /// </summary>
        public MTrk[] Tracks { get; set; }
        #endregion

        /// <summary>
        /// This will log all the information of the midi file
        /// </summary>
        /// <returns>All the midi information</returns>
        public override string ToString() {

            var trackInfo = new StringBuilder();

            for (int i = 0; i < Tracks.Count(); i++) {

                trackInfo.Append($"------ Track number {i + 1} ------\n\n");

                foreach (var midiEvent in Tracks[i]) {

                    trackInfo.Append(midiEvent.EventType == "EndOfTrack"
                        ? $"{midiEvent}\n\n"
                        : $"{midiEvent}\n");
                }
            }

            return $@"Midi Version: {Version}
Number of tracks: {Tracks.Count()}
{(PulsesPerQuarterNote != 0
    ? $"Pulses per quarter note: {PulsesPerQuarterNote}"
    : $"SMTPE frames: Deltatime units per SMTPE frame: {SMTPEFrame.Item1} | SMTPE frames per second: {SMTPEFrame.Item2}")}

{trackInfo}";
        }

        #region Static Methods

        /// <summary>
        /// Reads a Midi file
        /// </summary>
        /// <param name="path">The path of the .mid that you want read</param>
        /// <returns><see cref="Midi"/></returns>
        public static Midi Read(string path) => MidiReader.Read(path);

        /// <summary>
        /// Reads a midi file then output the log on the same directory
        /// </summary>
        /// <param name="path">The path of the .mid that you want read</param>
        public static void ReadAndLogMidiInfo(string path) {

            Midi midi = MidiReader.Read(path);

            using (StreamWriter midiLog = new StreamWriter(path.Replace(".mid", ".txt")))
                midiLog.WriteLine(midi.ToString());
        }

        /// <summary>
        /// Log a <see cref="Midi"/> into a file
        /// </summary>
        /// <param name="midi">The midi object</param>
        /// <param name="path">The output will be by default on your Desktop</param>
        public static void MidiToLogFile(Midi midi, string path = "Desktop") {

            string logPath = path == "Desktop"
                ? Environment.GetFolderPath(Environment.SpecialFolder.Desktop)
                : path;

            using (StreamWriter midiLog = new StreamWriter(logPath))
                midiLog.WriteLine(midi.ToString());
        }
        #endregion
    }
}
