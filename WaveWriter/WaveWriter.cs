using System;
using System.IO;
using NAudio.Wave;
using WaveWriter.Models;

namespace WaveWriter {
    public static class WavePlayer {

        #region Public Fields

        private static MemoryStream waveStream = new MemoryStream();

        private static readonly WaveFileWriter waveWriter
            = new WaveFileWriter(waveStream, new WaveFormat(44100, 16, 1));
        #endregion

        #region Public Methods

        public static void WriteNote(MusicalNote note) {

            double Samples = 441 * note.Duration / 10;

            for (int n = 0; n < Samples; n++) {

                float sample = (float)(note.Amplitude * Math.Sin((2 * Math.PI * n * note.Frequency) / waveWriter.WaveFormat.SampleRate));
                waveWriter.WriteSample(sample);
            }
        }

        public static MemoryStream PrepareWavePlayer(MusicalNote[] notes) {

            try {

                foreach (var note in notes) {

                    WriteNote(note);
                }
            }

            finally {

                if (waveWriter != null) {

                    waveWriter.Dispose();

                    waveStream = new MemoryStream(waveStream.ToArray());
                }
            }

            return waveStream;
        }
        #endregion
    }
}
