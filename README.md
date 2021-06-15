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
A super lightweight [unit test framework for C#](https://github.com/cepthomas/NBagOfTricks/blob/master/Source/PNUT/PNUT.md).

# Parser
A simple roll your own parser. I have always meant to build one of these for odd jobs.
Code Project informed me about [Easier Hand Rolled Parsers](https://www.codeproject.com/Articles/1280230/Easier-Hand-Rolled-Parsers)
so I digested, then regurgitated, rinsed and repeated. The basic structure and concepts are hers (CPOL) but most of it
is pretty much mine.

# Various utilities and extensions
- KeyUtils: Keyboard input.
- MathUtils: Numbers are good.
- UiUtils: Control helpers.
- MiscUtils: Things that don't fit anywhere else.
- StringUtils: Mostly extensions.
- Dumper: Writes object tree contents.

# !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!

# PNUT
Practically Nonexistent Unit Tester

A public version of a super lightweight unit test framework for C#. It has gone through many 
useful and successful iterations and may as well bring you joy also.

It is based on a [C/C++ version](https://github.com/cepthomas/c-bag-of-tricks) which was inspired by [Quicktest](http://quicktest.sourceforge.net/) from long ago.

## Test Format
See the [test suite](https://github.com/cepthomas/NBagOfTricks/blob/master/Test/Test_PNUT.cs) for how to use this.

Something like this:
```c#
public class PNUT_ONE : TestSuite
{
    public override void RunSuite()
    {
        int int1 = 321;
        int int2 = 987;
        string str1 = "round and round";
        string str2 = "the mulberry bush";
        double dbl1 = 1.500;   
        double dbl2 = 1.600;
        double dblTol = 0.001;

        UT_INFO("Suite tests core functions.");

        UT_INFO("Test UT_INFO. Visually inspect that this appears in the output.");

        UT_INFO("Should fail on UT_STR_EQUAL.");
        UT_EQUAL(str1, str2);

        // Should pass on UT_STR_EQUAL.
        UT_EQUAL(str2, "the mulberry bush");

        UT_INFO("Should fail on UT_NOT_EQUAL.");
        UT_NOT_EQUAL(int1, 321);

        // etc....
    }
}
```

## Output Formats
There are two formats currently supported.

### Readable Text
For humans. ! indicates a test failure.

```
#------------------------------------------------------------------
# Unit Test Report
# Start Time: 2018-05-31 08:02:13
# Duration: 0
# Cases Run: 17
# Cases Failed: 7
# Test Result: Fail
#--------------------------------------------------------------------

Test Suite PNUT_1: Test basic check macros
Property version:xyz123
Property rand-seed:999
Visually inspect that this appears in the output with parm == 2. 2
! (c:\dev\pnut\test.cpp:25) PNUT_1.2 [1] should be greater than [2]

Test Suite PNUT_2: The remaining tests for pnut.h
! (c:\dev\pnut\test.cpp:43) PNUT_2.3 [round and round] should equal [the mulberry bush]
! (c:\dev\pnut\test.cpp:47) PNUT_2.5 [321] should not equal [321]
! (c:\dev\pnut\test.cpp:51) PNUT_2.7 [987] should be less than or equal to [321]
! (c:\dev\pnut\test.cpp:57) PNUT_2.10 [321] should be greater than [987]
! (c:\dev\pnut\test.cpp:61) PNUT_2.12 [321] should be greater than or equal to [987]
Inspect Clouseau
! (c:\dev\pnut\test.cpp:69) PNUT_2.15 [1.5] should be within 0.099 of [1.6]
```


### JUnit
A subset intended for consumption by CI. Not tested yet but looks nice.

```xml
<?xml version="1.0" encoding="UTF-8"?>
<testsuites tests=21 failures=0 time=00:00:00.012 >
    <testsuite name = PNUT_1>
        <testcase name="PNUT_1.1 " classname="PNUT_1">
            <failure message="C:\Dev\pnut\cs\test.cs:34 [round and round] != [the mulberry bush]"></failure>
        </testcase>
        <testcase name="PNUT_1.2" classname="PNUT_1" />
        <testcase name="PNUT_1.3" classname="PNUT_1" />
        <testcase name="PNUT_1.4 " classname="PNUT_1">
            <failure message="C:\Dev\pnut\cs\test.cs:42 [321] == [321]"></failure>
        </testcase>
        <testcase name="PNUT_1.5" classname="PNUT_1" />
        <properties>
            <property name="version" value="xyz123" />
            <property name="rand-seed" value="999" />
        </properties>
    </testsuite>
    <testsuite name = ETC_33>
        <testcase name="ETC_33.1" classname="ETC_33" />
        <testcase name="ETC_33.2 " classname="ETC_33">
            <failure message="C:\Dev\pnut\cs\test.cs:113 [lwil/4xG|] != [Should fail]"></failure>
        </testcase>
        <testcase name="ETC_33.3" classname="ETC_33" />
    </testsuite>
</testsuites>
```

# License
https://github.com/cepthomas/NBagOfTricks/blob/master/LICENSE
