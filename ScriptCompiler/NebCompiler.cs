using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;
using System.Diagnostics;
//using NLog;
using NBagOfTricks;
using NBagOfTricks.ScriptCompiler;
//using Nebulator.Common;
//using Nebulator.Script;


namespace Nebulator.App
{
    public class NebCompiler : ScriptCompilerCore
    {
        #region Properties
        /// <summary>Current active channels.</summary>
        public List<Channel> Channels { get; set; } = new();
        #endregion

        #region Fields
        /// <summary>Main source file name.</summary>
        readonly string _nebfn = "TTODO";// Definitions.UNKNOWN_STRING;

        /// <summary>Code lines that define channels.</summary>
        readonly List<string> _channelDescriptors = new();

        /// <summary>Current hash for lines of interest.</summary>
        int _chHash = 0;
        #endregion

        /// <inheritdoc />
        public override void PreExecute()
        {
            Channels.Clear();

            LocalDlls = new()
            {
                "NAudio", "NLog", "NBagOfTricks", "NebOsc", "Nebulator.Common", "Nebulator.Script"
            };

            Usings = new()
            {
                "static Nebulator.Script.ScriptUtils", "static Nebulator.Common.InstrumentDef",
                "static Nebulator.Common.DrumDef", "static Nebulator.Common.ControllerDef",
                "static Nebulator.Common.SequenceMode"
            };

            // Save hash of current channel descriptors to detect change in source code.
            _chHash = string.Join("", _channelDescriptors).GetHashCode();
            _channelDescriptors.Clear();
        }

        /// <inheritdoc />
        public override void PostExecute()
        {
            // Check for changed channel descriptors.
            if (string.Join("", _channelDescriptors).GetHashCode() != _chHash)
            {
               Channels = ProcessChannelDescs();
            }

        }

        /// <inheritdoc />
        public override bool PreprocessFile(string sline, FileContext pcont)
        {
            bool handled = false;

            if (sline.StartsWith("Channel"))
            {
               // Exclude from output file.
               _channelDescriptors.Add(sline);
               handled = true;
            }

            return handled;
        }

        /// <summary>
        /// Convert channel descriptors into partial Channel objects.
        /// </summary>
        /// <returns></returns>
        List<Channel> ProcessChannelDescs()
        {
           List<Channel> channels = new();

           // Build new channels.
           foreach (string sch in _channelDescriptors)
           {
               try
               {
                   List<string> parts = sch.SplitByTokens("(),;");

                   Channel ch = new()
                   {
                       ChannelName = parts[1].Replace("\"", ""),
                       DeviceType = (DeviceType)Enum.Parse(typeof(DeviceType), parts[2]),
                       ChannelNumber = int.Parse(parts[3]),
                       Patch = (InstrumentDef)Enum.Parse(typeof(InstrumentDef), parts[4]),
                       VolumeWobbleRange = double.Parse(parts[5])
                   };

                   channels.Add(ch);
               }
               catch (Exception)
               {
                   Results.Add(new()
                   {
                       ResultType = CompileResultType.Error,
                       Message = $"Bad statement:{sch}",
                       SourceFile = _nebfn,
                       LineNumber = -1
                   });
               }
           }

           return channels;
        }
    }

    //TODO remove these....
    public enum InstrumentDef { AcousticGrandPiano };
    
    public enum DeviceType { None };

    public class Channel
    {
        /// <summary>Same as midi.</summary>
        public const int NUM_CHANNELS = 16;

        #region Properties - editable
        /// <summary>UI label and script reference.</summary>
        public string ChannelName { get; set; } = "TODO";// Definitions.UNKNOWN_STRING;

        /// <summary>The associated numerical (midi) channel to use</summary>
        public int ChannelNumber { get; set; } = 1;

        /// <summary>Optional patch to send at startup.</summary>
        public InstrumentDef Patch { get; set; } = InstrumentDef.AcousticGrandPiano;

        /// <summary>The device type for this channel. Used to find and bind the device at runtime.</summary>
        public DeviceType DeviceType { get; set; } = DeviceType.None;

        public double VolumeWobbleRange { get; set; } = 0;
        #endregion
    }
}
