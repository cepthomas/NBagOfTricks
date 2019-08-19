# NBagOfTricks
A collection of the C# things I seem to use repeatedly.

No dependencies on third party components.

![logo](https://github.com/cepthomas/NBagOfTricks/blob/master/felix.jpg)

# PNUT
Practically Nonexistent Unit Tester

A public version of a super lightweight unit test framework for C/C++. It has gone through many 
useful and successful iterations and may as well bring you joy also.

Inspired by [Quicktest](http://quicktest.sourceforge.net/) from long ago.

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
A subset intended for consumption by Jenkins. Not tested yet but looks nice.

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
            <failure message="C:\Dev\pnut\cs\test.cs:113 [lwil/"4xG|] != [Should fail]"></failure>
        </testcase>
        <testcase name="ETC_33.3" classname="ETC_33" />
    </testsuite>
</testsuites>
```

# NStateMachine
- Semi-hierarchical state machine for .NET.
- Generates diagrams via dot.
- See Test_SM.cs for an example of usage.

# Components
- MmTimerEx: A theoretically better multimedia timer with improved accuracy for sub 10 msec period.
- MultiFileWatcher: Multiple file change watcher.
- TimingAnalyzer: High speed event statistics.

# Various utilities and extensions
- KeyUtils
- MathUtils
- MiscUtils
- StringUtils
- Dumper writes object tree contents.

# UI controls for audio apps
- Meter
- Pot
- Slider
- VirtualKeyboard: Control based on Le[Leslie Sanford's Midi Toolkit](https://github.com/tebjan/Sanford.Multimedia.Midi) piano.

# General purpose UI components
- PropertyGridEx
- WaitCursor

# License
https://github.com/cepthomas/NBagOfTricks/blob/master/LICENSE
