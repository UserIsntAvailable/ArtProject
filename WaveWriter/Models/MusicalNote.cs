using System;

namespace WaveWriter.Models {
    public class MusicalNote {

        #region Const Notes Frequencies

        public const double C = 16.35;

        public const double Csharp = 17.32;

        public const double D = 18.35;

        public const double Dsharp = 19.45;

        public const double E = 20.60;

        public const double F = 21.83;

        public const double Fsharp = 23.12;

        public const double G = 24.50;

        public const double Gsharp = 25.96;

        public const double A = 27.50;

        public const double Asharp = 29.14;

        public const double B = 30.87;
        #endregion

        #region Constructor

        public MusicalNote(string note, double amplitude, double duration) {

            Note = note; Amplitude = amplitude; Duration = duration;
        }

        public override string ToString() => $"{Note} | Amplitude: {Amplitude} | Duration: {Duration}";
        #endregion

        #region Public Properties

        /// <summary>
        /// The string representation of the Note
        /// </summary>
        public string Note { get; set; }

        /// <summary>
        /// The volume of the Note
        /// </summary>
        public double Amplitude { get; set; }

        /// <summary>
        /// The Frequency of the Note
        /// </summary>
        public double Frequency => GetNoteFrequency(Note);

        /// <summary>
        /// The time in milliseconds that the Note is pressed
        /// </summary>
        public double Duration { get; set; }
        #endregion

        /// <summary>
        /// Get a note frequency from his note string representation
        /// </summary>
        /// <param name="note">the string note representation</param>
        /// <returns>The frequency</returns>
        public static int GetNoteFrequency(string note) {

            int noteOctave = int.TryParse(note[^1].ToString(), out int octave)
                ? octave : 0;

            string midiNote = note.Length == 2
                ? note[..1]
                : note.Replace("#", "sharp")[..6];

            double noteFrequencyValue = (double)typeof(MusicalNote)
                .GetField(midiNote)
                .GetValue("IDK why this is necessary");

            for (int i = 0; i < noteOctave; i++) {
                noteFrequencyValue *= 2;
            }

            return (int)Math.Truncate(noteFrequencyValue);
        }
    }
}
