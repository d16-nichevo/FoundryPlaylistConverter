namespace FoundryPlaylistConverter
{
    internal class Program
    {
        // All alpha and numeric characters that can be
        // used to make up a Foundry ID (A-Z, a-z, 0-9).
        private const string IdChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

        private static Random Random = new Random();
        private static List<string> MusicFiles = new List<string>();

        /// <summary>
        /// Entry point.
        /// </summary>
        static void Main(string[] args)
        {
            try
            {
                // Sanity check args:
                if (args.Length != 3)
                {
                    // Bad args. Show usage info:
                    Usage();
                    Environment.Exit(1);
                }
                // Do work if args are okay:
                ReadPlaylist(args[1]);
                WriteJson(args[0], args[2]);
                Environment.Exit(0);
            }
            catch (Exception ex)
            {
                // Last-chance exception handler:
                Console.WriteLine($"ERROR: Unhandled expcetion:");
                Console.WriteLine(ex.ToString());
                Environment.Exit(1);
            }
        }

        /// <summary>
        /// Reads a M3U playlist or text file full of filenames.
        /// </summary>
        /// <param name="path">The path to the file to read.</param>
        private static void ReadPlaylist(string path)
        {
            // Bail if file doesn't exist:
            if (!File.Exists(path))
            {
                throw new FileNotFoundException(path);
            }

            // Read each line:
            foreach (string line in File.ReadLines(path))
            {
                if (line.StartsWith("file://"))
                {
                    // This seems to be a file protocol line (as is common in M3Us).
                    // Treat it accordingly. Note: it is URL-encoded.
                    // Example: file:///C:/Music/Games/Game%20123/track%20five.mp3
                    Uri u = new Uri(line);
                    // LocalPath gets us the OS-specific, non-file-protocol path.
                    if (File.Exists(u.LocalPath))
                    {
                        MusicFiles.Add(u.LocalPath);
                        // Console.WriteLine($"Found: {u.LocalPath}");
                    }
                }
                else if (File.Exists(line))
                {
                    // This is not a file protocol line.
                    // See if it's a file, ignore otherwise:
                    MusicFiles.Add(Path.GetFullPath(line));
                    // Console.WriteLine($"Found: {line}");
                }
            }

            // If we didn't find any music files, something went wrong:
            if (MusicFiles.Count == 0)
            {
                throw new InvalidDataException($"Found no files inside playlist: {path}");
            }
        }

        /// <summary>
        /// Writes the list of files to a JSON format
        /// understandable by Foundry.
        /// </summary>
        /// <param name="userDataPath">The user data directory used by Foundry.</param>
        /// <param name="outputPath">The path to the JSON file to write.</param>
        private static void WriteJson(string userDataPath, string outputPath)
        {
            // Bail out if the specified directory isn't real:
            if (!Directory.Exists(userDataPath))
            {
                throw new DirectoryNotFoundException(userDataPath);
            }

            // Write to the output file:
            using (StreamWriter outputFile = new StreamWriter(outputPath))
            {
                // First, write "preamble" stuff:
                outputFile.WriteLine(
$@"{{
  ""folder"": ""{GetId()}"",
  ""name"": ""{Path.GetFileNameWithoutExtension(outputPath)}"",
  ""sounds"": [");

                // Then write JSON data for each file in the list:
                for (int i = 0; i < MusicFiles.Count; i++)
                {
                    outputFile.WriteLine(GenerateJsonSoundArrayElement(MusicFiles[i], userDataPath, i != MusicFiles.Count - 1));
                }

                // Finally, write "afterwards" stuff:
                outputFile.WriteLine(
$@"],
  ""channel"": ""music"",
  ""mode"": 1,
  ""playing"": false,
  ""fade"": null,
  ""sorting"": ""a"",
  ""seed"": 0,
  ""flags"": {{
    ""exportSource"": {{
      ""world"": ""foundry-playlist-convertor"",
      ""system"": ""pf2e"",
      ""coreVersion"": ""12.331"",
      ""systemVersion"": ""6.11.1""
    }}
  }},
  ""_stats"": {{
    ""coreVersion"": ""12.331"",
    ""systemId"": ""pf2e"",
    ""systemVersion"": ""6.8.5"",
    ""createdTime"": {DateTimeOffset.Now.ToUnixTimeSeconds()},
    ""modifiedTime"": {DateTimeOffset.Now.ToUnixTimeSeconds()},
    ""lastModifiedBy"": ""{GetId()}""
  }},
  ""description"": """"
}}
");
            }

            // Note result to user:
            Console.WriteLine($"SUCCESS: Wrote {MusicFiles.Count} items to {outputPath}");
        }

        /// <summary>
        /// Write a chunk of Foundry-compatible JSON for a single audio file.
        /// </summary>
        /// <param name="soundPath">The path to the audio file, as taken from a playlist.</param>
        /// <param name="userDataPath">The user data directory used by Foundry.</param>
        /// <param name="trailingComma">Whether to put a trailing comma. Usually this is true except for the last item.</param>
        private static string GenerateJsonSoundArrayElement(string soundPath, string userDataPath, bool trailingComma)
        {
            string name = Path.GetFileNameWithoutExtension(soundPath);
            string path = Path.GetRelativePath(userDataPath, soundPath).Replace('\\', '/');
            string id = GetId();
            string comma = trailingComma ? "," : String.Empty;
            return $@"    {{
      ""name"": ""{name}"",
      ""path"": ""{path}"",
      ""_id"": ""{id}"",
      ""channel"": ""music"",
      ""playing"": false,
      ""pausedTime"": null,
      ""repeat"": false,
      ""volume"": 0.5,
      ""fade"": null,
      ""sort"": 0,
      ""flags"": {{}}
    }}{comma}";
        }

        /// <summary>
        /// Generate a Foundry-compatible ID.
        /// </summary>
        /// <returns>
        /// This is fully random. Is this how Foundry does it?
        /// Probably not. But it works.
        /// </returns>
        private static string GetId()
        {
            var stringChars = new char[16];
            for (int i = 0; i < stringChars.Length; i++)
            {
                stringChars[i] = IdChars[Random.Next(IdChars.Length)];
            }
            return new String(stringChars);
        }

        /// <summary>
        /// Display usage information.
        /// </summary>
        private static void Usage()
        {
            string exeName;
            try
            {
                exeName = Path.GetFileNameWithoutExtension(Environment.ProcessPath)!.ToUpper();
            }
            catch
            {
                exeName = "FOUNDRYPLAYLISTCONVERTOR";
            }
            Console.WriteLine(
$@"Converts a list of files or M3U playlist into a Playlist JSON importable by Foundry VTT.

For more information, visit:
https://github.com/d16-nichevo/FoundryPlaylistConverter

{exeName} [userdataroot] [playlist] [output]

  userdataroot  Path to foundry's data root.  
  playlist      Path to the input list.
  output        Path to write the JSON output."
            );
        }
    }
}