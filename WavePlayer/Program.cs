using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Linq;
using System.Media;

namespace WavePlayer {
    class Program {

        static int Main(params string[] args) {

            RootCommand rootCommand = new RootCommand("A simple wave player") {

                new Option(
                    new string[] { "--file-path", "-f" },
                    "The absolute path of your wave file"
                ) {

                    Argument = new Argument<FileInfo>()
                },

                new Option(
                    new string[] { "--unmanaged-memory", "-u" },
                    "Play the file from a Memory Mapped Stream already started"
                ) {

                    Argument = new Argument<string>()
                }
            };

            rootCommand.Handler = CommandHandler.Create<FileInfo, string>((filePath, unmanagedMemory) => {

                if (filePath == null && unmanagedMemory == null) {

                    Console.WriteLine(@"SoundPlayerTest:
  A simple wave player

Usage:
  SoundPlayerTest [options]

Options:
  -f, --file-path <file-path>                  The absolute path of your wave file
  -u, --unmanaged-memory <unmanaged-memory>    Play the file from a Memory Mapped Stream already started
  --version                                    Display version information
");
                }

                else if (filePath != null && unmanagedMemory != null) {

                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("--file-path and --ummanaged-memory can't be used at the same time");
                    Console.ResetColor();
                }

                else {
                    Play(filePath, unmanagedMemory);
                }
            });

            return rootCommand.InvokeAsync(args).Result;
        }

        static void Play(FileInfo file, string unmmanagedMemoryName) {

            if (unmmanagedMemoryName == null) {

                try {

                    Console.WriteLine($"Playing: {file?.FullName.Split('\\').Last()}");

                    using (SoundPlayer player = new SoundPlayer(file.FullName)) {

                        player.PlaySync();
                    }
                }

                catch {

                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("The current file doesn't exist or is not readable");
                    Console.ResetColor();
                }
            }

            else {

                using (MemoryMappedFile mmFile = MemoryMappedFile.OpenExisting(unmmanagedMemoryName)) {
                    using (MemoryMappedViewStream mmStream = mmFile.CreateViewStream()) {
                        using (SoundPlayer player = new SoundPlayer(mmStream)) {
                            player.PlaySync();
                        }
                    }
                }
            }
        }
    }
}
