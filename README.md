# NBagOfTricks
A collection of the C# things I seem to use repeatedly.

No dependencies on third party components.

TODO doc the other components.
TODO relocate WinForms parts?


# PNUT
Practically Nonexistent Unit Tester

A public version of a super lightweight unit test framework for C/C++ and C#. It has gone through many 
useful and successful iterations and may as well bring you joy also.

Inspired by [Quicktest](http://quicktest.sourceforge.net/) from long ago.

The C/C++ version is plain C++ with a little bit of stl so will build and run on any win or nx platform using any compiler. A VS2017 solution is provided. Alternatively you can use Qt Creator for vanilla (non-Qt) C/C++ projects.

See test.cpp/cs for an example of how to write unit tests and main.cpp/program.cs of how to run them.

## Output Formats
There are two formats currently supported.

### Readable Text
For humans.

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
        <testcase name="PNUT_1.6 " classname="PNUT_1">
            <failure message="C:\Dev\pnut\cs\test.cs:48 [987] not less than or equal [321]"></failure>
        </testcase>
        <testcase name="PNUT_1.7" classname="PNUT_1" />
        <testcase name="PNUT_1.8" classname="PNUT_1" />
        <testcase name="PNUT_1.9 " classname="PNUT_1">
            <failure message="C:\Dev\pnut\cs\test.cs:57 [321] not greater than [987]"></failure>
        </testcase>
        <testcase name="PNUT_1.10" classname="PNUT_1" />
        <testcase name="PNUT_1.11 " classname="PNUT_1">
            <failure message="C:\Dev\pnut\cs\test.cs:63 [321] not greater than or equal [987]"></failure>
        </testcase>
        <testcase name="PNUT_1.12" classname="PNUT_1" />
        <testcase name="PNUT_1.13" classname="PNUT_1" />
        <testcase name="PNUT_1.14" classname="PNUT_1" />
        <testcase name="PNUT_1.15 " classname="PNUT_1">
            <failure message="C:\Dev\pnut\cs\test.cs:75 [1.5] not close enough to [1.498]"></failure>
        </testcase>
    </testsuite>
    <testsuite name = PNUT_2>
        <testcase name="PNUT_2.1" classname="PNUT_2" />
        <testcase name="PNUT_2.2 " classname="PNUT_2">
            <failure message="C:\Dev\pnut\cs\test.cs:97 [1] not greater than [2]"></failure>
        </testcase>
        <testcase name="PNUT_2.3" classname="PNUT_2" />
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

## NStateMachine
Semi-hierarchical state machine for .NET. Generates diagrams via dot. See Test_SM.cs for an example of usage.

