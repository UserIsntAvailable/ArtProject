using System;
using System.IO;
using System.Text;
using MidiReader.Models;
using MidiReader.Models.Enums;
using MidiReader.Models.Events;
using MidiReader.Models.Events.MessageEvents;
using MidiReader.Models.Events.MessageEvents.SystemMessageEvents;
using MidiReader.Models.Events.MessageEvents.ChannelMessageEvents;

namespace MidiReader.Utils {
    internal static class BinaryReaderUtils {

        #region Private Fields

        private static int channel;
        private static string eventData;
        private static string eventIndicator;
        private static string lastEventIndicator;
        private static bool isRunningStatus = false;
        #endregion

        #region Internal Extentions

        /// <summary>
        /// Reads a Midi Event from this reader
        /// </summary>
        /// <param name="reader">The reader</param>
        /// <returns>The Midi Event</returns>
        internal static IMidiEvent ReadMidiEvent(this BinaryReader reader) {

            if (!isRunningStatus) {
                eventIndicator = reader.ReadByte().ToString("X");
            }
            else {
                eventIndicator = lastEventIndicator;
            }

            // Meta Events
            if (eventIndicator == "FF") {

                return reader.ReadMetaEvent();
            }

            // Sysex Events
            else if (eventIndicator == "F0" || eventIndicator == "F7") {

                if (eventIndicator == "F0") {

                }
                else {

                }

                throw new NotImplementedException("SysexEvents aren't implemented yet");
            }

            // Midi Channel Message Events
            else {

                // Channel Message Events
                if (eventIndicator[0] != 'F') {

                    // 127 cuz all the Status Bytes are equal or below 127 ( 7F )
                    isRunningStatus = int.Parse(eventIndicator, System.Globalization.NumberStyles.HexNumber) <= 127;

                    // This will be already stored if Running Status is On
                    if (!isRunningStatus) {

                        channel = int.Parse(
                            eventIndicator[1].ToString(),
                            System.Globalization.NumberStyles.HexNumber) + 1;
                    }

                    // Channel Voice Message Event
                    if (eventIndicator[0] != 'B') {
                        return reader.ReadChannelVoiceMessageEvent();
                    }

                    // Channel Mode Message Event
                    else {
                        return reader.ReadChannelModeMessageEvent();
                    }
                }

                // System Message Events
                else {
                    throw new NotImplementedException("This is poorlyy designed i will do it later");
                    return reader.ReadSystemMessageEvent();
                }
            }
        }

        /// <summary>
        /// Reads a 7-bit encoded variable-length quantity from this BinaryReader
        /// </summary>
        /// <returns>The int tha represented the 7-bit</returns>
        internal static uint ReadVariableLengthQuantity(this BinaryReader reader) {

            var index = 0;
            uint buffer = 0;
            byte current;

            do {

                if (index++ == 8) {
                    throw new FormatException("Could not read variable-length quantity from provided stream.");
                }

                buffer <<= 7;
                current = reader.ReadByte();
                buffer |= (current & 0x7FU);
            } while ((current & 0x80) != 0);

            return buffer;
        }
        #endregion

        #region Private Extentions

        /// <summary>
        /// Reads a Meta Event from this reader
        /// </summary>
        /// <param name="reader">The reader</param>
        /// <returns>The <see cref="MetaEvent"/></returns>
        private static MetaEvent ReadMetaEvent(this BinaryReader reader) {

            var eventType = (MetaEvents)reader.ReadByte();

            switch (eventType) {

                case MetaEvents.SequenceNumber: {

                        // Useless
                        reader.ReadByte();

                        eventData = reader.ReadInt16().ToString();
                    }
                    break;

                case MetaEvents.TextEvent:
                case MetaEvents.CopyrightNotice:
                case MetaEvents.TrackName:
                case MetaEvents.InstrumentName:
                case MetaEvents.Lyric:
                case MetaEvents.Marker:
                case MetaEvents.CuePoint: {

                        eventData = Encoding.ASCII.GetString(
                            reader.ReadBytes(
                                (int)reader.ReadVariableLengthQuantity()
                                )
                            );
                    }
                    break;

                case MetaEvents.MidiChannelPrefix: {

                        // Useless
                        reader.ReadByte();

                        eventData = reader.ReadByte().ToString();
                    }
                    break;

                case MetaEvents.EndOfTrack: {

                        // Useless
                        reader.ReadByte();

                        eventData = "End of track";
                    }
                    break;

                // If not specified, the default tempo is 120 beats/minute,
                // which is equivalent to tttttt = 500000
                case MetaEvents.SetTempo: {

                        // Useless
                        reader.ReadByte();

                        var tempo = (int)Math.Round(60 / ((double)reader.ReadBytes(3).HexToInt() / 1000000));

                        eventData = $"{tempo} BPM";
                    }
                    break;

                case MetaEvents.SMTPEOffset: {

                        var hours = reader.ReadByte();

                        var minutes = reader.ReadByte();

                        var seconds = reader.ReadByte();

                        var frames = reader.ReadByte();

                        var fractionalFrames = reader.ReadByte();

                        eventData = $"{hours}/{minutes}/{seconds}:{frames} {fractionalFrames}";
                    }
                    break;

                case MetaEvents.TimeSignature: {

                        // Useless
                        reader.ReadByte();

                        var numerator = reader.ReadByte();

                        var denominator = Math.Pow(2, reader.ReadByte());

                        // Normally, there are 24 MIDI Clocks per quarter note
                        var clockPerMetronomeTick = reader.ReadByte();

                        // 8 is standard
                        var thirteethInQuarter = reader.ReadByte();

                        eventData = $"{numerator}/{denominator} {clockPerMetronomeTick} {thirteethInQuarter}";
                    }
                    break;

                case MetaEvents.KeySignature: {

                        // Useless ( 02 ) 
                        reader.ReadByte();

                        string sharpsOrFlats;

                        var indicator = reader.ReadByte();

                        if (indicator == 0) {
                            sharpsOrFlats = "C";
                        }
                        else if (indicator > 0) {
                            sharpsOrFlats = $"{indicator} sharps";
                        }
                        else {
                            sharpsOrFlats = $"{indicator} flats";
                        }

                        eventData = $"{sharpsOrFlats} {(reader.ReadByte().Equals(0) ? "Major" : "Minor")}";
                    }
                    break;

                // http://www.personal.kent.edu/~sbirch/Music_Production/MP-II/MIDI/midi_system_exclusive_messages.htm
                case MetaEvents.SystemExclusiveMessage: {

                        throw new NotImplementedException("MetaEvents.SystemExclusiveMessage isn't supported yet");
                    }

                default:
                    throw new Exception($"{(int)eventType} is not supported");
            }

            return new MetaEvent(eventType, eventData);
        }

        /// <summary>
        /// This is poorly designed. I will do it someday better but I need to study...
        /// </summary>
        /// <param name="reader">The reader</param>
        /// <seealso cref="http://www.personal.kent.edu/~sbirch/Music_Production/MP-II/MIDI/midi_system_real.htm"/>
        /// <seealso cref="http://www.personal.kent.edu/~sbirch/Music_Production/MP-II/MIDI/midi_system_common_messages.htm"/>
        /// <seealso cref="http://www.personal.kent.edu/~sbirch/Music_Production/MP-II/MIDI/midi_system_exclusive_messages.htm"/>
        /// <returns><see cref="MessageEvent"/></returns> 
        private static MessageEvent ReadSystemMessageEvent(this BinaryReader reader) {

            ISystemMessageEvent systemEvent = null;

            SystemMessageEvents systemEventType = (SystemMessageEvents)int.Parse(
                eventIndicator.ToString(),
                System.Globalization.NumberStyles.HexNumber);

            switch (systemEventType) {

                // I didn't do a Enum for each Manufacturer ID
                // http://www.personal.kent.edu/~sbirch/Music_Production/MP-II/MIDI/midi_system_exclusive_messages.htm
                case SystemMessageEvents.StartOfSystemExclusive: {

                        var id = reader.ReadByte();

                        if (id == 0) {

                            // I don't need this
                            var id2 = reader.ReadByte();

                            // This either
                            var id3 = reader.ReadByte();

                            eventData = $"{systemEvent} {id2} {id3}";
                        }
                        else {
                            eventData = id.ToString();
                        }

                        systemEvent = new SystemExclusiveMessageEvent(systemEventType, eventData);
                    }
                    break;
                case SystemMessageEvents.EndOfSystemExclusive: {
                        systemEvent = new SystemExclusiveMessageEvent(systemEventType, "");
                    }
                    break;

                case SystemMessageEvents.MidiTimeCodeQuarterFrame: {

                    }
                    break;
                case SystemMessageEvents.SongPositionPointer:
                    break;
                case SystemMessageEvents.SongSelect:
                    break;
                case SystemMessageEvents.Undefined1:
                    break;
                case SystemMessageEvents.Undefined2:
                    break;
                case SystemMessageEvents.TuneRequest:
                    break;

                case SystemMessageEvents.TimingClock:
                    break;
                case SystemMessageEvents.Undefined3:
                    break;
                case SystemMessageEvents.Start:
                    break;
                case SystemMessageEvents.Continue:
                    break;
                case SystemMessageEvents.Stop:
                    break;
                case SystemMessageEvents.Undefined4:
                    break;
                case SystemMessageEvents.ActiveSensing:
                    break;
                case SystemMessageEvents.SystemReset:
                    break;

                default:
                    throw new Exception($"{(int)systemEventType} is not supported");

            }

            return new MessageEvent(systemEvent);
        }

        /// <summary>
        /// Reads a Channel Voice Message Event from this reader
        /// </summary>
        /// <seealso cref="http://www.personal.kent.edu/~sbirch/Music_Production/MP-II/MIDI/midi_channel_voice_messages.htm"/>
        /// <param name="reader">The reader</param>
        /// <returns><see cref="MessageEvent"/></returns>
        private static MessageEvent ReadChannelVoiceMessageEvent(this BinaryReader reader) {

            ChannelVoiceMessageEvents voiceEventType = (ChannelVoiceMessageEvents)int.Parse(
                eventIndicator[0].ToString(),
                System.Globalization.NumberStyles.HexNumber);

            switch (voiceEventType) {

                case ChannelVoiceMessageEvents.NoteOff:
                // A corresponding note-off message must be sent for each and every note-on message
                case ChannelVoiceMessageEvents.NoteOn: {

                        // The Key/Note that is pressed or relased
                        // Each value is a 'half-step' above or below the adjacent values
                        string key = MidiNote.GetMidiNoteFromInt(reader.ReadByte());

                        // Devices which are not velocity sensitive should send vv = 40
                        var velocity = reader.ReadByte();

                        eventData = $"{channel} {key} {velocity}";
                    }
                    break;

                case ChannelVoiceMessageEvents.PolyphonicKeyPressure: {

                        // The Key/Note that is pressed or relased
                        // Each value is a 'half-step' above or below the adjacent values
                        string key = MidiNote.GetMidiNoteFromInt(reader.ReadByte());

                        // Pressure with which key is being pressed
                        var preassure = reader.ReadByte();

                        eventData = $"{channel} {key} {preassure}";
                    }
                    break;

                case ChannelVoiceMessageEvents.ControllerChange: {

                        // Too lazy for create an enum for each controller
                        // Read http://www.personal.kent.edu/~sbirch/Music_Production/MP-II/MIDI/midi_control_change_messages.htm
                        // if you neeed them
                        var controllerNumber = reader.ReadByte();

                        var controllerValue = reader.ReadByte();

                        eventData = $"{channel} {controllerNumber} {controllerValue}";
                    }
                    break;

                case ChannelVoiceMessageEvents.ProgramChange: {

                        var newPogramNumber = reader.ReadByte() + 1;

                        eventData = $"{channel} {newPogramNumber}";
                    }
                    break;

                case ChannelVoiceMessageEvents.ChannelKeyPressure: {

                        var channelPressureValue = reader.ReadByte();

                        eventData = $"{channel} {channelPressureValue}";
                    }
                    break;

                case ChannelVoiceMessageEvents.PitchBend: {

                        int leastSignificantByte = reader.ReadBytes(1).GetLeastSignificantBit();

                        int mostSignificantByte = reader.ReadBytes(1).GetMostSignificantBit();

                        eventData = $"{channel} {leastSignificantByte} {mostSignificantByte}";
                    }
                    break;

                default:
                    throw new Exception($"{(int)voiceEventType} is not supported");
            }

            lastEventIndicator = eventIndicator;

            return new MessageEvent(
                new ChannelVoiceMessageEvent(voiceEventType, eventData));
        }

        /// <summary>
        /// Reads a Channel Mode Message Event from this reader
        /// </summary>
        /// <seealso cref="http://www.personal.kent.edu/~sbirch/Music_Production/MP-II/MIDI/midi_channel_mode_messages.htm"/>
        /// <param name="reader">The reader</param>
        /// <returns><see cref="MessageEvent"/></returns>
        private static MessageEvent ReadChannelModeMessageEvent(this BinaryReader reader) {

            ChannelModeMessageEvents modeEventType = (ChannelModeMessageEvents)reader.ReadByte();

            switch (modeEventType) {
                case ChannelModeMessageEvents.AllSoundOff:
                case ChannelModeMessageEvents.ResetAllControllers:
                case ChannelModeMessageEvents.AllNotesOff:
                case ChannelModeMessageEvents.OminiModeOFf:
                case ChannelModeMessageEvents.OmniModeOn:
                case ChannelModeMessageEvents.PolyMode: {

                        // Useless ( 00 )
                        reader.ReadByte();

                        eventData = $"{channel}";
                    }
                    break;

                case ChannelModeMessageEvents.LocalControl: {

                        string enable = reader.ReadByte() == 0 ? "disconected" : "reconnected";

                        eventData = $"{channel} Local Keyboard {enable}";
                    }
                    break;

                case ChannelModeMessageEvents.MonoModeOn: {

                        // Number of MIDI Channels to use when in Mode 4
                        var channels = reader.ReadByte();

                        eventData = $"{channel} {channels}";
                    }
                    break;

                default:
                    throw new Exception($"{(int)modeEventType} is not supported");
            }

            lastEventIndicator = eventIndicator;

            return new MessageEvent(
                new ChannelModeMessageEvent(modeEventType, eventData));
        }
        #endregion
    }
}
