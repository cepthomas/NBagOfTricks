using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using NBagOfTricks;
using System.Diagnostics;

namespace NBagOfTricks
{
    /// <summary>Definitions for use inside scripts. For doc see MusicDefinitions.md.</summary>
    public static class MusicDefinitions
    {
        #region Fields
        /// <summary>The chord/scale note definitions. Key is chord/scale name, value is list of constituent notes.</summary>
        static Dictionary<string, List<string>> _chordsScales = new Dictionary<string, List<string>>();

        /// <summary>all the notes.</summary>
        const int NOTES_PER_OCTAVE = 12;
        #endregion

        /// <summary>
        /// Load chord and scale definitions.
        /// </summary>
        static MusicDefinitions()
        {
            _chordsScales.Clear();

            foreach(string sl in _chordDefs.Concat(_scaleDefs))
            {
                List<string> parts = sl.SplitByToken(" ");
                var name = parts[0];
                parts.RemoveAt(0);
                _chordsScales[name] = parts;
            }
        }

        #region Note definitions
        /// <summary>All possible note names and aliases.</summary>
        static readonly List<string> _noteNames = new()
        {
            "C", "Db", "D", "Eb", "E", "F", "Gb", "G", "Ab", "A", "Bb", "B",
            "B#", "C#", "", "D#", "Fb", "E#", "F#", "", "G#", "", "A#", "Cb",
            "1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "11", "12"
        };

        /// <summary>Helpers.</summary>
        static readonly List<int> _naturals = new()
        {
            0, 2, 4, 5, 7, 9, 11
        };

        /// <summary>Helpers.</summary>
        static readonly List<string> _intervals = new()
        {
            "1", "b2", "2", "b3", "3", "4", "b5", "5", "#5", "6", "b7", "7",
            "", "", "9", "#9", "", "11", "#11", "", "", "13", "", ""
        };

        /// <summary>All the builtin chord defs.</summary>
        static readonly List<string> _chordDefs = new()
        {
            "M 1 3 5", "m 1 b3 5", "7 1 3 5 b7", "M7 1 3 5 7", "m7 1 b3 5 b7", "6 1 3 5 6", "m6 1 b3 5 6", "o 1 b3 b5", "o7 1 b3 b5 bb7",
            "m7b5 1 b3 b5 b7", "\\+ 1 3 #5", "7#5 1 3 #5 b7", "9 1 3 5 b7 9", "7#9 1 3 5 b7 #9", "M9 1 3 5 7 9", "Madd9 1 3 5 9", "m9 1 b3 5 b7 9",
            "madd9 1 b3 5 9", "11 1 3 5 b7 9 11", "m11 1 b3 5 b7 9 11", "7#11 1 3 5 b7 #11", "M7#11 1 3 5 7 9 #11", "13 1 3 5 b7 9 11 13",
            "M13 1 3 5 7 9 11 13", "m13 1 b3 5 b7 9 11 13", "sus4 1 4 5", "sus2 1 2 5", "5 1 5"
        };

        /// <summary>All the builtin scale defs.</summary>
        static readonly List<string> _scaleDefs = new()
        {
            "Acoustic 1 2 3 #4 5 6 b7", "Aeolian 1 2 b3 4 5 b6 b7", "NaturalMinor 1 2 b3 4 5 b6 b7", "Algerian 1 2 b3 #4 5 b6 7",
            "Altered 1 b2 b3 b4 b5 b6 b7", "Augmented 1 b3 3 5 #5 7", "Bebop 1 2 3 4 5 6 b7 7", "Blues 1 b3 4 b5 5 b7",
            "Chromatic 1 #1 2 #2 3 4 #4 5 #5 6 #6 7", "Dorian 1 2 b3 4 5 6 b7", "DoubleHarmonic 1 b2 3 4 5 b6 7", "Enigmatic 1 b2 3 #4 #5 #6 7",
            "Flamenco 1 b2 3 4 5 b6 7", "Gypsy 1 2 b3 #4 5 b6 b7", "HalfDiminished 1 2 b3 4 b5 b6 b7", "HarmonicMajor 1 2 3 4 5 b6 7",
            "HarmonicMinor 1 2 b3 4 5 b6 7", "Hirajoshi 1 3 #4 5 7", "HungarianGypsy 1 2 b3 #4 5 b6 7", "HungarianMinor 1 2 b3 #4 5 b6 7",
            "In 1 b2 4 5 b6", "Insen 1 b2 4 5 b7", "Ionian 1 2 3 4 5 6 7", "Istrian 1 b2 b3 b4 b5 5", "Iwato 1 b2 4 b5 b7", "Locrian 1 b2 b3 4 b5 b6 b7",
            "LydianAugmented 1 2 3 #4 #5 6 7", "Lydian 1 2 3 #4 5 6 7", "Major 1 2 3 4 5 6 7", "MajorBebop 1 2 3 4 5 #5 6 7", "MajorLocrian 1 2 3 4 b5 b6 b7",
            "MajorPentatonic 1 2 3 5 6", "MelodicMinorAscending 1 2 b3 4 5 6 7", "MelodicMinorDescending 1 2 b3 4 5 b6 b7 8", "MinorPentatonic 1 b3 4 5 b7",
            "Mixolydian 1 2 3 4 5 6 b7", "NeapolitanMajor 1 b2 b3 4 5 6 7", "NeapolitanMinor 1 b2 b3 4 5 b6 7", "Octatonic 1 2 b3 4 b5 b6 6 7",
            "Persian 1 b2 3 4 b5 b6 7", "PhrygianDominant 1 b2 3 4 5 b6 b7", "Phrygian 1 b2 b3 4 5 b6 b7", "Prometheus 1 2 3 #4 6 b7",
            "Tritone 1 b2 3 b5 5 b7", "UkrainianDorian 1 2 b3 #4 5 6 b7", "WholeTone 1 2 3 #4 #5 #6", "Yo 1 b3 4 5 b7", 
        };
        #endregion

        #region Note manipulation functions
        /// <summary>
        /// Convert note number into name.
        /// </summary>
        /// <param name="inote"></param>
        /// <returns></returns>
        public static string NoteNumberToName(int inote)
        {
            var split = SplitNoteNumber(inote);
            string s = $"{_noteNames[split.root]}{split.octave}";
            return s;
        }

        /// <summary>
        /// Convert note name into number.
        /// </summary>
        /// <param name="snote">The root of the note without octave.</param>
        /// <returns>The number or null if invalid.</returns>
        public static int? NoteNameToNumber(string snote)
        {
            int inote = _noteNames.IndexOf(snote) % NOTES_PER_OCTAVE;
            return inote == -1 ? null : inote;
        }

        /// <summary>
        /// Parse note or notes from input value.
        /// </summary>
        /// <param name="noteString">Standard string to parse.</param>
        /// <returns>List of note numbers - empty if invalid.</returns>
        public static List<int> GetNotesFromString(string noteString)
        {
            List<int> notes = new();

            // Parse the input value.
            // Note: Need exception handling here to protect from user script errors.
            try
            {
                // Could be:
                // F4 - named note
                // F4.dim7 - named key/chord
                // F4.FOO - user defined key/chord or scale
                // F4.major - named key/scale

                // Break it up.
                var parts = noteString.SplitByToken(".");
                string snote = parts[0];

                // Start with octave.
                int octave = 4; // default is middle C
                string soct = parts[0].Last().ToString();

                if (soct.IsInteger())
                {
                    octave = int.Parse(soct);
                    snote = snote.Remove(snote.Length - 1);
                }

                // Figure out the root note.
                int? noteNum = NoteNameToNumber(snote);
                if (noteNum is not null)
                {
                    // Transpose octave.
                    noteNum += (octave + 1) * NOTES_PER_OCTAVE;
                }
                else
                {
                    throw new InvalidOperationException($"Invalid note: {parts[0]}");
                }

                if (parts.Count > 1)
                {
                    // It's a chord. M, M7, m, m7, etc. Determine the constituents.
                    var chordNotes = _chordsScales[parts[1]];
                    //var chordNotes = chordParts[0].SplitByToken(" ");

                    for (int p = 0; p < chordNotes.Count; p++)
                    {
                        string interval = chordNotes[p];
                        bool down = false;

                        if (interval.StartsWith("-"))
                        {
                            down = true;
                            interval = interval.Replace("-", "");
                        }

                        int? iint = GetInterval(interval);
                        if (iint is not null)
                        {
                            iint = down ? iint - NOTES_PER_OCTAVE : iint;
                            notes.Add(noteNum.Value + iint.Value);
                        }
                    }
                }
                else
                {
                    // Just the root.
                    notes.Add(noteNum.Value);
                }
            }
            catch (Exception)
            {
                notes.Clear();
                throw new InvalidOperationException("Invalid note or chord: " + noteString);
            }

            return notes;
        }

        /// <summary>
        /// Is it a white key?
        /// </summary>
        /// <param name="notenum">Which note</param>
        /// <returns>True/false</returns>
        public static bool IsNatural(int notenum)
        {
            return _naturals.Contains(SplitNoteNumber(notenum).root % NOTES_PER_OCTAVE);
        }

        /// <summary>
        /// Split a midi note number into root note and octave.
        /// </summary>
        /// <param name="notenum">Absolute note number</param>
        /// <returns>tuple of root and octave</returns>
        public static (int root, int octave) SplitNoteNumber(int notenum)
        {
            int root = notenum % NOTES_PER_OCTAVE;
            int octave = (notenum / NOTES_PER_OCTAVE) - 1;
            return (root, octave);
        }

        /// <summary>
        /// Get interval offset from name.
        /// </summary>
        /// <param name="sinterval"></param>
        /// <returns>Offset or null if invalid.</returns>
        public static int? GetInterval(string sinterval)
        {
            int flats = sinterval.Count(c => c == 'b');
            int sharps = sinterval.Count(c => c == '#');
            sinterval = sinterval.Replace(" ", "").Replace("b", "").Replace("#", "");

            int iinterval = _intervals.IndexOf(sinterval);
            return iinterval == -1 ? null : iinterval + sharps - flats;
        }

        /// <summary>
        /// Get interval name from note number offset.
        /// </summary>
        /// <param name="iint">The name or empty if invalid.</param>
        /// <returns></returns>
        public static string? GetInterval(int iint)
        {
            return iint >= _intervals.Count ? null : _intervals[iint % _intervals.Count];
        }

        /// <summary>
        /// Try to make a note and/or chord string from the param. If it can't find a chord return the individual notes.
        /// </summary>
        /// <param name="notes"></param>
        /// <returns></returns>
        public static List<string> FormatNotes(List<int> notes)
        {
            List<string> snotes = new();

            // Dissect root note.
            foreach (int n in notes)
            {
                int octave = SplitNoteNumber(n).octave;
                int root = SplitNoteNumber(n).root;
                snotes.Add($"\"{NoteNumberToName(root)}{octave}\"");
            }

            return snotes;
        }

        /// <summary>
        /// Add a new chord or scale definition.
        /// </summary>
        /// <param name="name">which</param>
        /// <param name="notes">what</param>
        public static void AddChordScale(string name, string notes)
        {
            _chordsScales[name] = notes.SplitByToken(" ");
        }

        /// <summary>
        /// Get a defined chord or scale definition.
        /// </summary>
        /// <param name="name">which</param>
        /// <returns>The notes.</returns>
        public static List<string> GetChordScale(string name)
        {
            if(_chordsScales.ContainsKey(name))
            {
                return _chordsScales[name];
            }
            throw new ArgumentException($"Invalid chord or scale {name}");
        }
        #endregion
    }
}
