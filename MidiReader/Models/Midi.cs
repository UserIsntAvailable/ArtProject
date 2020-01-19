using System.Collections;
using System.Collections.Generic;

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
        /// I prefer BPM tbh
        /// </summary>
        public double PulsesPerQuarterNote { get; set; }

        /// <summary>
        /// The base BMP of the Midi file
        /// </summary>
        public double BeatsPerMinute => 60 * 1e6 / PulsesPerQuarterNote; // IDK if this works

        /// <summary>
        /// Each track of the Midi file ( 1 if version = 0 )
        /// </summary>
        public MTrk[] Tracks { get; set; }
        #endregion

        #region Static Methods

        public static Midi Read(string path) => MidiReader.Read(path);
        #endregion
    }
}
