using System.Collections;
using System.Collections.Generic;
using MidiReader.Models.Events;

namespace MidiReader.Models {
    public class MTrk : IEnumerable<MidiEvent> {

        #region IEnumerable Implementation

        public IEnumerator<MidiEvent> GetEnumerator() {

            foreach(var midiEvent in Events) {

                yield return midiEvent;
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        #endregion

        /// <summary>
        /// Each event on the midi file
        /// </summary>
        public List<MidiEvent> Events = new List<MidiEvent>();
    }
}
