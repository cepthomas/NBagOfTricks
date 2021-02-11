# NBagOfTricks
C# things collected over the years.

Targets .NET Framework 4.7.2. No dependencies on third party components.

Probably I should make this into a nuget package at some point.

![logo](felix.jpg)

# Components
- MmTimerEx: A theoretically better multimedia timer with improved accuracy for sub 10 msec period.
- MultiFileWatcher: Multiple file change watcher.
- TimingAnalyzer: High speed event statistics.
- Command line arg parser.

# UI controls for audio (or other) apps
- Meter: Linear or log.
- Pot: Just like on your guitar.
- Slider: Just like on your mixer.
- VirtualKeyboard: Piano control based loosely on Leslie Sanford's [Midi Toolkit](https://github.com/tebjan/Sanford.Multimedia.Midi).

# General purpose UI components
- PropertyGridEx: Added a few features.
- FilTree: Folder/file tree control with tags/filters and notifications.
- OptionsEditor: User can select from a list of strings, or add/delete elements.
- ClickGrid: Essentially a grid array of buttons.
- TimeBar: Elapsed time control.
- BarBar: Similar to TimeBar but shows musical bars and beats.
- CpuMeter: Standalone display control.
- TextViewer: With colorizing.
- WaitCursor: Easy to use cursor.

# PNUT
- A super lightweight [unit test framework for C#](https://github.com/cepthomas/NBagOfTricks/blob/master/Source/PNUT/PNUT.md).

# Various utilities and extensions
- KeyUtils: Keyboard input.
- MathUtils: Numbers are good.
- UiUtils: Control helpers.
- MiscUtils: Things that don't fit anywhere else.
- StringUtils: Mostly extensions.
- Dumper: Writes object tree contents.
