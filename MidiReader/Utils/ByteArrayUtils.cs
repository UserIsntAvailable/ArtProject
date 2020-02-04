using System.Linq;

namespace MidiReader.Utils {
    internal static class ByteArrayUtils {

        /// <summary>
        /// Transform Hex representation to Int
        /// </summary>
        internal static int HexToInt(this byte[] array) {

            string hex = string.Join("", array.Select(n => n.ToString("X2")));

            return int.Parse(hex, System.Globalization.NumberStyles.HexNumber);
        }

        /// <summary>
        /// Get the Most Significant Bit from the hex that represents your byte array
        /// </summary>
        /// <returns>The MSB of your byte array</returns>
        internal static int GetMostSignificantBit(this byte[] array) {

            string hex = string.Join("", array.Select(n => n.ToString("X2")));

            return int.Parse(hex[^1].ToString(), System.Globalization.NumberStyles.HexNumber);
        }

        /// <summary>
        /// Get the Least Significant Bit from the hex that represents your byte array
        /// </summary>
        /// <returns>The MSB of your byte array</returns>
        internal static int GetLeastSignificantBit(this byte[] array) {

            string hex = string.Join("", array.Select(n => n.ToString("X2")));

            return int.Parse(hex[0].ToString(), System.Globalization.NumberStyles.HexNumber);
        }

        /// <summary>
        /// Get a SMTPEFrames Division from <see cref="Midi.Division"/>
        /// </summary>
        /// <returns>The SMTPEFrames Format</returns>
        internal static (int, int) GetSMTPEFrames(this byte[] array) {

            var ticksPerFrame = array[0];

            var framesPerSecond = (~array[1] & 0x000000FF) + 1;

            return (ticksPerFrame, framesPerSecond);
        }

    }
}
