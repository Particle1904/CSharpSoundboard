# Particle Soundboard App 
The Avalonia C# Soundboard App is a lightweight, cross-platform soundboard application designed for streaming .mp3 and .wav files to both local speakers and VB Cable Input simultaneously. Whether you're a content creator, streamer, or just want a simple way to play sounds during voice chats, this app has you covered.

![Alt Text](https://github.com/Particle1904/CSharpSoundboard/blob/be0eb8e2217500759ea714827b7373b754a2d9e9/SoundboardDescriptionImage.png?raw=true)

## Features
- Simultaneous Playback: Stream .mp3 and .wav files to both the local speakers and VB Cable Input at the same time.
- Stop All Sounds: Easily stop all playing sounds with a single click.
- Volume Control: Configure local and Cable Input volumes separately for precise control.
- Custom Sound Folder: Choose your preferred folder to read sound files, with a default folder in the root directory of the application.
- Multi-Sound Playback: Play two or more sounds simultaneously, avoiding the exact same sound.
- Audio Overlap Toggle: Toggle between overlapping audio playback or non-overlapping audio.
- Cross-Platform GUI: Capable of running on Linux and MacOS with minor code adjustments and a different Virtual Audio Cable (VAC) software/driver, as VB Cable is Windows and Mac only.
![Alt Text](https://github.com/Particle1904/CSharpSoundBoard/blob/main/WLSExample.png?raw=true)

## Getting Started
To get started with the Particle Soundboard, download the provided release (for Windows) or build yourself.
To build clone this repository and open the project in Visual Studio 2022 or later. You can then build and run the project.

## Requirements
This software requires the .NET7 runtime:
- [.NET Desktop Runtime 7.0.4](https://dotnet.microsoft.com/en-us/download/dotnet/7.0)

## Usage
- Select your sound folder.
- Configure local and Cable Input volumes.
- Toggle audio overlap according to your preference.
- Play your desired sounds with ease.
- Stop all sounds when necessary.

## Technologies
- [ManagedBass](https://github.com/ManagedBass/ManagedBass) [(un4seen bass)](https://www.un4seen.com): Utilized for audio management.
- [Avalonia](https://avaloniaui.net): Employed for creating a multiplatform graphical user interface (GUI).
- [.NET 7 and C#](https://dotnet.microsoft.com/en-us/download/dotnet/7.0): The core development stack.
- [VB Cable](https://vb-audio.com/Cable/): Used for audio routing on Windows and MacOS.

## Contributing
Contributions to the Particle Soundboard are welcome. If you would like to contribute, fork this repository, make your changes, and create a pull request.

## License
The Particle Soundboard is licensed under the MIT License. See the LICENSE file for more information.
