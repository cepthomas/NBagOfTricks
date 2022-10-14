using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Ephemera.NBagOfTricks.SimpleIpc;
using Ephemera.NBagOfTricks.PNUT;


namespace Ephemera.NBagOfTricks.Test
{
    public class IPC_BASIC : TestSuite
    {
        //const string TS_FORMAT = @"mm\:ss\.fff";
        const string PIPE_NAME = "058F684D-AF82-4FE5-BD1E-9FD031FE28CF";
        const string LOGFILE_NAME = @"..\..\out\test_ipc_log.txt";
        readonly MpLog _log = new(LOGFILE_NAME, "TESTER");

        public override void RunSuite()
        {
            UT_INFO("Tests simple IPC.");
            int NumIterators = 9;

            // Server
            using Server server = new(PIPE_NAME, LOGFILE_NAME);

            int iter = 0;

            server.ServerEvent += Server_IpcEvent;
            server.Start();

            void Server_IpcEvent(object? sender, ServerEventArgs e)
            {
                UT_FALSE(e.Error);
                UT_EQUAL(e.Message, $"ABC{iter * 111}");
            }

            // Client - TODO put clients in separate process.
            // new Process { StartInfo = new ProcessStartInfo(fn) { UseShellExecute = true } }.Start();
            for (iter = 0; iter < NumIterators; iter++)
            {
                Client client = new(PIPE_NAME, LOGFILE_NAME);
                var res = client.Send($"ABC{(iter + 1) * 111}", 1000);

                switch (res)
                {
                    case ClientStatus.Ok:
                        _log.Write($"Client ok");
                        break;

                    case ClientStatus.Error:
                        _log.Write($"Client error:{client.Error}", true);
                        break;

                    case ClientStatus.Timeout:
                        _log.Write($"Client timeout", true);
                        break;
                }
            }
        }
    }
}
