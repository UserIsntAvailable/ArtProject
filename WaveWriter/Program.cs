using System;
using System.IO;
using System.Linq;
using System.CommandLine;
using System.Diagnostics;
using System.Collections.Generic;
using System.IO.MemoryMappedFiles;
using System.CommandLine.Invocation;
using WaveWriter.Models;
using MidiReader.Models;
using MidiReader.Models.Events;

namespace WaveWriter {
    class Program {

        static int Main(string[] args) {

            RootCommand rootCommand = new RootCommand("A simple midi converter to wave") {

                new Option(
                    new string[] {"--input", "-i"},
                    "The midi file path that you want read")
                    { Argument = new Argument<FileInfo>() },

                new Option(
                    new string[] {"--output", "-o"},
                    "The final name of your file ( without extention )")
                    { Argument = new Argument<string>() },

                new Option(
                    new string[] {"--play", "-p"},
                    "Play the file instead of create a new file")
                    { Argument = new Argument<bool>() },
            };

            rootCommand.Handler = CommandHandler.Create<FileInfo, string, bool>((input, output, play) => {

                if (input == null && output == null && !play) {

                    Console.WriteLine(@"WaveWriter:
  A simple midi converter to wave

Usage:
  WaveWriter [options]

Options:
  -i, --input <input>      The midi file path that you want read
  -o, --output <output>    The final name of the file
  -p, --play               Play the file instead of create a new file
  --version                Show version information
  -?, -h, --help           Show help and usage information");
                }

                else if (input == null && output != null) {

                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("You can't set the output without setting your input");
                    Console.ResetColor();
                }

                else if (input == null && play) {

                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("You can't play a file without setting your input");
                    Console.ResetColor();
                }

                else {

                    MusicalNote[] notes = MidiToMusicalNotes(input.FullName);

                    MemoryStream waveStream = WavePlayer.PrepareWavePlayer(notes);

                    Console.WriteLine($"Playing {input.FullName.Split("\\")[^1].Replace(".mid", "")}");

                    if (!play) {

                        var newFileName = output == null
                            ? input.FullName.Replace(".mid", ".wav")
                            : string.Join("\\", input.FullName.Split("\\")[..^1]) + $"\\{output}.wav";

                        using (FileStream fileStream = File.Create(newFileName)) {

                            waveStream.CopyTo(fileStream);
                        }
                    }

                    else {

                        ReplaySong:

                        var mapName = "Arknights is the best gacha game in my heart";

                        using (MemoryMappedFile mmFile = MemoryMappedFile.CreateNew(mapName, waveStream.Length)) {
                            using (MemoryMappedViewStream mmStream = mmFile.CreateViewStream()) {

                                waveStream.Seek(0, SeekOrigin.Begin);
                                waveStream.CopyTo(mmStream);

                                using (System.Diagnostics.Process wavePlayer = new System.Diagnostics.Process()) {

                                    wavePlayer.StartInfo = new ProcessStartInfo() {

                                        FileName = @"C:\Users\Angel Pineda\Desktop\C# Projects\Git\Art Project\WavePlayer\bin\Debug\WavePlayer.exe",
                                        Arguments = $"-u \"{mapName}\"",
                                        CreateNoWindow = false,
                                    };

                                    wavePlayer.Start();
                                    wavePlayer.WaitForExit();
                                }
                            }
                        }

                        Console.WriteLine();
                        Console.WriteLine("Do you want replay the song? (y/n)");

                        if (Console.ReadLine() == "y") {

                            goto ReplaySong;
                        }
                    }
                }
            });

            return rootCommand.InvokeAsync(args).Result;
        }

        static MusicalNote[] MidiToMusicalNotes(string path) {

            Midi midi = Midi.Read(path);

            int BPM = midi.BeatsPerMinute;

            int PPQN = midi.PulsesPerQuarterNote;

            MusicalNote GetMusicalNote(MidiEvent firstNoteEvent,
               MidiEvent secondNoteEvent,
               double amplitude) {

                var µsPerQuarter = (60.0 / BPM) * 1000000;
                var msPerTick = µsPerQuarter / PPQN / 1000;

                var duration = secondNoteEvent.AbsoluteTime - firstNoteEvent.AbsoluteTime;

                var note = firstNoteEvent.EventData.Split(" ")[1];

                return new MusicalNote(note, amplitude, duration * msPerTick);
            }

            var mainEvents = midi.Tracks
                .OrderBy(track => track.Events.Count())
                .Last()
                .Where(midiEvent => midiEvent.EventType == "NoteOn" ||
                       midiEvent.EventType == "NoteOff" ||
                       midiEvent.EventType == "SetTempo")
                .ToArray();

            var searchingNoteOnEvent = false;

            var lastNoteEvent = new MidiEvent();

            var musicalNotes = new List<MusicalNote>();

            for (int i = 0; i < mainEvents.Count(); i++) {

                var currentEvent = mainEvents[i];

                switch (currentEvent.EventType) {

                    case "NoteOn": {

                            if (searchingNoteOnEvent) {

                                musicalNotes.Add(GetMusicalNote(lastNoteEvent, currentEvent, 0));

                                searchingNoteOnEvent = false;
                            }

                            var multiplesNoteOnEvents = mainEvents[i..^1]
                                .TakeWhile(nextEvent => nextEvent.AbsoluteTime == currentEvent.AbsoluteTime)
                                .ToArray();

                            if (multiplesNoteOnEvents.Count() == 1) {

                                lastNoteEvent = currentEvent;
                            }

                            else {

                                lastNoteEvent = multiplesNoteOnEvents[0];

                                i += multiplesNoteOnEvents.Count() - 1;
                            }

                        }
                        break;

                    case "NoteOff": {

                            bool isSameChannel = currentEvent.EventData.Split(" ")[0] == lastNoteEvent.EventData.Split(" ")[0];

                            bool isSameChord = currentEvent.EventData.Split(" ")[1] == lastNoteEvent.EventData.Split(" ")[1].Split("+")[^1];

                            if (isSameChannel && isSameChord) {

                                musicalNotes.Add(GetMusicalNote(lastNoteEvent, currentEvent, 0.25));
                            }

                            else {
                                continue;
                            }

                            searchingNoteOnEvent = true;

                            lastNoteEvent = currentEvent;
                        }
                        break;

                    case "SetTempo": {

                            BPM = int.Parse(currentEvent.EventData.Replace(" BPM", ""));
                        }
                        break;
                }
            }

            return musicalNotes.ToArray();
        }
    }
}
