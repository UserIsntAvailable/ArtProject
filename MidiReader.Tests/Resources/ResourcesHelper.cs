using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;

namespace MidiReader.Tests.Resources {
    internal static class ResourcesHelper {

        /// <summary>
        /// Get a resource file path from Resources folder
        /// </summary>
        /// <param name="filename">The filename</param>
        /// <returns>The full path of the file</returns>
        internal static string GetResourcePathFile(string filename) {

            /* I don't know if this is the best way to do this method
             * I will try to improve it later : / */

            string assemblypath = Assembly.GetExecutingAssembly().Location;

            Match match = Regex.Match(assemblypath, @"^.+Art Project");

            return Path.Join(match.Value + "\\MidiReader.Tests\\Resources", filename);
        }
    }
}
