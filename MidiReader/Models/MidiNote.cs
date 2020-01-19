using System;
using System.Linq;

namespace MidiReader.Models {
    internal class MidiNote {

        #region Const Notes Identifier

        private const double C = 0;

        private const double Csharp = noteIdentifer;

        private const double D = 2 * noteIdentifer;

        private const double Dsharp = 3 * noteIdentifer;

        private const double E = 4 * noteIdentifer;

        private const double F = 5 * noteIdentifer;

        private const double Fsharp = 6 * noteIdentifer;

        private const double G = 7 * noteIdentifer;

        private const double Gsharp = 8 * noteIdentifer;

        private const double A = 9 * noteIdentifer;

        private const double Asharp = 10 * noteIdentifer;

        private const double B = 11 * noteIdentifer;
        
        /// <summary>
        /// This is not the real identifer. I made this for distinguish each note more easily
        /// </summary>
        private const double noteIdentifer = ((13d / 12) - 1);
        #endregion

        #region Constructor

        public MidiNote(byte midiNote) {
            Note = GetMidiNote(midiNote);
        }

        public override string ToString() => Note;
        #endregion

        #region Public Properties

        public string Note { get; set; }
        #endregion

        #region Static Public Methods

        /// <summary>
        /// Get a Midi Note from a midi identifer note
        /// </summary>
        /// <param name="midiNote">The midi identifier</param>
        /// <seealso cref="http://www.music.mcgill.ca/~ich/classes/mumt306/StandardMIDIfileformat.html#BMA1_3"/>
        /// <returns>The note, ex: C4</returns>
        public static string GetMidiNote(int midiNote) {

            double note = midiNote / 12d;

            int noteOctave = (int)Math.Truncate(note - 1);

            double noteIdentifier = Math.Round(note - noteOctave - 1, 3);

            var noteName = typeof(MidiNote).GetFields()
                .Where(field => Math.Round((double)field.GetValue("peepoWeird"), 3) == noteIdentifier)
                .First()
                .Name
                .Replace("sharp", "#");

            return noteName + noteOctave;
        }

        /// <summary>
        /// Get the Midi Identifier from a midi note
        /// </summary>
        /// <param name="note">The note, ex: C4</param>
        /// <seealso cref="http://www.music.mcgill.ca/~ich/classes/mumt306/StandardMIDIfileformat.html#BMA1_3"/>
        /// <returns>The midi identifier, ex: C4 = 60</returns>
        public static int GetMidiIdentifier(string note) {

            int noteOctave = int.Parse(note[^1].ToString());

            string midiNote = note.Length == 2
                ? note[..1]
                : note.Replace("#", "sharp")[..6];

            double midiNoteValue = (double)typeof(MidiNote)
                .GetField(midiNote)
                .GetValue("IDK why this is necessary");

            return (int)(12 * (midiNoteValue + 1) + 12 * noteOctave);
        }
        #endregion
    }
}
