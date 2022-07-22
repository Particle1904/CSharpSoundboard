# Particle Soundboard - Using C#, NAudio Library and VBCable.
 
**Why I made it:** 
So I used to use this free Soundboard App that I've found out there so I could stream audio directly to my microphone in Voice Apps (mostly in Discord)... problem was it was using about half a gb of Ram for a non stop stream of advertisements. I was literally freezing the app every once in a while! So I decided to make my own Soundboard. Very simple app with just one functionality: 
# **Stream Mp3 files directly to the microphone Output.**

![Alt Text](https://github.com/LeonardoFer/CSharpSoundBoard/blob/main/SoundboardDescriptionImage.png?raw=true)

Just clone and build it if you want to use it.

### How to use: 
1. Put the **MP3** files into the **Sounds folder*(**inside the app bin directory or where the app executable is**).

2. [You need to configure the VBCable Sound drivers](https://youtu.be/wUFbKww5-Vg?t=42).

3. **Select CABLE Output for your INPUT DEVICE in Discord to use it!**
4. Open your microphone and click the button you want to play the audio (it works better with **Discord Push-to-Talk** :)).
5. **Have fun!**

### **It only supports .Mp3 files at the moment!**

## Tech used and where to find them:
 - **C#** with **.NET6**.
 - **WPF** - I'm planning on migrating it to MAUI to learn MAUI when its released.
 - [NAudio and NAudio.Core NuGet Packages](https://www.nuget.org/packages/NAudio/) - This is the guy thats doing all the magic behind the scenes! Amazing C# Audio Library by Mark Heath and it runs on .NET5 and .NET6.

### Feel free to copy, change and improve it however you like!
