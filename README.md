# NBagOfTricks
C# things collected over the years.

Requires VS2022 and .NET8.

No dependencies on external components.

Probably I should make this into a nuget package at some point.

![logo](felix.jpg)

# Core

## Components
- MmTimerEx: A theoretically better multimedia timer with improved accuracy for sub 10 msec period.
- MultiFileWatcher: Multiple file change watcher.
- TimeIt: High speed event statistics.

## Various utilities and extensions
- MathUtils: Numbers are good.
- MiscUtils: Things that don't fit anywhere else.
- StringUtils: Mostly low-level extensions.
- MusicDefinitions: Scales and chords.
- ColorUtils: Conversion between `System.Color`, `ConsoleColor` and ANSI format.
- KeyUtils: Keyboard input.
- BitmapUtils: Make it easy to manipulate bitmaps.

# Simple Logger
- Singleton manager.
- Client creates multiple named loggers.
- Log records go to log file and/or notification event hook for UI.
- Note: Be careful with handling notifications - don't call Logger functions in UI constructors as the handle is not assigned
   and Invoke() will fail.

# PNUT
Practically Nonexistent Unit Tester

A public version of a super lightweight unit test framework for C#. It has gone through many 
useful and successful iterations and may as well bring you joy also.

Based on a [C/C++ version](https://github.com/cepthomas/c-bag-of-tricks/blob/main/README.md) which was
inspired by [Quicktest](http://quicktest.sourceforge.net/) from long ago.

## Test Format
See the [test suite](https://github.com/cepthomas/NBagOfTricks/blob/master/Test/Test_PNUT.cs) for how to use this.

File look something like this:
```c#
public class PNUT_ONE : TestSuite
{
    public override void RunSuite()
    {
        int int1 = 321;
        string str1 = "round and round";
        string str2 = "the mulberry bush";

        UT_INFO("Suite tests core functions.");

        // Should fail on UT_STR_EQUAL.
        UT_EQUAL(str1, str2);

        // Should pass on UT_STR_EQUAL.
        UT_EQUAL(str2, "the mulberry bush");

        // Should fail on UT_NOT_EQUAL.
        UT_NOT_EQUAL(int1, 321);
    }
}
```

## Output Formats
There are two formats supported.

Readable text For humans. ! indicates a test failure.
```
Test Suite PNUT_1: Test basic check macros
Property version:xyz123
Property rand-seed:999
Visually inspect that this appears in the output with parm == 2. 2
! (c:\dev\pnut\test.cpp:25) PNUT_1.2 [1] should be greater than [2]
! (c:\dev\pnut\test.cpp:43) PNUT_2.3 [round and round] should equal [the mulberry bush]
Inspect Clouseau
! (c:\dev\pnut\test.cpp:69) PNUT_2.15 [1.5] should be within 0.099 of [1.6]
```

A JUnit style intended for consumption by CI. Not tested yet but looks nice.
```xml
<?xml version="1.0" encoding="UTF-8"?>
<testsuites tests=21 failures=0 time=00:00:00.012 >
    <testsuite name = ETC_33>
        <testcase name="ETC_33.1" classname="ETC_33" />
        <testcase name="ETC_33.2 " classname="ETC_33">
            <failure message="C:\foo\test.cs:113 [lwil/4xG|] != [Should fail]"></failure>
        </testcase>
        <testcase name="ETC_33.3" classname="ETC_33" />
    </testsuite>
</testsuites>
```
