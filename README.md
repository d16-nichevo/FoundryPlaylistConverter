# About This Tool

This is a command-like tool that will take either:

* `M3U` [playlist](https://en.wikipedia.org/wiki/M3U)
  * Many audio players can create `M3U` playists, including FooBar and VLC Player.
* A text file containing a list of filenames.

... and it will convert that file to ...

* A `JSON` file compatible with [Foundry VTT](https://foundryvtt.com/). Specifically, Foundry's Playlists feature.

I created this tool because it's much easier to work with playlists inside audio player software than it is to work with the Playlists in Foundry.

# ⚠️ Warning

I created this tool for my own use. It is not highly-polished. Note that:

1. It has not been extensively tested.
   * This tool theoretically should be able to run on Linux, but I use a Windows machine, and have not tested on Linux at all.
1. It probably contains bugs.
1. I haven't gone to great efforts to make it user-friendly.

If this tool proves to be popular I may refine it. If you'd like to express interest, see the contact section at the bottom of this readme.

# Installation

(to be filled in)

# Usage

Using the command line is out-of-scope for this readme. If you need help, there are guides on the internet. [Here is one.](https://www.freecodecamp.org/news/how-to-use-the-cli-beginner-guide/)

The usage information will appear if you use the tool without correct arguments:

```
Converts a list of files or M3U playlist into a Playlist JSON importable by Foundry VTT.

For more information, visit:
https://github.com/nichevo/blah

FOUNDRYPLAYLISTCONVERTOR [userdataroot] [playlist] [output]

  userdataroot  Path to foundry's data root.
  playlist      Path to the input list.
  output        Path to write the JSON output.
```

⚠️ Remember: it's good practise to enclose each argument in quotes `"`.

An example valid command line to use this tool:

* `FoundryPlaylistConvertor "C:/Documents/Foundry App Data/Data" "C:/playlists/combat-music.m3u" "C:/downloads/foundry-combat-playlist.json"`

## `userdataroot` Argument

This is the [User Data location for Foundry](https://foundryvtt.com/article/user-data/).

If you're not sure where this is:

1. Open Foundry.
1. On the main screen, open the settings in the top-right.
1. Look for a setting called **User Data Path**.
   * On my computer (Windows) the value is `C:/Documents/Foundry App Data`

The `userdataroot` argument is as per this value, but with `/Data` added. For example:

* `C:/Documents/Foundry App Data/Data`

# `playlist` Argument

This is the full path to the file to read from. (It may be possible to use a relative path, I didn't test.)

The file can either be:

1. A valid [M3U file](https://en.wikipedia.org/wiki/M3U).
   * Many audio players can create `M3U` playists, including FooBar and VLC Player.
1. A text file where each line contains a full path to an audio file.
   * It shouldn't be a problem if the file contains lines that non-path content. But don't include files that aren't audio files.

⚠️ Important: All files in the playlist or text file list must be somewhere inside the `userdataroot` directory. Foundry simply will not serve files from outside the User Data location.

So if your `userdataroot` is `C:/Documents/Foundry App Data/Data`:

* Your playlist could contain the file `C:/Documents/Foundry App Data/Data/Music/Combat/track1.mp3`
* Your playlist should NOT contain the file `C:/My Music/Combat/track1.mp3`

## `output` Argument

This is simply the full path to where you want the `JSON` file to be written.

Ideally you should name the file with a `.JSON` extension, but you don't have to.

# Contact the Author

If you find this tool useful, and you'd like to see it better-developed, please let me know.

* https://deck16.net/contact