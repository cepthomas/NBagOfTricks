using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.Json.Serialization;
using System.Drawing;
using System.Diagnostics;
using System.Threading;
using Ephemera.NBagOfTricks;
using Ephemera.NBagOfTricks.PNUT;


namespace Ephemera.NBagOfTricks.Test
{
    public class CONSEX_MISC : TestSuite //TODO1
    {
        public override void RunSuite()
        {
            UT_INFO("Test ConsoleEx.");

            //bool bret;

            MockConsole console = new();
           // var cli = new Cli("none", console);

            string prompt = ">";

            console.Clear();

            console.StdinRead = "bbbbb";
            //bret = cli.DoCommand();

            // UT_EQUAL(console.StdoutCapture.Count, 2);
            // UT_EQUAL(console.StdoutCapture[0], $"Invalid command");
            // UT_EQUAL(console.StdoutCapture[1], prompt);

            // // Window move/size.
            // Print($"{_console.WindowHeight} {_console.WindowWidth}");
            // _console.WindowHeight = _console.WindowHeight - 10;
            // _console.WindowWidth = _console.WindowWidth - 10;
            // Print($"{_console.WindowHeight} {_console.WindowWidth}");

        }
    }
}

    // ///////////////////////////////// test stuff ??? ////////////////////////////////////
    // public class CliHost : IDisposable
    // {
    //     #region Fields
    //     /// <summary>Resource management.</summary>
    //     bool _disposed = false;

    //     /// <summary>CLI.</summary>
    //     readonly IConsole _console;

    //     /// <summary>CLI prompt.</summary>
    //     readonly string _prompt = ">";
    //     #endregion

    //     #region Lifecycle
    //     /// <summary>
    //     /// Constructor inits stuff.
    //     /// </summary>
    //     /// <param name="scriptFn">Cli version requires cl script name.</param>
    //     /// <param name="console">Mock</param>
    //     public CliHost(string scriptFn, IConsole console)
    //     {
    //         _console = console;
    //     }

    //     public void Dispose()
    //     {
    //         throw new NotImplementedException();
    //     }
    //     #endregion
    // }
