# What It Is
A hodge-podge Sublime Text plugin containing odds and ends missing from or over-developed in other packages.
The focus is not on code development but rather general text processing.

No support as yet for PackageControl.

Built for Windows and ST4. Other OSes and ST versions will require some hacking.

![logo](scooter.gif)


# Commands and Settings

## Highlighting
- Word colorizing similar to [StyleToken](https://github.com/vcharnahrebel/style-token).
- Persists to sbot project file.
- Also a handy scope popup that shows you the style associated with each scope.

### Commands

| Command                  | Description |
|:--------                 |:-------     |
| sbot_highlight_text      | Highlight text 1 through 6 from `highlight_scopes` |
| sbot_clear_highlight     | Remove highlight in selection |
| sbot_clear_all_highlights| Remove all highlights |
| sbot_show_scopes         | Popup that shows style for scopes |
| sbot_show_eol            | Toggles showing EOLs |

### Code
```xml
<?xml version="1.0" encoding="UTF-8"?>
<testsuites tests=21 failures=0 time=00:00:00.012 >
    <testsuite name = ETC_33>
        <testcase name="ETC_33.1" classname="ETC_33" />
        <testcase name="ETC_33.2 " classname="ETC_33">
            <failure message="C:\Dev\pnut\cs\test.cs:113 [lwil/4xG|] != [Should fail]"></failure>
        </testcase>
        <testcase name="ETC_33.3" classname="ETC_33" />
    </testsuite>
</testsuites>
```


## Signets (bookmarks)

> Is this a block quote?
>

Enhanced bookmarks:
- `Bookmark` and `mark` are already taken so I shall use `signet` which means in French:
> "Petit ruban ou filet qu'on insère entre les feuillets d'un livre pour marquer l'endroit que l'on veut retrouver."
- Persists to sbot-sigs file.
- Next/previous (optionally) traverses files in project - like VS.
- Bookmark key mappings have been stolen:
    - `ctrl+f2`: sbot_toggle_signet
    - `f2`: sbot_next_signet
    - `shift+f2`: sbot_previous_signet
    - `ctrl+shift+f2`: sbot_clear_signets

# Here's a startup sequence
```
ST: reloading plugin SublimeBagOfTricks.__init__
ST: reloading plugin SublimeBagOfTricks.sbot
Python: load sbot_common
Python: load sbot
Python: load sbot_clean
Python: load sbot_common
Python: load sbot_format
Python: load sbot_highlight
Python: load sbot_misc
Python: load sbot_render
Python: load sbot_sidebar
Python: load sbot_signet
>>> Re-saved sbot_common.py
ST: reloading plugin SublimeBagOfTricks.sbot_common
Python: load sbot_common
```
